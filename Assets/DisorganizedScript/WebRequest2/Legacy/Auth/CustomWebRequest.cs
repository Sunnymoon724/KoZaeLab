// #if UNITY_EDITOR
// using Newtonsoft.Json;
// using Newtonsoft.Json.Linq;
// using Newtonsoft.Json.Serialization;
// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using System.Reflection;
// using UnityEngine;
// using UnityEngine.Networking;

// namespace KZLib.Auth
// {
// 	public abstract class CustomWebRequest : IDisposable
// 	{

// 		#region Query Parameter
// 		/// <summary>
// 		/// Property will be included in the query portion of the request URL.
// 		/// </summary>
// 		[AttributeUsage(AttributeTargets.Property,AllowMultiple = false)]
// 		protected sealed class QueryParameterAttribute : System.Attribute { }
// 		#endregion Query Parameter

// 		public bool IsDone { get; protected set; }

// 		public abstract bool IsRunning { get; }
// 		public abstract float Progress { get; }


// 		protected bool IsDispose => m_Disposed;
// 		private bool m_Disposed;

// 		// public abstract string Uri { get; protected set; }
// 		// public abstract string Method { get; protected set; }

// 		// protected abstract UnityWebRequest GetWebRequest();

// 		public CustomWebRequest() { }

// 		~CustomWebRequest() => Release(false);

// 		// public const string LOOPBACK_RESPONSE_HTML = "<html><h1>Please return to the app.</h1></html>";

// 		// public const string REDIRECT_URL = "http://localhost:0715";

// 		public abstract GoogleYieldInstruction SendNonGeneric();

// 		public abstract void Abort();

// 		public void Dispose()
// 		{
// 			Release(true);
			
// 			GC.SuppressFinalize(this);
// 		}

// 		protected virtual void Release(bool _disposing)
// 		{
// 			if(IsDispose == true)
// 			{
// 				return;
// 			}
			
// 			m_Disposed = true;
// 		}
// 	}

// 	public abstract class CustomWebRequest<T> : CustomWebRequest
// 	{
// 		private const int UNAUTHORIZED_RESPONSE_CODE = 401;

// 		#region Query
// 		[QueryParameter] public string Alt { get; set; }
// 		[QueryParameter] public List<string> Fields { get; set; }
// 		[QueryParameter] public bool? PrettyPrint { get; set; }
// 		[QueryParameter] public string QuotaUser { get; set; }
// 		[QueryParameter] public string UserIp { get; set; }
// 		#endregion Query

// 		protected UnityWebRequest m_WebRequest;
// 		protected GoogleYieldInstruction<T> m_YieldInstruction;

// 		private bool m_IsRefreshingAccessToken;

// 		public virtual T ResponseData { get; protected set; }

// 		public override bool IsRunning => m_YieldInstruction != null && IsDone == false;
// 		public override float Progress => m_WebRequest != null ? m_WebRequest.downloadProgress : 0.0f;

// 		protected virtual bool AutoCompleteOnDone => true;

// 		private Action<bool> m_OnAccessTokenRefreshed;

// 		protected abstract UnityWebRequest CreateWebRequest();

// 		public abstract void DoCancelAuth();

// 		protected abstract void DoSetUnauthorizedResponse();

		
// 		// public CustomWebRequest(string _uri,string _method)
// 		// {
// 		// 	// Uri = uri;
// 		// 	// Method = method;

// 		// 	// if (Settings == null) Settings = GoogleDriveSettings.LoadFromResources();
// 		// }

// 		public override GoogleYieldInstruction SendNonGeneric()
// 		{
// 			return Send();
// 		}

// 		public GoogleYieldInstruction<T> Send()
// 		{
// 			if(IsRunning == false)
// 			{
// 				m_YieldInstruction = new GoogleYieldInstruction<T>(this);

// 				SendWebRequest();
// 			}

// 			return m_YieldInstruction;
// 		}

// 		public override void Abort()
// 		{
// 			if(m_WebRequest != null && IsRunning == true)
// 			{
// 				m_WebRequest.Abort();
// 			}
// 		}

// 		protected override void Release(bool _disposing)
// 		{
// 			if(IsDispose == true)
// 			{
// 				return;
// 			}

// 			if(_disposing == true)
// 			{
// 				if(m_WebRequest != null)
// 				{
// 					m_WebRequest.Dispose();
// 				}
// 			}
			
// 			base.Release(_disposing);
// 		}

// 		public event Action<T> OnCompleted;

// 		protected void CompleteRequest()
// 		{
// 			IsDone = true;

// 			OnCompleted?.Invoke(ResponseData);

// 			if(m_WebRequest != null)
// 			{
// 				m_WebRequest.Dispose();
// 				m_WebRequest = null;
// 			}
// 		}

// 		protected virtual void SendWebRequest()
// 		{
// 			IsDone = false;

// 			if(m_WebRequest != null)
// 			{
// 				m_WebRequest.Abort();
// 				m_WebRequest.Dispose();
// 			}

// 			m_WebRequest = CreateWebRequest();
// 			m_WebRequest.SendWebRequest().completed += OnWebRequestComplete;
// 		}

