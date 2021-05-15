using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnrealByte.TrelloForUnity;
using System.Linq;

public class TrelloObject: MonoBehaviour {

	public static TrelloObject trelloInstance;
	private TrelloConnect trelloConnect;

    [Header("Trello connection settings")]
    public string apiKey = "Paste here your API Key";
    public string token = "Paste here your Token";
    public string selectedBoard = "The board name to use";

	[Space(10)]
    [Header("Feedback types.")]
	[Header("This are the lists you have in the selected board.")]
    public List<Dropdown.OptionData> feedbackTypes;

	[Space(10)]
	[Header("UI settings")]
	public GameObject busyUI;
	public GameObject fillUI;
	public GameObject feedbackForm;

	[Space(10)]
	[Header("Log settings")]
	public bool logActive;
	public bool logInCustomFile;
	public string logFilePath;
	public string initLogMessage;
	public bool debugLog;
	public bool includeWarnings;

	public bool takeScreenshot;

	private TLog tLog;

	void Awake() {
		if (trelloInstance == null) {
			trelloInstance = this;
		} else if (trelloInstance != this) {
			Destroy (gameObject);
		}
        DontDestroyOnLoad(gameObject);

		if (logActive) {
			tLog = new TLog ();
			tLog.initializeLog (false, "", initLogMessage, debugLog, includeWarnings);
			this.logFilePath = tLog.logFilePath;
		}
    }

    public IEnumerator Start() {

		trelloConnect = new TrelloConnect(apiKey, token);

		yield return trelloConnect.DownloadBoards();

		trelloConnect.useBoard(selectedBoard);

		yield return trelloConnect.DownloadLists();

		//Debug.Log ("[Mem] Boards: "+trelloConnect.tLists.Count);
		//Debug.Log ("[Mem] Board in use: "+trelloConnect.selectedBoardId);
		//Debug.Log ("[Mem] Lists: "+trelloConnect.tLists.Count);

		if(trelloConnect.tLists.Any(any=>any.name.Equals("버그 리포트")))
        {
			var newList = trelloConnect.CreateList("버그 리포트");
			yield return trelloConnect.PostList(newList);
		}
    }

	/// <summary>
	/// Once you have the card instantiated, call this method. The card will be sent to Trello.
	/// </summary>
	/// <param name="card">A Trello card object (TCard).</param>
	public IEnumerator SendCardPost (TCard card) {
		if (takeScreenshot) {
			yield return new WaitForEndOfFrame ();
			string path = "/screenshot.png";
			feedbackForm.SetActive (false);
			Texture2D screenImage = new Texture2D (Screen.width, Screen.height);
			//Get Image from screen
			screenImage.ReadPixels (new Rect (0, 0, Screen.width, Screen.height), 0, 0);
			screenImage.Apply ();
			//Convert to png
			byte[] imageBytes = screenImage.EncodeToPNG ();

			Debug.Log (Application.persistentDataPath);
			//Save image to file
			System.IO.File.WriteAllBytes (Application.persistentDataPath + path, imageBytes);
		}

		busyUI.SetActive(true);

		yield return trelloConnect.PostCard(card, logActive, logFilePath, takeScreenshot);

		yield return new WaitForSeconds(1);

		busyUI.SetActive(false);
	}

	/// <summary>
	/// Activates a GameObject
	/// Mostly used to show a message when the user not fill required inputs.
	/// </summary>
	/// <param name="gameObject">Game object.</param>
	/// <param name="timeInSeconds">Time in seconds.</param>
	/// <param name="setActive">If set to <c>true</c> set active.</param>
	public IEnumerator ActivateGO(GameObject gameObject, float timeInSeconds, bool setActive = true) {
		gameObject.SetActive(setActive);
		yield return new WaitForSeconds(timeInSeconds);
		gameObject.SetActive(!setActive);
	}

	/// <summary>
	/// Validates the form data, creates and send the Trello card
	/// </summary>
	/// <param name="title">The form input data for card title</param>
	/// <param name="description">The form input data for card description</param>
	/// <param name="listName">The list name to put the card in</param>
	public Coroutine SendFeedbackForm (string title, string description, string listName) {
		if (title == "" || description == "") {
			
			StartCoroutine(ActivateGO(fillUI, 2));
			return null;
		}

		string descriptionToSend = "**Summary:** \n"+description+"\n\n";

		TCard card = trelloConnect.CreateCard(title, (descriptionToSend), listName);
		return StartCoroutine(SendCardPost(card));
	}
}