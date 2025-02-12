using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using KZLib;
using KZLib.KZNetwork;
using Newtonsoft.Json.Linq;
using UnityEngine;

public static partial class CommonUtility
{
	private const string c_bug_report = "Bug Report";

	#region Discord
	public static void PostWebHook_Discord(string content,IEnumerable<MessageData> messageGroup,byte[] file = null)
	{
		PostWebHook_DiscordAsync(content,messageGroup,file).Forget();
	}

	public static void PostWebHook_Discord(string link,string content,IEnumerable<MessageData> messageGroup,byte[] file = null)
	{
		PostWebHook_DiscordAsync(link,content,messageGroup,file).Forget();
	}

	public static async UniTask PostWebHook_DiscordAsync(string content,IEnumerable<MessageData> messageGroup,byte[] file = null)
	{
		var networkConfig = ConfigManager.In.Access<ConfigData.NetworkConfig>();
		var link = networkConfig.GetDiscordLink(content);

		if(link.IsEmpty())
		{
			LogTag.System.E("Link is empty");

			return;
		}

		await PostWebHook_DiscordAsync(link,content,messageGroup,file);
	}

	public static async UniTask PostWebHook_DiscordAsync(string link,string content,IEnumerable<MessageData> messageGroup,byte[] file = null)
	{
		var request = DiscordPostWebHookWebRequest.Create(link,content,messageGroup,file);

		if(request == null)
		{
			LogTag.System.E("Discord post webHook request is null");

			return;
		}

		await request.SendAsync();
	}
	#endregion Discord

	#region Trello
	public static void FindBoard_Trello(Action<List<string>> onAction)
	{
		FindBoard_TrelloAsync().ContinueWith(onAction);
	}

	public static void FindBoard_Trello(string coreKey,Action<List<string>> onAction)
	{
		FindBoard_TrelloAsync(coreKey).ContinueWith(onAction);
	}

	public static async UniTask<List<string>> FindBoard_TrelloAsync()
	{
		var networkConfig = ConfigManager.In.Access<ConfigData.NetworkConfig>();
		var coreKey = networkConfig.TrelloCoreKey;

		if(coreKey.IsEmpty())
		{
			LogTag.System.E("Trello core key is empty");

			return null;
		}

		return await FindBoard_TrelloAsync(coreKey);
	}

	public static async UniTask<List<string>> FindBoard_TrelloAsync(string coreKey)
	{
		var request = TrelloGetBoardWebRequest.Create(coreKey);

		if(request == null)
		{
			LogTag.System.E("Trello get board request is null");

			return null;
		}

		return _ConvertTrelloData(await request.SendAsync(),"boards");
	}

	public static void FindList_Trello(string boardId,Action<List<string>> onAction)
	{
		FindList_TrelloAsync(boardId).ContinueWith(onAction);
	}

	public static void FindList_Trello(string coreKey,string boardId,Action<List<string>> onAction)
	{
		FindList_TrelloAsync(coreKey,boardId).ContinueWith(onAction);
	}

	public static async UniTask<List<string>> FindList_TrelloAsync(string boardId)
	{
		var networkConfig = ConfigManager.In.Access<ConfigData.NetworkConfig>();
		var coreKey = networkConfig.TrelloCoreKey;

		if(coreKey.IsEmpty())
		{
			LogTag.System.E("Trello core key is empty");

			return null;
		}

		return await FindList_TrelloAsync(coreKey,boardId);
	}

	public static async UniTask<List<string>> FindList_TrelloAsync(string coreKey,string boardId)
	{
		if(boardId.IsEmpty())
		{
			LogTag.System.E("Trello board id is empty");

			return null;
		}

		var request = TrelloGetListWebRequest.Create(coreKey,boardId);

		if(request == null)
		{
			LogTag.System.E("Trello get list request is null");

			return null;
		}

		return _ConvertTrelloData(await request.SendAsync(),"lists");
	}

	public static void FindCard_Trello(string listId,Action<List<string>> onAction)
	{
		FindCard_TrelloAsync(listId).ContinueWith(onAction);
	}