// 		protected virtual void OnWebRequestComplete(AsyncOperation _requestYield)
// 		{
// 			//? https://ooz.co.kr/260 에러 체크

// 			Log.Library.I(m_WebRequest.responseCode);
			
// 			if(m_WebRequest.responseCode == UNAUTHORIZED_RESPONSE_CODE)
// 			{
// 				//인증이 안되서 인증 처리
// 				SetUnauthorizedResponse();

// 				return;
// 			}
// 			else if(m_WebRequest.responseCode == 403)
// 			{
// 				Log.Library.E("권한이 없습니다."); // 이부분은 아직 생각 중

// 				return;
// 			}
			
// 			// 나머지 처리
// 			if(m_WebRequest.error.IsEmpty() == false)
// 			{
// 				Log.Library.E(m_WebRequest.error);

// 				return;
// 			}

// 			SetResponseData(m_WebRequest.downloadHandler);

// 			if(AutoCompleteOnDone == true)
// 			{
// 				CompleteRequest();
// 			}
// 		}

// 		protected virtual void SetResponseData(DownloadHandler _handler)
// 		{
// 			var responseText = _handler.text;

// 			if(responseText.IsEmpty() == true)
// 			{
// 				return;
// 			}

// 			try
// 			{
// 				ResponseData = JObject.Parse(responseText).ToObject<T>();
// 			}
// 			catch(JsonException _ex)
// 			{
// 				Log.Library.E(_ex);
// 			}
// 		}

// 		protected void SetUnauthorizedResponse()
// 		{
// 			m_OnAccessTokenRefreshed += OnAccessTokenRefreshed;

// 			if(m_IsRefreshingAccessToken == true)
// 			{
// 				return;
// 			}

// 			m_IsRefreshingAccessToken = true;

// 			DoSetUnauthorizedResponse();
// 		}

// 		public void CancelAuth()
// 		{
// 			if(m_IsRefreshingAccessToken == true)
// 			{
// 				DoCancelAuth();
// 			}
// 		}

// 		protected virtual void OnAccessTokenProviderCompleted(AccessTokenProvider _provider)
// 		{
// 			_provider.RemoveOnComplete(OnAccessTokenProviderCompleted);

// 			var failed = _provider.IsDone == false || _provider.IsError;

// 			if(failed == true)
// 			{
// 				Debug.LogError("Failed to execute authorization procedure. Check application settings and credentials.");
// 			}
			
// 			m_IsRefreshingAccessToken = false;

// 			m_OnAccessTokenRefreshed?.Invoke(failed == false);
// 		}

// 		protected void SetAuthorizationHeader(UnityWebRequest _webRequest)
// 		{
// 			_webRequest.SetRequestHeader("Authorization",string.Format("Bearer {0}",AuthSettings.In.GoogleAccessToken));
// 		}

// 		protected void SetHtmlContentHeader(UnityWebRequest _webRequest)
// 		{
// 			_webRequest.SetRequestHeader("Content-Type","application/x-www-form-urlencoded");
// 		}

// 		protected void SetJsonContentHeader(UnityWebRequest _webRequest)
// 		{
// 			_webRequest.SetRequestHeader("Content-Type","application/json; charset=UTF-8");
// 		}

// 		protected void SetQueryPayload(UnityWebRequest _webRequest)
// 		{
// 			_webRequest.url = string.Format("{0}?{1}",_webRequest.url,GenerateQueryString());
// 		}
		
// 		private void OnAccessTokenRefreshed(bool _success)
// 		{
// 			m_OnAccessTokenRefreshed -= OnAccessTokenRefreshed;

// 			if(_success == true)
// 			{
// 				SendWebRequest();

// 				return;
// 			}

// 			Log.Library.E("Authorization error.");

// 			CompleteRequest();
// 		}
		
// 		private string GenerateQueryString()
// 		{
// 			// Get all query properties on the object.
// 			var propertyDict = GetType().GetProperties().Where(x=>x.IsDefined(typeof(QueryParameterAttribute),false) && x.CanRead && x.GetValue(this,null) != null).ToDictionary(y=>y.Name.ToFirstCharToLower(),z=>z.GetValue(this,null));

// 			// Get names for all IEnumerable properties (excl. string).
// 			var propertyNameList = propertyDict.Where(x=>(x.Value is string) == false && x.Value is IEnumerable).Select(y=>y.Key).ToList();

// 			// Concat all IEnumerable properties into a comma separated string.
// 			foreach(var propertyName in propertyNameList)
// 			{
// 				var valueType = propertyDict[propertyName].GetType();
// 				var valueElemType = valueType.IsGenericType ? valueType.GetGenericArguments()[0] : valueType.GetElementType();

// 				if(valueElemType.IsPrimitive == true || valueElemType == typeof(string))
// 				{
// 					var enumerable = propertyDict[propertyName] as IEnumerable;

// 					propertyDict[propertyName] = string.Join(",",enumerable.Cast<string>().ToArray());
// 				}
// 			}

// 			// Concat all key/value pairs into a string separated by ampersand.
// 			return string.Join("&",propertyDict.Select(x=>string.Format("{0}={1}",Uri.EscapeDataString(x.Key),Uri.EscapeDataString(x.Value.ToString()))).ToArray());
// 		}
// 	}
// }
// #endif