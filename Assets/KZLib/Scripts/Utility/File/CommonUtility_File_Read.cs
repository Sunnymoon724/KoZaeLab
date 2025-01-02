using System;
using System.IO;

public static partial class CommonUtility
{
	/// <param name="filePath">The absolute file path.</param>
	public static string ReadFileToText(string filePath)
	{
		if(!IsFileExist(filePath,true))
		{
			return null;
		}

		return ReadFile(filePath,File.ReadAllText);
	}

	/// <param name="filePath">The absolute file path.</param>
	public static byte[] ReadFileToBytes(string filePath)
	{
		if(!IsFileExist(filePath,true))
		{
			return null;
		}

		return ReadFile(filePath,File.ReadAllBytes);
	}

	private static TRead ReadFile<TRead>(string filePath,Func<string,TRead> _onRead)
	{
		return _onRead(filePath);
	}
}