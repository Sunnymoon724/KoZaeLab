#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor-only utility methods for OS file and folder picker dialogs.
/// </summary>
public static partial class KZEditorKit
{
	/// <summary>
	/// Opens an OS file panel rooted at the project's Assets folder and returns the selected absolute path.
	/// </summary>
	public static string OpenFilePanel(string title,string kind = "*")
	{
		return EditorUtility.OpenFilePanel(title,Application.dataPath,kind);
	}

	/// <summary>
	/// Opens an OS folder panel rooted at the project's Assets folder and returns the selected absolute path.
	/// </summary>
	public static string OpenFolderPanel(string title)
	{
		return EditorUtility.OpenFolderPanel(title,Application.dataPath,string.Empty);
	}

	/// <summary>
	/// Prompts for a TSV file and returns its text content. Returns null when cancelled.
	/// </summary>
	public static string ReadTsvFile()
	{
		return _ReadFile("Find tsv file","*.tsv");
	}

	/// <summary>
	/// Opens an OS file panel filtered for Excel files and returns the selected absolute path.
	/// </summary>
	public static string OpenExcelFilePanel()
	{
		var filterArray = new List<string>();

		var excelExtensionArray = new string[] { ".xls", ".xlsx", ".xlsm" };
		
		for(var i=0;i<excelExtensionArray.Length;i++)
		{
			filterArray.Add($"*{excelExtensionArray[i]}");
		}

		return OpenFilePanel("Find excel file",string.Join(';',filterArray));
	}

	/// <summary>
	/// Prompts for a JSON file and returns its text content. Returns null when cancelled.
	/// </summary>
	public static string ReadJsonFile()
	{
		return _ReadFile("Find json file","*.json");
	}

	/// <summary>
	/// Prompts for any file and returns its text content. Returns null when cancelled.
	/// </summary>
	public static string ReadTestFile()
	{
		return _ReadFile("Find test file","*.*");
	}

	private static string _ReadFile(string title,string kind)
	{
		var filePath = OpenFilePanel(title,kind);

		return filePath.IsEmpty() ? null : KZFileKit.ReadTextFromFile(filePath);
	}
}
#endif
