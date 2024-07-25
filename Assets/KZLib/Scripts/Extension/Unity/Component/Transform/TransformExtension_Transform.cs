using UnityEngine;

public static partial class TransformExtension
{
	#region Set Position
	public static void SetPosition(this Transform _transform,Vector3 _position)
	{
		_transform.position = _position;
	}

	public static void SetPositionXY(this Transform _transform,Vector2 _point)
	{
		_transform.SetPosition(new(_point.x,_point.y,_transform.position.z));
	}

	public static void SetPositionXZ(this Transform _transform,Vector2 _point)
	{
		_transform.SetPosition(new(_point.x,_transform.position.y,_point.y));
	}

	public static void SetPositionYZ(this Transform _transform,Vector2 _point)
	{
		_transform.SetPosition(new(_transform.position.x,_point.x,_point.y));
	}

	public static void SetPositionX(this Transform _transform,float _x)
	{
		_transform.SetPosition(new(_x,_transform.position.y,_transform.position.z));
	}
	
	public static void SetPositionY(this Transform _transform,float _y)
	{
		_transform.SetPosition(new(_transform.position.x,_y,_transform.position.z));
	}

	public static void SetPositionZ(this Transform _transform,float _z)
	{
		_transform.SetPosition(new(_transform.position.x,_transform.position.y,_z));
	}
	#endregion Set Position

	#region Set Local Position
	public static void SetLocalPosition(this Transform _transform,Vector3 _position)
	{
		_transform.localPosition = _position;
	}

	public static void SetLocalPositionXY(this Transform _transform,Vector2 _point)
	{
		_transform.SetLocalPosition(new(_point.x,_point.y,_transform.localPosition.z));
	}

	public static void SetLocalPositionXZ(this Transform _transform,Vector2 _point)
	{
		_transform.SetLocalPosition(new(_point.x,_transform.localPosition.y,_point.y));
	}

	public static void SetLocalPositionYZ(this Transform _transform,Vector2 _point)
	{
		_transform.SetLocalPosition(new(_transform.localPosition.x,_point.x,_point.y));
	}

	public static void SetLocalPositionX(this Transform _transform,float _x)
	{
		_transform.SetLocalPosition(new(_x,_transform.localPosition.y,_transform.localPosition.z));
	}
	
	public static void SetLocalPositionY(this Transform _transform,float _y)
	{
		_transform.SetLocalPosition(new(_transform.localPosition.x,_y,_transform.localPosition.z));
	}

	public static void SetLocalPositionZ(this Transform _transform,float _z)
	{
		_transform.SetLocalPosition(new(_transform.localPosition.x,_transform.localPosition.y,_z));
	}
	#endregion Set Local Position

	public static Vector2 ScreenPosition(this Transform _transform)
	{
		return ScreenPosition(_transform,Camera.main);
	}
	
	public static Vector2 ScreenPosition(this Transform _transform,Camera _camera)
	{
		return RectTransformUtility.WorldToScreenPoint(_camera,_transform.position);
	}

	public static Vector3 ViewportPosition(this Transform _transform)
	{
		return ViewportPosition(_transform,Camera.main);
	}
	
	public static Vector3 ViewportPosition(this Transform _transform,Camera _camera)
	{
		return RectTransformUtility.WorldToScreenPoint(_camera,_transform.position);
	}

	#region Set Rotation
	public static void SetRotation(this Transform _transform,Vector3 _rotation)
	{
		_transform.rotation = Quaternion.Euler(_rotation);
	}

	public static void SetRotationXY(this Transform _transform,Vector2 _angle)
	{
		_transform.SetRotation(new(_angle.x,_angle.y,_transform.rotation.eulerAngles.z));
	}

	public static void SetRotationXZ(this Transform _transform,Vector2 _angle)
	{
		_transform.SetRotation(new(_angle.x,_transform.rotation.eulerAngles.y,_angle.y));
	}

	public static void SetRotationYZ(this Transform _transform,Vector2 _angle)
	{
		_transform.SetRotation(new(_transform.rotation.eulerAngles.x,_angle.x,_angle.y));
	}

