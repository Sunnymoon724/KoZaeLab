using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class ObjectExtension
{
	public static Object CopyObject(this Object origin,Transform parent = null)
	{
		if(!_IsValid(origin,true))
		{
			return null;
		}

		var instance = Object.Instantiate(origin,parent);
		instance.name = origin.name;

		return instance;
	}

	public static void DestroyObject(this Object origin)
	{
		if(!_IsValid(origin,false))
		{
			return;
		}

		if(Application.isPlaying)
		{
			Object.Destroy(origin);
		}
		else
		{
			Object.DestroyImmediate(origin);
		}
	}

	public static byte[] ToByte(this Object origin)
	{
		if(!_IsValid(origin,true))
		{
			return null;
		}

		using var stream = new MemoryStream();
		var binary = new BinaryFormatter();

		binary.Serialize(stream,origin);

		return stream.ToArray();
	}

	private static bool _IsValid(Object value,bool isShowLog)
	{
		if(!value)
		{
			if(isShowLog)
			{
				LogTag.System.E("Object is null");
			}

			return false;
		}

		return true;
	}
}