using System;
using System.Text;

public static partial class CommonUtility
{
	public static string Base64Encode(string source)
	{
		return Convert.ToBase64String(Encoding.UTF8.GetBytes(source));
	}

	public static string Base64Decode(string source)
	{
		return Encoding.UTF8.GetString(Convert.FromBase64String(source));
	}

	public static byte[] Base64DecodeToBytes(string source)
	{
		return Convert.FromBase64String(source);
	}
}