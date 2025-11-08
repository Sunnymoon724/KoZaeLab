using System.Collections.Generic;
using UnityEngine;

public static partial class ContainerExtension
{
	public static void AddNotOverlap<TValue>(this ICollection<TValue> collection,TValue value)
	{
		if(!_IsValid(collection))
		{
			return;
		}

		if(!collection.Contains(value))
		{
			collection.Add(value);
		}
	}

	public static void AddCount<TValue>(this ICollection<TValue> collection,TValue value,int count)
	{
		if(!_IsValid(collection))
		{
			return;
		}

		for(var i=0;i<count;i++)
		{
			collection.Add(value);
		}
	}

	public static bool ContainsIndex<TValue>(this ICollection<TValue> collection,int index)
	{
		return _IsValid(collection) && 0 <= index && index < collection.Count;
	}

	public static TValue Middle<TValue>(this ICollection<TValue> collection)
	{
		if(!_IsValid(collection))
		{
			return default;
		}

		var count = collection.Count;

		if(count == 0)
		{
			LogSvc.System.E("Collection is empty");

			return default;
		}

		if(count == 1)
		{
			return collection._GetValue(1);
		}

		var index = Mathf.RoundToInt(collection.Count/2);

		return collection._GetValue(index);
	}

	public static bool RemoveSafe<TValue>(this ICollection<TValue> collection,TValue value)
	{
		return _IsValid(collection) && collection.Contains(value) && collection.Remove(value);
	}

	public static void RemoveRange<TValue>(this ICollection<TValue> collection,IList<TValue> valueList)
	{
		if(!_IsValid(collection))
		{
			return;
		}

		for(var i=0;i<valueList.Count;i++)
		{
			collection.RemoveSafe(valueList[i]);
		}
	}

	public static TValue GetValueByIndex<TValue>(this ICollection<TValue> collection,int index)
	{
		return _IsValid(collection) && collection.TryGetValueByIndex(index, out var value) ? value : default;
	}

	public static bool TryGetValueByIndex<TValue>(this ICollection<TValue> collection,int index,out TValue value)
	{
		value = default;

		if(!_IsValid(collection) || !collection.ContainsIndex(index))
		{
			return false;
		}

		value = collection._GetValue(index);

		return true;
	}

	public static void Initialize<TValue>(this ICollection<TValue> collection,TValue value)
	{
		if(!_IsValid(collection))
		{
			return;
		}

		collection.Clear();
		collection.Add(value);
	}
}