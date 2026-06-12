using System;
using System.Text;

/// <summary>
/// Utility methods for Base64 encoding and decoding of UTF-8 strings.
/// </summary>
public static class KZStringKit
{
	/// <summary>
	/// Encodes a UTF-8 string to its Base64 representation.
	/// </summary>
	public static string Base64Encode(string source)
	{
		if(source.IsEmpty(true))
		{
			throw new ArgumentNullException($"{nameof(source)} is empty. Source must be assigned.");
		}

		return Convert.ToBase64String(Encoding.UTF8.GetBytes(source));
	}

	/// <summary>
	/// Decodes a Base64 string back to a UTF-8 string.
	/// </summary>
	public static string Base64Decode(string source)
	{
		if(source.IsEmpty(true))
		{
			throw new ArgumentNullException($"{nameof(source)} is empty. Source must be assigned.");
		}

		return Encoding.UTF8.GetString(Convert.FromBase64String(source));
	}

	/// <summary>
	/// Decodes a Base64 string into raw bytes.
	/// </summary>
	public static byte[] Base64DecodeToBytes(string source)
	{
		if(source.IsEmpty(true))
		{
			throw new ArgumentNullException($"{nameof(source)} is empty. Source must be assigned.");
		}

		return Convert.FromBase64String(source);
	}
}
