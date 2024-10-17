#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static partial class FileUtility
{
	public static string GetFilePathInPanel(string _header,string _kind = "*")
	{
		var filePath = EditorUtility.OpenFilePanel(_header,Application.dataPath,_kind);

		if(filePath.IsEmpty())
		{
			return string.Empty;
		}

		IsExist(filePath,true);

		return filePath;
	}

	public static string GetFolderPathInPanel(string _header)
	{
		var folderPath = EditorUtility.OpenFolderPanel(_header,Application.dataPath,string.Empty);

		if(folderPath.IsEmpty())
		{
			return string.Empty;
		}

		IsExist(folderPath);

		return folderPath;
	}

	public static string GetTSVFile()
	{
		return GetFile("Find tsv file","tsv");
	}

	public static string GetExcelFilePath()
	{
#if UNITY_EDITOR_WIN
		return GetFilePathInPanel("Find excel file","excel files;*.xls;*.xlsx;*.xlsm");
#else // for UNITY_EDITOR_OSX
		return GetFilePathInPanel("Find excel file","*.xlsx;*.xlsm");
#endif
	}

	public static string GetJsonFile()
	{
		return GetFile("Find json file","json");
	}

	public static string GetTestFile()
	{
		return GetFile("Find text file.","*.*");
	}

	private static string GetFile(string _header,string _kind)
	{
		var filePath = GetFilePathInPanel(_header,_kind);

		if(filePath.IsEmpty())
		{
			return string.Empty;
		}

		return ReadFileToText(filePath);
	}
}
#endif