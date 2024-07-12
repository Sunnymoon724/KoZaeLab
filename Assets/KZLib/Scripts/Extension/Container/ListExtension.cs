using System.Collections.Generic;
using UnityEngine;

public static class ListExtension
{
	/// <summary>
	/// 리스트가 비어있지 않고 인덱스가 유효한지 확인합니다.
	/// </summary>
	public static bool ContainsIndex<TValue>(this IList<TValue> _sources,int _index)
	{
		return !_sources.IsNullOrEmpty() && 0 <= _index && _index < _sources.Count;
	}

	public static bool TryGetValueByIndex<TValue>(this IList<TValue> _sources,int _index,out TValue _value)
	{
		_value = default;

		if(_sources.ContainsIndex(_index))
		{
			_value = _sources[_index];

			return true;
		}

		return false;
	}

	/// <summary>
	/// 리스트에서 지정한 범위 내의 값을 가져옵니다. 인덱스가 범위를 벗어나면 가장 가까운 유효한 인덱스의 값을 반환합니다.
	/// </summary>
	public static TValue GetValueInRange<TValue>(this IList<TValue> _sources,int _index)
	{
		return _sources[Mathf.Clamp(_index,0,_sources.Count-1)];
	}

	/// <summary>
	/// 리스트에서 지정된 값을 안전하게 제거합니다.
	/// </summary>
	public static bool RemoveSafe<TValue>(this IList<TValue> _sources,TValue _value)
	{
		return _sources.Contains(_value) && _sources.Remove(_value);
	}

	/// <summary>
	/// 리스트에서 지정된 데이터 리스트에 포함된 값을 제거합니다.
	/// </summary>
	public static void RemoveRange<TValue>(this IList<TValue> _sources,IList<TValue> _dataList)
	{
		for(var i=0;i<_dataList.Count;i++)
		{
			_sources.RemoveSafe(_dataList[i]);
		}
	}

	/// <summary>
	/// 리스트에서 임의의 값을 안전하게 제거하고 제거한 값을 반환합니다.
	/// </summary>
	public static bool RemoveRndValue<TValue>(this IList<TValue> _sources,out TValue _value)
	{
		if(_sources.IsNullOrEmpty())
		{
			_value = default;

			return false;
		}

		_value = _sources.GetRndValue();
		_sources.Remove(_value);

		return true;
	}

	/// <summary>
	/// 리스트에 지정된 값을 지정된 횟수만큼 추가합니다.
	/// </summary>
	public static void AddCount<TValue>(this IList<TValue> _sources,TValue _value,int _count)
	{
		for(var i=0;i<_count;i++)
		{
			_sources.Add(_value);
		}
	}

	/// <summary>
	/// 리스트에 지정된 값을 중복되지 않게 추가합니다.
	/// </summary>
	public static void AddNotOverlap<TValue>(this IList<TValue> _sources,TValue _value)
	{
		if(!_sources.Contains(_value))
		{
			_sources.Add(_value);
		}
	}

	/// <summary>
	/// 값의 위치를 옮긴다.
	/// </summary>
	public static void Move<TValue>(this IList<TValue> _sources,TValue _value,int _newIndex)
	{
		_sources.Move(_sources.IndexOf(_value),_newIndex);
	}

	//// <summary>
	/// 값의 위치를 옮긴다.
	/// </summary>
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

	/// <summary>
	/// 리스트에서 임의의 값을 지정된 횟수만큼 가져옵니다.
	/// </summary>
	public static List<TValue> GetRndValueList<TValue>(this IList<TValue> _sources,int _count)
	{
		var resultList = new List<TValue>(_count);

		while(_count > 0 && _sources.RemoveRndValue(out var value))
		{
			resultList.Add(value);
			_count--;
		}

		return resultList;
	}

	/// <summary>
	/// 리스트의 항목을 무작위로 섞습니다.
	/// </summary>
	public static void Randomize<TValue>(this IList<TValue> _sources)
	{
		for(var i=_sources.Count-1;i>0;i--)
		{
			var index = CommonUtility.GetRndInt(0,i);

			(_sources[index],_sources[i]) = (_sources[i],_sources[index]);
		}
	}
}