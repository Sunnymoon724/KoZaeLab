using System;
using System.Collections.Generic;
using UnityEngine;

public static partial class CommonUtility
{
	public static TValue[][] ConvertToJaggedArray<TValue>(TValue[] valueArray,int elementPerRow)
	{
		if(elementPerRow <= 0)
		{
			throw new ArgumentOutOfRangeException($"{elementPerRow} must be a positive integer.");
		}

		var totalLength = valueArray.Length;
		var rowCount = Mathf.CeilToInt((float)totalLength/elementPerRow);
		var resultArray = new TValue[rowCount][];

		for(var i=0;i<rowCount;i++)
		{
			var startIndex = i * elementPerRow;
			var currentLength = Math.Min(elementPerRow,totalLength-startIndex);

			resultArray[i] = new TValue[currentLength];

			Array.Copy(valueArray,startIndex,resultArray[i],0,currentLength);
		}

		return resultArray;
	}

	public static TValue[] ConvertToFlatArray<TValue>(TValue[][][] valueArray)
	{
		var resultList = new List<TValue>();

		for(var i=0;i<valueArray.Length;i++)
		{
			var outerArray = valueArray[i];

			for(var j=0;j<outerArray.Length;j++)
			{
				var innerArray = outerArray[j];

				for(var k=0;k<innerArray.Length;k++)
				{
					var value = innerArray[k];

					resultList.Add(value);
				}
			}
		}

		return resultList.ToArray();
	}

	public static TValue[] ConvertToFlatArray<TValue>(TValue[][] valueArray)
	{
		var resultList = new List<TValue>();

		for(var i=0;i<valueArray.Length;i++)
		{
			var innerArray = valueArray[i];

			for(var j=0;j<innerArray.Length;j++)
			{
				var value = innerArray[j];

				resultList.Add(value);
			}
		}

		return resultList.ToArray();
	}

	public static TValue[,] ConvertJaggedToRectangularArray<TValue>(TValue[][] valueArray)
	{
		if(valueArray.IsNullOrEmpty())
		{
			throw new ArgumentException("Array cannot be null or empty.");
		}

		var columnCount = valueArray[0].Length;
		var rowCount = valueArray.Length;

		if(columnCount == 0)
		{
			throw new ArgumentException("The first row of the jagged array cannot be empty.");
		}

		for(var i=0;i<rowCount;i++)
		{
			if(valueArray[i] == null || valueArray[i].Length != columnCount)
			{
				throw new ArgumentException("The jagged array must be rectangular to be converted to a rectangular array.");
			}
		}

		var resultArray = new TValue[rowCount,columnCount];

		for(var i=0;i<rowCount;i++)
		{
			for(var j=0;j<columnCount;j++)
			{
				resultArray[i,j] = valueArray[i][j];
			}
		}

		return resultArray;
	}

	public static int Convert2DIndexTo1D(Vector2Int vector,int length)
	{
		return Convert2DIndexTo1D(vector.x,vector.y,length);
	}

	public static int Convert2DIndexTo1D(int x,int y,int length)
	{
		return (length*y)+x;
	}

	public static Vector2Int Convert1DIndexTo2D(int index,int length)
	{
		var y = index/length;
		var x = index-(length*y);

		return new Vector2Int(x,y);
	}
}