using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using KZLib.Utilities;
using Newtonsoft.Json.Linq;

namespace KZLib.Webs
{
	public partial class WebhookManager : Singleton<WebhookManager>
	{
		private const string c_trelloBoard = "boards";
		private const string c_trelloClosed = "closed";
		private const string c_trelloId = "id";
		private const string c_trelloName = "name";
		
		public void GetTrelloBoard(Action<List<string>> onAction)
		{
			GetTrelloBoardAsync().ContinueWith(onAction).Forget();
		}

		public void GetTrelloBoard(string trelloKey,Action<List<string>> onAction)
		{
			GetTrelloBoardAsync(trelloKey).ContinueWith(onAction).Forget();
		}

		public async UniTask<List<string>> GetTrelloBoardAsync()
		{
			return await GetTrelloBoardAsync(WebhookCfg.TrelloKey);
		}

		public async UniTask<List<string>> GetTrelloBoardAsync(string trelloKey)
		{
			if(!_IsValidTrelloKey(trelloKey))
			{
				return null;
			}

			var info = await _SendWebRequest(GetTrelloBoardWebRequest.Create(trelloKey));

			return _ParseTrelloData(info,c_trelloBoard);
		}

		public void GetTrelloList(string boardId,Action<List<string>> onAction)
		{
			GetTrelloListAsync(boardId).ContinueWith(onAction).Forget();
		}

		public void GetTrelloList(string coreKey,string boardId,Action<List<string>> onAction)
		{
			GetTrelloListAsync(coreKey,boardId).ContinueWith(onAction).Forget();
		}

		public async UniTask<List<string>> GetTrelloListAsync(string boardId)
		{
			return await GetTrelloListAsync(WebhookCfg.TrelloKey,boardId);
		}

		public async UniTask<List<string>> GetTrelloListAsync(string trelloKey,string boardId)
		{
			if(!_IsValidTrelloKey(trelloKey))
			{
				return null;
			}

			if(!_IsValidTrelloBoardId(boardId))
			{
				return null;
			}

			var info = await _SendWebRequest(GetTrelloListWebRequest.Create(trelloKey,boardId));

			return _ParseTrelloData(info,c_trelloBoard);
		}

		public void GetTrelloCard(string listId,Action<List<string>> onAction)
		{
			GetTrelloCardAsync(listId).ContinueWith(onAction).Forget();
		}

		public void GetTrelloCard(string trelloKey,string listId,Action<List<string>> onAction)
		{
			GetTrelloCardAsync(trelloKey,listId).ContinueWith(onAction).Forget();
		}

		public async UniTask<List<string>> GetTrelloCardAsync(string listId)
		{
			return await GetTrelloCardAsync(WebhookCfg.TrelloKey,listId);
		}

		public async UniTask<List<string>> GetTrelloCardAsync(string trelloKey,string listId)
		{
			if(!_IsValidTrelloKey(trelloKey))
			{
				return null;
			}

			if(!_IsValidTrelloListId(listId))
			{
				return null;
			}

			var info = await _SendWebRequest(GetTrelloCardWebRequest.Create(trelloKey,listId));

			return _ParseTrelloData(info,c_trelloBoard);
		}

		private static List<string> _ParseTrelloData(ResponseInfo responseInfo,string key)
		{
			if(responseInfo.Result)
			{
				try
				{
					var json = JObject.Parse(responseInfo.Content);
					var tokenList = new List<string>();

					if(json.TryGetValue(key,out var tokenArray))
					{
						foreach(var token in tokenArray)
						{
							var isClosed = token.Value<bool>(c_trelloClosed);

							if(isClosed)
							{
								continue;
							}

							tokenList.Add(token.ToString());
						}
					}

					return tokenList;
				}
				catch(Exception exception)
				{
					LogChannel.Web.E($"Parse is failed - {exception}");
				}
			}
			else
			{
				LogChannel.Web.E("Result is failed");
			}

			return null;
		}

		public void PostTrelloList(string boardId,string name)
		{
			PostTrelloListAsync(boardId,name).Forget();
		}

		public void PostTrelloList(string trelloKey,string boardId,string name)
		{
			PostTrelloListAsync(trelloKey,boardId,name).Forget();
		}

		public async UniTask PostTrelloListAsync(string boardId,string name)
		{
			await PostTrelloListAsync(WebhookCfg.TrelloKey,boardId,name);
		}

		public async UniTask PostTrelloListAsync(string trelloKey,string boardId,string name)
		{
			if(!_IsValidTrelloKey(trelloKey))
			{
				return;
			}

			await _SendWebRequest(PostTrelloListWebRequest.Create(trelloKey,boardId,name));
		}

