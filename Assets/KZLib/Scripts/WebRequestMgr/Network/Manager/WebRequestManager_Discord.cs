using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using KZLib.Data;
using KZLib.Utilities;

namespace KZLib.Networking
{
	public partial class WebRequestManager : Singleton<WebRequestManager>
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
			var serviceCfg = ConfigManager.In.Access<ServiceConfig>();

			await PostDiscordWebHookAsync(serviceCfg.GetDiscordLink(content),content,messageGroup,file);
		}

		public async UniTask PostDiscordWebHookAsync(string link,string content,IEnumerable<MessageInfo> messageGroup = null,byte[] file = null)
		{
			if(link.IsEmpty())
			{
				LogChannel.Network.E("Link is empty");

				return;
			}

			await _SendWebRequest(PostDiscordWebHookWebRequest.Create(link,content,messageGroup,file));
		}
	}
}