using System.IO;

public static partial class FileUtility
{
	public static byte[] ReadFile(string _filePath)
	{
		return IsExist(_filePath,true) ? File.ReadAllBytes(_filePath) : null;
	}

	public static bool TryReadDataFromFile(string _filePath,out string _text)
	{
		if(!IsExist(_filePath,true))
		{
			_text = null;

			return false;
		}

		using var fileStream = File.Open(_filePath,FileMode.Open,FileAccess.Read);
		using var stream = new StreamReader(fileStream);

		_text = stream.ReadToEnd();

		return true;
	}

	public static string ReadDataFromFile(string _filePath)
	{
		return TryReadDataFromFile(_filePath,out var result) ? result : string.Empty;
	}
}