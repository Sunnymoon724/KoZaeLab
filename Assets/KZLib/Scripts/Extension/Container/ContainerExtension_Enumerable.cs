﻿using System;
using System.Collections.Generic;
using UnityEngine;

public static partial class ContainerExtension
{
	public static TValue FindValueInRange<TValue>(this IEnumerable<TValue> enumerable,int index)
	{
		if(!IsValid(enumerable))
		{
			return default;
		}

		var count = 0;

		foreach(var value in enumerable)
		{
			count++;
		}

		return enumerable.GetValue(count == 1 ? 0 : Mathf.Clamp(index,0,count-1));
	}

	public static TValue GenerateRandomValue<TValue>(this IEnumerable<TValue> enumerable)
	{
		if(!IsValid(enumerable))
		{
			return default;
		}

		var count = 0;

		foreach(var value in enumerable)
		{
			count++;
		}

		return enumerable.GetValue(count == 1 ? 0 : CommonUtility.GenerateRandomInt(0,count-1));
	}

	public static TValue GenerateWeightedRandomValue<TValue>(this IEnumerable<TValue> enumerable,float[] weightedArray)
	{
		if(!IsValid(enumerable))
		{
			return default;
		}

		var index = CommonUtility.GenerateWeightedRandomInt(weightedArray);

		if(index == Global.INVALID_INDEX)
		{
			return default;
		}

		return enumerable.GetValue(index);
	}

	public static IEnumerable<TValue> GenerateRandomValueGroup<TValue>(this IEnumerable<TValue> enumerable,int count,bool allowDuplicate = true)
	{
		if(!IsValid(enumerable))
		{
			yield break;
		}

		if(allowDuplicate)
		{
			for(var i=0;i<count;i++)
			{
				yield return enumerable.GenerateRandomValue();
			}
		}
		else
		{
			var list = new List<TValue>(enumerable);
			var maxCount = Mathf.Max(count,list.Count);

			for(var i=0;i<maxCount;i++)
			{
				var value = list.GenerateRandomValue();

				yield return value;

				list.Remove(value);
			}
		}
	}

	public static TValue FindOrFirst<TValue>(this IEnumerable<TValue> enumerable,Func<TValue,bool> onFunc)
	{
		if(!IsValid(enumerable))
		{
			return default;
		}

		foreach(var value in enumerable)
		{
			if(onFunc(value))
			{
				return value;
			}
		}

		return enumerable.GetFirstValue();
	}

	public static int FindMinIndex<TValue,TCompare>(this IEnumerable<TValue> enumerable,Func<TValue,TCompare> onFunc) where TCompare : IComparable
	{
		if(!IsValid(enumerable))
		{
			return Global.INVALID_INDEX;
		}

		var minValue = onFunc(enumerable.GetFirstValue());
		var minIndex = -1;
		var curIndex = 0;

		foreach(var value in enumerable)
		{
			var compareValue = onFunc(value);

			if(minValue.CompareTo(compareValue) > 0)
			{
				minValue = compareValue;
				minIndex = curIndex;
			}

			curIndex++;
		}

		return minIndex;
	}

	public static int FindMaxIndex<TValue,TCompare>(this IEnumerable<TValue> enumerable,Func<TValue,TCompare> onFunc) where TCompare : IComparable
	{
		if(!IsValid(enumerable))
		{
			return Global.INVALID_INDEX;
		}

		var maxValue = onFunc(enumerable.GetFirstValue());
		var maxIndex = 0;
		var curIndex = 0;

		foreach(var value in enumerable)
		{
			var compareValue = onFunc(value);

			if(maxValue.CompareTo(compareValue) < 0)
			{
				maxValue = compareValue;
				maxIndex = curIndex;
			}

			curIndex++;
		}

		return maxIndex;
	}

	public static int IndexOf<TValue>(this IEnumerable<TValue> enumerable,Predicate<TValue> onPredicate)
	{
		if(!IsValid(enumerable))
		{
			return Global.INVALID_INDEX;
		}

		if(!IsEmpty(enumerable))
		{
			return Global.INVALID_INDEX;
		}

		var index = 0;

		foreach(var value in enumerable)
		{
			if(onPredicate(value))
			{
				return index;
			}

			index++;
		}

		return Global.INVALID_INDEX;
	}