	public static void SetRotationX(this Transform _transform,float _x)
	{
		_transform.SetRotation(new(_x,_transform.rotation.eulerAngles.y,_transform.rotation.eulerAngles.z));
	}
	
	public static void SetRotationY(this Transform _transform,float _y)
	{
		_transform.SetRotation(new(_transform.rotation.eulerAngles.x,_y,_transform.rotation.eulerAngles.z));
	}

	public static void SetRotationZ(this Transform _transform,float _z)
	{
		_transform.SetRotation(new(_transform.rotation.eulerAngles.x,_transform.rotation.eulerAngles.y,_z));
	}
	#endregion Set Rotation

	#region Set Local Rotation
	public static void SetLocalRotation(this Transform _transform,Vector3 _rotation)
	{
		_transform.rotation = Quaternion.Euler(_rotation);
	}

	public static void SetLocalRotationXY(this Transform _transform,Vector2 _angle)
	{
		_transform.SetLocalRotation(new(_angle.x,_angle.y,_transform.localRotation.eulerAngles.z));
	}

	public static void SetLocalRotationXZ(this Transform _transform,Vector2 _angle)
	{
		_transform.SetLocalRotation(new(_angle.x,_transform.localRotation.eulerAngles.y,_angle.y));
	}

	public static void SetLocalRotationYZ(this Transform _transform,Vector2 _angle)
	{
		_transform.SetLocalRotation(new(_transform.localRotation.eulerAngles.x,_angle.x,_angle.y));
	}

	public static void SetLocalRotationX(this Transform _transform,float _x)
	{
		_transform.SetLocalRotation(new(_x,_transform.localRotation.eulerAngles.y,_transform.localRotation.eulerAngles.z));
	}
	
	public static void SetLocalRotationY(this Transform _transform,float _y)
	{
		_transform.SetLocalRotation(new(_transform.localRotation.eulerAngles.x,_y,_transform.localRotation.eulerAngles.z));
	}

	public static void SetLocalRotationZ(this Transform _transform,float _z)
	{
		_transform.SetLocalRotation(new(_transform.localRotation.eulerAngles.x,_transform.localRotation.eulerAngles.y,_z));
	}
	#endregion Set Local Rotation

	public static void RotateAroundTarget(this Transform _transform,Vector3 _target,Vector3 _axis,float _speed,bool _look)
	{
		var delta = Quaternion.AngleAxis(_speed,_look ? Vector3.up : _axis);
        var offset = delta*(_transform.position-_target);

        _transform.position = _target+offset;

        if(_look)
        {
            _transform.rotation = Quaternion.LookRotation(-offset,_axis);
        }
	}

	#region Set Local Scale
	public static void SetLocalScale(this Transform _transform,Vector3 _scale)
	{
		_transform.localScale = _scale;
	}

	public static void SetLocalScaleXY(this Transform _transform,Vector2 _size)
	{
		_transform.SetLocalScale(new(_size.x,_size.y,_transform.localScale.z));
	}

	public static void SetLocalScaleXZ(this Transform _transform,Vector2 _size)
	{
		_transform.SetLocalScale(new(_size.x,_transform.localScale.y,_size.y));
	}

	public static void SetLocalScaleYZ(this Transform _transform,Vector2 _size)
	{
		_transform.SetLocalRotation(new(_transform.localScale.x,_size.x,_size.y));
	}

	public static void SetLocalScaleX(this Transform _transform,float _x)
	{
		_transform.SetLocalRotation(new(_x,_transform.localScale.y,_transform.localScale.z));
	}

	public static void SetLocalScaleY(this Transform _transform,float _y)
	{
		_transform.SetLocalRotation(new(_transform.localScale.x,_y,_transform.localScale.z));
	}

	public static void SetLocalScaleZ(this Transform _transform,float _z)
	{
		_transform.SetLocalRotation(new(_transform.localScale.x,_transform.localScale.y,_z));
	}
	#endregion Set Local Scale

	public static void SetLossyScale(this Transform _transform,Vector3 _scale)
	{
		_transform.localScale = Vector3.one;

		var lossyScale = _transform.lossyScale;

		_transform.localScale = new Vector3(_scale.x/lossyScale.x,_scale.y/lossyScale.y,_scale.z/lossyScale.z);
	}
}