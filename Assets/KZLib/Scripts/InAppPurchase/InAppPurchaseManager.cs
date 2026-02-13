#if KZLIB_IN_APP_PURCHASE
using System;
using System.Collections.Generic;
using KZLib.Utilities;
using UnityEngine;
using UnityEngine.Purchasing;
using Cysharp.Threading.Tasks;
using System.Threading;
using R3;
using System.Text;
using KZLib.Networking;

#if !UNITY_EDITOR

using UnityEngine.Purchasing.Security;
using System.Collections;
using Newtonsoft.Json;

#endif

namespace KZLib.Purchasing
{
	public class InAppPurchaseManager : Singleton<InAppPurchaseManager>
	{
#if !UNITY_EDITOR
		private enum PurchaseResultType { Unknown, Success, Fail, }
		private record PurchaseResultInfo(PurchaseResultType Type,string TransactionID,PurchaseFailureReason FailureReason);
#endif
		private const float c_retryPeriod = 2;
		private const int c_maxRetryCount = 5;
		private const int c_maxFetchCount = 50;

		private const float c_timeOut = 10.0f;

		private StoreController m_storeController = null;

#if !UNITY_EDITOR
		private CrossPlatformValidator m_validator;
#endif
		private HashSet<string> m_fetchSuccessHashSet;
		private HashSet<string> m_fetchFailHashSet;
		private int m_fetchFailCount;

		private List<ProductDefinition> m_fetchingProductList;

		private string m_lastPurchaseProductId = string.Empty;

#if !UNITY_EDITOR
		private PurchaseResultInfo m_purchaseResultInfo = null;
#endif

		private Func<string,string> m_onConvertToSkuFromPid = null;

		private CancellationTokenSource m_tokenSource = null;

		private readonly Subject<string> m_resultSendSubject = new();
		public Observable<string> OnSentResult => m_resultSendSubject;

		private readonly Subject<Unit> m_unfinishedProductPurchasedSubject = new();
		public Observable<Unit> OnPurchasedUnfinishedProduct => m_unfinishedProductPurchasedSubject;

		private InAppPurchaseManager() { }

		protected override void _Initialize()
		{
			base._Initialize();

			m_storeController = UnityIAPServices.StoreController();

			m_fetchingProductList = new List<ProductDefinition>();

			m_fetchSuccessHashSet = new HashSet<string>();
			m_fetchFailHashSet = new HashSet<string>();
		}

		protected override void _Release(bool disposing)
		{
			if(disposing)
			{
				m_storeController = null;

				m_fetchingProductList.Clear();

				m_fetchSuccessHashSet.Clear();
				m_fetchFailHashSet.Clear();

#if !UNITY_EDITOR
				m_validator = null;
#endif
			}

			base._Release(disposing);
		}

		public void SetInAppPurchase(List<string> skuList,Func<string,string> onConvertToSkuFromPid)
		{
			m_onConvertToSkuFromPid = onConvertToSkuFromPid;

			m_storeController.OnPurchasePending += _OnPurchasePending;
			m_storeController.OnPurchaseFailed += _OnPurchaseFailed;

			m_storeController.OnStoreDisconnected += _OnStoreDisconnected;

			// 초기화 끝나고 상품 세팅
			m_storeController.OnProductsFetchFailed += _OnProductsFetchFailed;
			m_storeController.OnProductsFetched += _OnProductsFetched;

			m_storeController.OnPurchasesFetched += _OnPurchasesFetched;

			CommonUtility.RecycleTokenSource(ref m_tokenSource);

			_SetInAppPurchaseAsync(skuList,m_tokenSource.Token).Forget();
		}

