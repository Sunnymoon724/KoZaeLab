using System;
using System.Collections.Generic;

public static partial class ContainerExtension
{
	public static bool TryGetValueByIndex<TValue>(this IList<TValue> list,int index,out TValue value)
	{
		if(!IsValid(list))
		{
			value = default;

			return default;
		}

		if(!ContainsIndex(list,index))
		{
			value = default;

			return false;
		}

		value = list[index];

		return true;
	}

	public static TValue FindNext<TValue>(this IList<TValue> list,TValue value,int count)
	{
		if(!IsValid(list))
		{
			return default;
		}

		if(!list.Contains(value))
		{
			LogTag.System.E($"List does not include {value}");

			return default;
		}

		var index = list.IndexOf(value)+count;

		return list.ContainsIndex(index) ? list[index] : default;
	}

	public static void Move<TValue>(this IList<TValue> list,TValue value,int newIndex)
	{
		if(!IsValid(list))
		{
			return;
		}

		list.Move(list.IndexOf(value),newIndex);
	}

	public static void Move<TValue>(this IList<TValue> list,int oldIndex,int newIndex)
	{
		var data = list[oldIndex];

		list.RemoveAt(oldIndex);

		if(newIndex > oldIndex)
		{
			newIndex--;
		}

		list.Insert(newIndex,data);
	}

	public static void Swap<TValue>(this IList<TValue> list,int index1,int index2)
	{
		if(!IsValid(list))
		{
			return;
		}

		if(!list.ContainsIndex(index1) || !list.ContainsIndex(index2))
		{
			LogTag.System.E($"Sources does not include {index1} or {index2}");

			return;
		}

		if(index1 == index2)
		{
			return;
		}

		(list[index2],list[index1]) = (list[index1],list[index2]);
	}

	public static void Randomize<TValue>(this IList<TValue> list)
	{
		if(!IsValid(list))
		{
			return;
		}

		for(var i=list.Count-1;i>0;i--)
		{
			list.Swap(CommonUtility.GenerateRandomInt(0,i),i);
		}
	}

	public static TValue PopFront<TValue>(this IList<TValue> list)
	{
		if(!IsValid(list))
		{
			return default;
		}

		return list.Pop(0);
	}

	public static TValue PopBack<TValue>(this IList<TValue> list)
	{
		if(!IsValid(list))
		{
			return default;
		}

		return list.Pop(list.Count-1);
	}

	public static TValue Pop<TValue>(this IList<TValue> list,Predicate<TValue> onPredicate)
	{
		if(!IsValid(list))
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

	public static TValue Pop<TValue>(this IList<TValue> list,TValue value)
	{
		if(!IsValid(list))
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

	public static TValue Pop<TValue>(this IList<TValue> list,int index)
	{
		if(!IsValid(list))
		{
			return default;
		}

		var result = list[index];

		list.RemoveAt(index);

		return result;
	}
}