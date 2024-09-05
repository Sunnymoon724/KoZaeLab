using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using KZLib.KZNetwork;

public static class WebRequestUtility
{
	private const string BUG_REPORT = "Bug Report";
	private const string BUILD_REPORT = "Build Report";

	[Flags]
	private enum SendType
	{
		None = 0,
		Discord = 1 << 0,
		Trello = 1 << 1,
		GoogleSheet = 1 << 2,
		All = -1,
	}

	public static void SendReportOnlyDiscord(string _title,IEnumerable<MessageData> _dataGroup,byte[] _file)
	{
		SendReportAsync(_title,_title,_dataGroup,_file,SendType.Discord).Forget();
	}

	public static async UniTask SendBugReportAsync(IEnumerable<MessageData> _dataGroup,byte[] _file)
	{
		await SendReportAsync(BUG_REPORT,BUG_REPORT,_dataGroup,_file,SendType.Discord | SendType.Trello);
	}

	public static async UniTask SendBuildReportAsync(IEnumerable<MessageData> _dataGroup)
	{
		await SendReportAsync(BUILD_REPORT,BUILD_REPORT,_dataGroup,null,SendType.Discord);
	}

	private static async UniTask SendReportAsync(string _key,string _title,IEnumerable<MessageData> _dataGroup,byte[] _file,SendType _sendType)
	{
		var taskList = new List<UniTask>();

		if(_sendType.HasFlag(SendType.Discord))
		{
			taskList.Add(SendRequestAsync_Discord(_key,_title,_dataGroup,_file));
		}

		if(_sendType.HasFlag(SendType.Trello))
		{
			// var builder = new StringBuilder();

			// foreach(var data in _dataGroup)
			// {
			// 	builder.AppendFormat("**{0}**\n{1}\n\n",data.Head,data.Body);
			// }

			// var listName = _dataGroup.Last().Body.Replace("\n","");

			// taskList.Add(SendRequestAsync_Trello(_key,listName,GameSettings.In.GetPresetOrDeviceID(),builder.ToString(),_file));
		}

		await UniTask.WhenAll(taskList);
	}

	private static async UniTask SendRequestAsync_Discord(string _key,string _title,IEnumerable<MessageData> _dataGroup,byte[] _file)
	{
		var link = NetworkSettings.In.GetDiscordLink(_key);

		//? 디코 사용중이 아님
		if(link.IsEmpty())
		{
			return;
		}

		var request = DiscordPostWebHookWebRequest.Create(link,_title,_dataGroup,_file);

		if(request != null)
		{
			await request.SendAsync();
		}
	}

	#region Trello
	private static async UniTask SendRequestAsync_Trello(string _key,string _listName,string _cardName,string _cardDescription,byte[] _file)
	{
		var id = NetworkSettings.In.GetTrelloBoardId(_key);
		var coreKey = NetworkSettings.In.GetTrelloCoreKey();

		var listId = string.Empty;
		var dataList = new List<string>();
		var listRequest = TrelloGetListsWebRequest.Create(coreKey,id,(result)=> { dataList.AddRange(result); });

		await listRequest.SendAsync();

		foreach(var data in dataList)
		{
			// var item = JObject.Parse(data);
			// var name = item["name"].ToString();

			// if(name.IsEqual(_listName))
			// {
			// 	listId = item["id"].ToString();
			// 	break;
			// }
		}

		if(listId.IsEmpty())
		{
			var postRequest = TrelloPostListWebRequest.Create(coreKey,id,_listName,(result)=> { listId = result; });

			await postRequest.SendAsync();
		}

		var cardId = string.Empty;
		var cardRequest = TrelloPostCardWebRequest.Create(coreKey,listId,_cardName,_cardDescription,(result)=> { cardId = result; });

		await cardRequest.SendAsync();

		// if(_file == null || cardId.IsEmpty())
		// {
		// 	return;
		// }

		// Log.InGame.I("Fin");

		// var attachmentRequest = TrelloPostAttachmentCardWebRequest.Create(_coreKey,cardId,_file);

		// await attachmentRequest.SendAsync();
	}
	#endregion Trello

	#region GoogleSheet
	public static async UniTask<string> PostAppendRow_GoogleSheetAsync(string _fileId,int _sheetOrder,string _text)
	{
		var request = GoogleSheetPostAppendRowWebRequest.Create(_fileId,_sheetOrder,_text);

		var data = await request.SendAsync();

		return data.Result? data.Text : string.Empty;
	}

	public static async UniTask<string> GetSheet_GoogleSheetAsync(string _fileId,int _sheetOrder)
	{
		var request = GoogleSheetGetSheetWebRequest.Create(_fileId,_sheetOrder);

		var data = await request.SendAsync();

		return data.Result? data.Text : string.Empty;
	}
	#endregion GoogleSheet
}