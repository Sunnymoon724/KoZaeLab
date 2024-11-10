using System.Collections.Generic;

public static partial class CommonUtility
{
	/// <summary>
	/// .meta is not included
	/// </summary>
	/// <param name="_folderPath">The absolute folder path.</param>
	public static IEnumerable<string> GetAllFilePathInFolder(string _folderPath,bool _includeSubFolders = false)
	{
		if(!IsFolderExist(_folderPath,true))
		{
			yield break;
		}

		if(_includeSubFolders)
		{
			foreach(var folderPath in GetFolderPathArray(_folderPath))
			{
				foreach(var filePath in GetAllFilePathInFolder(folderPath,true))
				{
					yield return filePath;
				}
			}
		}

		foreach(var filePath in GetFilePathArray(_folderPath))
		{
			if(filePath.EndsWith(".meta"))
			{
				continue;
			}

			yield return filePath;
		}
	}

	/// <param name="_folderPath">The absolute folder path.</param>
	public static string SearchFileInFolder(string _folderPath,string _fileName)
	{
		if(!IsFolderExist(_folderPath,true))
		{
			return null;
		}

		foreach(var filePath in GetFilePathArray(_folderPath))
		{
			var fileName = GetFileName(filePath);

			if(fileName.IsEqual(_fileName))
			{
				return filePath;
			}
		}

		foreach(var folderPath in GetFolderPathArray(_folderPath))
		{
			var result = SearchFileInFolder(folderPath,_fileName);

			if(result != null)
			{
				return result;
			}
		}

		return null;
	}

	/// <param name="_folderPath">The absolute folder path.</param>
	public static IEnumerable<string> SearchExtensionInFolder(string _folderPath,string _extension)
	{
		if(!IsFolderExist(_folderPath,true))
		{
			yield break;
		}

		foreach(var filePath in GetFilePathArray(_folderPath,_extension))
		{
			yield return filePath;
		}

		foreach(var folderPath in GetFolderPathArray(_folderPath))
		{
			foreach(var filePath in SearchExtensionInFolder(folderPath,_extension))
			{
				yield return filePath;
			}
		}
	}
}