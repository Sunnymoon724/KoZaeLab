using System.Text;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// Extension methods for Unity <see cref="Object"/> instantiation, destruction, and serialization.
/// </summary>
public static class ObjectExtension
{
	/// <summary>
	/// Instantiates a copy of the object, optionally parented, preserving the original name.
	/// </summary>
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

	/// <summary>
	/// Destroys the object immediately in edit mode or at end of frame during play mode.
	/// </summary>
	public static void DestroyObject(this Object target)
	{
		if(!_IsValid(target,false))
		{
			return;
		}

		if(Application.isPlaying)
		{
			Object.Destroy(target);
		}
		else
		{
			Object.DestroyImmediate(target);
		}
	}

	/// <summary>
	/// Serializes the object to JSON and returns the result as UTF-8 bytes.
	/// </summary>
	public static byte[] ToByte(this Object target)
	{
		if(!_IsValid(target,true))
		{
			return null;
		}

		var json = JsonConvert.SerializeObject(target);

		if(json.IsEmpty(true) || json == "{}")
		{
			LogChannel.Kit.W($"{target.GetType().Name} serialized to empty JSON. Check that public fields or properties are exposed for serialization.");
		}

		return Encoding.UTF8.GetBytes(json);
	}

	private static bool _IsValid(Object value,bool isShowLog)
	{
		if(!value)
		{
			if(isShowLog)
			{
				LogChannel.Kit.E("Object is null");
			}

			return false;
		}

		return true;
	}
}