		private async UniTaskVoid _SetInAppPurchaseAsync(List<string> skuList,CancellationToken token)
		{
			var logBuilder = new StringBuilder();

			for(var i=1;i<=c_maxRetryCount;i++)
			{
				var connectTask = m_storeController.Connect();
				var timeoutTask = UniTask.Delay(TimeSpan.FromSeconds(c_timeOut),cancellationToken: token);

				var (winArgumentIndex,_,_) = await UniTask.WhenAny(connectTask.AsUniTask().SuppressCancellationThrow(),timeoutTask.SuppressCancellationThrow());

				if(winArgumentIndex == 0)
				{
					LogChannel.External.I("[IAP] Initialize succeeded");

					await _FetchProductAsync(skuList,token);

					return;
				}

				var exception = connectTask.Exception?.InnerException ?? connectTask.Exception ?? new Exception("Unknown error");

				LogChannel.External.E($"[IAP] Initialize failed.. Retrying in {c_retryPeriod} seconds.. [{i}/{c_maxRetryCount}]");

				logBuilder.Append($"Exception {i}: {exception.Message}");

				await UniTask.Delay(TimeSpan.FromSeconds(c_retryPeriod),cancellationToken: token).SuppressCancellationThrow();
			}

			LogChannel.External.E($"[IAP] Initialize failed.. Max retry attempts reached.");

#if !UNITY_EDITOR
			m_resultSendSubject.OnNext(JsonConvert.SerializeObject(new Dictionary<string,string>()
			{
				{ "type","initialize failed" },
				{ "reason", $"{logBuilder.ToString()}" },
			}));
#endif
		}

		private async UniTask _FetchProductAsync(List<string> skuList,CancellationToken token)
		{
#if !UNITY_EDITOR
			if(_IsCurrentStoreSupportedByValidator())
			{
				m_validator = new CrossPlatformValidator(GooglePlayTangle.Data(),AppleTangle.Data(),Application.identifier);
			}
			else
			{
				Debug.LogError( $"[IAP] 지원하지 않는 스토어 입니다. {UnityIAPServices.DefaultStore()}" );
			}
#endif
			m_fetchingProductList.Clear();

			for(var i=0;i<skuList.Count;i++)
			{
				m_fetchingProductList.Add(new ProductDefinition(skuList[i],ProductType.Consumable));
			}

			var attempt = 0;
			m_fetchFailHashSet.Clear();

			while(m_fetchingProductList.Count > 0)
			{
				var fetchCount = Mathf.Min(m_fetchingProductList.Count,c_maxFetchCount);
				var fetchList = m_fetchingProductList.GetRange(0,fetchCount);

				LogChannel.External.I($"[IAP] {fetchCount} products fetching started.");

				m_fetchFailCount = 0;
				m_fetchSuccessHashSet.Clear();

				m_storeController.FetchProducts(fetchList);

				bool _FinishFetch()
				{
					return (m_fetchSuccessHashSet.Count+m_fetchFailCount)>=fetchCount;
				}

				await UniTask.WaitUntil(_FinishFetch,cancellationToken: token).SuppressCancellationThrow();

				if(m_fetchSuccessHashSet.Count != 0)
				{
					for(var i = m_fetchingProductList.Count-1;i>=0;i--)
					{
						if(m_fetchSuccessHashSet.Contains(m_fetchingProductList[i].id))
						{
							m_fetchingProductList.RemoveAt(i);
						}
					}

					attempt = 0;
				}
				else
				{
					attempt++;

					if(attempt >= c_maxRetryCount)
					{
						LogChannel.External.E($"[IAP] Fetching failed.. Max retry attempts reached: {attempt}");

						break;
					}
					else
					{
						LogChannel.External.E($"[IAP] Fetching failed without success.. Retrying.. ({attempt}/{c_maxRetryCount})");
					}
				}
			}

			if(m_fetchFailHashSet.Count != 0)
			{
#if !UNITY_EDITOR
				m_resultSendSubject.OnNext(JsonConvert.SerializeObject(new Dictionary<string,string>()
				{
					{ "type","fetch failed" },
					{ "reason", $"{string.Join(" - ",m_fetchFailHashSet)}" },
				}));
#endif
			}

			m_storeController.FetchPurchases();
		}

