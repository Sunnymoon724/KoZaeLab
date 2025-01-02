#if UNITY_EDITOR
using System;
using System.IO;

public static partial class CommonUtility
{
	/// <param name="startPath">The absolute path of the folder.</param>
	public static void DeleteEmptyDirectory(string startPath,Action onComplete = null)
	{
		if(!IsFolderExist(startPath,true))
		{
			return;
		}

		_DeleteEmptyDirectory(startPath);

		onComplete?.Invoke();
	}

	private static void _DeleteEmptyDirectory(string startPath)
	{
		foreach(var folderPath in GetFolderPathArray(startPath))
		{
			_DeleteEmptyDirectory(folderPath);

			var innerFolderPathArray = GetFolderPathArray(folderPath);

			if(innerFolderPathArray.Length > 0)
			{
				continue;
			}

			var filePathArray = Directory.GetFileSystemEntries(folderPath);

			if(filePathArray.Length == 0 || (filePathArray.Length == 1 && filePathArray[0].EndsWith(".DS_Store")))
			{
				foreach(var filePath in filePathArray)
				{
					DeleteFile(filePath);
				}

				DeleteFile($"{folderPath}.meta");

				Directory.Delete(folderPath,false);
			}
		}
	}

	/// <param name="filePath">The absolute path of the file.</param>
	public static void DeleteFile(string filePath,Action onComplete = null)
	{
		if(!IsFileExist(filePath,true))
		{
			return;
		}

		File.Delete(filePath);

		if(filePath.Contains(".meta"))
		{
			return;
		}

		var meta = $"{filePath}.meta";

		DeleteFile(meta);

		onComplete?.Invoke();
	}
}
#endif