// using System;
// using System.Net;
// using System.Threading;
// using UnityEngine;
// using UnityEngine.Networking;

// namespace KZLib.Auth
// {
// 	public abstract class AccessTokenProvider
// 	{
// 		protected const string LOOPBACK_RESPONSE_HTML = "<html><h1>Please return to the app.</h1></html>";

// 		private SynchronizationContext m_UnitySyncContext;

// 		protected AccessTokenHandler m_AccessTokenRefresher;
// 		protected AccessTokenHandler m_AccessTokenExchanger;

// 		private Action<AccessTokenProvider> m_OnComplete;

// 		protected abstract string CachedAccessToken { get; }
// 		protected abstract string CachedRefreshToken { get; }

// 		public bool IsDone { get; private set; }
// 		public bool IsError { get; private set; }		

// 		protected abstract bool IsExistAuthData { get; }

// 		protected abstract void ExecuteFullAuth();

// 		protected abstract void OnHttpCallBack(object _state);

// 		protected abstract UnityWebRequest GetExchangeAuthCode(string _code);
// 		protected abstract UnityWebRequest GetRefreshAccessToken();

// 		protected abstract void OnExchangeToken(string _accessToken,string _refreshToken);
		
// 		protected abstract void OnRefreshToken(string _accessToken,string _refreshToken);

// 		public AccessTokenProvider()
// 		{
// 			m_UnitySyncContext = SynchronizationContext.Current;

// 			m_AccessTokenRefresher = new AccessTokenHandler(OnAccessTokenRefresh,OnRefreshToken);

// 			m_AccessTokenExchanger = new AccessTokenHandler(OnAccessTokenExchange,OnExchangeToken);
// 		}

// 		private void OnAccessTokenRefresh(AccessTokenHandler _handler)
// 		{
// 			if(_handler.IsError == false)
// 			{
// 				ProvideAccessTokenComplete();

// 				return;
// 			}
			
// 			ExecuteFullAuth();
// 		}

// 		private void OnAccessTokenExchange(AccessTokenHandler _handler)
// 		{
// 			ProvideAccessTokenComplete(_handler.IsError == true);
// 		}

// 		protected void ProvideAccessTokenComplete(bool _error = false)
// 		{
// 			IsError = _error;
// 			IsDone = true;

// 			m_OnComplete?.Invoke(this);
// 		}

// 		public void ProvideAccessToken(Action<AccessTokenProvider> _onComplete)
// 		{
// 			m_OnComplete += _onComplete;
			
// 			if(IsExistAuthData == false)
// 			{
// 				Log.Library.E("인증 데이터가 없습니다.");
				
// 				ProvideAccessTokenComplete(true);

// 				return;
// 			}

// 			if(CachedRefreshToken.IsEmpty() == true)
// 			{
// 				ExecuteFullAuth();

// 				return;
// 			}

// 			m_AccessTokenExchanger.GetAuthCode(GetRefreshAccessToken());
// 		}

// 		protected void SetHttpListener(HttpListener _listener)
// 		{
// 			var result = _listener.BeginGetContext((result) => { m_UnitySyncContext.Send(OnHttpCallBack,result); },_listener);

// 			if(Application.runInBackground == false)
// 			{
// 				result.AsyncWaitHandle.WaitOne();
// 			}
// 		}

// 		public void RemoveOnComplete(Action<AccessTokenProvider> _onComplete)
// 		{
// 			m_OnComplete -= _onComplete;
// 		}
// 	}
// }