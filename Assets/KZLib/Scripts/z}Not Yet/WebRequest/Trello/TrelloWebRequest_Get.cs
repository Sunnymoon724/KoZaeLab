using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace KZLib.KZNetwork
{
	public abstract class TrelloGetWebRequest : TrelloWebRequest
	{
		protected TrelloGetWebRequest(string _uri) : base(_uri,UnityWebRequest.kHttpVerbGET) { }
	}

	public class TrelloGetBoardsWebRequest : TrelloGetWebRequest
	{
		public static TrelloGetBoardsWebRequest Create(string _coreKey,Action<List<string>> _onComplete)
		{
			return new TrelloGetBoardsWebRequest(string.Format(@"https://api.trello.com/1/members/me?{0}&boards=all",_coreKey));
			// ,(result,data)=>
			// {
			// 	if(!result)
			// 	{
			// 		_onComplete?.Invoke(null);

			// 		return;
			// 	}

			// 	var trello = JsonConvert.DeserializeObject<TrelloData>(data);
			// 	var dataList = new List<string>();

			// 	foreach(var board in trello.BoardArray)
			// 	{
			// 		if(board.IsClosed)
			// 		{
			// 			continue;
			// 		}

			// 		dataList.Add(JsonConvert.SerializeObject(board));
			// 	}

			// 	_onComplete?.Invoke(dataList.IsNullOrEmpty() ? null : dataList);
			// });
		}

		private TrelloGetBoardsWebRequest(string _uri) : base(_uri) { }
	}

	public class TrelloGetListsWebRequest : TrelloGetWebRequest
	{
		public static TrelloGetListsWebRequest Create(string _coreKey,string _boardId,Action<List<string>> _onComplete)
		{
			return new TrelloGetListsWebRequest(string.Format(@"https://api.trello.com/1/boards/{0}?{1}&lists=all",_boardId,_coreKey));
			// ,(result,data)=>
			// {
			// 	if(!result)
			// 	{
			// 		_onComplete?.Invoke(new List<string>());

			// 		return;
			// 	}

			// 	var board = JsonConvert.DeserializeObject<BoardData>(data);
			// 	var dataList = new List<string>();

			// 	foreach(var list in board.ListArray)
			// 	{
			// 		if(list.IsClosed)
			// 		{
			// 			continue;
			// 		}

			// 		dataList.Add(JsonConvert.SerializeObject(list));
			// 	}

			// 	_onComplete?.Invoke(dataList);
			// });
		}

		private TrelloGetListsWebRequest(string _uri) : base(_uri) { }
	}

	public class TrelloGetCardsWebRequest : TrelloGetWebRequest
	{
		public static TrelloGetCardsWebRequest Create(string _coreKey,string _listId,Action<List<string>> _onComplete)
		{
			return new TrelloGetCardsWebRequest(string.Format(@"https://api.trello.com/1/lists/{0}?{1}&cards=all",_listId,_coreKey));
			// ,(result,data)=>
			// {
			// 	if(!result)
			// 	{
			// 		_onComplete?.Invoke(new List<string>());

			// 		return;
			// 	}

			// 	var list = JsonConvert.DeserializeObject<ListData>(data);
			// 	var dataList = new List<string>();

			// 	foreach(var card in list.CardArray)
			// 	{
			// 		if(card.IsClosed)
			// 		{
			// 			continue;
			// 		}

			// 		dataList.Add(JsonConvert.SerializeObject(card));
			// 	}

			// 	_onComplete?.Invoke(dataList);
			// });
		}

		private TrelloGetCardsWebRequest(string _uri) : base(_uri) { }
	}
}