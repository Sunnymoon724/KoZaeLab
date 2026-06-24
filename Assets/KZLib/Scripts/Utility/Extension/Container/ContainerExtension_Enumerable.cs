using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// IEnumerable extension methods for searching, comparing, cloning, and formatting sequences.
/// </summary>
public static partial class ContainerExtension
{
	/// <summary>
	/// Counts the sequence once, then returns the element at a clamped <paramref name="index"/>.
	/// When the sequence has one item, that item is always returned.
	/// </summary>
	public static TValue PickClampedFromGroup<TValue>(this IEnumerable<TValue> enumerable,int index)
	{
		if(!_IsValid(enumerable))
		{
			return default;
		}

		var count = 0;

		foreach(var value in enumerable)
		{
			count++;
		}

		if(count == 0)
		{
			return default;
		}

		return enumerable._GetValue(count == 1 ? 0 : Mathf.Clamp(index,0,count-1));
	}

	/// <summary>
	/// Returns the first matching value, or the first element when no match is found.
	/// </summary>
	public static TValue FindOrFirst<TValue>(this IEnumerable<TValue> enumerable,Func<TValue,bool> onFunc)
	{
		if(!_IsValid(enumerable))
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

		return enumerable._GetFirstValue();
	}

	/// <summary>
	/// Returns the index of the minimum projected value, or invalid when empty.
	/// </summary>
	public static int FindMinIndex<TValue,TCompare>(this IEnumerable<TValue> enumerable,Func<TValue,TCompare> onFunc) where TCompare : IComparable
	{
		var minIndex = Global.InvalidIndex;

		if(_IsValid(enumerable) && !_IsEmpty(enumerable))
		{
			minIndex = 0;

			var minValue = onFunc(enumerable._GetFirstValue());
			var curIndex = 0;

			foreach(var value in enumerable)
			{
				if(curIndex > 0)
				{
					var compareValue = onFunc(value);

					if(minValue.CompareTo(compareValue) > 0)
					{
						minValue = compareValue;
						minIndex = curIndex;
					}
				}

				curIndex++;
			}
		}

		return minIndex;
	}

	/// <summary>
	/// Returns the index of the maximum projected value, or invalid when empty.
	/// </summary>
	public static int FindMaxIndex<TValue,TCompare>(this IEnumerable<TValue> enumerable,Func<TValue,TCompare> onFunc) where TCompare : IComparable
	{
		var maxIndex = Global.InvalidIndex;

		if(_IsValid(enumerable) && !_IsEmpty(enumerable))
		{
			maxIndex = 0;

			var maxValue = onFunc(enumerable._GetFirstValue());
			var curIndex = 0;

			foreach(var value in enumerable)
			{
				if(curIndex > 0)
				{
					var compareValue = onFunc(value);

					if(maxValue.CompareTo(compareValue) < 0)
					{
						maxValue = compareValue;
						maxIndex = curIndex;
					}
				}

				curIndex++;
			}
		}

		return maxIndex;
	}

	/// <summary>
	/// Returns the first index matching the predicate, or invalid when none.
	/// </summary>
	public static int IndexOf<TValue>(this IEnumerable<TValue> enumerable,Predicate<TValue> onPredicate)
	{
		if(_IsValid(enumerable) && !_IsEmpty(enumerable))
		{
			var index = 0;

			foreach(var value in enumerable)
			{
				if(onPredicate(value))
				{
					return index;
				}

				index++;
			}
		}

		return Global.InvalidIndex;
	}

