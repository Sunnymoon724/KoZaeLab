#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static partial class CommonUtility
{
	/// <summary>
	/// 팝업 창을 이용하여 경로 수동 설정
	/// </summary>
	public static string GetFilePathInPanel(string _header,string _kind = "*")
	{
		var filePath = EditorUtility.OpenFilePanel(_header,Application.dataPath,_kind);

		if(filePath.IsEmpty())
		{
			return string.Empty;
		}

		IsExistFile(filePath,true);

		return filePath;
	}

	/// <summary>
	/// 팝업 창을 이용하여 경로 수동 설정
	/// </summary>
	public static string GetFolderPathInPanel(string _header)
	{
		var folderPath = EditorUtility.OpenFolderPanel(_header,Application.dataPath,string.Empty);

		if(folderPath.IsEmpty())
		{
			return string.Empty;
		}

		IsExistFolder(folderPath);

		return folderPath;
	}

	public static string GetTSVFile()
	{
		return GetFile("tsv 파일 빨리줘요 현기증 난단 말이에요!!","tsv");
	}

	public static string GetExcelFilePath()
	{
#if UNITY_EDITOR_WIN
		return GetFilePathInPanel("엑셀 파일 어딨어!!!!!","excel files;*.xls;*.xlsx;*.xlsm");
#else // for UNITY_EDITOR_OSX
		return GetFilePathInPanel("엑셀 파일 어딨어!!!!!","*.xlsx;*.xlsm");
#endif
	}

	public static string GetJsonFile()
	{
		return GetFile("아 몰랑 Json파일 어딨어!!","json");
	}

	public static string GetTestFile()
	{
		return GetFile("테스트할 파일 찾습니다. 용량이 작을수록 좋아요ㅋ","*.*");
	}

	private static string GetFile(string _header,string _kind)
	{
		var filePath = GetFilePathInPanel(_header,_kind);

		if(filePath.IsEmpty())
		{
			return string.Empty;
		}

		return ReadDataFromFile(filePath);
	}
}
#endif