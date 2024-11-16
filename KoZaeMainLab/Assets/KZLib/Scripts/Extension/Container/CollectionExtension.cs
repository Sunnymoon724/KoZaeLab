using System;
using System.Collections.Generic;
using UnityEngine;

public static class CollectionExtension
{
	public static void AddNotOverlap<TValue>(this ICollection<TValue> _sources,TValue _value)
	{
		if(!_sources.Contains(_value))
		{
			_sources.Add(_value);
		}
	}

	public static void AddCount<TValue>(this ICollection<TValue> _sources,TValue _value,int _cnt)
	{
		for(var i=0;i<_cnt;i++)
		{
			_sources.Add(_value);
		}
	}

	public static bool ContainsIndex<TValue>(this ICollection<TValue> _sources,int _idx)
	{
		return 0 <= _idx && _idx < _sources.Count;
	}


	public static TValue Middle<TValue>(this ICollection<TValue> _sources)
	{
		if(_sources.IsNullOrEmpty())
		{
			LogTag.System.E("Sources is null or empty");

			return default;
		}

		var len = _sources.Count;

		if(len == 1)
		{
			return default;
		}

		var idx = Mathf.RoundToInt(_sources.Count/2);

		return _sources.GetValue(idx);
	}

	public static bool RemoveSafe<TValue>(this ICollection<TValue> _sources,TValue _value)
	{
		return _sources.Contains(_value) && _sources.Remove(_value);
	}

	public static void RemoveRange<TValue>(this ICollection<TValue> _sources,TValue[] _dataArray)
	{
		for(var i=0;i<_dataArray.Length;i++)
		{
			_sources.RemoveSafe(_dataArray[i]);
		}
	}

	public static void RemoveRange<TValue>(this ICollection<TValue> _sources,IList<TValue> _dataList)
	{
		for(var i=0;i<_dataList.Count;i++)
		{
			_sources.RemoveSafe(_dataList[i]);
		}
	}

	public static bool RemoveRndValue<TValue>(this ICollection<TValue> _sources,out TValue _value)
	{
		if(_sources.IsNullOrEmpty())
		{
			LogTag.System.E("Sources is null or empty");

			_value = default;

			return false;
		}

		var len = _sources.Count;

		if(len == 1)
		{
			_value = _sources.GetFirstValue();

			return true;
		}

		_value = _sources.GetRndValue();
		_sources.Remove(_value);

		return true;
	}

	public static TValue GetValueByIndex<TValue>(this ICollection<TValue> _sources,int _idx)
	{
		return _sources.TryGetValueByIndex(_idx,out var value) ? value : default;
	}

	public static bool TryGetValueByIndex<TValue>(this ICollection<TValue> _sources,int _idx,out TValue _value)
	{
		if(_sources.ContainsIndex(_idx))
		{
			_value = _sources.GetValue(_idx);

			return true;
		}
		else
		{
			_value = default;

			return false;
		}
	}

	public static TValue GetValueInRange<TValue>(this IEnumerable<TValue> _sources,int _idx)
	{
		if(_sources.IsNullOrEmpty())
		{
			LogTag.System.E("Sources is null or empty");

			return default;
		}

		var len = 0;

		foreach(var item in _sources)
		{
			len++;
		}

		return _sources.GetValue(len == 1 ? 0 : Mathf.Clamp(_idx,0,len-1));
	}

	public static TValue GetRndValue<TValue>(this IEnumerable<TValue> _sources)
	{
		if(_sources.IsNullOrEmpty())
		{
			LogTag.System.E("Sources is null or empty");

			return default;
		}

		var len = 0;

		foreach(var item in _sources)
		{
			len++;
		}

		return _sources.GetValue(len == 1 ? 0 : CommonUtility.GetRndInt(0,len-1));
	}

	public static TValue GetWeightedRndValue<TValue>(this IEnumerable<TValue> _sources,float[] _weightedArray)
	{
		if(_sources.IsNullOrEmpty())
		{
			LogTag.System.E("Sources is null or empty");

			return default;
		}

		var idx = CommonUtility.GetWeightedRndInt(_weightedArray);

		if(idx == Global.INVALID_INDEX)
		{
			return default;
		}

		return _sources.GetValue(idx);
	}

	public static List<TValue> GetRndValueList<TValue>(this ICollection<TValue> _sources,int _cnt,bool _overlap = true)
	{
		if(_sources.IsNullOrEmpty())
		{
			LogTag.System.E("Sources is null or empty");

			return default;
		}

		var resultList = new List<TValue>();

		if(_overlap)
		{
			for(var i=0;i<_cnt;i++)
			{
				resultList.Add(_sources.GetRndValue());
			}
		}
		else
		{
			var len = _sources.Count;

			var cnt = Mathf.Max(_cnt,len);

			for(var i=0;i<cnt;i++)
			{
				var value = _sources.GetRndValue();

				resultList.Add(value);

				_sources.Remove(value);
			}
		}

		resultList.Randomize();

		return resultList;
	}