	public static void FindCard_Trello(string coreKey,string listId,Action<List<string>> onAction)
	{
		FindCard_TrelloAsync(coreKey,listId).ContinueWith(onAction);
	}

	public static async UniTask<List<string>> FindCard_TrelloAsync(string listId)
	{
		var networkConfig = ConfigManager.In.Access<ConfigData.NetworkConfig>();
		var coreKey = networkConfig.TrelloCoreKey;

		if(coreKey.IsEmpty())
		{
			LogTag.System.E("Trello core key is empty");

			return null;
		}

		return await FindCard_TrelloAsync(coreKey,listId);
	}

	public static async UniTask<List<string>> FindCard_TrelloAsync(string coreKey,string listId)
	{
		if(listId.IsEmpty())
		{
			LogTag.System.E("Trello list id is empty");

			return null;
		}

		var request = TrelloGetCardWebRequest.Create(coreKey,listId);

		if(request == null)
		{
			LogTag.System.E("Trello get card request is null");

			return null;
		}

		return _ConvertTrelloData(await request.SendAsync(),"cards");
	}

	private static List<string> _ConvertTrelloData(RequestData requestData,string key)
	{
		if(requestData.Result)
		{
			try
			{
				var trello = JObject.Parse(requestData.Text);
				var dataList = new List<string>();

				foreach(var token in trello[key])
				{
					if((bool) token["closed"])
					{
						continue;
					}

					dataList.Add(token.ToString());
				}

				return dataList;
			}
			catch(Exception exception)
			{
				LogTag.System.E($"Convert is failed - {exception}");
			}
		}
		else
		{
			LogTag.System.E("Result is failed");
		}

		return null;
	}

	public static void PostList_Trello(string boardId,string name)
	{
		PostList_TrelloAsync(boardId,name).Forget();
	}

	public static void PostList_Trello(string coreKey,string boardId,string name)
	{
		PostList_TrelloAsync(coreKey,boardId,name).Forget();
	}

	public static async UniTask PostList_TrelloAsync(string boardId,string name)
	{
		var networkConfig = ConfigManager.In.Access<ConfigData.NetworkConfig>();
		var coreKey = networkConfig.TrelloCoreKey;

		if(coreKey.IsEmpty())
		{
			LogTag.System.E("Trello core key is empty");

			return;
		}

		await PostList_TrelloAsync(coreKey,boardId,name);
	}

	public static async UniTask PostList_TrelloAsync(string coreKey,string boardId,string name)
	{
		var request = TrelloPostListWebRequest.Create(coreKey,boardId,name);

		if(request == null)
		{
			LogTag.System.E("Trello post list request is null");

			return;
		}

		await request.SendAsync();
	}

	public static void PostCard_Trello(string listId,string name,string description,byte[] file = null)
	{
		PostCard_TrelloAsync(listId,name,description,file).Forget();
	}

	public static void PostCard_Trello(string coreKey,string listId,string name,string description,byte[] file = null)
	{
		PostCard_TrelloAsync(coreKey,listId,name,description,file).Forget();
	}

	public static async UniTask PostCard_TrelloAsync(string listId,string name,string description,byte[] file = null)
	{
		var networkConfig = ConfigManager.In.Access<ConfigData.NetworkConfig>();
		var coreKey = networkConfig.TrelloCoreKey;

		if(coreKey.IsEmpty())
		{
			LogTag.System.E("Trello core key is empty");

			return;
		}

		await PostCard_TrelloAsync(coreKey,listId,name,description,file);
	}

	public static async UniTask PostCard_TrelloAsync(string coreKey,string listId,string name,string description,byte[] file = null)
	{
		var cardRequest = TrelloPostCardWebRequest.Create(coreKey,listId,name,description);

		if(cardRequest == null)
		{
			LogTag.System.E("Post card request is null");

			return;
		}

		var requestData = await cardRequest.SendAsync();

		if(file == null)
		{
			return;
		}

		var cardId = JObject.Parse(requestData.Text)["id"].ToString();

		if(cardId.IsEmpty())
		{
			LogTag.System.E("Card id is empty");

			return;
		}

		var fileRequest = TrelloPostAttachmentCardWebRequest.Create(coreKey,cardId,file);

		if(fileRequest == null)
		{
			LogTag.System.E("Post attachment card request is null");

			return;
		}

		await fileRequest.SendAsync();
	}