		private void _OnPurchasePending(PendingOrder pendingOrder)
		{
#if !UNITY_EDITOR
			var product = _GetFirstProductInOrder( pendingOrder );

			if(product == null)
			{
				LogChannel.External.E( "[IAP] No product info." );

				return;
			}

			// 자체 검증
			if(!_IsValidPurchase())
			{
				LogChannel.External.E("[IAP] Unsupported purchase method.");

				return;
			}

			m_purchaseResultInfo = new PurchaseResultInfo(PurchaseResultType.Success,pendingOrder.Info.TransactionID,PurchaseFailureReason.Unknown);
#endif
		}

		private void _OnPurchaseFailed(FailedOrder order)
		{
#if !UNITY_EDITOR
			var product = _GetFirstProductInOrder(order);

			if(product == null)
			{
				LogChannel.External.E("[IAP] No product info.");

				return;
			}

			var sku = product.definition.id;
			var failureReason = order.FailureReason;

			LogChannel.External.E( $"[IAP] Purchase failed: {sku}, Reason: {failureReason} - {order.Details}" );

			if(failureReason != PurchaseFailureReason.UserCancelled)
			{
				m_resultSendSubject.OnNext(JsonConvert.SerializeObject(new Dictionary<string,string>()
				{
					{ "type","purchase failed" },
					{ "sku", $"{sku}"},
					{ "reason", $"{failureReason}" },
				}));
			}

			m_purchaseResultInfo = new PurchaseResultInfo(PurchaseResultType.Fail,string.Empty,failureReason);
#endif
		}

		private void _OnStoreDisconnected(StoreConnectionFailureDescription description)
		{
			LogChannel.External.E( $"[IAP] Store disconnected. ({description.Message})" );
		}

		private void _OnProductsFetchFailed(ProductFetchFailed fetchFailed)
		{
			var productList = fetchFailed.FailedFetchProducts;

			for(var i=0;i<productList.Count;i++ )
			{
				m_fetchFailHashSet.Add(productList[ i ].id);
			}

			LogChannel.External.E( $"[IAP] Product fetch failed - Count: {productList.Count} Reason: {fetchFailed.FailureReason}" );

			m_fetchFailCount = productList.Count;
		}

		private void _OnProductsFetched(List<Product> productList)
		{
			LogChannel.External.I( $"[IAP] Product fetch succeeded - Count: {productList.Count}" );

			for(var i=0;i<productList.Count;i++)
			{
				m_fetchSuccessHashSet.Add(productList[ i ].definition.id);
			}
		}

		private void _OnPurchasesFetched(Orders orders)
		{
			LogChannel.External.I( $"[IAP] Checking pending orders" );

#if !UNITY_EDITOR
			var pendingOrderArray = orders.PendingOrders;

			for(var i=0;i<pendingOrderArray.Count;i++)
			{
				var pendingOrder = pendingOrderArray[i];

				var product = _GetFirstProductInOrder(pendingOrder);

				if(product == null)
				{
					continue;
				}

				var sku = product.definition.id;

				LogChannel.External.I( $"[IAP] Detected pending purchase: {sku} - attempting recovery (external)" );

				_PurchaseUnfinishedProductAsync(pendingOrder,sku).Forget();
			}
#endif
		}

