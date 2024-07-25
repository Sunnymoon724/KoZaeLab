using System;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

namespace KZLib.KZNetwork
{
	public record RequestData(bool Result,string Text);

	public abstract class BaseWebRequest : IDisposable
	{
		protected const string REDIRECT_URL = @"http://127.0.0.1";
		protected UnityWebRequest m_WebRequest = null;

		protected BaseWebRequest(string _uri,string _method)
		{
			m_WebRequest = new UnityWebRequest(_uri,_method) { downloadHandler = new DownloadHandlerBuffer() };
		}

		~BaseWebRequest()
		{
			if(m_WebRequest != null)
			{
				m_WebRequest.Dispose();
				m_WebRequest = null;
			}
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

				// //? https://ooz.co.kr/260 에러 체크
				if(m_WebRequest.error.IsEmpty())
				{
					if(m_WebRequest.downloadHandler != null)
					{
						return new RequestData(true,m_WebRequest.downloadHandler.text);
					}
					else
					{
						throw new Exception("DownloadHandler가 null입니다.");
					}
				}
				else
				{
					throw new Exception(string.Format("{0} / {1}",m_WebRequest.error,m_WebRequest.downloadHandler.text));
				}
			}
			catch(Exception _exception)
			{
				LogTag.System.E("{0}",_exception.Message);

				return new RequestData(false,null);
			}
			finally
			{
				m_WebRequest.Dispose();
			}
		}

		public void Dispose()
		{
			m_WebRequest?.Dispose();
		}
	}
}