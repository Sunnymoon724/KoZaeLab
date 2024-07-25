#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;

public static partial class CommonUtility
{
	/// <summary>
	/// 실제 경로를 생성함 (폴더면 폴더 생성 / 파일이면 부모 폴더 생성)
	/// </summary>
	public static void CreateFolder(string _path)
	{
		if(_path.IsEmpty())
		{
			throw new NullReferenceException("폴더의 경로가 null 입니다.");
		}

		// 현재 경로가 파일이면 부모 폴더 경로를 아니면 현재 경로의 풀 경로를 받아옴
		var fullPath = GetFullPath(IsFilePath(_path) ? GetProjectPath(_path) : _path);

		if(!Directory.Exists(fullPath))
		{
			Directory.CreateDirectory(fullPath);
		}
	}

	/// <summary>
	/// 에디터에서 해당 폴더나 파일을 오픈합니다.
	/// </summary>
	/// <param name="_path"></param>
	public static void OpenFolder(string _fullPath)
	{
		if(IsExistFolder(_fullPath))
		{
			EditorUtility.OpenWithDefaultApp(_fullPath);

			return;
		}

		UnityUtility.DisplayErrorPathLink(_fullPath);
	}

	public static void OpenFile(string _fullPath)
	{
		if(IsExistFile(_fullPath))
		{
			EditorUtility.OpenWithDefaultApp(_fullPath);

			return;
		}

		UnityUtility.DisplayErrorPathLink(_fullPath);
	}

	/// <summary>
	/// 템플릿 코드 추가 or 갱신 하기
	/// </summary>
	public static void AddOrUpdateTemplateText(string _folderPath,string _templateName,string _scriptName,string _newData,Func<string,string> _onUpdate)
	{
		var fullPath = GetFullPath(PathCombine(_folderPath,_scriptName));
		var templateText = IsExistFile(fullPath) ? ReadDataFromFile(fullPath) : GetTemplateText(_templateName);

		if(templateText.IsEmpty() || templateText.Contains(_newData))
		{
			return;
		}

		templateText = _onUpdate(templateText);

		WriteDataToFile(fullPath,templateText);

		AssetDatabase.Refresh();
	}

	/// <summary>
	/// 템플릿 코드 가져오기
	/// </summary>
	public static string GetTemplateText(string _templatePath)
	{
		var fullPath = GameUtility.GetTemplateFullFilePath(_templatePath);
		var data = ReadDataFromFile(fullPath);

		if(data.IsEmpty())
		{
			UnityUtility.DisplayErrorPathLink(fullPath);
		}

		return data;
	}
}
#endif