using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using KZLib.KZNetwork;
using Newtonsoft.Json.Linq;

public static partial class CommonUtility
{
	private const string BUG_REPORT = "Bug Report";

	
	[Flags]
	private enum PostType
	{
		None = 0,
		Discord = 1 << 0,
		Trello = 1 << 1,
		GoogleSheet = 1 << 2,
		All = -1,
	}

	#region Discord
	public static void PostWebHook_Discord(string _title,IEnumerable<MessageData> _dataGroup,byte[] _file = null)
	{
		PostWebHook_DiscordAsync(_title,_dataGroup,_file).Forget();
	}

	public static async UniTask PostWebHook_DiscordAsync(string _title,IEnumerable<MessageData> _dataGroup,byte[] _file = null)
	{
		var link = NetworkSettings.In.GetDiscordLink(_title);

		if(link.IsEmpty())
		{
			LogTag.System.E("Link is empty");

			return;
		}

		var request = DiscordPostWebHookWebRequest.Create(link,_title,_dataGroup,_file);

		if(request != null)
		{
			await request.SendAsync();
		}
	}
	#endregion Discord

	#region Trello
	public static async UniTask<List<string>> FindBoard_TrelloAsync()
	{
		var coreKey = NetworkSettings.In.TrelloCoreKey;

		if(coreKey.IsEmpty())
		{
			return null;
		}

		var request = TrelloGetBoardsWebRequest.Create(coreKey);

		if(request == null)
		{
			return null;
		}

		var data = await request.SendAsync();

		if(!data.Result)
		{
			return null;
		}

		var trello = JObject.Parse(data.Text);
		var dataList = new List<string>();

		foreach(var board in trello["boards"])
		{
			if((bool) board["closed"])
			{
				continue;
			}

			dataList.Add(board.ToString());
		}

		return dataList;
	}

	public static async UniTask<List<string>> FindList_TrelloAsync(string _boardId)
	{
		var coreKey = NetworkSettings.In.TrelloCoreKey;

		if(coreKey.IsEmpty())
		{
			return null;
		}

		var request = TrelloGetListsWebRequest.Create(coreKey,_boardId);

		if(request == null)
		{
			return null;
		}

		var data = await request.SendAsync();

		if(!data.Result)
		{
			return null;
		}

		var trello = JObject.Parse(data.Text);
		var dataList = new List<string>();

		foreach(var list in trello["lists"])
		{
			if((bool) list["closed"])
			{
				continue;
			}

			dataList.Add(list.ToString());
		}

		return dataList;
	}


	public static async UniTask<List<string>> FindCard_TrelloAsync(string _listId)
	{
		var coreKey = NetworkSettings.In.TrelloCoreKey;

		if(coreKey.IsEmpty())
		{
			return null;
		}

		var request = TrelloGetCardsWebRequest.Create(coreKey,_listId);

		if(request == null)
		{
			return null;
		}

		var data = await request.SendAsync();

		if(!data.Result)
		{
			return null;
		}

		var trello = JObject.Parse(data.Text);
		var dataList = new List<string>();

		foreach(var card in trello["cards"])
		{
			if((bool) card["closed"])
			{
				continue;
			}

			dataList.Add(card.ToString());
		}

		return dataList;
	}

	public static void PostList_Trello(string _boardId,string _name)
	{
		PostList_TrelloAsync(_boardId,_name).Forget();
	}

	public static async UniTask PostList_TrelloAsync(string _boardId,string _name)
	{
		var coreKey = NetworkSettings.In.TrelloCoreKey;

		if(coreKey.IsEmpty())
		{
			return;
		}

		var request = TrelloPostListWebRequest.Create(coreKey,_boardId,_name);

		if(request != null)
		{
			await request.SendAsync();
		}
	}

	public static void PostCard_Trello(string _listId,string _name,string _description,byte[] _file = null)
	{
		PostCard_TrelloAsync(_listId,_name,_description,_file).Forget();
	}

	public static async UniTask PostCard_TrelloAsync(string _listId,string _name,string _description,byte[] _file = null)
	{
		var coreKey = NetworkSettings.In.TrelloCoreKey;

		if(coreKey.IsEmpty())
		{
			return;
		}

		var request1 = TrelloPostCardWebRequest.Create(coreKey,_listId,_name,_description);

		if(request1 == null)
		{
			return;
		}

		var data = await request1.SendAsync();

		if(_file == null)
		{
			return;
		}

		var cardId = JObject.Parse(data.Text)["id"].ToString();

		if(cardId.IsEmpty())
		{
			return;
		}

		var request2 = TrelloPostAttachmentCardWebRequest.Create(coreKey,cardId,_file);

		if(request2 != null)
		{
			await request2.SendAsync();
		}
	}