	public static void PostListInCard_Trello(string boardName,string listName,string cardName,string cardDescription,byte[] file = null)
	{
		PostListInCard_TrelloAsync(boardName,listName,cardName,cardDescription,file).Forget();
	}

	public static void PostListInCard_Trello(string coreKey,string boardName,string listName,string cardName,string cardDescription,byte[] file = null)
	{
		PostListInCard_TrelloAsync(coreKey,boardName,listName,cardName,cardDescription,file).Forget();
	}

	public static async UniTask PostListInCard_TrelloAsync(string boardName,string listName,string cardName,string cardDescription,byte[] file = null)
	{
		var networkConfig = ConfigManager.In.Access<ConfigData.NetworkConfig>();
		var coreKey = networkConfig.TrelloCoreKey;

		if(coreKey.IsEmpty())
		{
			LogTag.System.E("Trello core key is empty");

			return;
		}

		await PostListInCard_TrelloAsync(coreKey,boardName,listName,cardName,cardDescription,file);
	}

	public static async UniTask PostListInCard_TrelloAsync(string coreKey,string boardName,string listName,string cardName,string cardDescription,byte[] file = null)
	{
		var boardDataList = await FindBoard_TrelloAsync(coreKey);

		if(boardDataList.IsNullOrEmpty())
		{
			LogTag.System.E("Trello board is empty");

			return;
		}

		var boardId = _FindId(boardDataList,boardName);

		if(boardId.IsEmpty())
		{
			LogTag.System.E("Trello board id is null");

			return;
		}

		var listDataList = await FindList_TrelloAsync(boardId);

		if(listDataList.IsNullOrEmpty())
		{
			LogTag.System.E("Trello list is empty");

			return;
		}

		var listId = _FindId(listDataList,listName);

		if(listId.IsEmpty())
		{
			await PostList_TrelloAsync(boardId,listName);
		}

		await PostCard_TrelloAsync(listId,cardName,cardDescription,file);
	}

	private static string _FindId(List<string> dataList,string name)
	{
		try
		{
			foreach(var data in dataList)
			{
				var jObject = JObject.Parse(data);

				var dataName = jObject["name"].ToString();

				if(name.IsEqual(dataName))
				{
					return jObject["id"].ToString();
				}
			}
		}
		catch(Exception exception)
		{
			LogTag.System.E($"Convert is failed - {exception}");
		}

		return null;
	}
	#endregion Trello

	#region GoogleSheet
	public static void PostAddRow_GoogleSheet(string sheetName,int sheetOrder,string text)
	{
		PostAddRow_GoogleSheetAsync(sheetName,sheetOrder,text).Forget();
	}

	public static async UniTask PostAddRow_GoogleSheetAsync(string sheetName,int sheetOrder,string content)
	{
		if(!TryGetSheetId(sheetName,out var sheetId))
		{
			return;
		}

		var request = GoogleSheetPostAddRowWebRequest.Create(sheetId,sheetOrder,content);

		if(request == null)
		{
			LogTag.System.E("GoogleSheet post addRow request is null");

			return;
		}

		await request.SendAsync();
	}

	public static void FindSheet_GoogleSheet(string sheetName,int sheetOrder,Action<string> onAction)
	{
		FindSheet_GoogleSheetAsync(sheetName,sheetOrder).ContinueWith(onAction);
	}

	public static async UniTask<string> FindSheet_GoogleSheetAsync(string sheetName,int sheetOrder)
	{
		if(!TryGetSheetId(sheetName,out var sheetId))
		{
			return null;
		}

		var request = GoogleSheetGetSheetWebRequest.Create(sheetId,sheetOrder);

		if(request == null)
		{
			LogTag.System.E("GoogleSheet get sheet request is null");

			return null;
		}

		var requestData = await request.SendAsync();

		return requestData.Result ? requestData.Text : string.Empty;
	}

