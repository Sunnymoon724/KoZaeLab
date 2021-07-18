using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using LitJson;

namespace UnrealByte.TrelloForUnity {

	public class TrelloConnect {

		private string membersURL = "https://api.trello.com/1/members/me";
		private string boardsURL = "https://api.trello.com/1/boards/";
		private string cardsURL = "https://api.trello.com/1/cards/";
		private string listURL = "https://api.trello.com/1/lists/";

		private string apiKey;
		private string oauthToken;

		public List<TBoard> tBoards;
		public List<TCard> tCards;
		public List<TList> tLists;
		public string selectedBoardId = "";

		public TrelloConnect (string apiKey, string oauthToken) {
			this.apiKey = apiKey;
			this.oauthToken = oauthToken;
		}

		/// <summary>
		/// Download all the boards and save them in tBoards List for future use.
		/// </summary>
		public IEnumerator DownloadBoards() {
			tBoards = null;

			string responseData = "";
			string requestURL = membersURL + "?" + "key=" + apiKey + "&token=" + oauthToken + "&boards=all";
			UnityWebRequest www = UnityWebRequest.Get(requestURL);

			yield return www.SendWebRequest();

			if (www.isNetworkError || www.isHttpError) {
				Debug.Log ("[TrelloForUnity] " + www.responseCode+" - "+www.downloadHandler.text);
			} else {
				responseData = www.downloadHandler.text;
			}

			JsonData data = JsonMapper.ToObject(responseData);
			if (data ["boards"].Count > 0) {
				tBoards = new List<TBoard> ();
				for (int i = 0; i < data ["boards"].Count; i++) {
					TBoard b = new TBoard ();
					b.name = data ["boards"] [i] ["name"].ToString ();
					bool closed = false;
					if (data ["boards"] [i] ["closed"].ToString ().Equals ("true")) {
						closed = true;
					}
					b.closed = closed;
					b.idOrganization = data["boards"][i]["idOrganization"]==null?"":data["boards"][i]["idOrganization"].ToString();
					b.id = data ["boards"] [i] ["id"].ToString ();
					this.tBoards.Add (b);
				}
			}
		}			

		/// <summary>
		/// Download the lists in the selected board and save them in tLists for future use.
		/// </summary>
		public IEnumerator DownloadLists() {
			tLists = null;

			if (selectedBoardId == "") {
				Debug.Log ("[TrelloForUnity] You must select a board name.");
			} else {

				string responseData = "";
				string requestURL = boardsURL + selectedBoardId + "?" + "key=" + apiKey + "&token=" + oauthToken + "&lists=all";
				UnityWebRequest www = UnityWebRequest.Get(requestURL);

				yield return www.SendWebRequest();

				if (www.isNetworkError || www.isHttpError) {
					Debug.Log ("[TrelloForUnity] " + www.responseCode+" - "+www.downloadHandler.text);
					responseData = www.error;
				} else {
					responseData = www.downloadHandler.text;
				}

				JsonData data = JsonMapper.ToObject(responseData);
				if (data ["lists"].Count > 0) {
					tLists = new List<TList> ();
					for (int i = 0; i < data ["lists"].Count; i++) {
						TList tl = new TList ();
						tl.idBoard = data ["lists"] [i] ["idBoard"].ToString();
						tl.name = data ["lists"] [i] ["name"].ToString();
						tl.pos = data ["lists"] [i] ["pos"].ToString();
						tl.id = data ["lists"] [i] ["id"].ToString();
						tLists.Add (tl);
					}
				}
			}
		}

		/// <summary>
		/// Download all the cards from the specified list. 
		/// </summary>
		/// <param name="listId">The id from the list you want all the cards.</param>
		public IEnumerator DownloadCardsFromList (string listId) {
			tCards = null;

			if (listId != "") {
				string responseData = "";
				string requestURL = listURL + listId + "?" + "key=" + apiKey + "&token=" + oauthToken + "&cards=all";
				UnityWebRequest www = UnityWebRequest.Get(requestURL);

				yield return www.SendWebRequest();

				if (www.isNetworkError || www.isHttpError) {
					Debug.Log ("[TrelloForUnity] " + www.responseCode+" - "+www.downloadHandler.text);
					responseData = www.error;
				} else {
					responseData = www.downloadHandler.text;
				}

				JsonData data = JsonMapper.ToObject(responseData);
				if (data ["cards"].Count > 0) {
					tCards = new List<TCard> ();
					for (int i = 0; i < data ["cards"].Count; i++) {
						TCard tCard = new TCard ();
						tCard.desc = data ["cards"][i]["desc"].ToString();
						tCard.descData = data ["cards"][i]["descData"].ToString();
						tCard.due = data ["cards"][i]["due"].ToString();
						tCard.email = data ["cards"][i]["email"].ToString();
						tCard.idList = data ["cards"][i]["idList"].ToString();
						tCard.name = data ["cards"][i]["name"].ToString();
						tCard.pos = data ["cards"][i]["pos"].ToString();
						tCard.urlSource = data ["cards"][i]["urlSource"].ToString();
					}
				}
			}
		}

