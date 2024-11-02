using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static partial class CommonUtility
{
	/// <summary>
	/// Encode UTF8 -> Encrypt by AES -> Convert Base64
	/// </summary>
	public static string AESEncryptData(string _password,string _source)
	{
		if(_source.IsEmpty())
		{
			return	_source;
		}

		var secretKey = new PasswordDeriveBytes(_password,Encoding.UTF8.GetBytes(_password.Length.ToString()));
		var managed = new RijndaelManaged
		{
			BlockSize	= 128,
			KeySize		= 256,
			Mode		= CipherMode.CBC,
			Padding		= PaddingMode.PKCS7,
			Key			= secretKey.GetBytes(32),
			IV			= secretKey.GetBytes(16),
		};
		var encrypt = managed.CreateEncryptor(managed.Key,managed.IV);
		byte[] buffer = null;

		using(var stream = new MemoryStream())
		{
			using(var crypto = new CryptoStream(stream,encrypt,CryptoStreamMode.Write))
			{
				var data = Encoding.UTF8.GetBytes(_source);

				crypto.Write(data,0,data.Length);
			}

			buffer = stream.ToArray();
		}

		return Convert.ToBase64String(buffer);
	}

	/// <summary>
	/// Convert Base64 -> Decrypt by AES -> Encode UTF8
	/// </summary>
	public static string AESDecryptData(string _password,string _source)
	{
		if(_source.IsEmpty())
		{
			return	_source;
		}

		var secretKey = new PasswordDeriveBytes(_password,Encoding.UTF8.GetBytes(_password.Length.ToString()));

		var managed = new RijndaelManaged
		{
			BlockSize	= 128,
			KeySize		= 256,
			Mode		= CipherMode.CBC,
			Padding		= PaddingMode.PKCS7,
			Key			= secretKey.GetBytes(32),
			IV			= secretKey.GetBytes(16),
		};

		var decrypt = managed.CreateDecryptor(managed.Key,managed.IV);
		byte[] buffer = null;

		using(var stream = new MemoryStream())
		{
			using(var crypto = new CryptoStream(stream,decrypt,CryptoStreamMode.Write))
			{
				var data = Convert.FromBase64String(_source);
				
				crypto.Write(data,0,data.Length);
			}

			buffer = stream.ToArray();
		}

		return Encoding.UTF8.GetString(buffer);
	}
	
	public static string Base64Encode(string _source)
	{
		return Convert.ToBase64String(Encoding.UTF8.GetBytes(_source));
	}

	public static string Base64Decode(string _source)
	{
		return Encoding.UTF8.GetString(Convert.FromBase64String(_source));
	}

	public static byte[] Base64DecodeToBytes(string _source)
	{
		return Convert.FromBase64String(_source);
	}

	public static string HMACSignature(byte[] _key,string _sign)
	{
		var data = new HMACSHA256()
		{
			Key = _key,
		};

		return Convert.ToBase64String(data.ComputeHash(Encoding.UTF8.GetBytes(_sign)));
	}

	public static string GetCheckSum(string _filePath)
	{
		IsFileExist(_filePath);

		using var stream = File.OpenRead(_filePath);
		var md5 = new MD5CryptoServiceProvider();
		var byteCheckSum = md5.ComputeHash(stream);

		return BitConverter.ToString(byteCheckSum).Replace("-", string.Empty);
	}
}