using System;
using System.Collections.Generic;

public static partial class CommonUtility
{
	public interface IComparePredicate<in TData1,TData2> { int Compare(TData1 _data1,TData2 _data2); }

	public static int BinarySearch<TData>(IList<TData> _list,TData _data,int _low,int _high) where TData : IComparable<TData>
	{
		return BinarySearch(_list,_data,_low,_high,Comparer<TData>.Default);
	}
	
	public static int BinarySearch<TData>(IList<TData> _list,TData _data,int _low,int _high,IComparer<TData> _comparer)
	{
		if(_comparer == null)
		{
			throw new ArgumentNullException("비교 방법이 없습니다.");
		}

		if(_low > _high)
		{
			return -1;
		}

		var mid = (_low+_high)/2;
		var comparer = _comparer.Compare(_data,_list[mid]);

		if(comparer > 0)
		{
			return BinarySearch(_list,_data,mid+1,_high,_comparer);
		}
		else if(comparer < 0)
		{
			return BinarySearch(_list,_data,_low,mid-1,_comparer);
		}
		else
		{
			return mid;
		}
	}
	
	public static int BinarySearch<TData1,TData2>(IList<TData1> _list,TData2 _data,int _low,int _high,IComparePredicate<TData1,TData2> _comparer)
	{
		if(_comparer == null)
		{
			throw new ArgumentNullException("비교 방법이 없습니다.");
		}

		var mid = (_low+_high)/2;
		var comparer = _comparer.Compare(_list[mid],_data);

		if(comparer > 0)
		{
			return BinarySearch(_list,_data,mid+1,_high,_comparer);
		}
		else if(comparer < 0)
		{
			return BinarySearch(_list,_data,_low,mid-1,_comparer);
		}
		else
		{
			return mid;
		}
	}
}