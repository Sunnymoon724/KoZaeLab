using System.Collections.Generic;
using UnityEngine;

public static partial class MathUtility
{
	#region Parabola
	public static Vector3 Parabola(Vector3 _start,Vector3 _velocity,float _time,float? _gravity = null)
	{
		var result = Vector3.zero;
		var gravity = GetGravity(_gravity);

		result.x = _start.x + _velocity.x*_time;
		result.y = _start.y + (_velocity.y*_time)-(0.5f*gravity*_time*_time);
		result.z = _start.z + _velocity.z*_time;

		return result;
	}
	#endregion Parabola

	public static Vector3 GetTrajectoryVelocity(Transform _projectile,Transform _target,float _angle,float? _gravity = null)
	{
		var startXZ = _projectile.position.MaskY();
		var endXZ = _target.position.MaskY();

		_projectile.LookAt(endXZ);

		var range = Vector3.Distance(startXZ,endXZ);
		var gravity = GetGravity(_gravity);
		var angle = Mathf.Tan(_angle*Mathf.Deg2Rad);
		var height = _target.position.y-_projectile.position.y;

		var velocity = Mathf.Sqrt(gravity*range*range/(2.0f*(height-range*angle)));

		return _projectile.TransformDirection(new Vector3(0.0f,angle*velocity,velocity));
	}

	private static float GetGravity(float? _gravity = null)
	{
		return _gravity ?? Physics.gravity.y;
	}

	#region Bezier Curve
	public static bool IsValidCubicBezier(int _count,bool _isClosed)
	{
		return _count >= (_isClosed ? 6 : 4);
	}

    public static Vector3[] GetCubicBezierCurve(Vector3[] _pointArray,bool _isClosed,float _resolution)
	{
		var length = _pointArray.Length;

		if(!IsValidCubicBezier(length,_isClosed))
		{
			return null;
		}

        var pointList = new List<Vector3>();
		var count = _isClosed ? length/3 : (length-1)/3;

		for(var i=0;i<count;i++)
		{
			pointList.AddRange(GetCubicBezierCurve(_pointArray[i*3+0],_pointArray[i*3+1],_pointArray[i*3+2],_pointArray[LoopClamp(i*3+3,length)],_resolution));
		}

		return pointList.ToArray();
	}

	public static Vector3[] GetCubicBezierCurve(Vector3 _point0,Vector3 _point1,Vector3 _point2,Vector3 _point3,float _resolution)
	{
		var pointList = new List<Vector3>();
		var count = Mathf.FloorToInt(1.0f*_resolution);

		for(var i=0;i<=count;i++)
		{
			var time = i/_resolution;
			var data = 1.0f-time;

			pointList.Add((data*data*data*_point0) + (3*data*data*time*_point1) + (3*data*time*time*_point2) + (time*time*time*_point3));
		}

		return pointList.ToArray();
	}
	#endregion Bezier
}