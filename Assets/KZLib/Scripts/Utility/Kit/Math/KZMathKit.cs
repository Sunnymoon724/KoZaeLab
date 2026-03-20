using System;
using System.Collections.Generic;
using UnityEngine;

public static partial class KZMathKit
{
	#region Distance
	public static float GetTotalDistance(IEnumerable<Vector3> positionGroup)
	{
		if(positionGroup == null)
		{
			return 0.0f;
		}

		using var enumerator = positionGroup.GetEnumerator();

		if(!enumerator.MoveNext())
		{
			return 0.0f;
		}

		var totalDistance = 0.0f;
		var prevPosition = enumerator.Current;

		while(enumerator.MoveNext())
		{
			totalDistance += Vector3.Distance(prevPosition,enumerator.Current);
			prevPosition = enumerator.Current;
		}

		return totalDistance;
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
			LogChannel.Kit.E($"{length} can not be less than 1");

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
			LogChannel.Kit.E("List is null or empty");

			return;
		}

		if(countX < 1 || countZ < 1)
		{
			LogChannel.Kit.E($"{(countX < 1 ? "countX" : "countZ")} can not be less than 1");

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
	
	#region Parabola
	public static Vector3 Parabola(Vector3 start,Vector3 velocity,float time,float? gravity = null)
	{
		var result = Vector3.zero;
		var realGravity = _GetGravity(gravity);

		result.x = start.x + velocity.x*time;
		result.y = start.y + (velocity.y*time)-(0.5f*realGravity*time*time);
		result.z = start.z + velocity.z*time;

		return result;
	}
	#endregion Parabola

	public static Vector3 CalculateLaunchVelocity(Transform projectile,Transform target,float launchAngle,float? gravity = null)
	{
		var startPosition = projectile.position.SetY(0);
		var targetPosition = target.position.SetY(0);

		projectile.LookAt(targetPosition);

		var horizontalDistance = Vector3.Distance(startPosition,targetPosition);
		var effectiveGravity = _GetGravity(gravity);
		var launchAngleTan = Mathf.Tan(launchAngle*Mathf.Deg2Rad);
		var heightDifference = target.position.y-projectile.position.y;
		var velocity = Mathf.Sqrt(effectiveGravity*horizontalDistance*horizontalDistance/(2.0f*(heightDifference-horizontalDistance*launchAngleTan)));

		return projectile.TransformDirection(new Vector3(0.0f,launchAngleTan*velocity,velocity));
	}

	private static float _GetGravity(float? gravity = null)
	{
		return gravity ?? Physics.gravity.y;
	}

	#region Bezier Curve
	public static bool IsValidCubicBezier(int _count,bool isClosed)
	{
		return _count >= (isClosed ? 6 : 4);
	}

	public static Vector3[] CalculateCubicBezierCurve(Vector3[] pointArray,bool isClosed,float resolution)
	{
		var length = pointArray.Length;

		if(!IsValidCubicBezier(length,isClosed))
		{
			return null;
		}

		var count = isClosed ? length/3 : (length-1)/3;
		var index = 0;
		var resultArray = new Vector3[count*(Mathf.FloorToInt(1.0f*resolution)+1)];

		for(var i=0;i<count;i++)
		{
			var segmentPointArray = CalculateCubicBezierCurve(pointArray[i*3+0],pointArray[i*3+1],pointArray[i*3+2],pointArray[LoopClamp(i*3+3,length)],resolution);
			
			Array.Copy(segmentPointArray,0,resultArray,index,segmentPointArray.Length);
			index += segmentPointArray.Length;
		}

		return resultArray;
	}

	public static Vector3[] CalculateCubicBezierCurve(Vector3 point0,Vector3 point1,Vector3 point2,Vector3 point3,float resolution)
	{
		var count = Mathf.FloorToInt(1.0f*resolution);
		var pointArray = new Vector3[count+1];

		for(var i=0;i<=count;i++)
		{
			var time = i/resolution;
			var value = 1.0f-time;

			pointArray[i] = (value*value*value*point0) + (3*value*value*time*point1) + (3*value*time*time*point2) + (time*time*time*point3);
		}

		return pointArray;
	}
	#endregion Bezier Curve
}