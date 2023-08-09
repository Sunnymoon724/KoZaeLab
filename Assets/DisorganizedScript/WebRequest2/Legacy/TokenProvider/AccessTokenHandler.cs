// using System;
// using System.Text;
// using Newtonsoft.Json.Linq;
// using UnityEngine;
// using UnityEngine.Networking;

// namespace KZLib.Auth
// {
// 	public class AccessTokenHandler
// 	{
// 		protected Action<AccessTokenHandler> m_OnComplete;
// 		protected Action<string,string> m_OnSetToken;

// 		public bool IsDone { get; private set; }
// 		public bool IsError { get; private set; }

// 		private UnityWebRequest m_WebRequest;

// 		public AccessTokenHandler(Action<AccessTokenHandler> _onComplete,Action<string,string> _onSetToken)
// 		{
// 			m_OnComplete = _onComplete;
// 			m_OnSetToken = _onSetToken;
// 		}

// 		public void GetAuthCode(UnityWebRequest _webRequest)
// 		{
// 			m_WebRequest = _webRequest;
// 			m_WebRequest.SendWebRequest().completed += OnRequestComplete;
// 		}

// 		private void Complete(bool _error = false)
// 		{
// 			IsError = _error;
// 			IsDone = true;

// 			m_OnComplete?.Invoke(this);
// 		}

// 		private void OnRequestComplete(AsyncOperation _operation)
// 		{
// 			if(CheckRequestErrors(m_WebRequest,out var data) == true)
// 			{
// 				Complete(true);

// 				return;
// 			}

// 			m_OnSetToken?.Invoke(data["access_token"].ToString(),data["refresh_token"].ToString());

// 			Complete();
// 		}

// 		private bool CheckRequestErrors(UnityWebRequest _request,out JObject _object)
// 		{
// 			_object = null;

// 			if(_request == null)
// 			{
// 				Log.Library.E("Exchange auth code request failed. Request object is null.");

// 				return true;
// 			}

// 			var builder = new StringBuilder();

// 			if(_request.error.IsEmpty() == false)
// 			{
// 				builder.AppendFormat(" HTTP Error: {0}",_request.error);
// 			}

// 			if(_request.downloadHandler != null && _request.downloadHandler.text.IsEmpty() == false)
// 			{
// 				_object = JObject.Parse(_request.downloadHandler.text);
				
// 				var errorText = _object["error"].ToString();

// 				if(errorText.IsEmpty() == false)
// 				{
// 					builder.AppendFormat(" API Error: {0} API Error Description: {1}",errorText,_object["error_description"].ToString());
// 				}
// 			}
			
// 			var isError = builder.Length > 0;

// 			if(isError == true)
// 			{
// 				Log.Library.E("Exchange auth code request failed. {0}",builder.ToString());
// 			}

// 			return isError;
// 		}
// 	}
// }