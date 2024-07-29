using System;
using System.Collections.Generic;
using System.IO;

public static partial class FileUtility
{
	/// <summary>
	/// .meta는 알아서 제외 합니다.
	/// </summary>
	public static string[] GetAllFilePathInFolder(string _folderPath)
	{
		IsExist(_folderPath,true);

		if(IsFilePath(_folderPath))
		{
			throw new NullReferenceException("파일 경로 입니다.");
		}
		
		var fileList = new List<string>();

		foreach(var filePath in Directory.GetFiles(_folderPath))
		{
			if(filePath.EndsWith(".meta"))
			{
				continue;
			}

			fileList.Add(filePath);
		}

		return fileList.ToArray();
	}

	public static void FileSearchInDirectory(string _path,string _file,ref string _searchPath)
	{
		var fileArray = Directory.GetFiles(_path);

		for(var i=0;i<fileArray.Length;i++)
		{
			var fileName = GetFileName(fileArray[i]);

			if(fileName.IsEqual(_file))
			{
				_searchPath = _path;
			}
		}

		var directoryArray = Directory.GetDirectories(_path);

		for(var i=0;i<directoryArray.Length;i++)
		{
			FileSearchInDirectory(directoryArray[i],_file,ref _searchPath);
		}
	}

	public static void SearchExtensionInDirectory(string _path,string _extension,ref List<string> _searchPathList,string _containText = "",string _sceneName = "")
	{
		var fileArray = Directory.GetFiles(_path,_extension);
		var checkContainString = false;
		var checkFile = true;

		if(!_sceneName.IsNullOrEmpty() && !_path.Contains(_sceneName))
		{
			checkFile = false;
		}

		if(!_containText.IsNullOrEmpty())
		{
			checkContainString = true;
		}

		if(checkFile)
		{
			for(var i=0;i<fileArray.Length;i++)
			{
				var file = fileArray[i].PathConvertSlash();

				if(checkContainString && !file.Contains(_containText))
				{
					continue;
				}

				_searchPathList.Add(file);
			}
		}

		var directoryArray = Directory.GetDirectories(_path);

		for(var i=0;i<directoryArray.Length;i++)
		{
			var directory = directoryArray[i].PathConvertSlash();

			SearchExtensionInDirectory(directory,_extension,ref _searchPathList,_containText);
		}
	}
}