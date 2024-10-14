using System;
using System.IO;

public static partial class FileUtility
{
	public static string ReadFileToText(string _filePath)
	{
		return ReadFile(_filePath,File.ReadAllText);
	}

	public static byte[] ReadFileToBytes(string _filePath)
	{
		return ReadFile(_filePath,File.ReadAllBytes);
	}

	private static TRead ReadFile<TRead>(string _filePath,Func<string,TRead> _onRead)
	{
		if(IsExist(_filePath,true))
		{
			try
			{
				return _onRead(_filePath);
			}
			catch(Exception _ex)
			{
				LogTag.File.E($"Error reading file: {_ex.Message}");
			}
		}

		return default;
	}
}