using System.IO;

public static partial class CommonUtility
{
	public static byte[] ReadFile(string _filePath)
	{
		var fullPath = GetFullPath(_filePath);

		IsExistFile(fullPath,true);

		return File.ReadAllBytes(fullPath);
	}

	public static bool TryReadDataFromFile(string _filePath,out string _text)
	{
		var fullPath = GetFullPath(_filePath);

		IsExistFile(fullPath,true);

		using var fileStream = File.Open(fullPath,FileMode.Open,FileAccess.Read);
		using var stream = new StreamReader(fileStream);

		_text = stream.ReadToEnd();

		return true;
	}

	public static string ReadDataFromFile(string _filePath)
	{
		return TryReadDataFromFile(_filePath,out var result) ? result : string.Empty;
	}
}