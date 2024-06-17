#if UNITY_EDITOR
using System;
using System.IO;

public static partial class CommonUtility
{
	// 비어 있는 서브 디렉토리 모두 삭제하기
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

	/// <summary>
	/// 해당 파일 제거
	/// </summary>
	public static bool DeleteFile(string _fullPath)
	{
		if(!IsExistFile(_fullPath))
		{
			return false;
		}

		File.Delete(_fullPath);

		var meta = string.Format("{0}.meta",_fullPath);

		return DeleteFile(meta);
	}
}
#endif