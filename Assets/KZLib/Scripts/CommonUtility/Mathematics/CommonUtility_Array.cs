using System.Collections.Generic;

public static partial class CommonUtility
{
	/// <summary>
	/// 1차원 배열을 가변 배열로 바꾼다.
	/// </summary>
	public static TData[][] ToJaggedArray<TData>(TData[] _array,int _row)
	{
		if(_row == 0)
		{
			return null;
		}

		var column = _array.Length/_row;
		var resultArray = new TData[_row][];

		for(var i=0;i<_row;i++)
		{
			resultArray[i] = new TData[column];

			for(var j=0;j<column;j++)
			{
				resultArray[i][j] = _array[i*column+j];
			}
		}

		return resultArray;
	}

	/// <summary>
	/// 가변 배열(2차원)을 1차원 배열로 바꾼다.
	/// </summary>
	public static TData[] ToFlatArray<TData>(TData[][] _array)
	{
		var resultList = new List<TData>();

		for(var i=0;i<_array.Length;i++)
		{
			for(var j=0;j<_array[i].Length;j++)
			{
				resultList.Add(_array[i][j]);
			}
		}

		return resultList.ToArray();
	}
	
	/// <summary>
	/// 가변 배열(2차원)을 2차원 배열로 바꾼다.
	/// </summary>
	public static TData[,] To2DArray<TData>(TData[][] _array)
	{
		var resultArray = new TData[_array.Length,_array[0].Length];

		for(var i=0;i<_array.Length;i++)
		{
			for(var j=0;j<_array[0].Length;j++)
			{
				resultArray[i,j] = _array[i][j];
			}
		}

		return resultArray;
	}
}