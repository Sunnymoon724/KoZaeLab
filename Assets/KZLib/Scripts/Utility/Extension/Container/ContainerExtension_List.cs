using System;
using System.Collections.Generic;

/// <summary>
/// List extension methods for indexing, reordering, and pop operations.
/// </summary>
public static partial class ContainerExtension
{
	/// <summary>
	/// Tries to get the element at <paramref name="index"/> when the index is in range.
	/// </summary>
	public static bool TryGetValueByIndex<TValue>(this IList<TValue> list,int index,out TValue value)
	{
		value = default;

		if(!_IsValid(list) || !ContainsIndex(list,index))
		{
			return false;
		}

		value = list[index];

		return true;
	}

	/// <summary>
	/// Returns the element offset from a known value, optionally wrapping at list boundaries.
	/// </summary>
	/// <param name="count">Steps to move from the current index; negative values move backward.</param>
	public static TValue FindNext<TValue>(this IList<TValue> list,TValue value,int count,bool canLoop)
	{
		if(!_IsValid(list))
		{
			return default;
		}

		if(!list.Contains(value))
		{
			LogChannel.Kit.E($"List does not include {value}");

			return default;
		}

		var index = list.IndexOf(value);

		if(canLoop)
		{
			index = KZMathKit.LoopClamp(index+count,list.Count);
		}
		else
		{
			index += count;
		}

		return list.ContainsIndex(index) ? list[index] : default;
	}

	/// <summary>
	/// Moves the first matching item to <paramref name="newIndex"/>.
	/// </summary>
	public static void Move<TValue>(this IList<TValue> list,TValue value,int newIndex)
	{
		if(!_IsValid(list))
		{
			return;
		}

		var index = list.IndexOf(value);

		if(index < 0)
		{
			LogChannel.Kit.E($"List does not include {value}");

			return;
		}

		list.Move(index,newIndex);
	}

	/// <summary>
	/// Moves an item from one index to another, adjusting for the removal shift when moving forward.
	/// </summary>
	public static void Move<TValue>(this IList<TValue> list,int oldIndex,int newIndex)
	{
		if(!_IsValid(list))
		{
			return;
		}

		if(!list.ContainsIndex(oldIndex) || !list.ContainsIndex(newIndex))
		{
			LogChannel.Kit.E($"Index out of range. oldIndex:{oldIndex} newIndex:{newIndex} count:{list.Count}");

			return;
		}

		var value = list[oldIndex];

		list.RemoveAt(oldIndex);

		if(newIndex > oldIndex)
		{
			newIndex--;
		}

		list.Insert(newIndex,value);
	}

	/// <summary>
	/// Swaps the elements at <paramref name="index1"/> and <paramref name="index2"/>.
	/// </summary>
	public static void Swap<TValue>(this IList<TValue> list,int index1,int index2)
	{
		if(!_IsValid(list))
		{
			return;
		}

		if(!list.ContainsIndex(index1) || !list.ContainsIndex(index2))
		{
			LogChannel.Kit.E($"Sources does not include {index1} or {index2}");

			return;
		}

		if(index1 == index2)
		{
			return;
		}

		(list[index2],list[index1]) = (list[index1],list[index2]);
	}

	/// <summary>
	/// Removes and returns the first element.
	/// </summary>
	public static TValue PopFront<TValue>(this IList<TValue> list)
	{
		return (_IsValid(list) && list.Count > 0) ? list.Pop(0) : default;
	}

	/// <summary>
	/// Removes and returns the last element.
	/// </summary>
	public static TValue PopBack<TValue>(this IList<TValue> list)
	{
		return (_IsValid(list) && list.Count > 0) ? list.Pop(list.Count-1) : default;
	}

	/// <summary>
	/// Removes and returns the first element that satisfies <paramref name="onPredicate"/>.
	/// </summary>
	public static TValue Pop<TValue>(this IList<TValue> list,Predicate<TValue> onPredicate)
	{
		if(!_IsValid(list))
		{
			return default;
		}

		var index = list.IndexOf(onPredicate);

		if(index < 0)
		{
			return default;
		}

		return list.Pop(index);
	}

	/// <summary>
	/// Removes and returns the first matching element.
	/// </summary>
	public static TValue Pop<TValue>(this IList<TValue> list,TValue value)
	{
		if(!_IsValid(list))
		{
			return default;
		}

		var index = list.IndexOf(value);

		if(index < 0)
		{
			return default;
		}

		return list.Pop(index);
	}

	/// <summary>
	/// Removes and returns the element at the given index.
	/// </summary>
	public static TValue Pop<TValue>(this IList<TValue> list,int index)
	{
		if(!_IsValid(list))
		{
			return default;
		}

		if(!list.ContainsIndex(index))
		{
			LogChannel.Kit.E($"Index {index} is out of range.");

			return default;
		}

		var result = list[index];

		list.RemoveAt(index);

		return result;
	}
}
