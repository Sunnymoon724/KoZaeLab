using System;
using System.IO;

public static partial class CommonUtility
{
	/// <param name="_filePath">The absolute file path.</param>
	public static string ReadFileToText(string _filePath)
	{
		if(!IsFileExist(_filePath,true))
		{
			return null;
		}

		return ReadFile(_filePath,File.ReadAllText);
	}

	/// <param name="_filePath">The absolute file path.</param>
	public static byte[] ReadFileToBytes(string _filePath)
	{
		if(!IsFileExist(_filePath,true))
		{
			return null;
		}

		return ReadFile(_filePath,File.ReadAllBytes);
	}

	private static TRead ReadFile<TRead>(string _filePath,Func<string,TRead> _onRead)
	{
		return _onRead(_filePath);
	}
}