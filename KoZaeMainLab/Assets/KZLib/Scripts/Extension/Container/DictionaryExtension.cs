using System;
using System.Collections.Generic;
using System.Text;
using Object = UnityEngine.Object;

public static class DictionaryExtension
{
	public static bool RemoveSafe<TKey,TValue>(this IDictionary<TKey,TValue> _sources,TKey _key)
	{
		return _key != null && _sources.ContainsKey(_key) && _sources.Remove(_key);
	}

	public static bool RemoveSafeValueInCollection<TKey,TValue>(this IDictionary<TKey,ICollection<TValue>> _sources,TKey _key,TValue _value)
	{
		return _key != null && _sources.ContainsKey(_key) && _value != null && _sources[_key].Remove(_value);
	}

	public static bool IsKeysEquals<TKey,TValue>(this IDictionary<TKey,TValue> _sources,ICollection<TKey> _keys)
	{
		return _sources.Keys.IsEquals(_keys);
	}

	public static bool IsValuesEquals<TKey,TValue>(this IDictionary<TKey,TValue> _sources,ICollection<TValue> _values)
	{
		return _sources.Values.IsEquals(_values);
	}

	public static void AddRange<TKey,TValue>(this IDictionary<TKey,TValue> _sources,ICollection<TValue> _values,Func<TValue,TKey> _predicate,Action _onAction = null)
	{
		foreach(var value in _values)
		{
			var key = _predicate(value);

			if(key == null || _sources.ContainsKey(key))
			{
				continue;
			}

			_sources.Add(key,value);
			_onAction?.Invoke();
		}
	}

	public static void AddRange<TValue>(this IDictionary<string,TValue> _sources,ICollection<TValue> _values,Action _onAction = null) where TValue : Object
	{
		_sources.AddRange(_values,value=>value.name,_onAction);
	}

	public static void AddOrCreate<TKey,TValue,TCollection>(this IDictionary<TKey,TCollection> _sources,TKey _key,TValue _value) where TCollection : ICollection<TValue>,new()
	{
		if(!_sources.TryGetValue(_key,out var sources))
		{
			sources = new TCollection();
			_sources.Add(_key,sources);
		}

		if(sources is List<TValue> sourceList)
		{
			sourceList.Add(_value);
		}
		else if(sources is Queue<TValue> sourceQueue)
		{
			sourceQueue.Enqueue(_value);
		}
		else if(sources is Stack<TValue> sourceStack)
		{
			sourceStack.Push(_value);
		}
		else
		{
			LogTag.System.E("Not Supported");
		}
	}

	public static void SortEachList<TKey,TValue>(this IDictionary<TKey,IList<TValue>> _sources,IComparer<TValue> _comparer)
	{
		foreach(var pair in _sources)
		{
			if(pair.Value is List<TValue> sourceList)
			{
				sourceList.Sort(_comparer);
			}
			else if(pair.Value is TValue[] sourceArray)
			{
				Array.Sort(sourceArray,_comparer);
			}
			else
			{
				LogTag.System.E("Not Supported");

				return;
			}
		}
	}

	public static void AddDictionary<TKey,TValue>(this IDictionary<TKey,TValue> _sources,params IDictionary<TKey,TValue>[] _sourcesArray)
	{
		foreach(var sources in _sourcesArray)
		{
			if(sources == null)
			{
				continue;
			}

			foreach(var pair in sources)
			{
				if(!_sources.ContainsKey(pair.Key))
				{
					_sources.Add(pair.Key,pair.Value);
				}
			}
		}
	}

	public static IEnumerable<(TKey,TValue)> FindAllPairGroup<TKey,TValue>(this IDictionary<TKey,ICollection<TValue>> _sources,Func<TValue,bool> _predicate)
	{
		foreach(var pair in _sources)
		{
			foreach(var value in pair.Value)
			{
				if(_predicate(value))
				{
					yield return (pair.Key,value);
				}
			}
		}
	}

	public static IEnumerable<TValue> MergeToGroup<TKey,TEnumerable,TValue>(this IDictionary<TKey,TEnumerable> _sources) where TEnumerable : IEnumerable<TValue>
	{
		foreach(var pair in _sources)
		{
			if(pair.Value == null)
			{
				continue;
			}

			foreach(var value in pair.Value)
			{
				yield return value;
			}
		}
	}

	public static string ToString<TKey,TValue>(this IDictionary<TKey,TValue> _sources,string _format)
	{
		if(_sources.IsNullOrEmpty())
		{
			LogTag.System.E("Sources is null or empty");

			return null;
		}

		if(!_format.Contains("{0}") || !_format.Contains("{1}"))
		{
			LogTag.System.E($"{_format} not include {{0}} or {{1}}.");

			return null;
		}

		var builder = new StringBuilder(_sources.Count*(_format.Length+16));

		foreach(var pair in _sources)
		{
			builder.AppendFormat(_format,pair.Key,pair.Value);
		}

		return builder.ToString();
	}
}