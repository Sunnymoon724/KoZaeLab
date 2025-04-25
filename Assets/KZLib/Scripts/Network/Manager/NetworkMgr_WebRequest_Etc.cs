using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using KZLib.KZUtility;
using UnityEngine;

namespace KZLib
{
	public partial class NetworkMgr : Singleton<NetworkMgr>
	{
		public void PostBugReportWebRequest(IEnumerable<MessageData> messageGroup,byte[] file)
		{
			PostBugReportWebRequestAsync(messageGroup,file).Forget();
		}

		public async UniTask PostBugReportWebRequestAsync(IEnumerable<MessageData> messageGroup,byte[] file)
		{
			var serviceCfg = ConfigMgr.In.Access<ConfigData.ServiceConfig>();
			var postHashSet = new HashSet<string>(serviceCfg.BugReportPostList);
			var taskList = new List<UniTask>();

			if(postHashSet.Contains("Discord"))
			{
				taskList.Add(PostDiscordWebHookAsync("Bug Report",messageGroup,file));
			}

			var stringBuilder = new StringBuilder();

			if(postHashSet.Contains("Trello"))
			{
				var listName = string.Empty;

				foreach(var message in messageGroup)
				{
					stringBuilder.AppendFormat("**{0}**\n{1}\n\n",message.Header,message.Body);
					listName = message.Body;
				}

				listName = listName.Replace("\n","");

				taskList.Add(PostTrelloListInCardAsync("Bug Report",listName,SystemInfo.deviceUniqueIdentifier,stringBuilder.ToString(),file));
			}

			await UniTask.WhenAll(taskList);
		}
	}
}