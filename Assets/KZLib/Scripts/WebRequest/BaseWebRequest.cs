using System;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;

namespace KZLib.KZNetwork
{
	public record RequestData(bool Result,string Text);

	public abstract class BaseWebRequest : IDisposable
	{
		private bool m_Disposed = false;

		protected const string REDIRECT_URL = @"http://127.0.0.1";
		protected UnityWebRequest m_WebRequest = null;

		protected BaseWebRequest(string _uri,string _method)
		{
			m_WebRequest = new UnityWebRequest(_uri,_method)
			{
				downloadHandler = new DownloadHandlerBuffer()
			};
		}

		~BaseWebRequest()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool _disposing)
		{
			if(m_Disposed)
			{
				return;
			}

			if(_disposing)
			{
				m_WebRequest?.Dispose();
			}

			m_WebRequest = null;
			m_Disposed = true;
		}

		public void Send()
		{
			SendAsync().Forget();
		}

		public async UniTask<RequestData> SendAsync()
		{
			try
			{
				await m_WebRequest.SendWebRequest();

				if(m_WebRequest.result == UnityWebRequest.Result.Success)
				{
					var json = JObject.Parse(m_WebRequest.downloadHandler.text);

					if(json.ContainsKey("result"))
					{
						var result = (bool) json["result"];

						if(result)
						{
							return new RequestData(true,json["data"].ToString());
						}
						else
						{
							throw new Exception($"Error: {json["code"] ?? "400"}/{json["error"] ?? "Unknown Error"}");
						}
					}
					else
					{
						return new RequestData(true,m_WebRequest.downloadHandler.text);
					}
				}
				else
				{
					//? https://ooz.co.kr/260 Error Check
					throw new Exception($"Error: {m_WebRequest.responseCode}/{m_WebRequest.error}");
				}
			}
			catch(Exception _exception)
			{
				LogTag.System.E($"{_exception.Message}");

				return new RequestData(false,_exception.Message);
			}
			finally
			{
				Dispose();
			}
		}
	}
}