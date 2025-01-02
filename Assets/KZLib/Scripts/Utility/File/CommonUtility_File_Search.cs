using System.Collections.Generic;

public static partial class CommonUtility
{
	/// <summary>
	/// .meta is not included
	/// </summary>
	/// <param name="folderPath">The absolute folder path.</param>
	public static IEnumerable<string> FindFilePathGroup(string folderPath,bool includeSubFolder = false)
	{
		if(!IsFolderExist(folderPath,true))
		{
			yield break;
		}

		var folderQueue = new Queue<string>();
		folderQueue.Enqueue(folderPath);

		while(folderQueue.Count > 0)
		{
			var currentFolderPath = folderQueue.Dequeue();

			foreach(var filePath in GetFilePathArray(folderPath))
			{
				if(filePath.EndsWith(".meta"))
				{
					continue;
				}

				yield return filePath;
			}

			if(includeSubFolder)
			{
				foreach(var subFolderPath in GetFolderPathArray(currentFolderPath))
				{
					folderQueue.Enqueue(subFolderPath);
				}
			}
		}
	}

	/// <param name="folderPath">The absolute folder path.</param>
	public static string SearchFileInFolder(string folderPath,string _fileName)
	{
		if(!IsFolderExist(folderPath,true))
		{
			return null;
		}

		foreach(var filePath in GetFilePathArray(folderPath))
		{
			var fileName = GetFileName(filePath);

			if(fileName.IsEqual(_fileName))
			{
				return filePath;
			}
		}

		foreach(var subFolderPath in GetFolderPathArray(folderPath))
		{
			var result = SearchFileInFolder(subFolderPath,_fileName);

			if(result != null)
			{
				return result;
			}
		}

		return null;
	}

	/// <param name="folderPath">The absolute folder path.</param>
	public static IEnumerable<string> SearchExtensionInFolder(string folderPath,string _extension)
	{
		if(!IsFolderExist(folderPath,true))
		{
			yield break;
		}

		foreach(var filePath in GetFilePathArray(folderPath,_extension))
		{
			yield return filePath;
		}

		foreach(var subFolderPath in GetFolderPathArray(folderPath))
		{
			foreach(var filePath in SearchExtensionInFolder(subFolderPath,_extension))
			{
				yield return filePath;
			}
		}
	}
}