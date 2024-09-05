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
	/// 리스트의 항목을 무작위로 섞습니다.
	/// </summary>
	public static void Randomize<TValue>(this IList<TValue> _sources)
	{
		for(var i=_sources.Count-1;i>0;i--)
		{
			var index = MathUtility.GetRndInt(0,i);

			(_sources[index],_sources[i]) = (_sources[i],_sources[index]);
		}
	}
}