		public void PostTrelloCard(string listId,string name,string description,byte[] file = null)
		{
			PostTrelloCardAsync(listId,name,description,file).Forget();
		}

		public void PostTrelloCard(string coreKey,string listId,string name,string description,byte[] file = null)
		{
			PostTrelloCardAsync(coreKey,listId,name,description,file).Forget();
		}

		public async UniTask PostTrelloCardAsync(string listId,string name,string description,byte[] file = null)
		{
			await PostTrelloCardAsync(WebhookCfg.TrelloKey,listId,name,description,file);
		}

		public async UniTask PostTrelloCardAsync(string trelloKey,string listId,string name,string description,byte[] file = null)
		{
			if(!_IsValidTrelloKey(trelloKey))
			{
				return;
			}

			var cardInfo = await _SendWebRequest(PostTrelloCardWebRequest.Create(trelloKey,listId,name,description));

			if(file == null)
			{
				return;
			}

			var json = JObject.Parse(cardInfo.Content);

			if(!json.TryGetValue(c_trelloId,out var value))
			{
				return;
			}

			var cardId = value.ToString();

			if(!_IsValidTrelloCardId(cardId))
			{
				return;
			}

			await _SendWebRequest(PostTrelloAttachmentCardWebRequest.Create(trelloKey,cardId,file));
		}

		public void PostTrelloListInCard(string boardName,string listName,string cardName,string cardDescription,byte[] file = null)
		{
			PostTrelloListInCardAsync(boardName,listName,cardName,cardDescription,file).Forget();
		}

		public void PostTrelloListInCard(string trelloKey,string boardName,string listName,string cardName,string cardDescription,byte[] file = null)
		{
			PostTrelloListInCardAsync(trelloKey,boardName,listName,cardName,cardDescription,file).Forget();
		}

		public async UniTask PostTrelloListInCardAsync(string boardName,string listName,string cardName,string cardDescription,byte[] file = null)
		{
			await PostTrelloListInCardAsync(WebhookCfg.TrelloKey,boardName,listName,cardName,cardDescription,file);
		}

		public async UniTask PostTrelloListInCardAsync(string trelloKey,string boardName,string listName,string cardName,string cardDescription,byte[] file = null)
		{
			if(!_IsValidTrelloKey(trelloKey))
			{
				return;
			}

			var boardList = await GetTrelloBoardAsync(trelloKey);

			if(!_IsExistTrelloBoardList(boardList))
			{
				return;
			}

			var boardId = _FindId(boardList,boardName);

			if(!_IsValidTrelloBoardId(boardId))
			{
				return;
			}

			var listList = await GetTrelloListAsync(boardId);

			if(!_IsValidTrelloListId(boardId))
			{
				return;
			}

			var listId = _FindId(listList,listName);

			if(listId.IsEmpty())
			{
				await PostTrelloListAsync(boardId,listName);
			}

			await PostTrelloCardAsync(listId,cardName,cardDescription,file);
		}

		private string _FindId(List<string> dataList,string name)
		{
			try
			{
				for(var i=0;i<dataList.Count;i++)
				{
					var json = JObject.Parse(dataList[i]);

					if(!json.TryGetValue(c_trelloName,out var value))
					{
						continue;
					}

					var dataName = value.ToString();

					if(name.IsEqual(dataName) && json.TryGetValue(c_trelloId,out var trelloId))
					{
						return trelloId.ToString();
					}
				}
			}
			catch(Exception exception)
			{
				LogChannel.Web.E($"Convert is failed - {exception}");
			}

			return null;
		}

		private bool _IsValidTrelloKey(string trelloKey)
		{
			if(trelloKey.IsEmpty())
			{
				LogChannel.Web.E("TrelloKey is empty");

				return false;
			}

			return true;
		}

		private bool _IsValidTrelloBoardId(string boardId)
		{
			if(boardId.IsEmpty())
			{
				LogChannel.Web.E("Trello boardId is empty");

				return false;
			}

			return true;
		}

		private bool _IsExistTrelloBoardList(List<string> boardList)
		{
			if(boardList.IsNullOrEmpty())
			{
				LogChannel.Web.W("Trello board is empty");

				return false;
			}

			return true;
		}

		private bool _IsValidTrelloListId(string listId)
		{
			if(listId.IsEmpty())
			{
				LogChannel.Web.E("Trello listId is empty");

				return false;
			}

			return true;
		}

		private bool _IsValidTrelloCardId(string cardId)
		{
			if(cardId.IsEmpty())
			{
				LogChannel.Web.E("Card id is empty");

				return false;
			}

			return true;
		}
	}
}