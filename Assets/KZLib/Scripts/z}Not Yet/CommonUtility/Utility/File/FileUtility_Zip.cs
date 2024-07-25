#if UNITY_EDITOR
using System;
using System.IO;
using System.IO.Compression;

public static partial class FileUtility
{
	public static void Compress(string _sourcePath,string _destinationPath)
	{
		var sourcePath = GetFullPath(_sourcePath);

		if(IsExist(sourcePath,true))
		{
			return;
		}

		var destinationPath = GetFullPath(_destinationPath);

		if(IsFilePath(destinationPath))
		{
			destinationPath = GetParentPath(destinationPath);
		}

		CreateFolder(destinationPath);

		var filePath = PathCombine(GetFullPath(_destinationPath),string.Format("{0}.zip",GetOnlyName(sourcePath)));

		if(IsFilePath(sourcePath))
		{
			ZipFile.CreateFromDirectory(sourcePath,filePath);
		}
		else
		{
			using var zipStream = new FileStream(filePath,FileMode.Create);
			using var archive = new ZipArchive(zipStream,ZipArchiveMode.Create);
			var entry = archive.CreateEntry(Path.GetFileName(sourcePath));

			using var entryStream = entry.Open();
			using var fileStream = new FileStream(sourcePath,FileMode.Open);
			fileStream.CopyTo(entryStream);
		}
	}

	public static void UnZipFile(string _sourceFilePath,string _destinationDirectoryPath = null)
	{
		var sourcePath = GetFullPath(_sourceFilePath);

		IsExistFile(sourcePath,true);

		var destinationPath = _destinationDirectoryPath.IsEmpty() ? GetPathWithoutExtension(sourcePath) : GetFullPath(_destinationDirectoryPath);
		
		CreateFolder(destinationPath);

		ZipFile.ExtractToDirectory(sourcePath,destinationPath);
	}
}
#endif