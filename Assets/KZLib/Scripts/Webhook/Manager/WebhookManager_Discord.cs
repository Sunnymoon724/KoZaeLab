using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using KZLib.Utilities;

namespace KZLib.Webs
{
	public partial class WebhookManager : Singleton<WebhookManager>
	{
		public void PostDiscordWebHook(string content,IEnumerable<MessageInfo> messageGroup = null,byte[] file = null)
		{
			PostDiscordWebHookAsync(content,messageGroup,file).Forget();
		}

		public void PostDiscordWebHook(string link,string content,IEnumerable<MessageInfo> messageGroup = null,byte[] file = null)
		{
			PostDiscordWebHookAsync(link,content,messageGroup,file).Forget();
		}

		public async UniTask PostDiscordWebHookAsync(string content,IEnumerable<MessageInfo> messageGroup = null,byte[] file = null)
		{
			await PostDiscordWebHookAsync(WebhookCfg.GetDiscordLink(content),content,messageGroup,file);
		}

		public async UniTask PostDiscordWebHookAsync(string link,string content,IEnumerable<MessageInfo> messageGroup = null,byte[] file = null)
		{
			if(link.IsEmpty())
			{
				LogChannel.Web.E("Link is empty");

				return;
			}

			await _SendWebRequest(PostDiscordWebHookWebRequest.Create(link,content,messageGroup,file));
		}
	}
}