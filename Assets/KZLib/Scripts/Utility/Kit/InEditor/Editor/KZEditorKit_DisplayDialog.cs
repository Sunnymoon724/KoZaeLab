#if UNITY_EDITOR
using System;
using UnityEditor;

public static partial class KZEditorKit
{
	public static void DisplayError(Exception exception)
	{
		EditorUtility.DisplayDialog("Error",exception.Message,"Ok");

		throw exception;
	}

	public static void DisplayInfo(string message)
	{
		EditorUtility.DisplayDialog("Info",message,"Ok");
	}

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