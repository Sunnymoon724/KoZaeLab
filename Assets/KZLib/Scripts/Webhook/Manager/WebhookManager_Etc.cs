using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using KZLib.Utilities;
using UnityEngine;

namespace KZLib.Webs
{
	public partial class WebhookManager : Singleton<WebhookManager>
	{
		public void PostBugReportWebRequest(IEnumerable<MessageInfo> messageGroup,byte[] file)
		{
			PostBugReportWebRequestAsync(messageGroup,file).Forget();
		}

		public async UniTask PostBugReportWebRequestAsync(IEnumerable<MessageInfo> messageInfoGroup,byte[] file)
		{
			var postHashSet = new HashSet<string>(WebhookCfg.BugReportPostList);
			var taskList = new List<UniTask>();

			if(postHashSet.Contains("Discord"))
			{
				taskList.Add(PostDiscordWebHookAsync("Bug Report",messageInfoGroup,file));
			}

			var stringBuilder = new StringBuilder();

			if(postHashSet.Contains("Trello"))
			{
				var listName = string.Empty;

				foreach(var messageInfo in messageInfoGroup)
				{
					stringBuilder.AppendFormat("**{0}**\n{1}\n\n",messageInfo.Header,messageInfo.Body);
					listName = messageInfo.Body;
				}

				listName = listName.Replace("\n","");

				taskList.Add(PostTrelloListInCardAsync("Bug Report",listName,SystemInfo.deviceUniqueIdentifier,stringBuilder.ToString(),file));
			}

			await UniTask.WhenAll(taskList).SuppressCancellationThrow();
		}
	}
}