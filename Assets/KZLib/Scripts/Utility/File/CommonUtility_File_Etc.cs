using System.IO;

public static partial class CommonUtility
{
	private const int KILO_BYTE = 1 << 10;
	private const int MEGA_BYTE = KILO_BYTE * KILO_BYTE;

	public static long GetFileSizeByte(string filePath)
	{
		return !IsFileExist(filePath,true) ? 0L : new FileInfo(filePath).Length;
	}

	public static long GetFileSizeKB(string filePath)
	{
		return (long) (GetFileSizeByte(filePath)/(double)KILO_BYTE);
	}

	public static long GetFileSizeMegaByte(string filePath)
	{
		return (long) (GetFileSizeByte(filePath)/(double)MEGA_BYTE);
	}
}