	public static void PostListInCard_Trello(string _boardName,string _listName,string _cardName,string _cardDescription,byte[] _file = null)
	{
		PostListInCard_TrelloAsync(_boardName,_listName,_cardName,_cardDescription,_file).Forget();
	}

	public static async UniTask PostListInCard_TrelloAsync(string _boardName,string _listName,string _cardName,string _cardDescription,byte[] _file = null)
	{
		var boardId = NetworkSettings.In.GetTrelloBoardId(_boardName);
		var coreKey = NetworkSettings.In.TrelloCoreKey;

		if(boardId.IsEmpty() || coreKey.IsEmpty())
		{
			return;
		}

		var dataList = await FindList_TrelloAsync(boardId);

		if(dataList.IsNullOrEmpty())
		{
			return;
		}

		var listId = string.Empty;

		foreach(var data in dataList)
		{
			var list = JObject.Parse(data);

			var name = list["name"].ToString();

			if(name.IsEqual(_listName))
			{
				listId = list["id"].ToString();

				break;
			}
		}

		if(listId.IsEmpty())
		{
			await PostList_TrelloAsync(boardId,_listName);
		}

		await PostCard_TrelloAsync(listId,_cardName,_cardDescription,_file);
	}
	#endregion Trello

	#region GoogleSheet
	public static void PostAddRow_GoogleSheet(string _fileName,int _sheetOrder,string _text)
	{
		PostAddRow_GoogleSheetAsync(_fileName,_sheetOrder,_text).Forget();
	}

	public static async UniTask PostAddRow_GoogleSheetAsync(string _fileName,int _sheetOrder,string _text)
	{
		var fileId = NetworkSettings.In.GetGoogleSheetFileId(_fileName);

		if(fileId.IsEmpty())
		{
			return;
		}

		var request = GoogleSheetPostAddRowWebRequest.Create(fileId,_sheetOrder,_text);

		if(request != null)
		{
			await request.SendAsync();
		}
	}

	public static async UniTask<string> FindSheet_GoogleSheetAsync(string _fileName,int _sheetOrder)
	{
		var fileId = NetworkSettings.In.GetGoogleSheetFileId(_fileName);

		if(fileId.IsEmpty())
		{
			return null;
		}

		var request = GoogleSheetGetSheetWebRequest.Create(fileId,_sheetOrder);
		var data = await request.SendAsync();

		return data.Result ? data.Text : string.Empty;
	}
	#endregion GoogleSheet

	#region GoogleDrive
	public static void PostFile_GoogleDrive(string _folderName,string _fileName,byte[] _file,string _mimeType)
	{
		PostFile_GoogleDriveAsync(_folderName,_fileName,_file,_mimeType).Forget();
	}

	public static async UniTask PostFile_GoogleDriveAsync(string _folderName,string _fileName,byte[] _file,string _mimeType)
	{
		var folderId = NetworkSettings.In.GetGoogleDriveFolderId(_folderName);

		if(folderId.IsEmpty())
		{
			return;
		}

		var request = GoogleDrivePostFileWebRequest.Create(folderId,_fileName,_file,_mimeType);

		if(request != null)
		{
			await request.SendAsync();
		}
	}

	public static async UniTask<List<string>> FindEntry_GoogleDriveAsync(string _folderName)
	{
		var folderId = NetworkSettings.In.GetGoogleDriveFolderId(_folderName);

		if(folderId.IsEmpty())
		{
			return null;
		}

		var request = GoogleDriveGetEntryWebRequest.Create(folderId);

		if(request == null)
		{
			return null;
		}

		var data = await request.SendAsync();

		if(!data.Result)
		{
			return null;
		}

		var drive = JArray.Parse(data.Text);
		var dataList = new List<string>();

		foreach(var info in drive)
		{
			dataList.Add(info.ToString());
		}

		return dataList;
	}
	#endregion GoogleDrive

	public static void PostBugReportWebRequest(IEnumerable<MessageData> _dataGroup,byte[] _file)
	{
		PostBugReportWebRequestAsync(_dataGroup,_file).Forget();
	}

	public static async UniTask PostBugReportWebRequestAsync(IEnumerable<MessageData> _dataGroup,byte[] _file)
	{
		var taskList = new List<UniTask>
		{
			PostWebHook_DiscordAsync(BUG_REPORT,_dataGroup,_file),
		};

		var builder = new StringBuilder();

		foreach(var data in _dataGroup)
		{
			builder.AppendFormat("**{0}**\n{1}\n\n",data.Header,data.Body);
		}

		var listName = _dataGroup.Last().Body.Replace("\n","");

		taskList.Add(PostListInCard_TrelloAsync(BUG_REPORT,listName,GameSettings.In.PresetNameOrDeviceId,builder.ToString(),_file));

		await UniTask.WhenAll(taskList);
	}
}