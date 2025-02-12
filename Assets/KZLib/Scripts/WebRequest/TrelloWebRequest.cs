using UnityEngine;
using UnityEngine.Networking;

namespace KZLib.KZNetwork
{
	public abstract class TrelloWebRequest : BaseWebRequest
	{
		protected const string BASE_TRELLO_URL = "https://api.trello.com/1/";

		protected TrelloWebRequest(string uri,string method) : base(uri,method) { }
	}

	#region Get
	public abstract class TrelloGetWebRequest : TrelloWebRequest
	{
		protected TrelloGetWebRequest(string uri) : base(uri,UnityWebRequest.kHttpVerbGET) { }
	}

	public class TrelloGetBoardWebRequest : TrelloGetWebRequest
	{
		public static TrelloGetBoardWebRequest Create(string coreKey)
		{
			return new TrelloGetBoardWebRequest($@"https://api.trello.com/1/members/me?{coreKey}&boards=all");
		}

		private TrelloGetBoardWebRequest(string uri) : base(uri) { }
	}

	public class TrelloGetListWebRequest : TrelloGetWebRequest
	{
		public static TrelloGetListWebRequest Create(string coreKey,string boardId)
		{
			return new TrelloGetListWebRequest($@"https://api.trello.com/1/boards/{boardId}?{coreKey}&lists=all");
		}

		private TrelloGetListWebRequest(string _uri) : base(_uri) { }
	}

	public class TrelloGetCardWebRequest : TrelloGetWebRequest
	{
		public static TrelloGetCardWebRequest Create(string coreKey,string listId)
		{
			return new TrelloGetCardWebRequest($@"https://api.trello.com/1/lists/{listId}?{coreKey}&cards=all");
		}

		private TrelloGetCardWebRequest(string _uri) : base(_uri) { }
	}
	#endregion Get

	#region Post
	public abstract class TrelloPostWebRequest : TrelloWebRequest
	{
		protected TrelloPostWebRequest(string uri) : base(uri,UnityWebRequest.kHttpVerbPOST) { }
	}

	public class TrelloPostListWebRequest : TrelloPostWebRequest
	{
		public static TrelloPostListWebRequest Create(string coreKey,string boardId,string name)
		{
			return new TrelloPostListWebRequest($@"https://api.trello.com/1/lists?name={name}&idBoard={boardId}&{coreKey}");
		}

		private TrelloPostListWebRequest(string uri) : base(uri) { }
	}

	public class TrelloPostCardWebRequest : TrelloPostWebRequest
	{
		public static TrelloPostCardWebRequest Create(string coreKey,string listId,string name,string description)
		{
			return new TrelloPostCardWebRequest($@"https://api.trello.com/1/cards?idList={listId}&{coreKey}&name={name}&desc={description}");
		}

		private TrelloPostCardWebRequest(string uri) : base(uri)
		{
			m_webRequest.SetRequestHeader("Accept","application/json");
		}
	}

	public class TrelloPostAttachmentCardWebRequest : TrelloPostWebRequest
	{
		public static TrelloPostAttachmentCardWebRequest Create(string coreKey,string cardId,byte[] file)
		{
			return new TrelloPostAttachmentCardWebRequest($@"https://api.trello.com/1/cards/{cardId}/attachments?{coreKey}",file);
		}

		private TrelloPostAttachmentCardWebRequest(string uri,byte[] file) : base(uri)
		{
			var form = new WWWForm();
			form.AddBinaryData("file",file,"screenShot.png","image/png");

			m_webRequest.uploadHandler = new UploadHandlerRaw(form.data) { contentType = form.headers["Content-Type"] };
			m_webRequest.SetRequestHeader("Accept","application/json");
		}
	}
	#endregion Post
}