		public async UniTask<InAppPurchaseResultType> PurchaseProductAsync(string productId,string sku)
		{
			if(m_lastPurchaseProductId.IsEqual(productId))
			{
				LogChannel.External.W($"[IAP] {productId} is already purchased.");

				return InAppPurchaseResultType.AlreadyPurchased;
			}

			m_lastPurchaseProductId = productId;

			InputManager.In.BlockInput(true);

#if !UNITY_EDITOR
			if(m_storeController == null)
			{
				LogChannel.External.E("[IAP] StoreController is null");

				InputManager.In.BlockInput(false);

				m_lastPurchaseProductId = string.Empty;

				return InAppPurchaseResultType.StoreControllerNotInitialized;
			}

			var product = m_storeController.GetProductById(sku);

			if(product == null)
			{
				LogChannel.External.W($"[IAP] {sku} is not found -> fetch start" );

				CommonUtility.RecycleTokenSource(ref m_tokenSource);

				var skuList = new List<string>(m_fetchingProductList.Count);

				for(var i=0;i<m_fetchingProductList.Count;i++)
				{
					skuList.Add(m_fetchingProductList[i].id);
				}

				skuList.Remove(sku);
				skuList.Insert(0,sku);

				await _FetchProductAsync(skuList);

				product = m_storeController.GetProductById(sku);

				if(product == null)
				{
					LogChannel.External.E($"[IAP] {sku} is not found after fetch.");

					m_lastPurchaseProductId = string.Empty;

					return InAppPurchaseResultType.ProductNotFound;
				}
			}

			m_purchaseResultInfo = null;

			_PurchaseProduct(product,productId);

			bool _IsPurchaseFinished()
			{
				return m_purchaseResultInfo != null;
			}

			await UniTask.WaitUntil(_IsPurchaseFinished);

			if(m_purchaseResultInfo.Type == PurchaseResultType.Fail)
			{
				InputManager.In.BlockInput(false);

				m_lastPurchaseProductId = string.Empty;

				switch(m_purchaseResultInfo.FailureReason)
				{
					case PurchaseFailureReason.UserCancelled:
						return InAppPurchaseResultType.UserCancelled;

					case PurchaseFailureReason.PaymentDeclined:
					case PurchaseFailureReason.DuplicateTransaction:
						return InAppPurchaseResultType.PaymentDeclined;

					case PurchaseFailureReason.ProductUnavailable:
					case PurchaseFailureReason.StoreNotConnected:
					case PurchaseFailureReason.PurchasingUnavailable:
						return InAppPurchaseResultType.ProductUnavailable;

					case PurchaseFailureReason.ValidationFailure:
					case PurchaseFailureReason.SignatureInvalid:
					case PurchaseFailureReason.PurchaseMissing:
					case PurchaseFailureReason.ExistingPurchasePending:
					case PurchaseFailureReason.Unknown:
					default:
						return InAppPurchaseResultType.Unknown;
				}
			}

			var pendingOrder = _GetPurchaseProduct(m_purchaseResultInfo.TransactionID);

			if(pendingOrder == null)
			{
				LogChannel.External.E("[IAP] PendingOrder is null after purchase.");

				m_lastPurchaseProductId = string.Empty;

				InputManager.In.BlockInput(false);

				return InAppPurchaseResultType.ProductNotFound;
			}
			
			var finishResult = await _RequestPurchaseShopProductPaymentAsync(pendingOrder,sku);
#else
			var finishResult = await NetworkManager.In.RequestPurchaseShopProductPaymentAsync(productId,"editor",string.Empty);
#endif

			m_lastPurchaseProductId = string.Empty;

			InputManager.In.BlockInput(false);

			if(!finishResult)
			{
				return InAppPurchaseResultType.NetworkError;
			}

#if !UNITY_EDITOR
			return _ConfirmPurchaseShopProduct(pendingOrder,productId);
#else
			return InAppPurchaseResultType.Success;
#endif
		}

#if !UNITY_EDITOR
		private async UniTask<bool> _RequestPurchaseShopProductPaymentAsync(PendingOrder pendingOrder,string sku)
		{
			var platform = string.Empty;
			var purchaseToken = string.Empty;

#if UNITY_ANDROID
			platform = "google";
			purchaseToken = _GetToken(pendingOrder.Info.Receipt);
#elif UNITY_IOS
			platform = "apple";
			purchaseToken = pendingOrder.Info.TransactionID;
#endif

			var productId = m_onConvertToSkuFromPid(sku);

			return await NetworkManager.In.RequestPurchaseShopProductPaymentAsync(productId,platform,purchaseToken);
		}

