using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using KZLib.KZUtility;
using Newtonsoft.Json.Linq;

namespace KZLib
{
	public partial class NetworkMgr : Singleton<NetworkMgr>
	{
		public void GetTrelloBoard(Action<List<string>> onAction)
		{
			GetTrelloBoardAsync().ContinueWith(onAction);
		}

		public void GetTrelloBoard(string trelloKey,Action<List<string>> onAction)
		{
			GetTrelloBoardAsync(trelloKey).ContinueWith(onAction);
		}

		public async UniTask<List<string>> GetTrelloBoardAsync()
		{
			var serviceCfg = ConfigMgr.In.Access<ConfigData.ServiceConfig>();

			return await GetTrelloBoardAsync(serviceCfg.TrelloKey);
		}

		public async UniTask<List<string>> GetTrelloBoardAsync(string trelloKey)
		{
			if(trelloKey.IsEmpty())
			{
				LogTag.System.E("TrelloKey is empty");

				return null;
			}

			var info = await _SendWebRequest(GetTrelloBoardWebRequest.Create(trelloKey));

			return _ParseTrelloData(info,"boards");
		}

		public void GetTrelloList(string boardId,Action<List<string>> onAction)
		{
			GetTrelloListAsync(boardId).ContinueWith(onAction);
		}

		public void GetTrelloList(string coreKey,string boardId,Action<List<string>> onAction)
		{
			GetTrelloListAsync(coreKey,boardId).ContinueWith(onAction);
		}

		public async UniTask<List<string>> GetTrelloListAsync(string boardId)
		{
			var serviceCfg = ConfigMgr.In.Access<ConfigData.ServiceConfig>();

			return await GetTrelloListAsync(serviceCfg.TrelloKey,boardId);
		}

		public async UniTask<List<string>> GetTrelloListAsync(string trelloKey,string boardId)
		{
			if(trelloKey.IsEmpty())
			{
				LogTag.Network.E("TrelloKey is empty");

				return null;
			}

			if(boardId.IsEmpty())
			{
				LogTag.Network.E("Trello boardId is empty");

				return null;
			}

			var info = await _SendWebRequest(GetTrelloListWebRequest.Create(trelloKey,boardId));

			return _ParseTrelloData(info,"boards");
		}

		public void GetTrelloCard(string listId,Action<List<string>> onAction)
		{
			GetTrelloCardAsync(listId).ContinueWith(onAction);
		}

		public void GetTrelloCard(string trelloKey,string listId,Action<List<string>> onAction)
		{
			GetTrelloCardAsync(trelloKey,listId).ContinueWith(onAction);
		}

		public async UniTask<List<string>> GetTrelloCardAsync(string listId)
		{
			var serviceCfg = ConfigMgr.In.Access<ConfigData.ServiceConfig>();

			return await GetTrelloCardAsync(serviceCfg.TrelloKey,listId);
		}

		public async UniTask<List<string>> GetTrelloCardAsync(string trelloKey,string listId)
		{
			if(trelloKey.IsEmpty())
			{
				LogTag.Network.E("TrelloKey is empty");

				return null;
			}

			if(listId.IsEmpty())
			{
				LogTag.Network.E("Trello listId is empty");

				return null;
			}

			var info = await _SendWebRequest(GetTrelloCardWebRequest.Create(trelloKey,listId));

			return _ParseTrelloData(info,"boards");
		}

		private static List<string> _ParseTrelloData(ResponseInfo responseInfo,string key)
		{
			if(responseInfo.Result)
			{
				try
				{
					var trello = JObject.Parse(responseInfo.Content);
					var tokenList = new List<string>();

					foreach(var token in trello[key])
					{
						if((bool) token["closed"])
						{
							continue;
						}

						tokenList.Add(token.ToString());
					}

					return tokenList;
				}
				catch(Exception exception)
				{
					LogTag.Network.E($"Parse is failed - {exception}");
				}
			}
			else
			{
				LogTag.Network.E("Result is failed");
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
			var serviceCfg = ConfigMgr.In.Access<ConfigData.ServiceConfig>();

			await PostTrelloListAsync(serviceCfg.TrelloKey,boardId,name);
		}

		public async UniTask PostTrelloListAsync(string trelloKey,string boardId,string name)
		{
			if(trelloKey.IsEmpty())
			{
				LogTag.Network.E("TrelloKey is empty");

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
			var serviceCfg = ConfigMgr.In.Access<ConfigData.ServiceConfig>();

			await PostTrelloCardAsync(serviceCfg.TrelloKey,listId,name,description,file);
		}

		public async UniTask PostTrelloCardAsync(string trelloKey,string listId,string name,string description,byte[] file = null)
		{
			if(trelloKey.IsEmpty())
			{
				LogTag.Network.E("TrelloKey is empty");

				return;
			}

			var cardInfo = await _SendWebRequest(PostTrelloCardWebRequest.Create(trelloKey,listId,name,description));

			if(file == null)
			{
				return;
			}

			var cardId = JObject.Parse(cardInfo.Content)["id"].ToString();

			if(cardId.IsEmpty())
			{
				LogTag.Network.E("Card id is empty");

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
			var serviceCfg = ConfigMgr.In.Access<ConfigData.ServiceConfig>();

			await PostTrelloListInCardAsync(serviceCfg.TrelloKey,boardName,listName,cardName,cardDescription,file);
		}

		public async UniTask PostTrelloListInCardAsync(string trelloKey,string boardName,string listName,string cardName,string cardDescription,byte[] file = null)
		{
			if(trelloKey.IsEmpty())
			{
				LogTag.Network.E("TrelloKey is empty");

				return;
			}

			var boardList = await GetTrelloBoardAsync(trelloKey);

			if(boardList.IsNullOrEmpty())
			{
				LogTag.Network.W("Trello board is empty");

				return;
			}

			var boardId = _FindId(boardList,boardName);

			if(boardId.IsEmpty())
			{
				LogTag.Network.E("Trello board id is null");

				return;
			}

			var listList = await GetTrelloListAsync(boardId);

			if(listList.IsNullOrEmpty())
			{
				LogTag.Network.W("Trello list is empty");

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
				LogTag.Network.E($"Convert is failed - {exception}");
			}

			return null;
		}
	}
}