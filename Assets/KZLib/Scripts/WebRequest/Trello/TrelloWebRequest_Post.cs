using System;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace KZLib.KZNetwork
{
	public abstract class TrelloPostWebRequest : TrelloWebRequest
	{
		protected TrelloPostWebRequest(string _uri) : base(_uri,UnityWebRequest.kHttpVerbPOST) { }
	}

	public class TrelloPostListWebRequest : TrelloPostWebRequest
	{
		public static TrelloPostListWebRequest Create(string _coreKey,string _boardId,string _name,Action<string> _onComplete = null)
		{
			return new TrelloPostListWebRequest(string.Format(@"https://api.trello.com/1/lists?name={0}&idBoard={1}&{2}",_name,_boardId,_coreKey));//,(result,data)=>
			// {
			// 	if(!result)
			// 	{
			// 		_onComplete?.Invoke(null);

			// 		return;
			// 	}

			// 	var trello = JObject.Parse(data);

			// 	if(trello.ContainsKey("id"))
			// 	{
			// 		return;
			// 	}

			// 	var token = trello["id"];

			// 	_onComplete?.Invoke(token.ToString());
			// });
		}

		private TrelloPostListWebRequest(string _uri) : base(_uri) { }
	}

	public class TrelloPostCardWebRequest : TrelloPostWebRequest
	{
		public static TrelloPostCardWebRequest Create(string _coreKey,string _listId,string _name,string _description,Action<string> _onComplete = null)
		{
			return new TrelloPostCardWebRequest(string.Format(@"https://api.trello.com/1/cards?idList={0}&{1}&name={2}&desc={3}",_listId,_coreKey,_name,_description));
			// ,(result,data)=>
			// {
			// 	if(!result)
			// 	{
			// 		_onComplete?.Invoke(null);

			// 		return;
			// 	}

			// 	var trello = JObject.Parse(data);

			// 	if(!trello.ContainsKey("id"))
			// 	{
			// 		return;
			// 	}

			// 	var token = trello["id"];

			// 	_onComplete?.Invoke(token.ToString());
			// });
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
			Debug.Log(string.Format(@"https://api.trello.com/1/cards/{0}/attachments?{1}",_cardId,_coreKey));

			return new TrelloPostAttachmentCardWebRequest(string.Format(@"https://api.trello.com/1/cards/{0}/attachments?{1}",_cardId,_coreKey),_file);
			// ,(result,data)=>
			// {
			// 	Debug.Log(data);
			// });
		}

		private TrelloPostAttachmentCardWebRequest(string _uri,byte[] _file) : base(_uri)
		{
			WWWForm form = new WWWForm();
			form.AddBinaryData("file",_file,"screenShot","image/png");

			// https://api.trello.com/1/cards/{id}/attachments?key=APIKey&token=APIToken

			m_WebRequest.SetRequestHeader("Accept","application/json");

			m_WebRequest.uploadHandler = new UploadHandlerRaw(form.data)
			{
				contentType = "multipart/form-data",
			};
		}
	}
}