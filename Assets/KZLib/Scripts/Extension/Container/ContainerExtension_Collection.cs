using System;
using System.Collections.Generic;
using UnityEngine;

public static partial class ContainerExtension
{
	public static void AddNotOverlap<TValue>(this ICollection<TValue> collection,TValue value)
	{
		if(!IsValid(collection))
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
		if(!IsValid(collection))
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
		if(!IsValid(collection))
		{
			return false;
		}

		return 0 <= index && index < collection.Count;
	}


	public static TValue Middle<TValue>(this ICollection<TValue> collection)
	{
		if(!IsValid(collection))
		{
			return default;
		}

		var count = collection.Count;

		if(count == 0)
		{
			LogTag.System.E("Collection is empty");

			return default;
		}

		if(count == 1)
		{
			return collection.GetValue(1);
		}

		var index = Mathf.RoundToInt(collection.Count/2);

		return collection.GetValue(index);
	}

	public static bool RemoveSafe<TValue>(this ICollection<TValue> collection,TValue value)
	{
		if(!IsValid(collection))
		{
			return false;
		}

		return collection.Contains(value) && collection.Remove(value);
	}

	public static void RemoveRange<TValue>(this ICollection<TValue> collection,IList<TValue> valueList)
	{
		if(!IsValid(collection))
		{
			return;
		}

		for(var i=0;i<valueList.Count;i++)
		{
			collection.RemoveSafe(valueList[i]);
		}
	}

	public static bool RemoveRandomValue<TValue>(this ICollection<TValue> collection,out TValue value)
	{
		if(!IsValid(collection))
		{
			value = default;

			return false;
		}

		var count = collection.Count;

		if(count == 1)
		{
			value = collection.GetFirstValue();

			return true;
		}

		value = collection.GenerateRandomValue();
		collection.Remove(value);

		return true;
	}

	public static TValue GetValueByIndex<TValue>(this ICollection<TValue> collection,int index)
	{
		if(!IsValid(collection))
		{
			return default;
		}

		return collection.TryGetValueByIndex(index,out var value) ? value : default;
	}

	public static bool TryGetValueByIndex<TValue>(this ICollection<TValue> collection,int index,out TValue value)
	{
		if(!IsValid(collection))
		{
			value = default;

			return false;
		}

		if(collection.ContainsIndex(index))
		{
			value = collection.GetValue(index);

			return true;
		}
		else
		{
			value = default;

			return false;
		}
	}

	public static void Initialize<TValue>(this ICollection<TValue> collection,TValue value)
	{
		if(!IsValid(collection))
		{
			return;
		}

		collection.Clear();
		collection.Add(value);
	}
}