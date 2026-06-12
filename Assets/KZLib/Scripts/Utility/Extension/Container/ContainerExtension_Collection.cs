using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Extension methods for collections, dictionaries, enumerables, and lists.
/// Provides safe mutation, indexing, searching, and formatting helpers.
/// </summary>
public static partial class ContainerExtension
{
	/// <summary>
	/// Adds a value only when it is not already contained in the collection.
	/// </summary>
	public static void AddNotOverlap<TValue>(this ICollection<TValue> collection,TValue val)
	{
		if(!_IsValid(collection))
		{
			return;
		}

		if(!collection.Contains(val))
		{
			collection.Add(val);
		}
	}

	public static void AddCount<TValue>(this ICollection<TValue> collection,TValue val,int cnt)
	{
		if(!_IsValid(collection))
		{
			return;
		}

		for(var i=0;i<cnt;i++)
		{
			collection.Add(val);
		}
	}

	public static bool ContainsIndex<TValue>(this ICollection<TValue> collection,int idx)
	{
		return _IsValid(collection) && 0 <= idx && idx < collection.Count;
	}

	/// <summary>
	/// Returns the middle element, using floor division for even counts.
	/// </summary>
	public static TValue Middle<TValue>(this ICollection<TValue> collection)
	{
		if(!_IsValid(collection))
		{
			return default;
		}

		var count = collection.Count;

		if(count == 0)
		{
			LogChannel.Kit.E("Collection is empty");

			return default;
		}

		if(count == 1)
		{
			return collection._GetValue(0);
		}

		var index = collection.Count/2;

		return collection._GetValue(index);
	}

	public static bool RemoveSafe<TValue>(this ICollection<TValue> collection,TValue val)
	{
		return _IsValid(collection) && collection.Contains(val) && collection.Remove(val);
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

	/// <summary>
	/// Gets an element by index for collections that do not expose list indexing directly.
	/// </summary>
	public static TValue GetValueByIndex<TValue>(this ICollection<TValue> collection,int idx)
	{
		return _IsValid(collection) && collection.TryGetValueByIndex(idx, out var value) ? value : default;
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

	/// <summary>
	/// Clears the collection and inserts a single replacement value.
	/// </summary>
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
