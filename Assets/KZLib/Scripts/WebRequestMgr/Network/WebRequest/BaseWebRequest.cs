using System;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using System.Text;

namespace KZLib.KZNetwork
{
	public record ResponseInfo(bool Result,long Code,string Content,string Error);
	public record DumpInfo(Uri Uri,string Method,string Payload,string ContentType);

	public abstract class BaseWebRequest : IDisposable
	{
		private bool m_disposed = false;

		protected const string c_redirectUrl = @"http://127.0.0.1";
		private const int c_internalServerError = 500; // error by server
		protected UnityWebRequest m_webRequest = null;

		protected string m_payloadJson = string.Empty;
		protected string m_contentType = string.Empty;

		public string Name { get; private set; }

		public Uri Uri => m_webRequest?.uri;
		public DumpInfo dumpInfo => m_webRequest == null ? null : new DumpInfo(m_webRequest.uri,m_webRequest.method,m_payloadJson,m_contentType);

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

		protected void _CreateUploadHandler(string payloadJson,string contentType)
		{
			m_payloadJson = payloadJson;

			_CreateUploadHandler(Encoding.UTF8.GetBytes(payloadJson),contentType);
		}

		protected void _CreateUploadHandler(byte[] rawData,string contentType)
		{
			m_contentType = contentType;

			m_webRequest.uploadHandler = new UploadHandlerRaw(rawData) { contentType = contentType, };
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
				LogSvc.System.E($"Exception : {exception.Message}");

				return new ResponseInfo(false,c_internalServerError,string.Empty,exception.Message);
			}
			finally
			{
				Dispose();
			}
		}
	}
}