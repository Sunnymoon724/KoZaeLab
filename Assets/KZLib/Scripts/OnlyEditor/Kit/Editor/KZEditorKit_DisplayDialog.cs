#if UNITY_EDITOR
using System;
using UnityEditor;

/// <summary>
/// Editor-only utility methods for modal error, info, and confirmation dialogs.
/// </summary>
public static partial class KZEditorKit
{
	/// <summary>
	/// Shows an error dialog and rethrows the exception.
	/// </summary>
	public static void DisplayError(Exception exception)
	{
		EditorUtility.DisplayDialog("Error",exception.Message,"Ok");

		throw exception;
	}

	/// <summary>
	/// Shows an informational dialog with a single Ok button.
	/// </summary>
	public static void DisplayInfo(string message)
	{
		EditorUtility.DisplayDialog("Info",message,"Ok");
	}

	/// <summary>
	/// Shows a yes/no confirmation dialog for the given action message.
	/// </summary>
	public static bool DisplayConfirm(string message)
	{
		return DisplayCheck("Confirm",$"{message}?");
	}

	/// <summary>
	/// Shows a yes/no confirmation dialog. Returns true when the user clicks Yes.
	/// </summary>
	public static bool DisplayCheck(string title,string message)
	{
		return EditorUtility.DisplayDialog(title,message,"Yes","No");
	}
}
#endif
