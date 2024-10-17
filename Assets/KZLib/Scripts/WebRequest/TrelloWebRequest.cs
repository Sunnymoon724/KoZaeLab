using UnityEngine;
using UnityEngine.Networking;

namespace KZLib.KZNetwork
{
	public abstract class TrelloWebRequest : BaseWebRequest
	{
		protected const string BASE_TRELLO_URL = "https://api.trello.com/1/";

		protected TrelloWebRequest(string _uri,string _method) : base(_uri,_method) { }
	}

	#region Get
	public abstract class TrelloGetWebRequest : TrelloWebRequest
	{
		protected TrelloGetWebRequest(string _uri) : base(_uri,UnityWebRequest.kHttpVerbGET) { }
	}

	public class TrelloGetBoardsWebRequest : TrelloGetWebRequest
	{
		public static TrelloGetBoardsWebRequest Create(string _coreKey)
		{
			return new TrelloGetBoardsWebRequest($@"https://api.trello.com/1/members/me?{_coreKey}&boards=all");
		}

		private TrelloGetBoardsWebRequest(string _uri) : base(_uri) { }
	}

	public class TrelloGetListsWebRequest : TrelloGetWebRequest
	{
		public static TrelloGetListsWebRequest Create(string _coreKey,string _boardId)
		{
			return new TrelloGetListsWebRequest($@"https://api.trello.com/1/boards/{_boardId}?{_coreKey}&lists=all");
		}

		private TrelloGetListsWebRequest(string _uri) : base(_uri) { }
	}

	public class TrelloGetCardsWebRequest : TrelloGetWebRequest
	{
		public static TrelloGetCardsWebRequest Create(string _coreKey,string _listId)
		{
			return new TrelloGetCardsWebRequest($@"https://api.trello.com/1/lists/{_listId}?{_coreKey}&cards=all");
		}

		private TrelloGetCardsWebRequest(string _uri) : base(_uri) { }
	}
	#endregion Get

	#region Post
	public abstract class TrelloPostWebRequest : TrelloWebRequest
	{
		protected TrelloPostWebRequest(string _uri) : base(_uri,UnityWebRequest.kHttpVerbPOST) { }
	}

	public class TrelloPostListWebRequest : TrelloPostWebRequest
	{
		public static TrelloPostListWebRequest Create(string _coreKey,string _boardId,string _name)
		{
			return new TrelloPostListWebRequest($@"https://api.trello.com/1/lists?name={_name}&idBoard={_boardId}&{_coreKey}");
		}

		private TrelloPostListWebRequest(string _uri) : base(_uri) { }
	}

	public class TrelloPostCardWebRequest : TrelloPostWebRequest
	{
		public static TrelloPostCardWebRequest Create(string _coreKey,string _listId,string _name,string _description)
		{
			return new TrelloPostCardWebRequest($@"https://api.trello.com/1/cards?idList={_listId}&{_coreKey}&name={_name}&desc={_description}");
		}

		private TrelloPostCardWebRequest(string _uri) : base(_uri)
		{
			m_WebRequest.SetRequestHeader("Accept","application/json");
		}
	}

	public class TrelloPostAttachmentCardWebRequest : TrelloPostWebRequest
	{
		public static TrelloPostAttachmentCardWebRequest Create(string _coreKey,string _cardId,byte[] _file)
		{
			return new TrelloPostAttachmentCardWebRequest($@"https://api.trello.com/1/cards/{_cardId}/attachments?{_coreKey}",_file);
		}

		private TrelloPostAttachmentCardWebRequest(string _uri,byte[] _file) : base(_uri)
		{
			var form = new WWWForm();
			form.AddBinaryData("file",_file,"screenShot.png","image/png");

			m_WebRequest.uploadHandler = new UploadHandlerRaw(form.data) { contentType = form.headers["Content-Type"] };
			m_WebRequest.SetRequestHeader("Accept","application/json");
		}
	}
	#endregion Post
}