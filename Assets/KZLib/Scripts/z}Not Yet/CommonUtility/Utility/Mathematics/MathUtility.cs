using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EaseType
{
	Linear,
	RiseFall, FallRise, Mountain, Valley,
	InSine, OutSine, InOutSine, OutInSine,

	InQuad, OutQuad, InOutQuad, OutInQuad,
	InCubic, OutCubic, InOutCubic, OutInCubic,
	InQuart, OutQuart, InOutQuart, OutInQuart,
	InQuint, OutQuint, InOutQuint, OutInQuint,

	InExpo, OutExpo, InOutExpo, OutInExpo,
	InCirc, OutCirc, InOutCirc, OutInCirc,

	InBounce, OutBounce, InOutBounce, OutInBounce,

	InElastic, OutElastic, InOutElastic, OutInElastic,

	InBack, OutBack, InOutBack, OutInBack,
	// Flash, InFlash, OutFlash, InOutFlash
}

public static partial class MathUtility
{
	#region Curve
	/// <summary>
	/// EaseType 변환
	/// </summary>
	public static AnimationCurve GetEaseCurve(EaseType _type)
	{
		switch(_type)
		{
			#region Sine
			case EaseType.InSine:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.0f),new Keyframe(1.0f,1.0f,1.5f,0.0f));
			}
			case EaseType.OutSine:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,1.5f),new Keyframe(1.0f,1.0f,0.0f,0.0f));
			}
			case EaseType.InOutSine:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.0f),new Keyframe(0.5f,0.5f,1.5f,1.5f),new Keyframe(1.0f,1.0f,0.0f,0.0f));
			}
			case EaseType.OutInSine:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,1.5f),new Keyframe(0.5f,0.5f,0.0f,0.0f),new Keyframe(1.0f,1.0f,1.5f,0.0f));
			}
			#endregion Sine

			#region Quad
			case EaseType.InQuad:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.0f),new Keyframe(1.0f,1.0f,2.0f,0.0f));
			}
			case EaseType.OutQuad:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,2.0f),new Keyframe(1.0f,1.0f,0.0f,0.0f));
			}
			case EaseType.InOutQuad:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.0f),new Keyframe(0.5f,0.5f,2.0f,2.0f),new Keyframe(1.0f,1.0f,0.0f,0.0f));
			}
			case EaseType.OutInQuad:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,2.0f),new Keyframe(0.5f,0.5f,0.0f,0.0f),new Keyframe(1.0f,1.0f,2.0f,0.0f));
			}
			#endregion Quad

			#region Cubic
			case EaseType.InCubic:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.0f),new Keyframe(1.0f,1.0f,3.0f,0.0f));
			}
			case EaseType.OutCubic:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,3.0f),new Keyframe(1.0f,1.0f,0.0f,0.0f));
			}
			case EaseType.InOutCubic:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.0f),new Keyframe(0.5f,0.5f,3.0f,3.0f),new Keyframe(1.0f,1.0f,0.0f,0.0f));
			}
			case EaseType.OutInCubic:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,3.0f),new Keyframe(0.5f,0.5f,0.0f,0.0f),new Keyframe(1.0f,1.0f,3.0f,0.0f));
			}
			#endregion Cubic

			#region Quartic
			case EaseType.InQuart:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.0f),new Keyframe(1.0f,1.0f,4.0f,0.0f));
			}
			case EaseType.OutQuart:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,4.0f),new Keyframe(1.0f,1.0f,0.0f,0.0f));
			}
			case EaseType.InOutQuart:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.0f),new Keyframe(0.5f,0.5f,4.0f,4.0f),new Keyframe(1.0f,1.0f,0.0f,0.0f));
			}
			case EaseType.OutInQuart:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,4.0f),new Keyframe(0.5f,0.5f,0.0f,0.0f),new Keyframe(1.0f,1.0f,4.0f,0.0f));
			}
			#endregion Quartic

			#region Quintic
			case EaseType.InQuint:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.0f),new Keyframe(1.0f,1.0f,5.0f,0.0f));
			}
			case EaseType.OutQuint:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,5.0f),new Keyframe(1.0f,1.0f,0.0f,0.0f));
			}
			case EaseType.InOutQuint:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.0f),new Keyframe(0.5f,0.5f,5.0f,5.0f),new Keyframe(1.0f,1.0f,0.0f,0.0f));
			}
			case EaseType.OutInQuint:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,5.0f),new Keyframe(0.5f,0.5f,0.0f,0.0f),new Keyframe(1.0f,1.0f,5.0f,0.0f));
			}
			#endregion Quintic

			#region Expo
			case EaseType.InExpo:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.0f),new Keyframe(1.0f,1.0f,7.0f,0.0f));
			}
			case EaseType.OutExpo:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,7.0f),new Keyframe(1.0f,1.0f,0.0f,0.0f));
			}
			case EaseType.InOutExpo:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.0f),new Keyframe(0.5f,0.5f,7.0f,7.0f),new Keyframe(1.0f,1.0f,0.0f,0.0f));
			}
			case EaseType.OutInExpo:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,7.0f),new Keyframe(0.5f,0.5f,0.0f,0.0f),new Keyframe(1.0f,1.0f,7.0f,0.0f));
			}
			#endregion Expo

			#region Circle
			case EaseType.InCirc:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.0f),new Keyframe(1.0f,1.0f,460.0f,0.0f));
			}
			case EaseType.OutCirc:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,460.0f),new Keyframe(1.0f,1.0f,0.0f,0.0f));
			}
			case EaseType.InOutCirc:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.0f),new Keyframe(0.5f,0.5f,460.0f,460.0f),new Keyframe(1.0f,1.0f,0.0f,0.0f));
			}
			case EaseType.OutInCirc:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,460.0f),new Keyframe(0.5f,0.5f,0.0f,0.0f),new Keyframe(1.0f,1.0f,460.0f,0.0f));
			}
			#endregion Circle

			#region Bounce
			case EaseType.InBounce:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.7f),new Keyframe(0.1f,0.0f,-0.7f,1.4f),new Keyframe(0.3f,0.0f,-1.4f,2.7f),new Keyframe(0.6f,0.0f,-2.7f,5.5f),new Keyframe(1.0f,1.0f,0.0f,0.0f));
			}
			case EaseType.OutBounce:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.0f),new Keyframe(0.4f,1.0f,5.5f,-2.7f),new Keyframe(0.7f,1.0f,2.7f,-1.4f),new Keyframe(0.9f,1.0f,1.4f,-0.7f),new Keyframe(1.0f,1.0f,0.7f,0.0f));
			}
			case EaseType.InOutBounce:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.7f),new Keyframe(0.1f,0.0f,-0.7f,1.4f),new Keyframe(0.2f,0.0f,-1.4f,2.7f),new Keyframe(0.3f,0.0f,-2.7f,5.5f),new Keyframe(0.5f,0.5f,0.0f,0.0f),new Keyframe(0.6f,1.0f,5.5f,-2.7f),new Keyframe(0.8f,1.0f,2.7f,-1.4f),new Keyframe(0.9f,1.0f,1.4f,-0.7f),new Keyframe(1.0f,1.0f,0.7f,0.0f));
			}
			case EaseType.OutInBounce:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.0f),new Keyframe(0.2f,1.0f,5.5f,-2.7f),new Keyframe(0.3f,1.0f,2.7f,-1.4f),new Keyframe(0.4f,1.0f,1.4f,-0.7f),new Keyframe(0.5f,0.5f,0.7f,0.7f),new Keyframe(0.6f,0.0f,-0.7f,1.4f),new Keyframe(0.7f,0.0f,-1.4f,2.7f),new Keyframe(0.8f,0.0f,-2.7f,5.5f),new Keyframe(1.0f,1.0f,0.0f,0.0f));
			}
			#endregion Bounce

			#region Back
			case EaseType.InBack:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.0f),new Keyframe(1.0f,1.0f,4.7f,0.0f));
			}
			case EaseType.OutBack:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,4.7f),new Keyframe(1.0f,1.0f,0.0f,0.0f));
			}
			case EaseType.InOutBack:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.0f),new Keyframe(0.5f,0.5f,4.7f,4.7f),new Keyframe(1.0f,1.0f,0.0f,0.0f));
			}
			case EaseType.OutInBack:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,4.7f),new Keyframe(0.5f,0.5f,0.0f,0.0f),new Keyframe(1.0f,1.0f,4.7f,0.0f));
			}
			#endregion Back

			#region Elastic
			case EaseType.InElastic:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.0f),new Keyframe(0.2f,0.0f,0.0f,-0.1f),new Keyframe(0.5f,0.0f,-0.4f,-0.6f),new Keyframe(0.8f,0.0f,-3.2f,-4.4f),new Keyframe(1.0f,1.0f,12.5f,0.0f));
			}
			case EaseType.OutElastic:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,12.5f),new Keyframe(0.2f,1.0f,-4.4f,-3.2f),new Keyframe(0.5f,1.0f,-0.6f,-0.4f),new Keyframe(0.8f,1.0f,-0.1f,0.0f),new Keyframe(1.0f,1.0f,0.5f,0.0f));
			}
			case EaseType.InOutElastic:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.0f),new Keyframe(0.1f,0.0f,0.0f,-0.1f),new Keyframe(0.3f,0.0f,-0.4f,-0.6f),new Keyframe(0.4f,0.0f,-3.2f,-4.4f),new Keyframe(0.5f,0.5f,12.5f,12.5f),new Keyframe(0.6f,1.0f,-4.4f,-3.2f),new Keyframe(0.7f,1.0f,-0.6f,-0.4f),new Keyframe(0.9f,1.0f,-0.1f,0.0f),new Keyframe(1.0f,1.0f,0.5f,0.0f));
			}
			case EaseType.OutInElastic:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,12.5f),new Keyframe(0.1f,1.0f,-4.4f,-3.2f),new Keyframe(0.3f,1.0f,-0.6f,-0.4f),new Keyframe(0.4f,1.0f,-0.1f,0.0f),new Keyframe(0.5f,0.5f,0.5f,0.0f),new Keyframe(0.6f,0.0f,0.0f,-0.1f),new Keyframe(0.7f,0.0f,-0.4f,-0.6f),new Keyframe(0.9f,0.0f,-3.2f,-4.4f),new Keyframe(1.0f,1.0f,12.5f,0.0f));
			}
			#endregion Elastic

			#region Linear
			case EaseType.Linear:
			{
				return AnimationCurve.Linear(0.0f,0.0f,1.0f,1.0f);
			}
			#endregion Linear

			#region Custom
			case EaseType.RiseFall:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,+2.0f),new Keyframe(0.5f,1.0f,+2.0f,-2.0f),new Keyframe(1.0f,0.0f,-2.0f,0.0f));
			}
			case EaseType.FallRise:
			{
				return new AnimationCurve(new Keyframe(0.0f,1.0f,0.0f,-2.0f),new Keyframe(0.5f,0.0f,-2.0f,+2.0f),new Keyframe(1.0f,1.0f,+2.0f,0.0f));
			}
			case EaseType.Mountain:
			{
				return new AnimationCurve(new Keyframe(0.0f,0.0f,0.0f,0.0f),new Keyframe(0.25f,0.0f,0.0f,+4.0f),new Keyframe(0.5f,1.0f,+4.0f,-4.0f),new Keyframe(0.75f,0.0f,-4.0f,0.0f),new Keyframe(1.0f,0.0f,0.0f,0.0f));
			}
			case EaseType.Valley:
			{
				return new AnimationCurve(new Keyframe(0.0f,1.0f,0.0f,0.0f),new Keyframe(0.25f,1.0f,0.0f,-4.0f),new Keyframe(0.5f,0.0f,-4.0f,+4.0f),new Keyframe(0.75f,1.0f,+4.0f,0.0f),new Keyframe(1.0f,1.0f,0.0f,0.0f));
			}
			#endregion Custom
		}

		throw new ArgumentException("정의 되어 있지 않습니다.");
	}

	/// <summary>
	/// 증가 -> 감소 or 감소 -> 증가 직선 그래프
	/// </summary>
	public static AnimationCurve GetIncrementalCurve(bool _increaseStart)
	{
		var curve = new AnimationCurve();

		if(_increaseStart)
		{
			curve.AddKey(new Keyframe(0.0f,0.0f,+0.0f,+2.0f));
			curve.AddKey(new Keyframe(0.5f,1.0f,+2.0f,-2.0f));
			curve.AddKey(new Keyframe(1.0f,0.0f,-2.0f,+0.0f));
		}
		else
		{
			curve.AddKey(new Keyframe(0.0f,1.0f,+0.0f,-2.0f));
			curve.AddKey(new Keyframe(0.5f,0.0f,-2.0f,+2.0f));
			curve.AddKey(new Keyframe(1.0f,1.0f,+2.0f,+0.0f));
		}

		return curve;
	}
	#endregion Curve

	#region Parabola
	/// <summary>
	/// 포물선 구하기
	/// </summary>
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

	/// <summary>
	/// 탄도 쏘는 함수
	/// </summary>
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
			pointList.AddRange(GetCubicBezierCurve(_pointArray[i*3+0],_pointArray[i*3+1],_pointArray[i*3+2],_pointArray[CommonUtility.LoopClamp(i*3+3,length)],_resolution));
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

	#region Distance
	public static float GetTotalDistance(IEnumerable<Vector3> _positionGroup)
	{
		var distance = 0.0f;
		var positionArray = _positionGroup.ToArray();

		for(var i=1;i<positionArray.Length;i++)
		{
			distance += Vector3.Distance(positionArray[i-1],positionArray[i]);
		}

		return distance;
	}
	#endregion Distance
}