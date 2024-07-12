using UnityEngine;

public static class Vector2Extension
{
	#region Mask
	public static Vector2 MaskX(this Vector2 _vector,float _x = 0.0f)
	{
		return new Vector2(_x,_vector.y);
	}

	public static Vector2 MaskY(this Vector2 _vector,float _y = 0.0f)
	{
		return new Vector2(_vector.x,_y);
	}
	#endregion Mask

	#region Offset
	public static Vector2 Offset(this Vector2 _vector,Vector2 _offset)
	{
		return new Vector2(_vector.x+_offset.x,_vector.y+_offset.y);
	}

	public static Vector2 OffsetX(this Vector2 _vector,float _x)
	{
		return _vector.Offset(new(_x,0.0f));
	}

	public static Vector2 OffsetY(this Vector2 _vector,float _y)
	{
		return _vector.Offset(new(0.0f,_y));
	}
    #endregion Offset

	#region Invert
	public static Vector2 Invert(this Vector2 _vector)
	{
		return new Vector2(-_vector.x,-_vector.y);
	}

	public static Vector2 InvertX(this Vector2 _vector)
	{
		return new Vector2(-_vector.x,_vector.y);
	}

	public static Vector2 InvertY(this Vector2 _vector)
	{
		return new Vector2(_vector.x,-_vector.y);
	}
	#endregion Invert

	#region ToVector
	public static Vector3 ToVector3(this Vector2 _vector,float _z = 0.0f)
	{
		return new(_vector.x,_vector.y,_z);
	}

	public static Vector4 ToVector4(this Vector3 _vector,float _z = 0.0f,float _w = 0.0f)
	{
		return new(_vector.x,_vector.y,_z,_w);
	}

	public static string ToVectorString(this Vector2 _vector,int _decimals = 2)
	{
		var format = string.Format("({1}{0}}}, {2}{0}}})",_decimals,"{0:f","{1:f");

		return string.Format(format,_vector.x,_vector.y);
	}
	#endregion ToVector


	public static Vector2 Transpose(this Vector2 _vector)
	{
		return new Vector2(_vector.y,_vector.x);
	}

	public static bool IsEquals(this Vector2 _vector1,Vector2 _vector2)
	{
		return _vector1.x.Approximately(_vector2.x) && _vector1.y.Approximately(_vector2.y);
	}
	
	public static bool IsZero(this Vector2 _vector) 
	{
		return _vector.x.ApproximatelyZero() && _vector.y.ApproximatelyZero();
	}

	public static Vector2 Reciprocal(this Vector2 _vector)
	{
		return new Vector2(1.0f/_vector.x,1.0f/_vector.y);
	}

	public static Vector2 DistanceEach(this Vector2 _vector1,Vector2 _vector2)
	{
		return new Vector2(Mathf.Abs(_vector1.x-_vector2.x),Mathf.Abs(_vector1.y-_vector2.y));
	}

	public static Vector2 Round(this Vector2 _vector)
	{
		return new Vector2(Mathf.Round(_vector.x),Mathf.Round(_vector.y));
	}

	public static Vector2 Ceil(this Vector2 _vector)
	{
		return new Vector2(Mathf.Ceil(_vector.x),Mathf.Ceil(_vector.y));
	}

	public static Vector2 Floor(this Vector2 _vector)
	{
		return new Vector2(Mathf.Floor(_vector.x),Mathf.Floor(_vector.y));
	}

	public static Vector2 Abs(this Vector2 _vector)
	{
		return new Vector2(Mathf.Abs(_vector.x),Mathf.Abs(_vector.y));
	}

	public static Vector2 Clamp(this Vector2 _vector,float _min,float _max)
	{
		return new Vector2(Mathf.Clamp(_vector.x,_min,_max),Mathf.Clamp(_vector.y,_min,_max));
	}

	public static Vector2 Clamp01(this Vector2 _vector)
	{
		return new Vector2(Mathf.Clamp01(_vector.x),Mathf.Clamp01(_vector.y));
	}

	public static float MaxValue(this Vector2 _vector)
	{
		return _vector.x >= _vector.y ? _vector.x : _vector.y;
	}

	public static float MinValue(this Vector2 _vector)
	{
		return _vector.x <= _vector.y ? _vector.x : _vector.y;
	}

	public static Vector2 MultiplyEach(this Vector2 _vector1,Vector2 _vector2)
	{
		return new Vector2(_vector1.x*_vector2.x,_vector1.y*_vector2.y);
	}

	public static Vector2 MiddleVector(this Vector2 _vector1,Vector2 _vector2)
	{
		return new Vector2((_vector2.x-_vector1.x)/2.0f,(_vector2.y-_vector1.y)/2.0f);
	}

	public static Vector2 Rotate(this Vector2 _vector,float _angle)
	{
		return Quaternion.AngleAxis(_angle,Vector3.forward)*_vector;
	}

	public static float Angle(this Vector2 _vector)
	{
		return _vector.y > 0.0f ? Vector2.Angle(Vector2.right,_vector) : -Vector2.Angle(Vector2.right,_vector);
	}

	public static Vector2 RotateAround(this Vector2 _point,Vector2 _origin,float _theta)
	{
		var sin = Mathf.Sin(_theta);
		var cos = Mathf.Cos(_theta);
		
		_point -= _origin;

		return new Vector2(_point.x*cos-_point.y*sin+_origin.x,_point.x*sin+_point.y*cos+_origin.y);
	}

	public static (Vector2 Point,int Index) GetClosestPoint(this Vector2 _point,params Vector2[] _pointArray)
	{
		var index = -1;
		var point = Vector2.zero;
		var closest = float.MaxValue;

		for(var i=0;i<_pointArray.Length;i++)
		{
			var distance = (_point-_pointArray[i]).sqrMagnitude;

			if(distance < closest)
			{
				closest = distance;

				index = i;
				point = _pointArray[i];
			}
		}

		return (point,index);
	}

	public static (Vector2 point,float distance) GetClosestPointOnRay(this Vector2 _point,Vector2 _origin,Vector2 _direction)
	{
		var distance = Vector2.Dot(_point-_origin,_direction);

		return (_origin+_direction*distance,distance);
	}

	public static (Vector2 point,float distance) GetClosestPointOnSegment(this Vector2 _point,Vector2 _start,Vector2 _end)
	{
		var direction = _end-_start;
		var magnitude = direction.magnitude;

		direction.Normalize();

		var distance = Mathf.Clamp(Vector2.Dot(_point-_start,direction),0.0f,magnitude);

		return (_start+direction*distance,distance);
	}

	public static Vector3 TransformInputBasedOnCanvasType(this Vector2 _point,RectTransform _rectTransform)
	{
		var canvas = _rectTransform.GetParentCanvas();

		if(_point == Vector2.zero || canvas.renderMode == RenderMode.ScreenSpaceOverlay)
		{
			return _point;
		}

		RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform,_point,canvas.GetEventCamera(),out var movePos);

		return canvas.transform.TransformPoint(movePos);
	}

	public static Vector2 TransformInputBasedOnCanvasType(this Vector2 _input,Canvas _canvas)
	{
		return _canvas.renderMode != RenderMode.ScreenSpaceOverlay ? _input : _canvas.GetEventCamera().ScreenToWorldPoint(_input);
	}

	public static Vector2 To(this Vector2 _source,Vector2 _destination)
	{
		return _destination-_source;
	}
}