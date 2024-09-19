using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
		_pairList = new List<KeyValuePair<TKey,TValue>>(_sources.Sum(x => x.Value.Count));

		foreach(var pair in _sources)
		{
			_pairList.AddRange(pair.Value.Where(_predicate).Select(x => new KeyValuePair<TKey,TValue>(pair.Key,x)));
		}

		return _pairList.Count != 0;
	}

	/// <summary>
	/// 딕셔너리에서 지정된 키와 인덱스를 사용하여 가장 가까운 항목을 찾습니다.
	/// </summary>
	public static TValue GetNearValue<TKey,TValue>(this IDictionary<TKey,TValue> _sources,TKey _key,int _near)
	{
		if(_sources.IsNullOrEmpty() && !_sources.ContainsKey(_key))
		{
			return default;
		}

		var keyList = _sources.Keys.ToList();

		var index = keyList.IndexOf(_key)+_near;

		if(index < 0 || index >= keyList.Count)
		{
			return default;
		}

		return _sources[keyList[index]];
	}

	/// <summary>
	/// 딕셔너리의 키와 지정된 키열이 일치하는지 확인합니다.
	/// </summary>
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

		return _sources.Keys.SequenceEqual(_keys);
	}

	/// <summary>
	/// 딕셔너리의 값과 지정된 값 열이 일치하는지 확인합니다.
	/// </summary>
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

		return _sources.Values.SequenceEqual(_values);
	}

	/// <summary>
	/// 지정된 조건을 사용하여 딕셔너리에 추가합니다. 
	/// </summary>
	public static void AddRange<TKey,TValue>(this IDictionary<TKey,TValue> _sources,IEnumerable<TValue> _values,Func<TValue,TKey> _predicate,Action _onAction = null)
	{
		foreach(var value in _values)
		{
			var key = _predicate(value);

			if(key == null || _sources.ContainsKey(key))
			{
				continue;
			}

			_onAction?.Invoke();
			_sources.Add(key,value);
		}
	}

	/// <summary>
	/// Group을 딕셔너리에 추가합니다. 
	/// </summary>
	public static void AddRange<TValue>(this IDictionary<string,TValue> _sources,IEnumerable<TValue> _values,Action _onAction = null) where TValue : Object
	{
		_sources.AddRange(_values,value=>value.name,_onAction);
	}

	/// <summary>
	/// Dict을 딕셔너리에 추가합니다. 
	/// </summary>
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

	/// <summary>
	/// 리스트에 값을 추가하거나 새로 생성합니다.
	/// </summary>
	public static void AddOrCreate<TKey,TValue>(this IDictionary<TKey,List<TValue>> _sources,TKey _key,TValue _value)
	{
		if(!_sources.TryGetValue(_key,out var list))
		{
			list = new List<TValue>();
			_sources.Add(_key,list);
		}

		list.Add(_value);
	}

	/// <summary>
	/// 큐에 값을 추가하거나 새로 생성합니다.
	/// </summary>
	public static void AddOrCreate<TKey,TValue>(this IDictionary<TKey,Queue<TValue>> _sources,TKey _key,TValue _value)
	{
		if(!_sources.TryGetValue(_key,out var queue))
		{
			queue = new Queue<TValue>();
			_sources.Add(_key,queue);
		}

		queue.Enqueue(_value);
	}

	/// <summary>
	/// 큐에 값을 추가하거나 새로 생성합니다.
	/// </summary>
	public static void AddOrCreate<TKey,TValue>(this IDictionary<TKey,Stack<TValue>> _sources,TKey _key,TValue _value)
	{
		if(!_sources.TryGetValue(_key,out var stack))
		{
			stack = new Stack<TValue>();
			_sources.Add(_key,stack);
		}

		stack.Push(_value);
	}

	/// <summary>
	/// 키가 이미 있으면 값을 업데이트하고, 없으면 추가합니다.
	/// </summary>
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
	
	/// <summary>
	/// 하나 이상의 딕셔너리를 현재 딕셔너리에 추가합니다.
	/// </summary>
	public static void AddDictionary<TKey,TValue>(this IDictionary<TKey,TValue> _sources,params IDictionary<TKey,TValue>[] _dictArray)
	{
		foreach(var dict in _dictArray)
		{
			if(dict == null)
			{
				continue;
			}

			foreach(var pair in dict)
			{
				if(!_sources.ContainsKey(pair.Key))
				{
					_sources.Add(pair.Key,pair.Value);
				}
			}
		}
	}

	/// <summary>
	/// 각 딕셔너리의 value를 하나의 리스트로 병합합니다.
	/// </summary>
	public static List<TValue> MergeToList<TKey,TEnumerable,TValue>(this IDictionary<TKey,TEnumerable> _sources) where TEnumerable : IEnumerable<TValue>
	{
		var valueList = new List<TValue>(_sources.Count);

		foreach(var pair in _sources)
		{
			if(pair.Value == null)
			{
				continue;
			}

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
		if(_dict.IsNullOrEmpty() || !_format.Contains("{0}") || !_format.Contains("{1}"))
		{
			return string.Empty;
		}

		var builder = new StringBuilder(_dict.Count*(_format.Length+16));

		foreach(var pair in _dict)
		{
			builder.AppendFormat(_format,pair.Key,pair.Value);
		}

		return builder.ToString();
	}
}