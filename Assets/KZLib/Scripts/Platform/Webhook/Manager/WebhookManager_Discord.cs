using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using KZLib.Utilities;

namespace KZLib.Webhooks
{
	public partial class WebhookManager : Singleton<WebhookManager>
	{
		/// <summary>
		/// Posts a Discord webhook message using the configured link for the given content key.
		/// </summary>
		public async UniTask PostDiscordWebHookAsync(string content,IEnumerable<MessageInfo> messageGroup = null,byte[] file = null)
		{
			await PostDiscordWebHookAsync(WebhookCfg.GetDiscordLink(content),content,messageGroup,file);
		}

		public void PostDiscordWebHook(string content,IEnumerable<MessageInfo> messageGroup = null,byte[] file = null)
		{
			PostDiscordWebHookAsync(content,messageGroup,file).Forget();
		}

		public void PostDiscordWebHook(string link,string content,IEnumerable<MessageInfo> messageGroup = null,byte[] file = null)
		{
			PostDiscordWebHookAsync(link,content,messageGroup,file).Forget();
		}

		/// <summary>
		/// Posts a Discord webhook message to the given webhook URL.
		/// </summary>
		public async UniTask PostDiscordWebHookAsync(string link,string content,IEnumerable<MessageInfo> messageGroup = null,byte[] file = null)
		{
			if(link.IsEmpty())
			{
				LogChannel.Webhook.E("Link is empty");

				return;
			}

			await _SendWebRequest(PostDiscordWebHookWebRequest.Create(link,content,messageGroup,file));
		}
	}
}