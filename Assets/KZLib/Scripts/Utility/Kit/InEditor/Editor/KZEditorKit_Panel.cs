#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static partial class KZEditorKit
{
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
}
#endif