	public static TValue FindOrFirst<TValue>(this IEnumerable<TValue> _sources,Func<TValue,bool> _predicate)
	{
		foreach(var data in _sources)
		{
			if(_predicate(data))
			{
				return data;
			}
		}

		return _sources.GetFirstValue();
	}

	public static int FindMinIndex<TValue,TCompare>(this IEnumerable<TValue> _sources,Func<TValue,TCompare> _predicate) where TCompare : IComparable
	{
		if(_sources.IsNullOrEmpty())
		{
			LogTag.System.E("Sources is null or empty");

			return default;
		}

		var minValue = _predicate(_sources.GetFirstValue());
		var minIdx = -1;
		var curIdx = 0;

		foreach(var item in _sources)
		{
			var compareValue = _predicate(item);

			if(minValue.CompareTo(compareValue) > 0)
			{
				minValue = compareValue;
				minIdx = curIdx;
			}

			curIdx++;
		}

		return minIdx;
	}

	public static int FindMaxIndex<TValue,TCompare>(this IEnumerable<TValue> _sources,Func<TValue,TCompare> _predicate) where TCompare : IComparable
	{
		if(_sources.IsNullOrEmpty())
		{
			LogTag.System.E("Sources is null or empty");

			return default;
		}

		var maxValue = _predicate(_sources.GetFirstValue());
		var maxIdx = 0;
		var curIdx = 0;

		foreach(var item in _sources)
		{
			var compareValue = _predicate(item);

			if(maxValue.CompareTo(compareValue) < 0)
			{
				maxValue = compareValue;
				maxIdx = curIdx;
			}

			curIdx++;
		}

		return maxIdx;
	}

	public static int IndexOf<TValue>(this IEnumerable<TValue> _sources,Predicate<TValue> _predicate)
	{
		if(_sources.IsNullOrEmpty())
		{
			LogTag.System.E("Sources is null or empty");

			return default;
		}

		using var enumerator = _sources.GetEnumerator();
		var idx = 0;

		while(enumerator.MoveNext())
		{
			if(_predicate(enumerator.Current))
			{
				return idx;
			}

			idx++;
		}

		return -1;
	}

	public static TValue GetNext<TValue>(this IList<TValue> _sources,TValue _value,int _cnt)
	{
		if(_sources.IsNullOrEmpty())
		{
			LogTag.System.E("Sources is null or empty");

			return default;
		}

		if(!_sources.Contains(_value))
		{
			LogTag.System.E($"Sources does not include {_value}");

			return default;
		}

		var idx = _sources.IndexOf(_value)+_cnt;

		return _sources.ContainsIndex(idx) ? _sources[idx] : default;
	}

	public static void Move<TValue>(this IList<TValue> _sources,TValue _value,int _newIndex)
	{
		_sources.Move(_sources.IndexOf(_value),_newIndex);
	}

	public static void Move<TValue>(this IList<TValue> _sources,int _oldIndex,int _newIndex)
	{
		var data = _sources[_oldIndex];

		_sources.RemoveAt(_oldIndex);

		if(_newIndex > _oldIndex)
		{
			_newIndex--;
		}

		_sources.Insert(_newIndex,data);
	}

	public static bool Exist<TValue>(this IEnumerable<TValue> _sources,Predicate<TValue> _predicate)
	{
		foreach(var item in _sources)
		{
			if(_predicate(item))
			{
				return true;
			}
		}

		return false;
	}

	public static void Swap<TValue>(this IList<TValue> _sources,int _idx1,int _idx2)
	{
		if(_sources.IsNullOrEmpty())
		{
			LogTag.System.E("Sources is null or empty");

			return;
		}

		if(!_sources.ContainsIndex(_idx1) || !_sources.ContainsIndex(_idx2))
		{
			LogTag.System.E($"Sources does not include {_idx1} or {_idx2}");

			return;
		}

		if(_idx1 == _idx2)
		{
			return;
		}

		(_sources[_idx2],_sources[_idx1]) = (_sources[_idx1],_sources[_idx2]);
	}

	public static void Randomize<TValue>(this IList<TValue> _sources)
	{
		for(var i=_sources.Count-1;i>0;i--)
		{
			_sources.Swap(CommonUtility.GetRndInt(0,i),i);
		}
	}

	public static void ResetWith<TValue>(this ICollection<TValue> _sources,TValue _value)
	{
		_sources.Clear();
		_sources.Add(_value);
	}

	public static void Enumerate<TValue>(this IEnumerable<TValue> _sources,Action<TValue,int> _predicate)
	{
		var idx = 0;

		foreach(var data in _sources)
		{
			_predicate(data,idx++);
		}
	}

