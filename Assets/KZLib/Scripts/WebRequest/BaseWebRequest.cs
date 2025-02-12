using System;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;

namespace KZLib.KZNetwork
{
	public record RequestData(bool Result,string Text);

	public abstract class BaseWebRequest : IDisposable
	{
		private bool m_disposed = false;

		protected const string c_redirect_url = @"http://127.0.0.1";
		protected UnityWebRequest m_webRequest = null;

		protected BaseWebRequest(string uri,string method)
		{
			m_webRequest = new UnityWebRequest(uri,method)
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

		protected virtual void Dispose(bool disposing)
		{
			if(m_disposed)
			{
				return;
			}

			if(disposing)
			{
				m_webRequest?.Dispose();
			}

			m_webRequest = null;
			m_disposed = true;
		}

		public void Send()
		{
			SendAsync().Forget();
		}

		public async UniTask<RequestData> SendAsync()
		{
			try
			{
				await m_webRequest.SendWebRequest();

				if(m_webRequest.result != UnityWebRequest.Result.Success)
				{
					//? https://ooz.co.kr/260 Error Check
					throw new Exception($"Error: {m_webRequest.responseCode}/{m_webRequest.error}");
				}

				var resultText = m_webRequest.downloadHandler.text;

				if(!resultText.IsEmpty())
				{
					var json = JObject.Parse(resultText);

					if(json.ContainsKey("result"))
					{
						var result = (bool)json["result"];

						if(!result)
						{
							throw new Exception($"Error: {json["code"] ?? "400"}/{json["error"] ?? "Unknown Error"}");
						}

						resultText = json["data"].ToString();
					}
				}

				return new RequestData(true,resultText);
			}
			catch(Exception exception)
			{
				LogTag.System.E($"{exception.Message}");

				return new RequestData(false,exception.Message);
			}
			finally
			{
				Dispose();
			}
		}
	}
}