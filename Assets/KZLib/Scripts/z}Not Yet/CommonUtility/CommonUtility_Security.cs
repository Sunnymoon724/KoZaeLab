using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static partial class CommonUtility
{
	/// <summary>
	/// 데이터를 UTF8으로 인코딩 하여 byte[]로 만들고 이걸 password를 받아서 AES로 암호화를 한다.
	/// 그 다음 Base64로 디코딩을 하여 데이터를 변경한다.
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
	/// 데이터를 Base64인코딩을 하여 byte[]로 만들고 이걸 password를 받아서 AES로 복호화를 한다.
	/// 그 다음 UTF8로 바꾸어 원래 데이터로 변경한다.
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

	/// <summary>
	/// HMAC인증을 위한 용도
	/// </summary>
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
		FileUtility.IsExistFile(_filePath,true);

		using var stream = File.OpenRead(_filePath);
		var md5 = new MD5CryptoServiceProvider();
		var byteCheckSum = md5.ComputeHash(stream);

		return BitConverter.ToString(byteCheckSum).Replace("-", string.Empty);
	}
}