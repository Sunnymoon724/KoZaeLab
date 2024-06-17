using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

public static class DictionaryExtension
{
	/// <summary>
	/// 안전하게 키를 사용하여 항목을 제거합니다.
	/// </summary>
	public static bool RemoveSafe<TKey,TValue>(this IDictionary<TKey,TValue> _sources,TKey _key)
	{
		return _key != null && _sources.ContainsKey(_key) && _sources.Remove(_key);
	}

	/// <summary>
    /// 안전하게 키를 사용하여 항목을 제거합니다.
    /// </summary>
	public static bool RemoveSafeValueInCollection<TKey,TValue>(this IDictionary<TKey,ICollection<TValue>> _sources,TKey _key,TValue _value)
	{
		return _key != null && _sources.ContainsKey(_key) && _value != null && _sources[_key].Remove(_value);
	}

	/// <summary>
	/// 키와 값을 가지고 있는 딕셔너리에서 지정된 조건을 충족하는 모든 쌍을 찾습니다.
	/// </summary>
	public static List<KeyValuePair<TKey,TValue>> FindAllPairList<TKey,TValue>(this IDictionary<TKey,List<TValue>> _sources,Func<TValue,bool> _predicate)
	{
		if(_sources.FindAllPairList(_predicate,out var pairList))
		{
			return pairList;
		}

		return null;
	}

	/// <summary>
	/// 키와 값을 가지고 있는 딕셔너리에서 지정된 조건을 충족하는 모든 쌍을 찾습니다.
	/// </summary>
	public static bool FindAllPairList<TKey,TValue>(this IDictionary<TKey,List<TValue>> _sources,Func<TValue,bool> _predicate,out List<KeyValuePair<TKey,TValue>> _pairList)
	{
		_pairList = new List<KeyValuePair<TKey,TValue>>();

		foreach(var pair in _sources)
		{
			foreach(var value in pair.Value)
			{
				if(_predicate(value))
				{
					_pairList.Add(new KeyValuePair<TKey,TValue>(pair.Key,value));
				}
			}
		}

		return _pairList.Count != 0;
	}
	
	public static TValue GetOrCreateValue<TKey,TValue>(this IDictionary<TKey,TValue> _sources,TKey _key) where TValue : new()
	{
		if(!_sources.ContainsKey(_key))
		{
			_sources.Add(_key,new TValue());
		}

		return _sources[_key];
	}

	public static TValue GetNearValue<TKey,TValue>(this IDictionary<TKey,TValue> _sources,TKey _key,int _near)
	{
		var keyList = _sources.Keys.ToList();

		for(var i=0;i<keyList.Count;i++)
		{
			if(keyList[i].Equals(_key))
			{
				var idx = Mathf.Clamp(i+_near,0,keyList.Count-1);

				return _sources[keyList[idx]];
			}
		}

		return default;
	}

	public static bool IsKeysEquals<TKey,TValue>(this IDictionary<TKey,TValue> _sources,IEnumerable<TKey> _keys)
	{
		if(_sources.IsNullOrEmpty() && _keys.IsNullOrEmpty())
		{
			return true;
		}

		if(_sources.IsNullOrEmpty() || _keys.IsNullOrEmpty())
		{
			return false;
		}

		return _sources.Keys.IsEquals(_keys);
	}

	public static bool IsValuesEquals<TKey,TValue>(this IDictionary<TKey,TValue> _sources,IEnumerable<TValue> _values)
	{
		if(_sources.IsNullOrEmpty() && _values.IsNullOrEmpty())
		{
			return true;
		}

		if(_sources.IsNullOrEmpty() || _values.IsNullOrEmpty())
		{
			return false;
		}

		return _sources.Values.IsEquals(_values);
	}

	public static bool? GetAsBool<TKey>(this IDictionary<TKey,object> _sources,TKey _key)
	{
		return _sources.TryGetValue(_key,out var value) ? value as bool? : null;
	}

	public static int? GetAsInt<TKey>(this IDictionary<TKey,object> _sources,TKey _key)
	{
		return _sources.TryGetValue(_key,out var value) ? value as int? : null;
	}

