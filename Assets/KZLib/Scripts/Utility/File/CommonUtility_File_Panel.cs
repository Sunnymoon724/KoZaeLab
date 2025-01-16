#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static partial class CommonUtility
{
	public static string GetFilePathInPanel(string title,string kind = "*")
	{
		return EditorUtility.OpenFilePanel(title,Application.dataPath,kind);
	}

	public static string GetFolderPathInPanel(string title)
	{
		return EditorUtility.OpenFolderPanel(title,Application.dataPath,string.Empty);
	}

	public static string GetTSVFile()
	{
		return GetFile("Find tsv file","*.tsv");
	}

	public static string GetExcelFilePath()
	{
		var filterArray = new List<string>();

		foreach(var extension in Global.EXCEL_EXTENSION_ARRAY)
		{
			filterArray.Add($"*{extension}");
		}

		return GetFilePathInPanel("Find excel file",string.Join(';',filterArray));
	}

	public static string GetJsonFile()
	{
		return GetFile("Find json file","*.json");
	}

	public static string GetTestFile()
	{
		return GetFile("Find text file.","*.*");
	}

	private static string GetFile(string title,string kind)
	{
		var filePath = GetFilePathInPanel(title,kind);

		return filePath.IsEmpty() ? null : ReadFileToText(filePath);
	}
}
#endif