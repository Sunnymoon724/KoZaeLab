using System;
using Cysharp.Threading.Tasks;
using KZLib.Data;
using KZLib.Utilities;
using MessagePipe;
using Newtonsoft.Json;

namespace KZLib.Networks
{
	/// <summary>
	/// Central gateway for server requests, PlayFab cloud-script execution, and response dispatch to FacetManager.
	/// Handles request locking, optional input blocking, error routing, and encrypted packet parsing.
	/// Incomplete: will be finalized when backend server integration and live testing are done.
	/// </summary>
	public partial class NetworkManager : Singleton<NetworkManager>
	{
		private const string c_titleScene = "TitleScene";

		private bool m_isRequesting = false;
		public bool IsRequesting => m_isRequesting;

		private AccountCredential m_accountCredential = null;
#if KZLIB_PLAY_FAB
		// Incomplete: RSA public key loading is not implemented yet.
		private string m_publicKey = null;
#endif
		public bool HasAccountToken => m_accountCredential.HasAccountToken;

		public string AccountId => m_accountCredential.AccountId;

		private NetworkManager() { }

		protected override void _Initialize()
		{
			base._Initialize();

#if KZLIB_PLAY_FAB
			m_accountCredential = new PlayFabAccountCredential();
#else
			m_accountCredential = new StubAccountCredential();
#endif
		}

		protected override void _Release(bool disposing)
		{
			if(disposing)
			{
				
			}

			base._Release(disposing);
		}

		/// <summary>
		/// Executes a PlayFab cloud script and applies the server response to FacetManager.
		/// </summary>
		public async UniTask<bool> RequestToServerAsync(string functionName,object parameter,bool touchBlock,CommonNoticeTag noticeTag)
		{
#if KZLIB_PLAY_FAB
			async UniTask<bool> _RequestAsync()
			{
				return _GetPlayFabResult(functionName,await PlayFabManager.In.ExecuteCloudScriptAsync(functionName,parameter));
			}

			return await _RequestToServerAsync(_RequestAsync,touchBlock,noticeTag);
#else
			await UniTask.Yield();

			return false;
#endif
		}

		private async UniTask<bool> _RequestToServerAsync(Func<UniTask<bool>> onRequestFunc,bool touchBlock,CommonNoticeTag noticeTag)
		{
			if(IsRequesting)
			{
				LogChannel.Network.W("Request ignored because another request is in progress.");

				return false;
			}

			m_isRequesting = true;

			if(touchBlock)
			{
				KZInputKit.LockInput();
			}

			try
			{
				var result = await onRequestFunc.Invoke();

				if(result && noticeTag != CommonNoticeTag.None)
				{
					GlobalMessagePipe.GetPublisher<CommonNoticeTag,EmptyNoticeInfo>().Publish(noticeTag,EmptyNoticeInfo.Empty);
				}

				return result;
			}
			catch(Exception exception)
			{
				LogChannel.Network.E($"Request failed with exception: {exception.Message}");

				return false;
			}
			finally
			{
				if(touchBlock)
				{
					KZInputKit.UnLockInput();
				}

				m_isRequesting = false;
			}
		}

#if KZLIB_PLAY_FAB
		private string _GetRespondMessage(NetworkPacketInfo respondPacket)
		{
			if(!respondPacket.IsEncrypted)
			{
				return respondPacket.Message;
			}

			if(m_publicKey.IsEmpty())
			{
				LogChannel.Network.E("Encrypted respond received but RSA public key is not loaded.");

				return null;
			}

			try
			{
				return KZCryptoKit.RSA.DecryptFromString(respondPacket.Message,m_publicKey);
			}
			catch(Exception exception)
			{
				LogChannel.Network.E($"Failed to decrypt respond message: {exception.Message}");

				return null;
			}
		}