		private void _PurchaseProduct(Product product,string productId)
		{
#if UNITY_ANDROID
				var googleService = m_storeController.GooglePlayStoreExtendedService;

				if(googleService != null)
				{
					var accountId = NetworkManager.In.AccountId;

					googleService.SetObfuscatedAccountId(accountId);
					googleService.SetObfuscatedProfileId(productId);
				}
				else
				{
					LogChannel.External.E( "[IAP] GooglePlayStoreExtendedService is null" );
				}
#endif
			m_storeController.PurchaseProduct(product);
		}

		private InAppPurchaseResultType _ConfirmPurchaseShopProduct(PendingOrder pendingOrder,string productId)
		{
			if(m_storeController == null)
			{
				LogChannel.External.E("[IAP] StoreController is null during confirm purchase.");

				return InAppPurchaseResultType.StoreControllerNotInitialized;
			}

			LogChannel.External.I("[IAP] Confirming purchase.");

			m_storeController.ConfirmPurchase(pendingOrder);

			return InAppPurchaseResultType.Success;
		}

		private PendingOrder _GetPurchaseProduct(string transactionId)
		{
			foreach(var order in m_storeController.GetPurchases())
			{
				if(order is	not PendingOrder pendingOrder)
				{
					continue;
				}

				if(pendingOrder.Info.TransactionID == transactionId)
				{
					return pendingOrder;
				}
			}

			return null;
		}

		private Product _GetFirstProductInOrder(Order order)
		{
			var itemList = order.CartOrdered.Items();

			return itemList.Count > 0 ? itemList[0].Product : null;
		}

		private string _GetToken(string receipt)
		{
			if(!receipt.IsEmpty())
			{
				try
				{
					foreach(var validReceipt in m_validator.Validate(receipt))
					{
						if(validReceipt is GooglePlayReceipt googleReceipt)
						{
							LogChannel.External.I($"[IAP] Token {googleReceipt.purchaseToken} / sku {googleReceipt.productID}");

							return googleReceipt.purchaseToken;
						}
					}
				}
				catch(Exception exception)
				{
					LogChannel.External.E($"[IAP] receipt validation failed: {exception.Message}");
				}
			}

			return string.Empty;
		}

		private async UniTask _PurchaseUnfinishedProductAsync(PendingOrder order,string sku)
		{
			var finishResult = await _RequestPurchaseShopProductPaymentAsync(order,sku);

			if(!finishResult)
			{
				return;
			}

			m_unfinishedProductPurchasedSubject.OnNext(Unit.Default);
		}
#endif

		public bool CanPurchaseProduct(string sku)
		{
			var product = m_storeController.GetProductById(sku);

			return product != null && product.availableToPurchase;
		}

		public string GetProductPrice(string sku)
		{
			var product = m_storeController.GetProductById(sku);
			
			return product == null ? string.Empty : product.metadata.localizedPriceString;
		}

#if !UNITY_EDITOR
		private bool _IsValidPurchase()
		{
#if UNITY_ANDROID
			return _IsGooglePlayStoreSelected();
#elif UNITY_IOS
			return _IsAppleAppStoreSelected();
#endif
			return false;
		}

		private bool _IsCurrentStoreSupportedByValidator()
		{
			return _IsGooglePlayStoreSelected() || _IsAppleAppStoreSelected();
		}

		private bool _IsGooglePlayStoreSelected()
		{
			return m_storeController.GooglePlayStoreExtendedService != null;
		}

		private bool _IsAppleAppStoreSelected()
		{
			return m_storeController.AppleStoreExtendedService != null;
		}
#endif
	}
}
#endif