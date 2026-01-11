using System;
using System.Collections.Generic;
using System.Text;
using Object = UnityEngine.Object;

public static partial class ContainerExtension
{
	public static bool RemoveSafe<TKey,TValue>(this IDictionary<TKey,TValue> dictionary,TKey key)
	{
		return _IsValid(dictionary) && key != null && dictionary.ContainsKey(key) && dictionary.Remove(key);
	}

	public static bool RemoveOut<TKey,TValue>(this IDictionary<TKey,TValue> dictionary,TKey key,out TValue value)
	{
		value = default;

		if(!_IsValid(dictionary))
		{
			return false;
		}

		value = dictionary[key];

		return dictionary.Remove(key);
	}

	public static bool RemoveSafeValueInCollection<TKey,TValue>(this IDictionary<TKey,ICollection<TValue>> dictionary,TKey key,TValue value)
	{
		return _IsValid(dictionary) && key != null && dictionary.ContainsKey(key) && value != null && dictionary[key].Remove(value);
	}

	public static bool IsKeysEquals<TKey,TValue>(this IDictionary<TKey,TValue> dictionary,ICollection<TKey> keyCollection)
	{
		return _IsValid(dictionary) && dictionary.Keys.IsEquals(keyCollection);
	}

	public static bool IsValuesEquals<TKey,TValue>(this IDictionary<TKey,TValue> dictionary,ICollection<TValue> valueCollection)
	{
		return _IsValid(dictionary) && dictionary.Values.IsEquals(valueCollection);
	}

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

		if(collection is List<TValue> list)
		{
			list.Add(value);
		}
		else if(collection is Queue<TValue> queue)
		{
			queue.Enqueue(value);
		}
		else if(collection is Stack<TValue> stack)
		{
			stack.Push(value);
		}
		else
		{
			LogChannel.System.E("Not Supported");
		}
	}

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
				LogChannel.System.E("Not Supported");

				return;
			}
		}
	}

	public static void AddDictionary<TKey,TValue>(this IDictionary<TKey,TValue> dictionary,params IDictionary<TKey,TValue>[] otherDictionaryArray)
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

	public static IEnumerable<(TKey,TValue)> FindAllPairGroup<TKey,TValue>(this IDictionary<TKey,ICollection<TValue>> dictionary,Func<TValue,bool> onFunc)
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

	public static IEnumerable<TValue> MergeToGroup<TKey,TEnumerable,TValue>(this IDictionary<TKey,TEnumerable> dictionary) where TEnumerable : IEnumerable<TValue>
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

	public static string ToString<TKey,TValue>(this IDictionary<TKey,TValue> dictionary,string format)
	{
		if(!_IsValid(dictionary))
		{
			return null;
		}

		if(!format.Contains("{0}") || !format.Contains("{1}"))
		{
			LogChannel.System.E($"{format} not include {{0}} or {{1}}.");

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