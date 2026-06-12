/// <summary>
/// Utility methods for formatting network transfer metrics.
/// </summary>
public static class KZNetworkKit
{
	/// <summary>
	/// Formats a byte-per-second rate as B/s or MB/s depending on magnitude.
	/// </summary>
	public static string GetDownloadSpeed(long bytesPerSecond)
	{
		var size = bytesPerSecond/1024.0d;

		return size > 0.0d ? $"{size / 1024.0d:f2} MB/s" : $"{bytesPerSecond} B/s";
	}
}
