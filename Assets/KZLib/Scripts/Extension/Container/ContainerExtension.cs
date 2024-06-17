using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ContainerExtension
{
	public static TValue FindOrFirst<TValue>(this IEnumerable<TValue> _sources,Func<TValue,bool> _predicate)
	{
		foreach(var data in _sources)
		{
			if(_predicate(data))
			{
				return data;
			}
		}

		return _sources.First();
	}

	public static int FindIndex<TValue>(this IEnumerable<TValue> _sources,Func<TValue,bool> _predicate)
	{
		var index = 0;

		foreach(var data in _sources)
		{
			if(_predicate(data))
			{
				return index;
			}

			index++;
		}

		return -1;
	}

	public static int FindMinIndex<TValue,TCompare>(this IEnumerable<TValue> _sources,Func<TValue,TCompare> _predicate) where TCompare : IComparable
	{
		var value = _predicate(_sources.First());
		var minimum = 0;
		var index = 0;

		foreach(var data in _sources)
		{
			var compare = _predicate(data);

			if(value.CompareTo(compare) > 0)
			{
				value = compare;
				minimum = index;
			}

			index++;
		}

		return minimum;
	}

	public static int FindMaxIndex<TValue,TCompare>(this IEnumerable<TValue> _sources,Func<TValue,TCompare> _predicate) where TCompare : IComparable
	{
		var value = _predicate(_sources.First());
		var maximum = 0;
		var index = 0;

		foreach(var data in _sources)
		{
			var compare = _predicate(data);

			if(value.CompareTo(compare) > 0)
			{
				value = compare;
				maximum = index;
			}

			index++;
		}

		return maximum;
	}

	public static TValue GetRndValue<TValue>(this IEnumerable<TValue> _sources)
	{
		if(_sources.IsNullOrEmpty())
		{
			return default;
		}

		if(_sources.Count() == 1)
		{
			_sources.First();
		}

		return _sources.ElementAt(CommonUtility.GetRndInt(0,_sources.Count()-1));
	}

	public static TValue GetWeightedRndValue<TValue>(this IEnumerable<TValue> _sources,float[] _weightedArray)
	{
		if(_sources.IsNullOrEmpty())
		{
			return default;
		}

		if(_sources.Count() == 1)
		{
			_sources.First();
		}

		var total = 0.0f;

		foreach(var weight in _weightedArray)
		{
			if(weight < 0.0f)
			{
				throw new ArgumentException("가중치가 음수입니다.");
			}

			total += weight;
		}

		var pivot = CommonUtility.GetRndFloat(0.0f,total);

		for(var i=0;i<_weightedArray.Length;i++)
		{
			if(pivot <= _weightedArray[i])
			{
				return _sources.ElementAt(i);
			}

			pivot -= _weightedArray[i];
		}

		return default;
	}

	public static TValue Middle<TValue>(this IEnumerable<TValue> _sources)
	{
		var count = 0;
		var index = Mathf.RoundToInt(_sources.Count()/2);

		foreach(var item in _sources)
		{
			if(count == index)
			{
				return item;
			}

			count++;
		}

		return default;
	}

	public static List<TValue> GetRndValueList<TValue>(this IEnumerable<TValue> _sources,int _count)
	{
		var resultList = new List<TValue>();

		for(var i=0;i<_count;i++)
		{
			resultList.Add(_sources.GetRndValue());
		}

		return resultList;
	}

	public static bool Exist<TValue>(this IEnumerable<TValue> _sources,Func<TValue,bool> _predicate)
	{
		foreach(var data in _sources)
		{
			if(_predicate(data))
			{
				return true;
			}
		}

		return false;
	}

	public static void Each<TValue>(this IEnumerable<TValue> _sources,Action<TValue,int> _predicate)
	{
		var index = 0;

		foreach(var data in _sources)
		{
			_predicate(data,index++);
		}
	}

	public static bool? AllEqual<TValue>(this IEnumerable<TValue> _sources)
	{
		var value = _sources.First();

		return _sources.All(x=>x.Equals(value));
	}

	public static bool IsNullOrEmpty<TValue>(this IEnumerable<TValue> _sources)
	{
		return _sources == null || !_sources.Any();
	}

	public static bool IsEquals<TValue>(this IEnumerable<TValue> _sources1,IEnumerable<TValue> _sources2)
	{
		if(_sources1.IsNullOrEmpty() && _sources2.IsNullOrEmpty())
		{
			return true;
		}

		if(_sources1.IsNullOrEmpty() || _sources2.IsNullOrEmpty())
		{
			return false;
		}

		if(_sources1.Count() != _sources2.Count())
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

	public static IEnumerable<TValue> DeepCopy<TValue>(this IEnumerable<TValue> _sources) where TValue : ICloneable
	{
		return _sources.Select(x=>(TValue) x.Clone());
	}

	public static IEnumerable<TResult> Zip<TValue1,TValue2,TResult>(this IEnumerable<TValue1> _sources1,IEnumerable<TValue2> _sources2,Func<TValue1,TValue2,TResult> _predicate)
	{
		var resultList = new List<TResult>();
		var iterator1 = _sources1.GetEnumerator();
		var iterator2 = _sources2.GetEnumerator();

		while(iterator1.MoveNext() && iterator2.MoveNext())
		{
			resultList.Add(_predicate(iterator1.Current,iterator2.Current));
		}

		return resultList;
	}

	public static string ToString<TValue>(this IEnumerable<TValue> _sources,string _separator)
	{
		return string.Join(_separator,_sources);
	}
}