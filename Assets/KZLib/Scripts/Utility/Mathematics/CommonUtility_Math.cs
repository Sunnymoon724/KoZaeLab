using System;
using System.Collections.Generic;
using UnityEngine;

public static partial class CommonUtility
{
	#region Distance
	public static float GetTotalDistance(IEnumerable<Vector3> positionGroup)
	{
		using var enumerator = positionGroup.GetEnumerator();

		if(!enumerator.MoveNext())
		{
			return 0.0f;
		}

		var distance = 0.0f;
		var prevPosition = enumerator.Current;

		while(enumerator.MoveNext())
		{
			distance += Vector3.Distance(prevPosition,enumerator.Current);
			prevPosition = enumerator.Current;
		}

		return distance;
	}

	#endregion Distance

	#region Clamp
	public static int LoopClamp(int index,int size)
	{
		if(size <= 0)
		{
			throw new ArgumentOutOfRangeException("size must be greater than zero.");
		}

		return (index%size+size)%size;
	}

	public static float LoopClamp(float index,int size)
	{
		if(size <= 0)
		{
			throw new ArgumentOutOfRangeException("size must be greater than zero.");
		}

		return (index%size+size)%size;
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
		if(length <= 0)
		{
			LogSvc.System.E($"{length} can not be less than 1");

			return null;
		}

		if(length == 1)
		{
			return new float[1] { 0.0f };
		}

		var resultArray = new float[length];
        var offset = (length-1)/2.0f;

        for(var i=0;i<length;i++)
        {
            resultArray[i] = i-offset;
        }

        return resultArray;
	}

	public static void SetAlignmentGameObjectList(List<GameObject> objectList,int countX,int countZ,float gapX,float gapZ,float positionY = 0.0f)
	{
		if(objectList.IsNullOrEmpty())
		{
			LogSvc.System.E("List is null or empty");

			return;
		}

		if(countX < 1 || countZ < 1)
		{
			LogSvc.System.E($"{(countX < 1 ? "countX" : "countZ")} can not be less than 1");

			return;
		}


		var xArray  = MiddleAlignment(countX);

		var zSize = Mathf.Max(1,Mathf.CeilToInt(objectList.Count/(float)countZ));
		var zArray = MiddleAlignment(zSize);

		for(var i=0;i<objectList.Count;i++)
		{
			var positionX = xArray[i%countX]*gapX;
			var positionZ = zArray[i%zSize]*gapZ;

			objectList[i].transform.localPosition = new Vector3(positionX,positionY,positionZ);
		}
	}
	#endregion Alignment
}