using UnityEngine;

public static class Vector3Extension
{
	#region Set
	public static Vector3 SetX(this Vector3 _vector,float _x = 0.0f)
	{
		return new Vector3(_x,_vector.y,_vector.z);
	}

	public static Vector3 SetY(this Vector3 _vector,float _y = 0.0f)
	{
		return new Vector3(_vector.x,_y,_vector.z);
	}

	public static Vector3 SetZ(this Vector3 _vector,float _z = 0.0f)
	{
		return new Vector3(_vector.x,_vector.y,_z);
	}

	public static Vector3 SetXY(this Vector3 _vector,float _x = 0.0f,float _y = 0.0f) 
	{
		return new Vector3(_x,_y,_vector.z);
	}

	public static Vector3 SetXY(this Vector3 _vector,Vector2 _point = new Vector2())
	{
		return _vector.SetXY(_point.x,_point.y);
	}

	public static Vector3 SetXZ(this Vector3 _vector,float _x = 0.0f,float _z = 0.0f) 
	{
		return new Vector3(_x,_vector.y,_z);
	}

	public static Vector3 SetXZ(this Vector3 _vector,Vector2 _point = new Vector2())
	{
		return _vector.SetXZ(_point.x,_point.y);
	}

	public static Vector3 SetYZ(this Vector3 _vector,float _y = 0.0f,float _z = 0.0f) 
	{
		return new Vector3(_vector.x,_y,_z);
	}

	public static Vector3 SetYZ(this Vector3 _vector,Vector2 _point = new Vector2())
	{
		return _vector.SetYZ(_point.x,_point.y);
	}
	#endregion Set

	#region Offset
	public static Vector3 Offset(this Vector3 _vector,Vector3 _offset)
	{
		return new Vector3(_vector.x+_offset.x,_vector.y+_offset.y,_vector.z+_offset.z);
	}

	public static Vector3 OffsetXY(this Vector3 _vector,Vector2 _offset)
	{
		return _vector.Offset(new(_offset.x,_offset.y,0.0f));
	}

	public static Vector3 OffsetXZ(this Vector3 _vector,Vector2 _offset)
	{
		return _vector.Offset(new(_offset.x,0.0f,_offset.y));
	}

	public static Vector3 OffsetYZ(this Vector3 _vector,Vector2 _offset)
	{
		return _vector.Offset(new(0.0f,_offset.x,_offset.y));
	}

	public static Vector3 OffsetX(this Vector3 _vector,float _x)
	{
		return _vector.Offset(new(_x,0.0f,0.0f));
	}

	public static Vector3 OffsetY(this Vector3 _vector,float _y)
	{
		return _vector.Offset(new(0.0f,_y,0.0f));
	}

	public static Vector3 OffsetZ(this Vector3 _vector,float _z)
	{
		return _vector.Offset(new(0.0f,0.0f,_z));
	}
	#endregion Offset

	#region Invert
	public static Vector3 Invert(this Vector3 _vector)
	{
		return new Vector3(-_vector.x,-_vector.y,-_vector.z);
	}

	public static Vector3 InvertX(this Vector3 _vector)
	{
		return new Vector3(-_vector.x,_vector.y,_vector.z);
	}

	public static Vector3 InvertY(this Vector3 _vector)
	{
		return new Vector3(_vector.x,-_vector.y,_vector.z);
	}

	public static Vector3 InvertZ(this Vector3 _vector)
	{
		return new Vector3(_vector.x,_vector.y,-_vector.z);
	}

	public static Vector3 InvertXY(this Vector3 _vector)
	{
		return new Vector3(-_vector.x,-_vector.y,_vector.z);
	}

	public static Vector3 InvertXZ(this Vector3 _vector)
	{
		return new Vector3(-_vector.x,_vector.y,-_vector.z);
	}

	public static Vector3 InvertYZ(this Vector3 _vector)
	{
		return new Vector3(_vector.x,-_vector.y,-_vector.z);
	}
	#endregion Invert

	#region ToVector
	public static Vector2 ToVector2(this Vector3 _vector)
	{
		return new(_vector.x,_vector.y);
	}

	public static Vector4 ToVector4(this Vector3 _vector,float _w = 0.0f)
	{
		return new(_vector.x,_vector.y,_vector.z,_w);
	}

