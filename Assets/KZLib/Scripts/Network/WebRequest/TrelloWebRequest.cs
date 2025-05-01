using UnityEngine;
using UnityEngine.Networking;

namespace KZLib.KZNetwork
{
	public abstract class TrelloWebRequest : BaseWebRequest
	{
		// trelloKey = "key={TrelloApiKey}&token={TrelloToken}"

		protected TrelloWebRequest(string name,string uri,string method) : base(name,uri,method) { }
	}

	public abstract class GetTrelloWebRequest : TrelloWebRequest
	{
		protected GetTrelloWebRequest(string name,string uri) : base(name,uri,UnityWebRequest.kHttpVerbGET) { }
	}

	public class GetTrelloBoardWebRequest : GetTrelloWebRequest
	{
		public static GetTrelloBoardWebRequest Create(string trelloKey)
		{
			return new GetTrelloBoardWebRequest($"https://api.trello.com/1/members/me?{trelloKey}&boards=all");
		}

		private GetTrelloBoardWebRequest(string uri) : base("Get TrelloBoard",uri) { }
	}

	public class GetTrelloListWebRequest : GetTrelloWebRequest
	{
		public static GetTrelloListWebRequest Create(string trelloKey,string boardId)
		{
			return new GetTrelloListWebRequest($"https://api.trello.com/1/boards/{boardId}?{trelloKey}&lists=all");
		}

		private GetTrelloListWebRequest(string uri) : base("Get TrelloList",uri) { }
	}

	public class GetTrelloCardWebRequest : GetTrelloWebRequest
	{
		public static GetTrelloCardWebRequest Create(string trelloKey,string listId)
		{
			return new GetTrelloCardWebRequest($"https://api.trello.com/1/lists/{listId}?{trelloKey}&cards=all");
		}

		private GetTrelloCardWebRequest(string uri) : base("Get TrelloCard",uri) { }
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public abstract class PostTrelloWebRequest : TrelloWebRequest
	{
		protected PostTrelloWebRequest(string name,string uri) : base(name,uri,UnityWebRequest.kHttpVerbPOST) { }
	}

	public class PostTrelloListWebRequest : PostTrelloWebRequest
	{
		public static PostTrelloListWebRequest Create(string trelloKey,string boardId,string name)
		{
			return new PostTrelloListWebRequest($"https://api.trello.com/1/lists?name={name}&idBoard={boardId}&{trelloKey}");
		}

		private PostTrelloListWebRequest(string uri) : base("Post TrelloList",uri) { }
	}

	public class PostTrelloCardWebRequest : PostTrelloWebRequest
	{
		public static PostTrelloCardWebRequest Create(string trelloKey,string listId,string name,string description)
		{
			return new PostTrelloCardWebRequest($"https://api.trello.com/1/cards?idList={listId}&{trelloKey}&name={name}&desc={description}");
		}

		private PostTrelloCardWebRequest(string uri) : base("Post TrelloCard",uri)
		{
			m_webRequest.SetRequestHeader("Accept","application/json");
		}
	}

	public class PostTrelloAttachmentCardWebRequest : PostTrelloWebRequest
	{
		public static PostTrelloAttachmentCardWebRequest Create(string trelloKey,string cardId,byte[] file)
		{
			return new PostTrelloAttachmentCardWebRequest($"https://api.trello.com/1/cards/{cardId}/attachments?{trelloKey}",file);
		}

		private PostTrelloAttachmentCardWebRequest(string uri,byte[] file) : base("Post TrelloAttachmentCard",uri)
		{
			var form = new WWWForm();
			form.AddBinaryData("file",file,"screenShot.png","image/png");

			var rawData = form.data; // create this time
			var contentType = form.headers["Content-Type"];

			_CreateUploadHandler(rawData,contentType);

			m_webRequest.SetRequestHeader("Accept","application/json");
		}
	}
}