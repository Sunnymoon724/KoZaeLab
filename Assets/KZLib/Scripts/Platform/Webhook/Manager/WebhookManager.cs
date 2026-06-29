using System.Diagnostics;
using Cysharp.Threading.Tasks;
using KZLib.Data;
using KZLib.Utilities;
using R3;

namespace KZLib.Webhooks
{
	/// <summary>
	/// Sends outbound webhook requests to Discord, Google Apps Script, and Trello.
	/// Centralizes HTTP dispatch, editor dump logging, and network error notification.
	/// </summary>
	public partial class WebhookManager : Singleton<WebhookManager>
	{
		private WebhookManager() { }

		private readonly Subject<string> m_networkErrorSubject = new();
		public Observable<string> OnShownNetworkError => m_networkErrorSubject;

		private WebhookConfig WebhookCfg => ConfigManager.In.Fetch<WebhookConfig>();

		protected override void _Release(bool disposing)
		{
			if(disposing)
			{
				m_networkErrorSubject.Dispose();
			}

			base._Release(disposing);
		}

		/// <summary>
		/// Publishes a network error message to observers such as UI overlays.
		/// </summary>
		public void SendNetworkError(string message)
		{
			m_networkErrorSubject.OnNext(message);
		}

		private async UniTask<ResponseInfo> _SendWebRequest(BaseWebRequest webRequest)
		{
			if(webRequest == null)
			{
				LogChannel.Webhook.E("WebRequest is null");

				return null;
			}

#if UNITY_EDITOR
			var dumpInfo = webRequest.dumpInfo;
			var stopwatch = Stopwatch.StartNew();
#endif

			var responseInfo = await webRequest.SendAsync();
#if UNITY_EDITOR
			stopwatch.Stop();

			KZDumpKit.WriteHttpDump($"{dumpInfo.Uri.AbsolutePath.Replace("/","#")}.log",stopwatch.ElapsedMilliseconds,responseInfo.Result,responseInfo.Code,responseInfo.Content,responseInfo.Error,dumpInfo.Uri.AbsoluteUri,dumpInfo.Method,dumpInfo.Payload,dumpInfo.ContentType);
#endif

			if(responseInfo.Result)
			{
				LogChannel.Webhook.I($"{webRequest.Name} is Success");
			}
			else
			{
				LogChannel.Webhook.E($"{webRequest.Name} is Failed");

				m_networkErrorSubject.OnNext(responseInfo.Error);
			}

			return responseInfo;
		}
	}
}