	public static bool Exist<TValue>(this IEnumerable<TValue> enumerable,Predicate<TValue> onPredicate)
	{
		foreach(var value in enumerable)
		{
			if(onPredicate(value))
			{
				return true;
			}
		}

		return false;
	}

	public static void Enumerate<TValue>(this IEnumerable<TValue> enumerable,Action<TValue,int> onPredicate)
	{
		var index = 0;

		foreach(var value in enumerable)
		{
			onPredicate(value,index++);
		}
	}

	public static IEnumerable<TResult> Zip<TValue1,TValue2,TResult>(this IEnumerable<TValue1> enumerable1,IEnumerable<TValue2> enumerable2,Func<TValue1,TValue2,TResult> _predicate)
	{
		if(!IsValid(enumerable1) || !IsValid(enumerable2))
		{
			yield break;
		}

		using var enumerator1 = enumerable1.GetEnumerator();
		using var enumerator2 = enumerable2.GetEnumerator();

		while(enumerator1.MoveNext() && enumerator2.MoveNext())
		{
			yield return _predicate(enumerator1.Current,enumerator2.Current);
		}
	}

	public static bool? AllEqual<TCompare>(this IEnumerable<TCompare> enumerable) where TCompare : IComparable
	{
		if(!IsValid(enumerable))
		{
			return false;
		}

		var firstValue = enumerable.GetFirstValue();

		foreach(var value in enumerable)
		{
			if(value.CompareTo(firstValue) != 0)
			{
				return false;
			}
		}

		return true;
	}

	public static bool IsEquals<TValue>(this IEnumerable<TValue> enumerable1,IEnumerable<TValue> enumerable2)
	{
		var result1 = enumerable1.IsNullOrEmpty();
		var result2 = enumerable2.IsNullOrEmpty();

		if(result1 && result2)
		{
			return true;
		}

		if(result1 || result2)
		{
			return false;
		}

		using var enumerator1 = enumerable1.GetEnumerator();
		using var enumerator2 = enumerable2.GetEnumerator();

		while (enumerator1.MoveNext() && enumerator2.MoveNext())
		{
			if (!EqualityComparer<TValue>.Default.Equals(enumerator1.Current, enumerator2.Current))
			{
				return false;
			}
		}

		return !enumerator1.MoveNext() && !enumerator2.MoveNext();
	}

	public static bool IsNullOrEmpty<TValue>(this IEnumerable<TValue> enumerable)
	{
		if(enumerable == null)
		{
			return true;
		}

		using var enumerator = enumerable.GetEnumerator();

		return !enumerator.MoveNext();
	}

	public static string ToString<TValue>(this IEnumerable<TValue> enumerable,string separator)
	{
		var count = CalculateCount(enumerable);

		return count == 0 ? $"Empty - [{enumerable}]" : $"{count} - [{string.Join(separator,enumerable)}]";
	}

	public static IEnumerable<TValue> DeepCopy<TValue>(this IEnumerable<TValue> enumerable) where TValue : ICloneable
	{
		if(!IsValid(enumerable))
		{
			yield break;
		}

		foreach(var value in enumerable)
		{
			yield return (TValue) value.Clone();
		}
	}

	private static TValue GetFirstValue<TValue>(this IEnumerable<TValue> enumerable)
	{
		using var enumerator = enumerable.GetEnumerator();

		if(enumerator.MoveNext())
		{
			return enumerator.Current;
		}

		return default;
	}

	private static TValue GetValue<TValue>(this IEnumerable<TValue> enumerable,int index)
	{
		if(enumerable is IList<TValue> list)
		{
			return list[index];
		}
		else
		{
			using var enumerator = enumerable.GetEnumerator();

			for(var i=0;i<=index;i++)
			{
				enumerator.MoveNext();
			}

			return enumerator.Current;
		}
	}

	private static bool IsValid<TValue>(IEnumerable<TValue> enumerable)
	{
		if(enumerable == null)
		{
			LogTag.System.E("Enumerable is null");

			return false;
		}

		return true;
	}

	private static bool IsEmpty<TValue>(IEnumerable<TValue> enumerable)
	{
		using var enumerator = enumerable.GetEnumerator();

		return !enumerator.MoveNext();
	}

	private static int CalculateCount<TValue>(IEnumerable<TValue> enumerable)
	{
		if(enumerable is ICollection<TValue> collection)
		{
			return collection.Count;
		}
		else
		{
			var count = 0;

			foreach(var value in enumerable)
			{
				count++;
			}

			return count;
		}
	}
}