	public static string ToVectorString(this Vector3 _vector,int _decimals = 2)
	{
		var format = $"({"{0:f"}{_decimals}}}, {"{1:f"}{_decimals}}}, {"{2:f"}{_decimals}}})";

		return string.Format(format,_vector.x,_vector.y,_vector.z);
	}
	#endregion ToVector

	public static Vector3 PlaneDirection(this Vector3 _vector,Vector3 _target)
	{
		var pivot = _vector.SetY();
		var target = _target.SetY();
		
		return (pivot-target).normalized;
	}

	public static float PlaneDistance(this Vector3 _vector,Vector3 _target)
	{
		var pivot = _vector.SetY();
		var target = _target.SetY();
		
		return Vector3.Distance(pivot,target);
	}

	public static bool IsEquals(this Vector3 _vector1,Vector3 _vector2)
	{
		return _vector1.x.Approximately(_vector2.x) && _vector1.y.Approximately(_vector2.y) && _vector1.z.Approximately(_vector2.z);
	}
	
	public static bool IsZero(this Vector3 _vector)
	{
		return _vector.x.ApproximatelyZero() && _vector.y.ApproximatelyZero() && _vector.z.ApproximatelyZero();
	}

	public static Vector3 Reciprocal(this Vector3 _vector)
	{
		return new Vector3(1.0f/_vector.x,1.0f/_vector.y,1.0f/_vector.z);
	}

	public static Vector3 DistanceEach(this Vector3 _vector1,Vector3 _vector2)
	{
		return new Vector3(Mathf.Abs(_vector1.x-_vector2.x),Mathf.Abs(_vector1.y-_vector2.y),Mathf.Abs(_vector1.z-_vector2.z));
	}

	public static Vector3 Round(this Vector3 _vector)
	{
		return new Vector3(Mathf.Round(_vector.x),Mathf.Round(_vector.y),Mathf.Round(_vector.z));
	}

	public static Vector3 Ceil(this Vector3 _vector)
	{
		return new Vector3(Mathf.Ceil(_vector.x),Mathf.Ceil(_vector.y),Mathf.Ceil(_vector.z));
	}

	public static Vector3 Floor(this Vector3 _vector)
	{
		return new Vector3(Mathf.Floor(_vector.x),Mathf.Floor(_vector.y),Mathf.Floor(_vector.z));
	}

	public static Vector3 Abs(this Vector3 _vector)
	{
		return new Vector3(Mathf.Abs(_vector.x),Mathf.Abs(_vector.y),Mathf.Abs(_vector.z));
	}

	public static Vector3 Clamp(this Vector3 _vector,float _min,float _max)
	{
		return new Vector3(Mathf.Clamp(_vector.x,_min,_max),Mathf.Clamp(_vector.y,_min,_max),Mathf.Clamp(_vector.z,_min,_max));
	}

	public static Vector3 Clamp01(this Vector3 _vector)
	{
		return new Vector3(Mathf.Clamp01(_vector.x),Mathf.Clamp01(_vector.y),Mathf.Clamp01(_vector.z));
	}

	public static float MaxValue(this Vector3 _vector)
	{
		return _vector.x >= _vector.y ? (_vector.z >= _vector.x ? _vector.z : _vector.x) : (_vector.z >= _vector.y ? _vector.z : _vector.y);
	}

	public static float MinValue(this Vector3 _vector)
	{
		return _vector.x <= _vector.y ? (_vector.z <= _vector.x ? _vector.z : _vector.x) : (_vector.z <= _vector.y ? _vector.z : _vector.y);
	}

	public static Vector3 MultiplyEach(this Vector3 _vector1,Vector3 _vector2)
	{
		return new Vector3(_vector1.x*_vector2.x,_vector1.y*_vector2.y,_vector1.z*_vector2.z);
	}

	public static Vector3 MiddleVector(this Vector3 _vector1,Vector3 _vector2)
	{
		return new Vector3((_vector2.x-_vector1.x)/2.0f,(_vector2.y-_vector1.y)/2.0f,(_vector2.z-_vector1.z)/2.0f);
	}

	public static Color ToRGB(this Vector3 _hsv)
	{
		return Color.HSVToRGB(_hsv.x,_hsv.y,_hsv.z);
	}

	public static Color ToColor(this Vector3 _vector)
	{
		return new Color(_vector.x/255.0f,_vector.y/255.0f,_vector.z/255.0f,1.0f);
	}

	public static (Vector3 Position,int Index) GetClosestPoint(this Vector3 _position,params Vector3[] _positionArray)
	{
		var index = -1;
		var position = Vector3.zero;
		var closest = float.MaxValue;

		for(var i=0;i<_positionArray.Length;i++)
		{
			var distance = (_position-_positionArray[i]).sqrMagnitude;

			if(distance < closest)
			{
				closest = distance;

				index = i;
				position = _positionArray[i];
			}
		}

		return (position,index);
	}

	public static (Vector3 Position,float Distance) GetClosestPointOnRay(this Vector3 _position,Vector3 _origin,Vector3 _direction)
	{
		var distance = Vector3.Dot(_position-_origin,_direction);

		return (_origin+_direction*distance,distance);
	}

	public static (Vector3 Position,float Distance) GetClosestPointOnSegment(this Vector3 _position,Vector3 _start,Vector3 _end)
	{
		var direction = _end-_start;
		var magnitude = direction.magnitude;

		direction.Normalize();

		var distance = Mathf.Clamp(Vector3.Dot(_position-_start,direction),0.0f,magnitude);

		return (_start+direction*distance,distance);
	}

	public static Vector2 GetAnglesTo(this Vector3 _vector,Vector3 _compare)
	{
		return new Vector2(-Mathf.Asin(Vector3.Cross(_compare,_vector).y)*Mathf.Rad2Deg,-Mathf.Asin(Vector3.Cross(_compare,_vector).x)*Mathf.Rad2Deg);
	}
	public static Vector3 RotateAround(this Vector3 _position,Vector3 _pivot,Quaternion _rotation)
	{
		return _rotation*(_position-_pivot)+_pivot;
	}
	
	public static Vector3 ApplyTransform(this Vector3 _vector,Transform _transform)
	{
		return _vector.Transform(_transform.position,_transform.rotation,_transform.lossyScale);
	}
	
	public static Vector3 Transform(this Vector3 _vector,Vector3 _position,Quaternion _rotation,Vector3 _scale)
	{
		_vector = Vector3.Scale(_vector,new Vector3(_scale.x,_scale.y,_scale.z));
		_vector = _vector.RotateAround(Vector3.zero,_rotation);
		_vector += _position;

		return _vector;
	}

	public static Vector3 InverseApplyTransform(this Vector3 _vector,Transform _transform)
	{
		return _vector.InverseTransform(_transform.position,_transform.rotation,_transform.lossyScale);
	}

	public static Vector3 InverseTransform(this Vector3 _vector,Vector3 _position,Quaternion _rotation,Vector3 _scale)
	{
		_vector -= _position;
		_vector = _vector.RotateAround(Vector3.zero,Quaternion.Inverse(_rotation));
		_vector = Vector3.Scale(_vector,_scale.Reciprocal());

		return _vector;
	}

	public static Vector3 ToLocalPosition(Vector3 _position,Camera _worldCam,Camera _uiCam,Transform _relativeTo)
	{
		_position = _worldCam.WorldToViewportPoint(_position);
		_position = _uiCam.ViewportToWorldPoint(_position);

		if(!_relativeTo || !_relativeTo.parent)
		{
			return _position;
		}

		return _relativeTo.parent.InverseTransformPoint(_position);
	}

	public static Vector3 VectorByTransform(this Vector3 _vector,Transform _target)
	{
		var matrix = Matrix4x4.TRS(Vector3.zero,_target.rotation,Vector3.one);
		var result = matrix.MultiplyPoint3x4(_vector);

		result += _target.position;

		return result;
	}

	public static Vector3 SmoothDampAngle(this Vector3 _vector,Vector3 _target,ref Vector3 _velocity,float _smoothTime)
	{
		return new Vector3(Mathf.SmoothDampAngle(_vector.x,_target.x,ref _velocity.x,_smoothTime),Mathf.SmoothDampAngle(_vector.y,_target.y,ref _velocity.y,_smoothTime),Mathf.SmoothDampAngle(_vector.z,_target.z,ref _velocity.z,_smoothTime));
	}

	public static bool IsInside(this Vector3 _vector,Collider _collider)
	{
		return _vector == _collider.ClosestPoint(_vector);
	}

	public static Vector3 To(this Vector3 _source,Vector3 _destination)
	{
		return _destination-_source;
	}

	public static Vector3 TransformPoint(this Vector3 _position,Transform _transform,SpaceType _type)
	{
		var (Position,Rotation,Scale) = LockTransformToSpace(_transform,_type);
		var position = _transform.TransformPoint(_position);

		_transform.SetPositionAndRotation(Position,Rotation);
		_transform.localScale = Scale;

		return position;
	}

	public static Vector3 InverseTransformPoint(this Vector3 _position,Transform _transform,SpaceType _type)
	{
		var (Position,Rotation,Scale) = LockTransformToSpace(_transform,_type);
		var position = _transform.InverseTransformPoint(_position);

		_transform.SetPositionAndRotation(Position,Rotation);
		_transform.localScale = Scale;

		return position;
	}

	private static (Vector3 Position,Quaternion Rotation,Vector3 Scale) LockTransformToSpace(Transform _transform,SpaceType _space)
	{
		_transform.GetPositionAndRotation(out var position,out var rotation);

        var scale = _transform.localScale;

		if(_space == SpaceType.xy)
		{
			position.z = 0.0f;
		}
		else if(_space == SpaceType.xz)
		{
			position.y = 0.0f;
		}

		var max = Mathf.Max(scale.x,scale.y,scale.z);

		_transform.SetPositionAndRotation(position,rotation);
		_transform.localScale = Vector3.one*max;

		return (position,rotation,scale);
	}
}