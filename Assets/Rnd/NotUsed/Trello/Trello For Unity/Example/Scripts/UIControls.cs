using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnrealByte.TrelloForUnity;

public class UIControls : MonoBehaviour
{
    [Header("UI Objects")]
    public InputField nameInput;
    public InputField descriptionInput;
    public Dropdown   ListTypesDropdown;

    void Start() {		
		if (TrelloObject.trelloInstance != null) {
			ListTypesDropdown.AddOptions (TrelloObject.trelloInstance.feedbackTypes);
		}
    }

	/// <summary>
	/// This is called in the onCLick event from the feedback form.
	/// </summary>
    public void SendForm() {
		if (TrelloObject.trelloInstance != null) {

			TrelloObject.trelloInstance.SendFeedbackForm(nameInput.text, descriptionInput.text, ListTypesDropdown.captionText.text);
            
			nameInput.text = "";
			descriptionInput.text = "";
        }
    }
}