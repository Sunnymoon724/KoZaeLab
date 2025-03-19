using System;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

namespace KZLib
{
	public record ResponseInfo(bool Result,long Code,string Content,string Error);

	public abstract class BaseWebRequest : IDisposable
	{
		private bool m_disposed = false;

		protected const string c_redirect_url = @"http://127.0.0.1";
		private const int c_internal_server_error = 500; // error by server
		protected UnityWebRequest m_webRequest = null;

		public string Name { get; private set; }

		protected BaseWebRequest(string name,string uri,string method)
		{
			Name = name;

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

		public async UniTask<ResponseInfo> SendAsync()
		{
			try
			{
				await m_webRequest.SendWebRequest();

				var result = m_webRequest.result == UnityWebRequest.Result.Success;

				return new ResponseInfo(result,m_webRequest.responseCode,m_webRequest.downloadHandler.text,m_webRequest.error);
			}
			catch(Exception exception)
			{
				LogTag.System.E($"Exception : {exception.Message}");

				return new ResponseInfo(false,c_internal_server_error,string.Empty,exception.Message);
			}
			finally
			{
				Dispose();
			}
		}
	}
}