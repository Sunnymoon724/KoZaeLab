using System;
using UnityEngine;

public static partial class CommonUtility
{
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
	#endregion Bezier
}