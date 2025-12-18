using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using KZLib.KZData;
using KZLib.KZUtility;
using UnityEngine;

namespace KZLib.KZNetwork
{
	public partial class WebRequestManager : Singleton<WebRequestManager>
	{
		public void PostBugReportWebRequest(IEnumerable<MessageInfo> messageGroup,byte[] file)
		{
			PostBugReportWebRequestAsync(messageGroup,file).Forget();
		}

		public async UniTask PostBugReportWebRequestAsync(IEnumerable<MessageInfo> messageGroup,byte[] file)
		{
			var serviceCfg = ConfigManager.In.Access<ServiceConfig>();
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