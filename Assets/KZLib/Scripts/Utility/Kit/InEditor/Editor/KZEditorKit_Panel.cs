#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Partial editor utility for native file and folder picker dialogs.
/// </summary>
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

	/// <summary>
	/// Opens a file picker for a TSV file and returns the selected file contents as text, or null when cancelled.
	/// </summary>
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

	/// <summary>
	/// Opens a file picker for a JSON file and returns the selected file contents as text, or null when cancelled.
	/// </summary>
	public static string FindJsonFile()
	{
		return _FindFile("Find json file","*.json");
	}

	/// <summary>
	/// Opens a file picker for any file type and returns the selected file contents as text, or null when cancelled.
	/// </summary>
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
