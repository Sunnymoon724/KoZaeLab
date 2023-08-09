// using System.IO;
// using System.Security.Cryptography;
// using System.Text;
// using UnityEngine;

// public static class KeyGenerator
// {
// 	private enum CRYPTOGRAPHY_TYPE {CRC32, MD5, SHA1, SHA256, SHA384, SHA512,}

// 	#region CRC32
// 	public static int GenerateKeyCrc32(string _path)
// 	{
// 		var key = GenerateKey(_path,CRYPTOGRAPHY_TYPE.CRC32);

// 		return key.IsEmpty() ? 0 : key.ToDecimal();
// 	}
// 	#endregion CRC32

// 	#region MD5
// 	public static string GenerateKeyMD5(string _path)
// 	{
// 		return GenerateKey(_path,CRYPTOGRAPHY_TYPE.MD5);
// 	}

// 	public static string GenerateKeyMD5(byte[] _bytes)
// 	{
// 		return GenerateKeyFromBuffer(_bytes,CRYPTOGRAPHY_TYPE.MD5);
// 	}

// 	public static string GenerateKeyMD5(Object _object)
// 	{
// 		return GenerateKeyFromBuffer(Tools.GetBytesFromObject(_object),CRYPTOGRAPHY_TYPE.MD5);
// 	}
// 	#endregion MD5

// 	#region SHA1
// 	public static string GenerateKeySHA1(string _path)
// 	{
// 		return GenerateKey(_path,CRYPTOGRAPHY_TYPE.SHA1);
// 	}

// 	public static string GenerateKeySHA1(byte[] bytes)
// 	{
// 		return GenerateKeyFromBuffer(bytes, CRYPTOGRAPHY_TYPE.SHA1);
// 	}

// 	public static string GenerateKeySHA1(Object _object)
// 	{
// 		return GenerateKeyFromBuffer(Tools.GetBytesFromObject(_object),CRYPTOGRAPHY_TYPE.SHA1);
// 	}
// 	#endregion SHA1

// 	#region SHA256	
// 	public static string GenerateKeySHA256(string _path)
// 	{
// 		return GenerateKey(_path,CRYPTOGRAPHY_TYPE.SHA256);
// 	}
// 	#endregion SHA256

// 	#region SHA384	
// 	public static string GenerateKeySHA384(string _path)
// 	{
// 		return GenerateKey(_path,CRYPTOGRAPHY_TYPE.SHA384);
// 	}
// 	#endregion SHA384

// 	#region SHA512	
// 	public static string GenerateKeySHA512(string _path)
// 	{
// 		return GenerateKey(_path,CRYPTOGRAPHY_TYPE.SHA512);
// 	}
// 	#endregion SHA512

// 	private static string GenerateKey(string _path,CRYPTOGRAPHY_TYPE _type)
// 	{
// 		return Tools.IsExistFile(_path) ? GenerateKeyFromBuffer(File.ReadAllBytes(_path),_type) : null;
// 	}

// 	private static string GenerateKeyFromBuffer(byte[] _bytes,CRYPTOGRAPHY_TYPE _type)
// 	{
// 		HashAlgorithm hash = null;

// 		switch(_type)
// 		{
// 			case CRYPTOGRAPHY_TYPE.CRC32:
// 				hash = new CRC32HashAlgorithm();
// 				break;
// 			case CRYPTOGRAPHY_TYPE.MD5:
// 				hash = new MD5CryptoServiceProvider();
// 				break;
// 			case CRYPTOGRAPHY_TYPE.SHA1:
// 				hash = new SHA1Managed();
// 				break;
// 			case CRYPTOGRAPHY_TYPE.SHA256:
// 				hash = new SHA256Managed();
// 				break;
// 			case CRYPTOGRAPHY_TYPE.SHA384:
// 				hash = new SHA384Managed();
// 				break;
// 			case CRYPTOGRAPHY_TYPE.SHA512:
// 				hash = new SHA512Managed();
// 				break;
// 			default:
// 				return string.Empty;
// 		}

// 		return BufferToHashHexString(hash.ComputeHash(_bytes));
// 	}
	
// 	private static string BufferToHashHexString(byte[] _buffer)
// 	{
// 		var iterator = _buffer.GetEnumerator();
// 		var builder = new StringBuilder();

// 		while(iterator.MoveNext())
// 		{
// 			builder.Append(string.Format("{0:X2}",(byte)iterator.Current));
// 		}

// 		return builder.ToString();
// 	}
// }