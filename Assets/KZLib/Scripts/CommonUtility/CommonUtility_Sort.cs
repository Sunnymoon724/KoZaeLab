using System;
using System.Collections.Generic;

public static partial class CommonUtility
{
	public static void MergeSort<TData>(IList<TData> _list) where TData : IComparable<TData>
	{
		MergeSort(_list,0,_list.Count-1,Comparer<TData>.Default);
	}
	
	public static void MergeSort<TData>(IList<TData> _list,IComparer<TData> _comparer)
	{
		MergeSort(_list,0,_list.Count-1,_comparer);
	}

	private static void MergeSort<TData>(IList<TData> _list,int _lowIndex,int _highIndex,IComparer<TData> _comparer)
	{
		if(_comparer == null)
		{
			throw new ArgumentNullException("비교 방법이 없습니다.");
		}

		if(_lowIndex >= _highIndex)
		{
			return;
		}

		int midIndex = (_lowIndex+_highIndex)/2;
		
		MergeSort(_list,_lowIndex,midIndex,_comparer);
		MergeSort(_list,midIndex+1,_highIndex,_comparer);
		
		int endLow = midIndex;
		int startHigh = midIndex+1;

		while((_lowIndex <= endLow) && (startHigh <= _highIndex))
		{
			if(_comparer.Compare(_list[_lowIndex],_list[startHigh]) <= 0)
			{
				_lowIndex++;

				continue;
			}
			
			var temp = _list[startHigh];

			for(var i=startHigh-1;i>=_lowIndex;i--)
			{
				_list[i+1] = _list[i];
			}

			_list[_lowIndex] = temp;
			_lowIndex++;
			endLow++;
			startHigh++;
		}
	}
}