	public static float? GetAsFloat<TKey>(this IDictionary<TKey,object> _sources,TKey _key)
	{
		return _sources.TryGetValue(_key,out var value) ? value as float? : null;
	}

	public static string GetAsString<TKey>(this IDictionary<TKey,object> _sources,TKey _key)
	{
		return _sources.TryGetValue(_key,out var value) ? value as string : null;
	}
	
	public static void AddRange<TKey,TValue>(this IDictionary<TKey,TValue> _sources,IEnumerable<TValue> _values,Func<TValue,TKey> _key,Action _onAction = null)
	{
		foreach(var value in _values)
		{
			var key = _key(value);

			if(key == null)
			{
				continue;
			}

			if(_sources.ContainsKey(key))
			{
				continue;
			}
			
			_onAction?.Invoke();
			_sources.Add(key,value);
		}
	}

	public static void AddRange<TValue>(this IDictionary<string,TValue> _sources,IEnumerable<TValue> _values,Action _onAction = null) where TValue : Object
	{
		_sources.AddRange(_values,(value)=>{ return value.name; },_onAction);
	}

	public static void AddRange<TKey,TValue>(this IDictionary<TKey,TValue> _sources,IDictionary<TKey,TValue> _values,Action _onAction = null)
	{
		foreach(var pair in _values)
		{
			if(_sources.ContainsKey(pair.Key))
			{
				continue;
			}

			_onAction?.Invoke();
			_sources.Add(pair.Key,pair.Value);
		}
	}

	public static void AddOrCreate<TKey,TValue>(this IDictionary<TKey,List<TValue>> _sources,TKey _key,TValue _value)
	{
		if(!_sources.ContainsKey(_key))
		{
			_sources.Add(_key,new List<TValue>());
		}

		_sources[_key].Add(_value);
	}

	public static void AddOrCreate<TKey,TValue>(this IDictionary<TKey,Queue<TValue>> _sources,TKey _key,TValue _value)
	{
		if(!_sources.ContainsKey(_key))
		{
			_sources.Add(_key,new Queue<TValue>());
		}

		_sources[_key].Enqueue(_value);
	}

	public static void AddOrUpdate<TKey,TValue>(this IDictionary<TKey,TValue> _sources,TKey _key,TValue _value)
	{
		if(_sources.ContainsKey(_key))
		{
			_sources[_key] = _value;
		}
		else
		{
			_sources.Add(_key,_value);
		}
	}
	
	/// <summary>
    /// 딕셔너리의 각 리스트를 정렬합니다.
    /// </summary>
	public static void SortEachList<TKey,TValue>(this IDictionary<TKey,List<TValue>> _sources,Func<TValue,TKey> _predicate)
	{
		foreach(var pair in _sources)
		{
			_sources[pair.Key] = pair.Value.OrderBy(_predicate).ToList();
		}
	}
	
	public static void AddDictionary<TKey,TValue>(this IDictionary<TKey,TValue> _sources,params IDictionary<TKey,TValue>[] _dictArray)
	{
		foreach(var dict in _dictArray)
		{
			foreach(var pair in dict)
			{
				if(!_sources.ContainsKey(pair.Key))
				{
					_sources.Add(pair.Key,pair.Value);
				}
			}
		}
	}

	public static List<TValue> MergeToList<TKey,TEnumerable,TValue>(this IDictionary<TKey,TEnumerable> _sources) where TEnumerable : IEnumerable<TValue>
	{
		var valueList = new List<TValue>(_sources.Count);

		foreach(var pair in _sources)
		{
			foreach(var value in pair.Value)
			{
				valueList.Add(value);
			}
		}

		return valueList;
	}

	/// <summary>
	/// 원하는 포멧의 스트링으로 변환
	/// </summary>
	public static string ToString<TKey,TValue>(this IDictionary<TKey,TValue> _dict,string _format)
	{
		var builder = new StringBuilder();

		if(_dict.IsNullOrEmpty() || !_format.Contains("{0}") || !_format.Contains("{1}"))
		{
			return string.Empty;
		}

		var iterator = _dict.GetEnumerator();

		while(iterator.MoveNext())
		{
			builder.AppendFormat(_format,iterator.Current.Key,iterator.Current.Value);
		}

		return builder.ToString();
	}
}