	/// <summary>
	/// Returns whether any element satisfies <paramref name="onPredicate"/>.
	/// </summary>
	public static bool Exists<TValue>(this IEnumerable<TValue> enumerable,Predicate<TValue> onPredicate)
	{
		if(!_IsValid(enumerable))
		{
			return false;
		}

		foreach(var value in enumerable)
		{
			if(onPredicate(value))
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Invokes <paramref name="onPredicate"/> for each element with its zero-based index.
	/// </summary>
	public static void ForEachWithIndex<TValue>(this IEnumerable<TValue> enumerable,Action<TValue,int> onPredicate)
	{
		var index = 0;

		foreach(var value in enumerable)
		{
			onPredicate(value,index++);
		}
	}

	/// <summary>
	/// Pairs two sequences element-wise until the shorter sequence ends.
	/// </summary>
	public static IEnumerable<TResult> Zip<TValue1,TValue2,TResult>(this IEnumerable<TValue1> enumerable1,IEnumerable<TValue2> enumerable2,Func<TValue1,TValue2,TResult> onPredicate)
	{
		if(_IsValid(enumerable1) && _IsValid(enumerable2))
		{
			using var enumerator1 = enumerable1.GetEnumerator();
			using var enumerator2 = enumerable2.GetEnumerator();

			while(enumerator1.MoveNext() && enumerator2.MoveNext())
			{
				yield return onPredicate(enumerator1.Current, enumerator2.Current);
			}
		}
	}

	/// <summary>
	/// Returns whether every comparable value equals the first value.
	/// Returns false when the sequence is null; returns true for an empty sequence.
	/// </summary>
	public static bool? AllEqual<TCompare>(this IEnumerable<TCompare> enumerable) where TCompare : IComparable
	{
		if(!_IsValid(enumerable))
		{
			return false;
		}

		var firstValue = enumerable._GetFirstValue();

		foreach(var value in enumerable)
		{
			if(value.CompareTo(firstValue) != 0)
			{
				return false;
			}
		}

		return true;
	}

	/// <summary>
	/// Compares two sequences for equal length and element-wise equality.
	/// </summary>
	public static bool AreEqual<TValue>(this IEnumerable<TValue> enumerable1,IEnumerable<TValue> enumerable2)
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

	/// <summary>
	/// Returns whether the sequence is null or contains no elements.
	/// </summary>
	public static bool IsNullOrEmpty<TValue>(this IEnumerable<TValue> enumerable)
	{
		if(enumerable == null)
		{
			return true;
		}

		using var enumerator = enumerable.GetEnumerator();

		return !enumerator.MoveNext();
	}

	/// <summary>
	/// Returns a debug string with the element count and joined values.
	/// </summary>
	public static string ToString<TValue>(this IEnumerable<TValue> enumerable,string separator)
	{
		var count = _CalculateCount(enumerable);

		return count == 0 ? $"Empty - [{enumerable}]" : $"{count} - [{string.Join(separator,enumerable)}]";
	}

	/// <summary>
	/// Yields a cloned copy of each element that implements <see cref="ICloneable"/>.
	/// </summary>
	public static IEnumerable<TValue> CloneElementGroup<TValue>(this IEnumerable<TValue> enumerable) where TValue : ICloneable
	{
		if(_IsValid(enumerable))
		{
			foreach(var value in enumerable)
			{
				yield return (TValue) value.Clone();
			}
		}
	}

	/// <summary>
	/// Builds a dictionary that maps each value to its zero-based index in the sequence.
	/// </summary>
	public static Dictionary<TValue,int> ToDictionary<TValue>(this IEnumerable<TValue> enumerable)
	{
		var dictionary = new Dictionary<TValue,int>();

		if(_IsValid(enumerable))
		{
			var index = 0;

			foreach(var value in enumerable)
			{
				dictionary.Add(value,index++);
			}
		}

		return dictionary;
	}

	private static TValue _GetFirstValue<TValue>(this IEnumerable<TValue> enumerable)
	{
		using var enumerator = enumerable.GetEnumerator();

		if(enumerator.MoveNext())
		{
			return enumerator.Current;
		}

		return default;
	}

	private static TValue _GetValue<TValue>(this IEnumerable<TValue> enumerable,int index)
	{
		if(enumerable is IList<TValue> list)
		{
			return list[index];
		}

		using var enumerator = enumerable.GetEnumerator();

		for(var i=0;i<=index;i++)
		{
			enumerator.MoveNext();
		}

		return enumerator.Current;
	}

	private static bool _IsValid<TValue>(IEnumerable<TValue> enumerable)
	{
		if(enumerable == null)
		{
			LogChannel.Kit.E("Enumerable is null");

			return false;
		}

		return true;
	}

	private static bool _IsEmpty<TValue>(IEnumerable<TValue> enumerable)
	{
		using var enumerator = enumerable.GetEnumerator();

		return !enumerator.MoveNext();
	}

	private static int _CalculateCount<TValue>(IEnumerable<TValue> enumerable)
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
