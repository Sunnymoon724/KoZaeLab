#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using KZLib.Utilities;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;

public static class KZEditorKit
{
	#region DisplayDialog
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
	#endregion DisplayDialog

	#region ProgressBar
	public static bool DisplayCancelableProgressBar(string title,string info,int current,int total)
	{
		return DisplayCancelableProgressBar(title,info,current/(float)total);
	}

	public static bool DisplayCancelableProgressBar(string title,string info,float progress)
	{
		return EditorUtility.DisplayCancelableProgressBar(title,info,progress);
	}

	public static void ClearProgressBar()
	{
		EditorUtility.ClearProgressBar();
	}
	#endregion ProgressBar

	#region Panel
	public static string FindFilePathInPanel(string title,string kind = "*")
	{
		return EditorUtility.OpenFilePanel(title,Application.dataPath,kind);
	}

	public static string FindFolderPathInPanel(string title)
	{
		return EditorUtility.OpenFolderPanel(title,Application.dataPath,string.Empty);
	}

	public static string FindTsvFile()
	{
		return _FindFile("Find tsv file","*.tsv");
	}

	public static string FindExcelFilePath()
	{
		var filterArray = new List<string>();

		var excelExtensionArray = new string[] { ".xls", ".xlsx", ".xlsm" };
		
		for(var i=0;i<excelExtensionArray.Length;i++)
		{
			filterArray.Add($"*{excelExtensionArray[i]}");
		}

		return FindFilePathInPanel("Find excel file",string.Join(';',filterArray));
	}

	public static string FindJsonFile()
	{
		return _FindFile("Find json file","*.json");
	}

	public static string FindTestFile()
	{
		return _FindFile("Find test file.","*.*");
	}

	private static string _FindFile(string title,string kind)
	{
		var filePath = FindFilePathInPanel(title,kind);

		return filePath.IsEmpty() ? null : KZFileKit.ReadFileToText(filePath);
	}
	#endregion Panel

	#region Open
	public static void Open(string absolutePath)
	{
		if(!KZFileKit.IsPathExist(absolutePath))
		{
			return;
		}

		EditorUtility.OpenWithDefaultApp(absolutePath);
	}

	public static void OpenBatchFile(string absolutePath)
	{
		if(!KZFileKit.IsPathExist(absolutePath))
		{
			return;
		}

		Process.Start(new ProcessStartInfo
		{
			FileName = absolutePath,
			UseShellExecute = true,
		});
	}

	public static void OpenTextFile(string absoluteFilePath,int line = 1)
	{
		if(!KZFileKit.IsFileExist(absoluteFilePath))
		{
			return;
		}

		line = line <= 0 ? 1 : line;

		InternalEditorUtility.OpenFileAtLineExternal(absoluteFilePath,line);
	}
	#endregion Open

	public static void OpenSceneInEditor(string sceneName)
	{
		if(!DisplayCheck($"Open {sceneName}",$"Do you want to open the {sceneName}?"))
		{
			return;
		}

		var guidArray = AssetDatabase.FindAssets($"t:Scene {sceneName}");
		var scenePath = guidArray.Length < 1 ? string.Empty : AssetDatabase.GUIDToAssetPath(guidArray[0]);

		if(scenePath.IsEmpty())
		{
			return;
		}

		EditorSceneManager.OpenScene(scenePath);
	}
}
#endif