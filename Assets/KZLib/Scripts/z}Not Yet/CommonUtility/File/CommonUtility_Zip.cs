#if UNITY_EDITOR
using System.IO.Compression;

public static partial class CommonUtility
{
	public static void ZipDirectory(string _sourceDirectoryPath,string _destinationDirectoryPath)
	{
		var sourcePath = GetFullPath(_sourceDirectoryPath);

		IsExistFolder(sourcePath,true);

		var destinationPath = GetFullPath(_destinationDirectoryPath);

		CreateFolder(destinationPath);

		ZipFile.CreateFromDirectory(sourcePath,string.Format("{0}/{1}.zip",destinationPath,GetOnlyName(sourcePath)));
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