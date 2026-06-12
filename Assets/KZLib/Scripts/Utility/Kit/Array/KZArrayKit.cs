using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Utility methods for array shape conversion and 1D/2D index mapping.
/// </summary>
public static class KZArrayKit
{
	/// <summary>
	/// Splits a flat array into a jagged array with the given number of elements per row.
	/// </summary>
	public static TValue[][] ConvertToJaggedArray<TValue>(TValue[] valueArray,int elementPerRow)
	{
		if(valueArray == null)
		{
			throw new ArgumentNullException($"{nameof(valueArray)} is null. Array must be assigned.");
		}

		if(elementPerRow <= 0)
		{
			throw new ArgumentOutOfRangeException($"{elementPerRow} must be a positive integer. ElementPerRow must be assigned.");
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

	/// <summary>
	/// Flattens a three-level jagged array into a single flat array, skipping null inner arrays.
	/// </summary>
	public static TValue[] ConvertToFlatArray<TValue>(TValue[][][] valueArray)
	{
		if(valueArray == null)
		{
			throw new ArgumentNullException($"{nameof(valueArray)} is null. ValueArray must be assigned.");
		}

		var resultList = new List<TValue>();

		for(var i=0;i<valueArray.Length;i++)
		{
			var outerArray = valueArray[i];

			if(outerArray == null)
			{
				continue;
			}

			for(var j=0;j<outerArray.Length;j++)
			{
				var innerArray = outerArray[j];

				if(innerArray == null)
				{
					continue;
				}

				for(var k=0;k<innerArray.Length;k++)
				{
					resultList.Add(innerArray[k]);
				}
			}
		}

		return resultList.ToArray();
	}

	/// <summary>
	/// Flattens a two-level jagged array into a single flat array, skipping null inner arrays.
	/// </summary>
	public static TValue[] ConvertToFlatArray<TValue>(TValue[][] valueArray)
	{
		if(valueArray == null)
		{
			throw new ArgumentNullException($"{nameof(valueArray)} is null. ValueArray must be assigned.");
		}

		var resultList = new List<TValue>();

		for(var i=0;i<valueArray.Length;i++)
		{
			var innerArray = valueArray[i];

			if(innerArray == null)
			{
				continue;
			}

			for(var j=0;j<innerArray.Length;j++)
			{
				resultList.Add(innerArray[j]);
			}
		}

		return resultList.ToArray();
	}

	/// <summary>
	/// Converts a rectangular jagged array into a two-dimensional rectangular array.
	/// All rows must have the same length.
	/// </summary>
	public static TValue[,] ConvertJaggedToRectangularArray<TValue>(TValue[][] valueArray)
	{
		if(valueArray.IsNullOrEmpty())
		{
			throw new ArgumentException($"{nameof(valueArray)} is null or empty. ValueArray must be assigned.");
		}

		var columnCount = valueArray[0].Length;
		var rowCount = valueArray.Length;

		if(columnCount == 0)
		{
			throw new ArgumentException($"The first row of the jagged array cannot be empty: {nameof(valueArray)}={valueArray}. ValueArray must be assigned.");
		}

		for(var i=0;i<rowCount;i++)
		{
			if(valueArray[i] == null || valueArray[i].Length != columnCount)
			{
				throw new ArgumentException($"The jagged array must be rectangular to be converted to a rectangular array: {nameof(valueArray)}={valueArray}. ValueArray must be assigned.");
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

	/// <summary>
	/// Converts a 2D grid coordinate to a 1D index using row-major order.
	/// </summary>
	public static int Convert2DIndexTo1D(Vector2Int vector,int length)
	{
		return Convert2DIndexTo1D(vector.x,vector.y,length);
	}

	/// <summary>
	/// Converts a 2D grid coordinate to a 1D index using row-major order.
	/// </summary>
	public static int Convert2DIndexTo1D(int x,int y,int length)
	{
		return (length*y)+x;
	}

	/// <summary>
	/// Converts a 1D index to a 2D grid coordinate using row-major order.
	/// </summary>
	public static Vector2Int Convert1DIndexTo2D(int index,int length)
	{
		if(length <= 0)
		{
			throw new ArgumentOutOfRangeException($"Length must be greater than zero: length={length}. Length must be assigned.");
		}

		var y = index/length;
		var x = index-(length*y);

		return new Vector2Int(x,y);
	}
}
