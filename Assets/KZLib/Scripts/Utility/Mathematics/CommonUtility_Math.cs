using System;
using System.Collections.Generic;
using UnityEngine;

public static partial class CommonUtility
{
	#region Distance
	public static float GetTotalDistance(IEnumerable<Vector3> _posGroup)
	{
		var distance = 0.0f;
		var prevPos = Vector3.zero;
		var isFirst = true;

		foreach(var pos in _posGroup)
		{
			if(!isFirst)
			{
				distance += Vector3.Distance(prevPos,pos);
			}
			else
			{
				isFirst = false;
			}

			prevPos = pos;
		}

		return distance;
	}
	#endregion Distance

	#region Clamp
	public static int LoopClamp(int index,int size)
	{
		return size < 1 ? 0 : index < 0 ? size-1+(index+1)%size : index%size;
	}

	public static float LoopClamp(float index,int size)
	{
		return size < 1 ? 0 : index < 0.0f ? size-1+(index+1)%size : index%size;
	}

	public static TCompare Clamp<TCompare>(TCompare value,TCompare minValue,TCompare maxValue) where TCompare : IComparable<TCompare>
	{
		return value.CompareTo(minValue) < 0 ? minValue : value.CompareTo(maxValue) > 0 ? maxValue : value;
	}

	public static TCompare MinClamp<TCompare>(TCompare value,TCompare minValue) where TCompare : IComparable<TCompare>
	{
		return Clamp(value,minValue,value);
	}

	public static TCompare MaxClamp<TCompare>(TCompare value,TCompare maxValue) where TCompare : IComparable<TCompare>
	{
		return Clamp(value,value,maxValue);
	}
	#endregion Clamp

	#region Slope
	public static float SlopeToRadian(float x,float y)
	{
		return SlopeToRadian(new Vector2(x,y));
	}

	public static float SlopeToRadian(Vector2 gradient)
	{
		return Mathf.Atan2(gradient.y,gradient.x);
	}

	public static Vector2 RadianToSlope(float angle)
	{
		return new Vector2(Mathf.Cos(angle),Mathf.Sin(angle));
	}

	public static float SlopeToDegree(float x,float y)
	{
		return SlopeToDegree(new Vector2(x,y));
	}

	public static float SlopeToDegree(Vector2 gradient)
	{
		return SlopeToRadian(gradient)*Mathf.Rad2Deg;
	}

	public static Vector2 DegreeToSlope(float angle)
	{
		return new Vector2(Mathf.Cos(angle*Mathf.Deg2Rad),Mathf.Sin(angle*Mathf.Deg2Rad));
	}
	#endregion Slope

	#region Alignment
	/// <summary>
	/// Set alignment ( 0 / -0.5 +0.5 / -1 0 +1 / -1.5 -0.5 +0.5 +1.5)
	/// </summary>
	public static float[] MiddleAlignment(int length)
	{
		var pivotArray = new float[length];
		var pivot = pivotArray.Length/2.0f;

		//? odd
		if(length%2 == 1)
		{
			for(var i=0;i<(int)pivot;i++)
			{
				pivotArray[(int) Mathf.Floor(pivot)-(i+1)]=-(i+1);
				pivotArray[(int) Mathf.Ceil(pivot)+(i+0)]=+(i+1);
			}
		}
		//? even
		else
		{
			var divide = 0.5f;

			for(var i=0;i<(int) pivot;i++)
			{
				pivotArray[(int) Mathf.Floor(pivot)-(i+1)]=-(2*i+1)*divide;
				pivotArray[(int) Mathf.Ceil(pivot)+(i+0)]=+(2*i+1)*divide;
			}
		}

		return pivotArray;
	}

	public static void SetAlignmentGameObjectList(List<GameObject> objectList,int xMax,int hMax,float xGap,float yGap)
	{
		if(objectList.IsNullOrEmpty())
		{
			LogTag.System.E("List is null or empty");

			return;
		}

		var widthAble   = xMax != -1;
		var heightAble  = xMax != -1;
		var widthArray  = widthAble ? MiddleAlignment(xMax) : null;
		var heightArray = heightAble ? MiddleAlignment(Mathf.CeilToInt(objectList.Count/(float) hMax)) : null;

		for(var i=0;i<objectList.Count;i++)
		{
			var width   = widthAble     ? widthArray[i%xMax]*xGap     : objectList[i].transform.position.x;
			var height  = heightAble    ? heightArray[i/hMax]*yGap    : objectList[i].transform.position.z;

			objectList[i].transform.position = new Vector3(width,0.0f,height);
		}
	}
	#endregion Alignment
}