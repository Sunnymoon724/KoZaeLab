using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static partial class CommonUtility
{
	public static Rect BoundsToRect(Bounds _bounds)
	{
		return Rect.MinMaxRect(_bounds.min.x,_bounds.min.y,_bounds.max.x,_bounds.max.y);
	}

	public static Vector2 Normalized(float _radians,float _distance = 1.0f)
	{
		return new Vector2(Mathf.Cos(_radians)*_distance,Mathf.Sin(_radians)*_distance);
	}
	
	public static Vector3 Normalized(Quaternion _rotation,float _distance = 1.0f)
	{
		return _rotation*Vector3.forward*_distance;
	}

	/// <summary>
	/// 선 2개 교차 여부
	/// </summary>
	public static bool IsIntersection(Vector2 _point1,Vector2 _point2,Vector2 _point3,Vector2 _point4)
	{
		var determinant = (_point2.x-_point1.x)*(_point4.y-_point3.y)-(_point2.y-_point1.y)*(_point4.x-_point3.x);

		if(determinant == 0.0f)
		{
			return false;
		}

		var gradient1 = ((_point3.x-_point1.x)*(_point4.y-_point3.y)-(_point3.y-_point1.y)*(_point4.x-_point3.x))/determinant;
		var gradient2 = ((_point3.x-_point1.x)*(_point2.y-_point1.y)-(_point3.y-_point1.y)*(_point2.x-_point1.x))/determinant;

		if(gradient1<0.0f || gradient1>1.0f || gradient2<0.0f || gradient2>1.0f)
		{
			return false;
		}

		return true;
	}

	/// <summary>
	/// 선 2개 교차 여부
	/// </summary>
	public static bool IsIntersection(Vector2 _point1,Vector2 _point2,Vector2 _point3,Vector2 _point4,out Vector2 _crossPoint)
	{
		if(!IsIntersection(_point1,_point2,_point3,_point4))
		{
			_crossPoint = Vector2.zero;

			return false;
		}

		var determinant = (_point2.x-_point1.x)*(_point4.y-_point3.y)-(_point2.y-_point1.y)*(_point4.x-_point3.x);

		var gradient = ((_point3.x-_point1.x)*(_point4.y-_point3.y)-(_point3.y-_point1.y)*(_point4.x-_point3.x))/determinant;

		_crossPoint.x = _point1.x+gradient*(_point2.x-_point1.x);
		_crossPoint.y = _point1.y+gradient*(_point2.y-_point1.y);

		return true;
	}

	public static bool IsIntersectionRect(Rect _rect1,Rect _rect2)
	{
		return _rect1.Overlaps(_rect2,true) || _rect2.Overlaps(_rect1,true);
	}
	
	public static bool IsIntersectionRect(Rect _rect1,Rect _rect2,out Rect _outRect)
	{
		if(!IsIntersectionRect(_rect1,_rect2))
		{
			_outRect = Rect.zero;

			return false;
		}

		float x = Mathf.Max(_rect1.xMin,_rect2.xMin);
		float y = Mathf.Max(_rect1.yMin,_rect2.yMin);
		float width = Mathf.Min(_rect1.xMax,_rect2.xMax);
		float heigh = Mathf.Min(_rect1.yMax,_rect2.yMax);

		_outRect = new Rect(x,y,width-x,heigh-y);

		return true;
	}

	private static (Vector3,Quaternion,Vector3) LockTransformToSpace(Transform _transform,SpaceType _space)
	{
		var position = _transform.position;
		var rotation = _transform.rotation;
		var scale = _transform.localScale;

		if(_space == SpaceType.xy)
		{
			_transform.eulerAngles = new Vector3(0.0f,0.0f,_transform.eulerAngles.z);
			_transform.position = new Vector3(_transform.position.x,_transform.position.y,0.0f);
		}
		else if(_space == SpaceType.xz)
		{
			_transform.eulerAngles = new Vector3(0.0f,_transform.eulerAngles.y,0.0f);
			_transform.position = new Vector3(_transform.position.x,0.0f,_transform.position.z);
		}

		var max = Mathf.Max(_transform.lossyScale.x,_transform.lossyScale.y,_transform.lossyScale.z);

		_transform.localScale = Vector3.one*max;

		return (position,rotation,scale);
	}

	public static Vector3 TransformPoint(Vector3 _position,Transform _transform,SpaceType _type)
	{
		var original = LockTransformToSpace(_transform,_type);

		var transformedPoint = _transform.TransformPoint(_position);

		_transform.SetPositionAndRotation(original.Item1,original.Item2);
		_transform.localScale = original.Item3;

		return transformedPoint;
	}

	public static Vector3 InverseTransformPoint(Vector3 _position,Transform _transform,SpaceType _type)
	{
		var original = LockTransformToSpace(_transform,_type);
		Vector3 transformedPoint =_transform.InverseTransformPoint(_position);

		_transform.SetPositionAndRotation(original.Item1,original.Item2);
		_transform.localScale = original.Item3;

		return transformedPoint;
	}

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
}