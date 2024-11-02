using System;
using System.IO;

public static partial class CommonUtility
{
	private const int KILO_BYTE = 1 << 10;
	private const int MEGA_BYTE = KILO_BYTE * KILO_BYTE;

	public static long GetFileSizeByte(string _filePath)
	{
		IsFileExist(_filePath,true);

		return new FileInfo(_filePath).Length;
	}

	public static long GetFileSizeKB(string _filePath)
	{
		return (long) (GetFileSizeByte(_filePath)/(double)KILO_BYTE);
	}

	public static long GetFileSizeMegaByte(string _filePath)
	{
		return (long) (GetFileSizeByte(_filePath)/(double)MEGA_BYTE);
	}
}