using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Cysharp.Threading.Tasks;
using KZLib.KZUtility;

namespace KZLib.KZNetwork
{
	public partial class WebRequestMgr : Singleton<WebRequestMgr>
	{
		private bool m_disposed = false;

		public event Action<string> OnShowNetworkError = null;

		protected override void Release(bool disposing)
		{
			if(m_disposed)
			{
				return;
			}

			if(disposing)
			{
				OnShowNetworkError = null;
			}

			m_disposed = true;

			base.Release(disposing);
		}

		public void SendNetworkError(string message)
		{
			OnShowNetworkError?.Invoke(message);
		}

		private async UniTask<ResponseInfo> _SendWebRequest(BaseWebRequest webRequest)
		{
			if(webRequest == null)
			{
				LogTag.Network.E("WebRequest is null");

				return null;
			}

#if UNITY_EDITOR
			var dumpInfo = webRequest.dumpInfo;
			var stopwatch = Stopwatch.StartNew();
#endif

			var responseInfo = await webRequest.SendAsync();
#if UNITY_EDITOR
			stopwatch.Stop();

			_WriteDump(responseInfo,dumpInfo,stopwatch.ElapsedMilliseconds);
#endif

			if(responseInfo.Result)
			{
				LogTag.Network.I($"{webRequest.Name} is Success");
			}
			else
			{
				LogTag.Network.E($"{webRequest.Name} is Failed");

				OnShowNetworkError?.Invoke(responseInfo.Error);
			}

			return responseInfo;
		}

#if UNITY_EDITOR
		private void _WriteDump(ResponseInfo responseInfo,DumpInfo dumpInfo,long responseTime)
		{
			var dumpBuilder = new StringBuilder();

			dumpBuilder.Append("================= [Web Request Dump] =================\n\n");
			dumpBuilder.Append($"[TIME]\n{DateTime.Now:yyyy\\/MM\\/dd\\ HH:mm:ss}\n\n");
			dumpBuilder.Append($"[FULL URL]\n{dumpInfo.Uri.AbsoluteUri}\n\n");
			dumpBuilder.Append($"[REQUEST]\n");
			dumpBuilder.Append($"[REQUEST METHOD]\n{dumpInfo.Method}\n\n");

			if(!dumpInfo.ContentType.IsEmpty())
			{
				if(!dumpInfo.Payload.IsEmpty())
				{
					dumpBuilder.Append($"[REQUEST PAYLOAD]\n{CommonUtility.PrettifyJson(dumpInfo.Payload)}\n");
				}
	
				dumpBuilder.Append($"\n[REQUEST CONTENT]\n{dumpInfo.ContentType}\n");
			}

			dumpBuilder.Append("\n[RESPONSE]\n");
			dumpBuilder.Append($"[RESPONSE TIME]\n{responseTime}\n");
			dumpBuilder.Append($"[RESPONSE RESULT]\n{(responseInfo.Result ? "SUCCESS" : "FAILURE")}\n");
			dumpBuilder.Append($"[RESPONSE CODE]\n{responseInfo.Code}\n");

			if(!responseInfo.Content.IsEmpty())
			{
				dumpBuilder.Append($"[RESPONSE CONTENT]\n{CommonUtility.PrettifyJson(responseInfo.Content)}\n");
			}

			if(!responseInfo.Result)
			{
				dumpBuilder.Append($"[RESPONSE ERROR MESSAGE]\n{responseInfo.Error}\n");
			}

			var filePath = Path.Combine(Global.PROJECT_PARENT_PATH,"NetworkDump",$"{dumpInfo.Uri.AbsolutePath.Replace("/","#")}.log");

			FileUtility.WriteTextToFile(filePath,dumpBuilder.ToString());
		}
#endif
	}
}