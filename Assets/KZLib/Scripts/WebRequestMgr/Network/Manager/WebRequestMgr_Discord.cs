using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using KZLib.KZData;
using KZLib.KZUtility;

namespace KZLib.KZNetwork
{
	public partial class WebRequestMgr : Singleton<WebRequestMgr>
	{
		public void PostDiscordWebHook(string content,IEnumerable<MessageData> messageGroup = null,byte[] file = null)
		{
			PostDiscordWebHookAsync(content,messageGroup,file).Forget();
		}

		public void PostDiscordWebHook(string link,string content,IEnumerable<MessageData> messageGroup = null,byte[] file = null)
		{
			PostDiscordWebHookAsync(link,content,messageGroup,file).Forget();
		}

		public async UniTask PostDiscordWebHookAsync(string content,IEnumerable<MessageData> messageGroup = null,byte[] file = null)
		{
			var serviceCfg = ConfigMgr.In.Access<ConfigData.ServiceConfig>();

			await PostDiscordWebHookAsync(serviceCfg.GetDiscordLink(content),content,messageGroup,file);
		}

		public async UniTask PostDiscordWebHookAsync(string link,string content,IEnumerable<MessageData> messageGroup = null,byte[] file = null)
		{
			if(link.IsEmpty())
			{
				KZLogType.Network.E("Link is empty");

				return;
			}

			await _SendWebRequest(PostDiscordWebHookWebRequest.Create(link,content,messageGroup,file));
		}
	}
}