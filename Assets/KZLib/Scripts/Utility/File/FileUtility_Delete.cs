#if UNITY_EDITOR
using System;
using System.IO;

public static partial class FileUtility
{
	public static void DeleteEmptyDirectory(string _startPath,Action _onComplete = null)
	{
		DeleteEmptyDirectoryInner(_startPath);

		_onComplete?.Invoke();
	}

	private static void DeleteEmptyDirectoryInner(string _startPath)
	{
		foreach(var directory in Directory.GetDirectories(_startPath))
		{
			DeleteEmptyDirectoryInner(directory);

			if(Directory.GetDirectories(directory).Length > 0)
			{
				continue;
			}

			var fileArray = Directory.GetFileSystemEntries(directory);

			if(fileArray.Length == 0 || (fileArray.Length == 1 && fileArray[0].EndsWith(".DS_Store")))
			{
				foreach(var file in fileArray)
				{
					File.Delete(file);
				}

				File.Delete(string.Format("{0}.meta",directory));

				Directory.Delete(directory,false);
			}
		}
	}

	public static bool DeleteFile(string _fullPath)
	{
		if(!IsExist(_fullPath))
		{
			return false;
		}

		File.Delete(_fullPath);

		var meta = string.Format("{0}.meta",_fullPath);

		return DeleteFile(meta);
	}
}
#endif