	private static bool TryGetSheetId(string sheetName,out string sheetId)
	{
		var networkConfig = ConfigManager.In.Access<ConfigData.NetworkConfig>();
		sheetId = networkConfig.GetGoogleSheetFileId(sheetName);

		var isEmpty = sheetId.IsEmpty();

		if(isEmpty)
		{
			LogTag.System.E("Sheet id is empty");
		}

		return !isEmpty;
	}
	#endregion GoogleSheet

	#region GoogleDrive
	public static void PostFile_GoogleDrive(string folderName,string fileName,byte[] file,string mimeType)
	{
		PostFile_GoogleDriveAsync(folderName,fileName,file,mimeType).Forget();
	}

	public static async UniTask PostFile_GoogleDriveAsync(string folderName,string fileName,byte[] file,string mimeType)
	{
		if(!TryGetFolderId(folderName,out var folderId))
		{
			return;
		}

		var request = GoogleDrivePostFileWebRequest.Create(folderId,fileName,file,mimeType);

		if(request == null)
		{
			LogTag.System.E("GoogleDrive post file request is null");

			return;
		}

		await request.SendAsync();
	}

	public static void FindEntry_GoogleDrive(string folderName,Action<List<string>> onAction)
	{
		FindEntry_GoogleDriveAsync(folderName).ContinueWith(onAction);
	}

	public static async UniTask<List<string>> FindEntry_GoogleDriveAsync(string folderName)
	{
		if(!TryGetFolderId(folderName,out var folderId))
		{
			return null;
		}

		var request = GoogleDriveGetEntryWebRequest.Create(folderId);

		if(request == null)
		{
			LogTag.System.E("GoogleDrive get entry request is null");

			return null;
		}

		var requestData = await request.SendAsync();

		if(requestData.Result)
		{
			try
			{
				var jArray = JArray.Parse(requestData.Text);
				var dataList = new List<string>();

				foreach(var token in jArray)
				{
					dataList.Add(token.ToString());
				}

				return dataList;
			}
			catch(Exception exception)
			{
				LogTag.System.E($"Convert is failed - {exception}");
			}
		}
		else
		{
			LogTag.System.E("Result is failed");
		}

		return null;
	}

	private static bool TryGetFolderId(string folderName,out string folderId)
	{
		var networkConfig = ConfigManager.In.Access<ConfigData.NetworkConfig>();
		folderId = networkConfig.GetGoogleDriveFolderId(folderName);

		var isEmpty = folderId.IsEmpty();

		if(isEmpty)
		{
			LogTag.System.E("Folder id is empty");
		}

		return !isEmpty;
	}
	#endregion GoogleDrive

	public static void PostBugReportWebRequest(IEnumerable<MessageData> messageGroup,byte[] file)
	{
		PostBugReportWebRequestAsync(messageGroup,file).Forget();
	}

	public static async UniTask PostBugReportWebRequestAsync(IEnumerable<MessageData> messageGroup,byte[] file)
	{
		var networkConfig = ConfigManager.In.Access<ConfigData.NetworkConfig>();
		var postHashSet = new HashSet<string>(networkConfig.BugReportPostList);
		var taskList = new List<UniTask>();

		if(postHashSet.Contains("Discord"))
		{
			taskList.Add(PostWebHook_DiscordAsync(c_bug_report,messageGroup,file));
		}

		var stringBuilder = new StringBuilder();

		if(postHashSet.Contains("Trello"))
		{
			foreach(var message in messageGroup)
			{
				stringBuilder.AppendFormat("**{0}**\n{1}\n\n",message.Header,message.Body);
			}

			var listName = messageGroup.Last().Body.Replace("\n","");

			taskList.Add(PostListInCard_TrelloAsync(c_bug_report,listName,SystemInfo.deviceUniqueIdentifier,stringBuilder.ToString(),file));
		}

		await UniTask.WhenAll(taskList);
	}
}