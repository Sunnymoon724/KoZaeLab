using System.Collections.Generic;

public static class CollectionExtension
{
	/// <summary>
	/// 지정된 값을 중복되지 않게 추가합니다.
	/// </summary>
	public static void AddNotOverlap<TValue>(this ICollection<TValue> _sources,TValue _value)
	{
		if(!_sources.Contains(_value))
		{
			_sources.Add(_value);
		}
	}

	/// <summary>
	/// 지정된 값을 안전하게 제거합니다.
	/// </summary>
	public static bool RemoveSafe<TValue>(this ICollection<TValue> _sources,TValue _value)
	{
		return _sources.Contains(_value) && _sources.Remove(_value);
	}

	/// <summary>
	/// 지정된 데이터 어레이에 포함된 값을 제거합니다.
	/// </summary>
	public static void RemoveRange<TValue>(this ICollection<TValue> _sources,TValue[] _dataArray)
	{
		for(var i=0;i<_dataArray.Length;i++)
		{
			_sources.RemoveSafe(_dataArray[i]);
		}
	}

	/// <summary>
	/// 지정된 데이터 리스트에 포함된 값을 제거합니다.
	/// </summary>
	public static void RemoveRange<TValue>(this ICollection<TValue> _sources,IList<TValue> _dataList)
	{
		for(var i=0;i<_dataList.Count;i++)
		{
			_sources.RemoveSafe(_dataList[i]);
		}
	}

	/// <summary>
	/// 임의의 값을 안전하게 제거하고 제거한 값을 반환합니다.
	/// </summary>
	public static bool RemoveRndValue<TValue>(this ICollection<TValue> _sources,out TValue _value)
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
	/// 지정된 값을 지정된 횟수만큼 추가합니다.
	/// </summary>
	public static void AddCount<TValue>(this ICollection<TValue> _sources,TValue _value,int _count)
	{
		for(var i=0;i<_count;i++)
		{
			_sources.Add(_value);
		}
	}
}