		private bool _GetPlayFabResult(string functionName,PlayFabPacketInfo playFabPacket)
		{
			var respondPacket = playFabPacket.RespondPacket;
			var code = respondPacket.Code;
			var isSuccess = code == 0;
			var message = _GetRespondMessage(respondPacket);
			var requestText = JsonConvert.SerializeObject(playFabPacket.RequestPacket,Formatting.Indented);

			KZDumpKit.WritePlayFabDump(functionName,requestText,isSuccess,message,playFabPacket.Duration);

			if(!isSuccess)
			{
				LogChannel.Network.E($"Respond Error : {code}");

				if(!ProtoManager.In.TryGet(code,typeof(NetworkErrorProto),out var errorProto) || errorProto is not INetworkErrorProto networkErrorProto)
				{
					var text = message.IsEmpty() ? code.ToString() : $"{code} : {message}";

					_ShowErrorPopup(text,null);
				}
				else
				{
					switch(networkErrorProto.ResultMainType)
					{
						case NetworkErrorResultType.Popup:
							{
								void _Close()
								{
									if(networkErrorProto.ResultSubType == NetworkErrorResultType.Title)
									{
										_ChangeTitleScene();
									}
								}

								_ShowErrorPopup(networkErrorProto.Description,_Close);
								break;
							}
						case NetworkErrorResultType.Toast:
							{
								_ShowErrorToast(networkErrorProto.Description);
								break;
							}
						case NetworkErrorResultType.Title:
							{
								_ChangeTitleScene();
								break;
							}
						default:
							{
								LogChannel.Network.W($"Unhandled network error result type: {networkErrorProto.ResultMainType}");

								_ShowErrorPopup(networkErrorProto.Description,null);
								break;
							}
					}
				}
			}
			else if(message.IsEmpty() || !_SetPacket(message))
			{
				isSuccess = false;
			}

			return isSuccess;
		}

		private void _ChangeTitleScene()
		{
			SceneStateManager.In.ChangeScene(c_titleScene,new SceneChangeInfo(CommonUINameTag.TransitionPanel));
		}
#endif

		/// <summary>Deserializes server facet payloads and applies them to FacetManager.</summary>
		private bool _SetPacket(string message)
		{
			if(message.IsEmpty())
			{
				LogChannel.Network.E("Server facet payload is empty.");

				return false;
			}

			try
			{
				var respondArray = JsonConvert.DeserializeObject<NetworkRespondInfo[]>(message);

				if(respondArray == null || respondArray.Length == 0)
				{
					LogChannel.Network.E("Server facet payload deserialized to null or empty array.");

					return false;
				}

				for(var i=0;i<respondArray.Length;i++)
				{
					var respond = respondArray[i];
					var typeText = respond.Type;
					var facetText = respond.Content;
					var type = _ResolveFacetType(typeText);

					if(type == null || !typeof(IFacet).IsAssignableFrom(type))
					{
						throw new InvalidOperationException($"{typeText} is not found or is not an IFacet type.");
					}

					var newFacet = JsonConvert.DeserializeObject(facetText,type) as IFacet;

					if(newFacet == null)
					{
						throw new InvalidOperationException($"Failed to deserialize facet payload as {type.Name}.");
					}

					FacetManager.In.Apply(newFacet);
				}

				return true;
			}
			catch(Exception exception)
			{
				LogChannel.Network.E($"Failed to deserialize server facet payload: {exception.Message}");

				return false;
			}
		}

		private static Type _ResolveFacetType(string typeText)
		{
			if(typeText.IsEmpty())
			{
				return null;
			}

			var type = Type.GetType(typeText);

			if(type != null)
			{
				return type;
			}

			type = KZReflectionKit.FindType(typeText);

			if(type != null)
			{
				return type;
			}

			var dotIndex = typeText.LastIndexOf('.');

			if(dotIndex > 0)
			{
				return KZReflectionKit.FindType(typeText.Substring(dotIndex + 1),typeText.Substring(0,dotIndex));
			}

			return null;
		}

		private void _ShowErrorPopup(string text,Action closeCallBack)
		{
			// Incomplete: error popup UI is not implemented yet.
		}

		private void _ShowErrorToast(string text)
		{
			// Incomplete: error toast UI is not implemented yet.
		}
	}
}