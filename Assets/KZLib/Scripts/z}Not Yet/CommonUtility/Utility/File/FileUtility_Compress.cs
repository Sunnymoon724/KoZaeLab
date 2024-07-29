#if UNITY_EDITOR
using System;
using System.IO;
using System.IO.Compression;

public static partial class FileUtility
{
	/// <summary>
	/// Assets 내부 폴더는 압축되지 않습니다.
	/// </summary>
	public static void CompressZip(string _sourcePath,string _destinationPath)
	{
		var sourcePath = GetAbsolutePath(_sourcePath,false);

		if(!IsExist(sourcePath,true))
		{
			return;
		}

		var extension = GetExtension(_destinationPath);

		if(extension.IsEmpty())
		{
			_destinationPath = string.Format("{0}.zip",_destinationPath);
		}
		else if(!extension.IsEqual(".zip"))
		{
			throw new ArgumentException(string.Format("지원하지 않는 확장자 입니다. [{0}]",_destinationPath));
		}

		var destinationPath = GetUniquePath(GetAbsolutePath(_destinationPath,false));

		CreateFolder(destinationPath);

		if(IsFilePath(sourcePath))
		{
			using var zipArchive = ZipFile.Open(destinationPath,ZipArchiveMode.Create);
			zipArchive.CreateEntryFromFile(sourcePath,Path.GetFileName(sourcePath));
		}
		else
		{
			ZipFile.CreateFromDirectory(sourcePath,destinationPath);
		}
	}

	/// <summary>
	/// Assets 내부 폴더는 압축 해제되지 않습니다.
	/// </summary>
	public static void DecompressZip(string _sourcePath,string _destinationPath)
	{
		var extension = GetExtension(_sourcePath);

		if(!extension.IsEqual(".zip"))
		{
			throw new ArgumentException(string.Format("지원하지 않는 확장자 입니다. [{0}]",_sourcePath));
		}

		var sourcePath = GetAbsolutePath(_sourcePath,false);

		if(!IsExist(sourcePath,true))
		{
			return;
		}

		var destinationPath = GetUniquePath(GetAbsolutePath(_destinationPath,false));

		CreateFolder(destinationPath);

		using var archive = ZipFile.OpenRead(sourcePath);

		foreach(var entry in archive.Entries)
		{
			var fileName = PathCombine(destinationPath,entry.FullName);

			CreateFolder(fileName);

			entry.ExtractToFile(fileName,true);
		}
	}
}
#endif