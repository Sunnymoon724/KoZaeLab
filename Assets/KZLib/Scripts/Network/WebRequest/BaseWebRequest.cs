using System;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using System.Diagnostics;
using System.Text;

#if UNITY_EDITOR

using System.IO;
using KZLib.KZUtility;

#endif

namespace KZLib.KZNetwork
{
	public record ResponseInfo(bool Result,long Code,string Content,string Error);

	public abstract class BaseWebRequest : IDisposable
	{
		private bool m_disposed = false;

		protected const string c_redirectUrl = @"http://127.0.0.1";
		private const int c_internalServerError = 500; // error by server
		protected UnityWebRequest m_webRequest = null;

		private long m_responseTime = 0L;
		protected string m_payloadJson = string.Empty;
		protected string m_contentType = string.Empty;

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
				var stopwatch = Stopwatch.StartNew();

				m_responseTime = 0L;

				await m_webRequest.SendWebRequest();

				stopwatch.Stop();

				m_responseTime = stopwatch.ElapsedMilliseconds;

				var result = m_webRequest.result == UnityWebRequest.Result.Success;

				return _WriteDump(new ResponseInfo(result,m_webRequest.responseCode,m_webRequest.downloadHandler.text,m_webRequest.error));
			}
			catch(Exception exception)
			{
				LogTag.System.E($"Exception : {exception.Message}");

				return _WriteDump(new ResponseInfo(false,c_internalServerError,string.Empty,exception.Message));
			}
			finally
			{
				Dispose();
			}
		}

		private ResponseInfo _WriteDump(ResponseInfo responseInfo)
		{
#if UNITY_EDITOR
			var apiUrl = m_webRequest.uri.AbsolutePath;

			CommonUtility.PrettifyJson(responseInfo.Content);

			var dumpBuilder = new StringBuilder();

			dumpBuilder.Append("================= [Web Request Dump] =================\n\n");
			dumpBuilder.Append($"[TIME]\n{DateTime.Now:yyyy\\/MM\\/dd\\ HH:mm:ss}\n\n");
			dumpBuilder.Append($"[FULL URL]\n{m_webRequest.uri.AbsoluteUri}\n\n");
			dumpBuilder.Append($"[API URL]\n{apiUrl}\n\n");
			dumpBuilder.Append($"[REQUEST]\n");
			dumpBuilder.Append($"[REQUEST METHOD]\n{m_webRequest.method}\n");

			if(m_webRequest.uploadHandler != null)
			{
				if(!m_payloadJson.IsEmpty())
				{
					dumpBuilder.Append($"[REQUEST PAYLOAD]\n{CommonUtility.PrettifyJson(m_payloadJson)}\n");
				}
	
				dumpBuilder.Append($"[REQUEST CONTENT]\n{m_contentType}\n");
			}

			dumpBuilder.Append("\n[RESPONSE]\n");
			dumpBuilder.Append($"[RESPONSE TIME]\n{m_responseTime}\n");
			dumpBuilder.Append($"[RESPONSE RESULT]\n{responseInfo.Result}\n");
			dumpBuilder.Append($"[RESPONSE CODE]\n{responseInfo.Code}\n");
			dumpBuilder.Append($"[RESPONSE CONTENT]\n{CommonUtility.PrettifyJson(responseInfo.Content)}\n");

			if(!responseInfo.Result)
			{
				dumpBuilder.Append($"[RESPONSE ERROR]\n{responseInfo.Error}\n");
			}

			var filePath = Path.Combine(Global.PROJECT_PARENT_PATH,"NetworkDump",$"{apiUrl.Replace("/","#")}.log");

			FileUtility.WriteTextToFile(filePath,dumpBuilder.ToString());
#endif

			return responseInfo;
		}
	}
}