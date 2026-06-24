using System;
using System.Collections.Generic;
using System.Text;
using Object = UnityEngine.Object;

/// <summary>
/// Dictionary extension methods for safe removal, merging, nested collections, and formatting.
/// </summary>
public static partial class ContainerExtension
{
	/// <summary>
	/// Removes the key only when it exists in the dictionary.
	/// </summary>
	public static bool RemoveSafe<TKey,TValue>(this IDictionary<TKey,TValue> dictionary,TKey key)
	{
		return _IsValid(dictionary) && key != null && dictionary.ContainsKey(key) && dictionary.Remove(key);
	}

	/// <summary>
	/// Removes a key and outputs its value when the key exists.
	/// </summary>
	public static bool TryRemove<TKey,TValue>(this IDictionary<TKey,TValue> dictionary,TKey key,out TValue value)
	{
		value = default;

		if(!_IsValid(dictionary))
		{
			return false;
		}

		if(!dictionary.TryGetValue(key,out value))
		{
			return false;
		}

		return dictionary.Remove(key);
	}

	/// <summary>
	/// Removes a value from the nested collection stored at <paramref name="key"/>.
	/// </summary>
	public static bool TryRemoveFromNestedCollection<TKey,TValue>(this IDictionary<TKey,ICollection<TValue>> dictionary,TKey key,TValue value)
	{
		return _IsValid(dictionary) && key != null && dictionary.ContainsKey(key) && value != null && dictionary[key].Remove(value);
	}

	/// <summary>
	/// Returns whether the dictionary keys are equal to the given collection.
	/// </summary>
	public static bool AreKeysEqual<TKey,TValue>(this IDictionary<TKey,TValue> dictionary,ICollection<TKey> keyCollection)
	{
		return _IsValid(dictionary) && dictionary.Keys.AreEqual(keyCollection);
	}

	/// <summary>
	/// Returns whether the dictionary values are equal to the given collection.
	/// </summary>
	public static bool AreValuesEqual<TKey,TValue>(this IDictionary<TKey,TValue> dictionary,ICollection<TValue> valueCollection)
	{
		return _IsValid(dictionary) && dictionary.Values.AreEqual(valueCollection);
	}

	/// <summary>
	/// Adds every key/value pair from <paramref name="dictionary2"/> into <paramref name="dictionary1"/>.
	/// </summary>
	public static void AddRange<TKey,TValue>(this IDictionary<TKey,TValue> dictionary1,IDictionary<TKey,TValue> dictionary2)
	{
		if(!_IsValid(dictionary1) || !_IsValid(dictionary2))
		{
			return;
		}

		foreach(var pair in dictionary2)
		{
			dictionary1.Add(pair.Key,pair.Value);
		}
	}

	/// <summary>
	/// Adds values from a collection, skipping entries whose derived key is null or already present.
	/// </summary>
	public static void AddRange<TKey,TValue>(this IDictionary<TKey,TValue> dictionary,ICollection<TValue> valueCollection,Func<TValue,TKey> onFunc,Action onAction = null)
	{
		if(!_IsValid(dictionary))
		{
			return;
		}

		foreach(var value in valueCollection)
		{
			var key = onFunc(value);

			if(key == null || dictionary.ContainsKey(key))
			{
				continue;
			}

			dictionary.Add(key,value);
			onAction?.Invoke();
		}
	}

	/// <summary>
	/// Adds Unity objects to a dictionary keyed by their <see cref="Object.name"/>.
	/// </summary>
	public static void AddRange<TValue>(this IDictionary<string,TValue> dictionary,ICollection<TValue> valueCollection,Action onAction = null) where TValue : Object
	{
		if(_IsValid(dictionary))
		{
			static string _GetKey(TValue value)
			{
				return value.name;
			}

			dictionary.AddRange(valueCollection,_GetKey,onAction);
		}
	}

	/// <summary>
	/// Ensures a nested collection exists for the key, then appends the value to it.
	/// </summary>
	public static void AddOrCreate<TKey,TValue,TCollection>(this IDictionary<TKey,TCollection> dictionary,TKey key,TValue value) where TCollection : ICollection<TValue>,new()
	{
		if(!_IsValid(dictionary))
		{
			return;
		}

		if(!dictionary.TryGetValue(key,out var collection))
		{
			collection = new TCollection();
			dictionary.Add(key,collection);
		}

		collection.Add(value);
	}

	/// <summary>
	/// Sorts each nested list or array value in place using the supplied comparer.
	/// </summary>
	public static void SortEachList<TKey,TValue>(this IDictionary<TKey,IList<TValue>> dictionary,IComparer<TValue> comparer)
	{
		if(!_IsValid(dictionary))
		{
			return;
		}

		foreach(var pair in dictionary)
		{
			if(pair.Value is List<TValue> list)
			{
				list.Sort(comparer);
			}
			else if(pair.Value is TValue[] array)
			{
				Array.Sort(array,comparer);
			}
			else
			{
				LogChannel.Kit.E("Not Supported");

				continue;
			}
		}
	}

	/// <summary>
	/// Merges additional dictionaries without overwriting existing keys.
	/// </summary>
	public static void MergeDictionary<TKey,TValue>(this IDictionary<TKey,TValue> dictionary,params IDictionary<TKey,TValue>[] otherDictionaryArray)
	{
		if(!_IsValid(dictionary))
		{
			return;
		}

		foreach(var otherDictionary in otherDictionaryArray)
		{
			if(otherDictionary.IsNullOrEmpty())
			{
				continue;
			}

			foreach(var otherPair in otherDictionary)
			{
				if(!dictionary.ContainsKey(otherPair.Key))
				{
					dictionary.Add(otherPair.Key,otherPair.Value);
				}
			}
		}
	}

	/// <summary>
	/// Yields key/value pairs from nested collections whose values satisfy the predicate.
	/// </summary>
	public static IEnumerable<(TKey,TValue)> SelectMatchingPairGroup<TKey,TValue>(this IDictionary<TKey,ICollection<TValue>> dictionary,Func<TValue,bool> onFunc)
	{
		if(_IsValid(dictionary))
		{
			foreach(var pair in dictionary)
			{
				foreach(var value in pair.Value)
				{
					if(onFunc(value))
					{
						yield return (pair.Key,value);
					}
				}
			}
		}
	}

	/// <summary>
	/// Flattens nested enumerable values from each dictionary entry into one sequence.
	/// </summary>
	public static IEnumerable<TValue> FlattenValueGroup<TKey,TEnumerable,TValue>(this IDictionary<TKey,TEnumerable> dictionary) where TEnumerable : IEnumerable<TValue>
	{
		if(_IsValid(dictionary))
		{
			foreach(var pair in dictionary)
			{
				if(pair.Value.IsNullOrEmpty())
				{
					continue;
				}

				foreach(var value in pair.Value)
				{
					yield return value;
				}
			}
		}
	}

	/// <summary>
	/// Formats every key/value pair with a template containing {0} and {1} placeholders.
	/// </summary>
	public static string ToString<TKey,TValue>(this IDictionary<TKey,TValue> dictionary,string format)
	{
		if(!_IsValid(dictionary))
		{
			return null;
		}

		if(!format.Contains("{0}") || !format.Contains("{1}"))
		{
			LogChannel.Kit.E($"{format} not include {{0}} or {{1}}.");

			return null;
		}

		var stringBuilder = new StringBuilder(dictionary.Count*(format.Length+16));

		foreach(var pair in dictionary)
		{
			stringBuilder.AppendFormat(format,pair.Key,pair.Value);
		}

		return stringBuilder.ToString();
	}
}
