using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static partial class CommonUtility
{
	#region AES
	public static string EncryptAES(string source,byte[] key)
	{
		if(source.IsEmpty())
		{
			LogTag.System.E("Source is empty");

			return source;
		}

		if(key == null)
		{
			LogTag.System.E("Key cannot be null.");

			return source;
		}

		if(key.Length != 16 && key.Length != 24 && key.Length != 32)
		{
			LogTag.System.E("Key must be 16 or 24 or 32 bytes.");

			return source;
		}

		using var aes = Aes.Create();
		aes.Key = key;
		aes.GenerateIV();

		using var encryptor = aes.CreateEncryptor(aes.Key,aes.IV);
		using var memoryStream = new MemoryStream();

		memoryStream.Write(aes.IV,0,aes.IV.Length);

		using(var cryptoStream = new CryptoStream(memoryStream,encryptor,CryptoStreamMode.Write))
		{
			using var writer = new StreamWriter(cryptoStream);

			writer.Write(source);
		}

		return Convert.ToBase64String(memoryStream.ToArray());
	}

	public static string DecryptAES(string source,byte[] key)
	{
		if(source.IsEmpty())
		{
			LogTag.System.E("Source is empty");

			return source;
		}

		if(key == null)
		{
			LogTag.System.E("Key cannot be null.");

			return source;
		}

		if(key.Length != 16 && key.Length != 24 && key.Length != 32)
		{
			LogTag.System.E("Key must be 16 or 24 or 32 bytes.");

			return source;
		}

		using var aes = Aes.Create();
		var data = Convert.FromBase64String(source);

		using var memoryStream = new MemoryStream(data);
		var iv = new byte[16];
		memoryStream.Read(iv,0,iv.Length);

		aes.Key = key;
		aes.IV = iv;

		using var decryptor = aes.CreateDecryptor(aes.Key,aes.IV);
		using var cryptoStream = new CryptoStream(memoryStream,decryptor,CryptoStreamMode.Read);
		using var reader = new StreamReader(cryptoStream);

		return reader .ReadToEnd();
	}

	public static byte[] GenerateAES16Key()
	{
		return GenerateAESKey(16);
	}

	public static byte[] GenerateAES24Key()
	{
		return GenerateAESKey(24);
	}

	public static byte[] GenerateAES32Key()
	{
		return GenerateAESKey(32);
	}

	public static byte[] GenerateAESRandomKey()
	{
		var sizeArray = new int[] { 16, 24, 32 };

		return GenerateAESKey(sizeArray[GenerateRandomInt(0,3)]);
	}

	private static byte[] GenerateAESKey(int size)
	{
		if(size != 16 && size != 24 && size != 32)
		{
			throw new ArgumentException("Key must be 16 or 24 or 32 bytes.");
		}

		using var rng = new RNGCryptoServiceProvider();
		byte[] key = new byte[size];
		rng.GetBytes(key);

		return key;
	}

	public static byte[] GenerateAESKeyByPassword(string password)
	{
		using var keyDerivation = new Rfc2898DeriveBytes(password,new byte[0],10000);
		var key = keyDerivation.GetBytes(16);

		return key;
	}
	#endregion AES

	#region RSA
	public static string EncryptRSA(string source,string publicKey)
	{
		if(source.IsEmpty())
		{
			LogTag.Security.E("Source is empty");

			return source;
		}

		if(publicKey.IsEmpty())
		{
			LogTag.Security.E("PublicKey is empty");

			return source;
		}

		using RSA rsa = RSA.Create();
		rsa.ImportRSAPublicKey(Convert.FromBase64String(publicKey),out _);

		var encrypt = Encoding.UTF8.GetBytes(source);

		return Convert.ToBase64String(rsa.Encrypt(encrypt,RSAEncryptionPadding.OaepSHA256));
	}

	public static string DecryptRSA(string source,string privateKey)
	{
		if(source.IsEmpty())
		{
			LogTag.Security.E("Source is empty");

			return source;
		}

		if(privateKey.IsEmpty())
		{
			LogTag.Security.E("PrivateKey is empty");

			return source;
		}

		using RSA rsa = RSA.Create();
		rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey),out _);

		var decrypt = rsa.Decrypt(Convert.FromBase64String(source),RSAEncryptionPadding.OaepSHA256);

		return Encoding.UTF8.GetString(decrypt);
	}

	public static void GenerateRSAKey(out string publicKey,out string privateKey)
	{
		using RSA rsa = RSA.Create();

		publicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());
		privateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());
	}
	#endregion RSA

	#region HMAC
	public static string GenerateHMACSHA256Hash(string input,string secretKey)
	{
		if(input.IsEmpty())
		{
			LogTag.Security.E("Input is empty.");

			return input;
		}

		if(secretKey.IsEmpty())
		{
			LogTag.Security.E("SecretKey is empty.");

			return input;
		}

		using var data = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
		var result = data.ComputeHash(Encoding.UTF8.GetBytes(input));

		return BitConverter.ToString(result).Replace("-","").ToLower();
	}
	#endregion HMAC

	#region CheckSum
	public static string GetCheckSum(string filePath)
	{
		IsFileExist(filePath);

		using var stream = File.OpenRead(filePath);
		var md5 = new MD5CryptoServiceProvider();
		var byteCheckSum = md5.ComputeHash(stream);

		return BitConverter.ToString(byteCheckSum).Replace("-", string.Empty);
	}
	#endregion CheckSum

	#region Encode
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
	#endregion Encode
}