	public static TValue PopFront<TValue>(this IList<TValue> _sources)
	{
		if(_sources.IsNullOrEmpty())
		{
			LogTag.System.E("Sources is null or empty");

			return default;
		}

		return _sources.Pop(0);
	}

	public static TValue PopBack<TValue>(this IList<TValue> _sources)
	{
		if(_sources.IsNullOrEmpty())
		{
			LogTag.System.E("Sources is null or empty");

			return default;
		}

		return _sources.Pop(_sources.Count-1);
	}

	public static TValue Pop<TValue>(this IList<TValue> _sources,Predicate<TValue> _predicate)
	{
		if(_sources.IsNullOrEmpty())
		{
			LogTag.System.E("Sources is null or empty");

			return default;
		}

		var idx = _sources.IndexOf(_predicate);

		if(idx < 0)
		{
			return default;
		}

		return _sources.Pop(idx);
	}

	public static TValue Pop<TValue>(this IList<TValue> _sources,TValue _value)
	{
		if(_sources.IsNullOrEmpty())
		{
			LogTag.System.E("Sources is null or empty");

			return default;
		}

		var idx = _sources.IndexOf(_value);

		if(idx < 0)
		{
			return default;
		}

		return _sources.Pop(idx);
	}

	public static TValue Pop<TValue>(this IList<TValue> _sources,int _idx)
	{
		if(_sources.IsNullOrEmpty())
		{
			LogTag.System.E("Sources is null or empty");

			return default;
		}

		var result = _sources[_idx];

		_sources.RemoveAt(_idx);

		return result;
	}

	public static IEnumerable<TResult> Zip<TValue1,TValue2,TResult>(this IEnumerable<TValue1> _sources1,IEnumerable<TValue2> _sources2,Func<TValue1,TValue2,TResult> _predicate)
	{
		if(_sources1.IsNullOrEmpty() || _sources2.IsNullOrEmpty())
		{
			LogTag.System.E($"Sources1 or sources2 is null or empty. [{_sources1} or {_sources2}]");

			yield break;
		}

		using var enumerator1 = _sources1.GetEnumerator();
		using var enumerator2 = _sources2.GetEnumerator();

		while(enumerator1.MoveNext() && enumerator2.MoveNext())
		{
			yield return _predicate(enumerator1.Current, enumerator2.Current);
		}
	}

	public static bool? AllEqual<TCompare>(this ICollection<TCompare> _sources) where TCompare : IComparable
	{
		if(_sources.IsNullOrEmpty())
		{
			LogTag.System.E("Sources is null or empty");

			return null;
		}

		var firstValue = _sources.GetFirstValue();

		foreach(var item in _sources)
		{
			if(item.CompareTo(firstValue) != 0)
			{
				return false;
			}
		}

		return true;
	}

	public static bool IsEquals<TValue>(this ICollection<TValue> _sources1,ICollection<TValue> _sources2)
	{
		var result1 = _sources1.IsNullOrEmpty();
		var result2 = _sources2.IsNullOrEmpty();

		if(result1 && result2)
		{
			return true;
		}

		if(result1 || result2)
		{
			return false;
		}

		if(_sources1.Count != _sources2.Count)
		{
			return false;
		}

		foreach(var data in _sources1)
		{
			if(!_sources2.Contains(data))
			{
				return false;
			}
		}

		return true;
	}

	public static bool IsNullOrEmpty<TValue>(this IEnumerable<TValue> _sources)
	{
		if(_sources == null)
		{
			return true;
		}

		using var enumerator = _sources.GetEnumerator();

		return !enumerator.MoveNext();
	}

	public static string ToString<TValue>(this ICollection<TValue> _sources,string _separator)
	{
		var len = _sources.Count;

		return len == 0 ? $"Empty - [{_sources}]" : $"{len} - [{string.Join(_separator,_sources)}]";
	}

	public static IEnumerable<TValue> DeepCopy<TValue>(this IEnumerable<TValue> _sources) where TValue : ICloneable
	{
		if(_sources.IsNullOrEmpty())
		{
			LogTag.System.E("Sources is null or empty");

			yield break;
		}

		foreach(var item in _sources)
		{
			yield return (TValue) item.Clone();
		}
	}

	private static TValue GetFirstValue<TValue>(this IEnumerable<TValue> _sources)
	{
		using var enumerator = _sources.GetEnumerator();

		if(enumerator.MoveNext())
		{
			return enumerator.Current;
		}

		return default;
	}

	private static TValue GetValue<TValue>(this IEnumerable<TValue> _sources,int _idx)
	{
		if(_sources is IList<TValue> sources)
		{
			return sources[_idx];
		}

		using var enumerator = _sources.GetEnumerator();

		for(var i=0;i<=_idx;i++)
		{
			enumerator.MoveNext();
		}

		return enumerator.Current;
	}
}