		/// <summary>
		/// Iterates throgh tBoards list searching the board.
		/// If the search hits the board, then sets as selectedBoard.
		/// </summary>
		/// <param name="boardName">Name of the board you want to use.</param>
		public void useBoard (string boardName) {
			bool boardExists = false;
			if (tBoards == null) {
				Debug.Log ("[TrelloForUnity] Error Board is null");
			} else {
				for (int i = 0; i < tBoards.Count; i++) {
					if (tBoards [i].name.Equals (boardName)) {
						selectedBoardId = tBoards [i].id;
						boardExists = true;
						return;
					}
				}
				if (!boardExists) {
					Debug.Log ("[TrelloForUnity] No such board found.");
				}
			}
		}

		/// <summary>
		/// Instatiate a new Trello card object.
		/// </summary>
		public TCard CreateCard(string cardName, string cardDescription, string cardListName) {
			TCard card = new TCard ();
			string selectedListId = "";
			for (int i = 0; i < tLists.Count; i++) {
				if (tLists [i].name == cardListName) {
					selectedListId = tLists [i].id;
					break;
				}
			}
			card.idList = selectedListId;
			card.name = cardName;
			card.desc = cardDescription;
			return card;
		}

		/// <summary>
		///  Instantiate a new Trello list object, setting up the parameter idBoard with the existing selectedBoardId
		///  It does not upload the list.
		/// </summary>
		public TList CreateList(string name)	{
			TList list = new TList();
			list.idBoard = selectedBoardId;
			list.name = name;
			return list;
		}

		/// <summary>
		/// Send a list for creation on Trello
		/// </summary>
		/// <returns>The created List ID.</returns>
		/// <param name="tList">The list object to create on Trello</param>
		public IEnumerator PostList(TList tList) {

			WWWForm form = new WWWForm();
			form.AddField("key", apiKey);
			form.AddField("token", oauthToken);
			form.AddField("name", tList.name);
			form.AddField("idBoard", tList.idBoard);

			UnityWebRequest www = UnityWebRequest.Post(listURL, form);

			yield return www.SendWebRequest();

			if(www.isNetworkError || www.isHttpError) {
				Debug.Log("[TrelloForUnity] " + www.responseCode+" - "+www.downloadHandler.text);
			} else {
				Debug.Log("[TrelloForUnity] List uploaded");
				yield return (string) "ok";
			}
		}

		/// <summary>
		/// Send a card for creation in Trello.
		/// </summary>
		/// <returns>The created Card ID</returns>
		/// <param name="tCard">The card that you want to create.</param>
		public IEnumerator PostCard (TCard tCard, bool logActive, string logFilePath, bool sendScreenshot) {

			WWWForm form = new WWWForm();
			form.AddField("key", apiKey);
			form.AddField("token", oauthToken);
			form.AddField("name", tCard.name);
			form.AddField("desc", tCard.desc);
			form.AddField("idList", tCard.idList);

			Debug.Log ("[TrelloForUnity] Sending Card to Trello");

			UnityWebRequest www = UnityWebRequest.Post(cardsURL, form);

			yield return www.SendWebRequest();

			if(www.isNetworkError || www.isHttpError) {
				Debug.Log("[TrelloForUnity] " + www.responseCode+" - "+www.downloadHandler.text);
			} else {
				Debug.Log("[TrelloForUnity] Feedback post complete!");
				yield return (string) "ok";
				JsonData data = JsonMapper.ToObject (www.downloadHandler.text);
				string id = data ["id"].ToString();

				if (sendScreenshot) {

					byte[] screenshotData = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/screenshot.png");

					form = new WWWForm();
					form.AddField("key", apiKey);
					form.AddField("token", oauthToken);
					form.AddField("name", "Screenshot");
					form.AddField("mimeType", "");
					form.AddBinaryData("file", screenshotData, "screenshot.png");

					www = UnityWebRequest.Post("https://api.trello.com/1/cards/"+id+"/attachments", form);
					www.chunkedTransfer = false;

					www.SendWebRequest();

					if (www.isNetworkError || www.isHttpError) {
						Debug.Log ("[TrelloForUnity] " + www.responseCode+" - "+www.downloadHandler.text);
					} else {
						Debug.Log("[TrelloForUnity] Screenshot Attach complete!");
					}

				}

				if (logActive) {					

					byte[] logData = System.IO.File.ReadAllBytes(logFilePath);
					string logName = "TFULog.txt";

					form = new WWWForm();
					form.AddField("key", apiKey);
					form.AddField("token", oauthToken);
					form.AddField("name", "TFULog");
					form.AddField("mimeType", "");
					form.AddBinaryData("file", logData, logName);

					www = UnityWebRequest.Post("https://api.trello.com/1/cards/"+id+"/attachments", form);
					www.chunkedTransfer = false;

					www.SendWebRequest();

					if (www.isNetworkError || www.isHttpError) {
						Debug.Log ("[TrelloForUnity] " + www.responseCode+" - "+www.downloadHandler.text);
					} else {
						Debug.Log("[TrelloForUnity] LogFile Attach complete!");
					}
				}
			}
		}
			
	}
}