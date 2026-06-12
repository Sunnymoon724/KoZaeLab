#if UNITY_EDITOR
using System;
using UnityEditor;

/// <summary>
/// Partial editor utility for displaying confirmation and information dialogs.
/// </summary>
public static partial class KZEditorKit
{
	/// <summary>
	/// Displays an error dialog with the exception message and rethrows the exception.
	/// </summary>
	public static void DisplayError(Exception exception)
	{
		EditorUtility.DisplayDialog("Error",exception.Message,"Ok");

		throw exception;
	}

	public static void DisplayInfo(string message)
	{
		EditorUtility.DisplayDialog("Info",message,"Ok");
	}

	/// <summary>
	/// Displays a Yes/No confirmation dialog using the message as both title and prompt with a trailing question mark.
	/// </summary>
	public static bool DisplayCheckBeforeExecute(string message)
	{
		return DisplayCheck($"{message}",$"{message}?");
	}

	public static bool DisplayCheck(string title,string message)
	{
		return EditorUtility.DisplayDialog(title,message,"Yes","No");
	}
}
#endif
