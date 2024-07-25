using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static partial class CommonUtility
{
	public static Vector2 Normalized(float _radians,float _distance = 1.0f)
	{
		return new Vector2(Mathf.Cos(_radians)*_distance,Mathf.Sin(_radians)*_distance);
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

	

	

	
}