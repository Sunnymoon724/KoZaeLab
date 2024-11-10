using KZLib;
using UnityEngine;

public static partial class TransformExtension
{
	#region Set Position
	public static void SetPositionXY(this Transform _transform,Vector2 _point)
	{
		if(!_transform)
		{
			LogTag.System.E("Transform is null.");

			return;
		}

		_transform.position = _transform.position.SetXY(_point);
	}

	public static void SetPositionXZ(this Transform _transform,Vector2 _point)
	{
		if(!_transform)
		{
			LogTag.System.E("Transform is null.");

			return;
		}

		_transform.position = _transform.position.SetXZ(_point);
	}

	public static void SetPositionYZ(this Transform _transform,Vector2 _point)
	{
		if(!_transform)
		{
			LogTag.System.E("Transform is null.");

			return;
		}

		_transform.position = _transform.position.SetYZ(_point);
	}

	public static void SetPositionX(this Transform _transform,float _x)
	{
		if(!_transform)
		{
			LogTag.System.E("Transform is null.");

			return;
		}

		_transform.position = _transform.position.SetX(_x);
	}
	
	public static void SetPositionY(this Transform _transform,float _y)
	{
		if(!_transform)
		{
			LogTag.System.E("Transform is null.");

			return;
		}

		_transform.position = _transform.position.SetY(_y);
	}

	public static void SetPositionZ(this Transform _transform,float _z)
	{
		if(!_transform)
		{
			LogTag.System.E("Transform is null.");

			return;
		}

		_transform.position = _transform.position.SetZ(_z);
	}
	#endregion Set Position

	#region Set Local Position
	public static void SetLocalPositionXY(this Transform _transform,Vector2 _point)
	{
		if(!_transform)
		{
			LogTag.System.E("Transform is null.");

			return;
		}

		_transform.localPosition = _transform.localPosition.SetXY(_point);
	}

	public static void SetLocalPositionXZ(this Transform _transform,Vector2 _point)
	{
		if(!_transform)
		{
			LogTag.System.E("Transform is null.");

			return;
		}

		_transform.localPosition = _transform.localPosition.SetXZ(_point);
	}

	public static void SetLocalPositionYZ(this Transform _transform,Vector2 _point)
	{
		if(!_transform)
		{
			LogTag.System.E("Transform is null.");

			return;
		}

		_transform.localPosition = _transform.localPosition.SetYZ(_point);
	}

	public static void SetLocalPositionX(this Transform _transform,float _x)
	{
		if(!_transform)
		{
			LogTag.System.E("Transform is null.");

			return;
		}

		_transform.localPosition = _transform.localPosition.SetX(_x);
	}
	
	public static void SetLocalPositionY(this Transform _transform,float _y)
	{
		if(!_transform)
		{
			LogTag.System.E("Transform is null.");

			return;
		}

		_transform.localPosition = _transform.localPosition.SetY(_y);
	}

	public static void SetLocalPositionZ(this Transform _transform,float _z)
	{
		if(!_transform)
		{
			LogTag.System.E("Transform is null.");

			return;
		}

		_transform.localPosition = _transform.localPosition.SetZ(_z);
	}
	#endregion Set Local Position

	public static Vector2 ScreenPosition(this Transform _transform)
	{
		if(!_transform)
		{
			LogTag.System.E("Transform is null.");

			return default;
		}

		return ScreenPosition(_transform,CameraMgr.HasInstance ? CameraMgr.In.CurrentCamera : Camera.main);
	}
	
	public static Vector2 ScreenPosition(this Transform _transform,Camera _camera)
	{
		if(!_transform || !_camera)
		{
			LogTag.System.E($"Transform or Camera is null. {_transform} or {_camera}");

			return default;
		}

		return RectTransformUtility.WorldToScreenPoint(_camera,_transform.position);
	}

	public static Vector3 ViewportPosition(this Transform _transform)
	{
		if(!_transform)
		{
			LogTag.System.E("Transform is null.");

			return default;
		}

		return ViewportPosition(_transform,CameraMgr.HasInstance ? CameraMgr.In.CurrentCamera : Camera.main);
	}
	
	public static Vector3 ViewportPosition(this Transform _transform,Camera _camera)
	{
		if(!_transform || !_camera)
		{
			LogTag.System.E($"Transform or Camera is null. {_transform} or {_camera}");

			return default;
		}

		return RectTransformUtility.WorldToScreenPoint(_camera,_transform.position);
	}

	#region Set Rotation
	public static void SetRotationXY(this Transform _transform,Vector2 _angle)
	{
		if(!_transform)
		{
			LogTag.System.E("Transform is null.");

			return;
		}

		_transform.rotation = Quaternion.Euler(_transform.rotation.eulerAngles.SetXY(_angle));
	}

	public static void SetRotationXZ(this Transform _transform,Vector2 _angle)
	{
		if(!_transform)
		{
			LogTag.System.E("Transform is null.");

			return;
		}

		_transform.rotation = Quaternion.Euler(_transform.rotation.eulerAngles.SetXZ(_angle));
	}

	public static void SetRotationYZ(this Transform _transform,Vector2 _angle)
	{
		if(!_transform)
		{
			LogTag.System.E("Transform is null.");

			return;
		}

		_transform.rotation = Quaternion.Euler(_transform.rotation.eulerAngles.SetYZ(_angle));
	}

	public static void SetRotationX(this Transform _transform,float _x)
	{
		if(!_transform)
		{
			LogTag.System.E("Transform is null.");

			return;
		}

		_transform.rotation = Quaternion.Euler(_transform.rotation.eulerAngles.SetX(_x));
	}
	
	public static void SetRotationY(this Transform _transform,float _y)
	{
		if(!_transform)
		{
			LogTag.System.E("Transform is null.");

			return;
		}

		_transform.rotation = Quaternion.Euler(_transform.rotation.eulerAngles.SetY(_y));
	}

	public static void SetRotationZ(this Transform _transform,float _z)
	{
		if(!_transform)
		{
			LogTag.System.E("Transform is null.");

			return;
		}

		_transform.rotation = Quaternion.Euler(_transform.rotation.eulerAngles.SetZ(_z));
	}
	#endregion Set Rotation

	#region Set Local Rotation
	public static void SetLocalRotationXY(this Transform _transform,Vector2 _angle)
	{
		if(!_transform)
		{
			LogTag.System.E("Transform is null.");

			return;
		}

		_transform.localRotation = Quaternion.Euler(_transform.localRotation.eulerAngles.SetXY(_angle));
	}

	public static void SetLocalRotationXZ(this Transform _transform,Vector2 _angle)
	{
		if(!_transform)
		{
			LogTag.System.E("Transform is null.");

			return;
		}

		_transform.localRotation = Quaternion.Euler(_transform.localRotation.eulerAngles.SetXZ(_angle));
	}

	public static void SetLocalRotationYZ(this Transform _transform,Vector2 _angle)
	{
		if(!_transform)
		{
			LogTag.System.E("Transform is null.");

			return;
		}

		_transform.localRotation = Quaternion.Euler(_transform.localRotation.eulerAngles.SetYZ(_angle));
	}

	public static void SetLocalRotationX(this Transform _transform,float _x)
	{
		if(!_transform)
		{
			LogTag.System.E("Transform is null.");

			return;
		}

		_transform.localRotation = Quaternion.Euler(_transform.localRotation.eulerAngles.SetX(_x));
	}
	
	public static void SetLocalRotationY(this Transform _transform,float _y)
	{
		if(!_transform)
		{
			LogTag.System.E("Transform is null.");

			return;
		}

		_transform.localRotation = Quaternion.Euler(_transform.localRotation.eulerAngles.SetY(_y));
	}

	public static void SetLocalRotationZ(this Transform _transform,float _z)
	{
		if(!_transform)
		{
			LogTag.System.E("Transform is null.");

			return;
		}

		_transform.localRotation = Quaternion.Euler(_transform.localRotation.eulerAngles.SetZ(_z));
	}
	#endregion Set Local Rotation

	public static void RotateAroundTarget(this Transform _transform,Vector3 _target,Vector3 _axis,float _speed,bool _look)
	{
		if(!_transform)
		{
			LogTag.System.E("Transform is null.");

			return;
		}

		var delta = Quaternion.AngleAxis(_speed,_look ? Vector3.up : _axis);
        var offset = delta*(_transform.position-_target);

        _transform.position = _target+offset;

        if(_look)
        {
            _transform.rotation = Quaternion.LookRotation(-offset,_axis);
        }
	}

	#region Set Local Scale
	public static void SetLocalScaleXY(this Transform _transform,Vector2 _size)
	{
		if(!_transform)
		{
			LogTag.System.E("Transform is null.");

			return;
		}

		_transform.localScale = _transform.localScale.SetXY(_size);
	}

	public static void SetLocalScaleXZ(this Transform _transform,Vector2 _size)
	{
		if(!_transform)
		{
			LogTag.System.E("Transform is null.");

			return;
		}

		_transform.localScale = _transform.localScale.SetXZ(_size);
	}

	public static void SetLocalScaleYZ(this Transform _transform,Vector2 _size)
	{
		if(!_transform)
		{
			LogTag.System.E("Transform is null.");

			return;
		}

		_transform.localScale = _transform.localScale.SetYZ(_size);
	}

	public static void SetLocalScaleX(this Transform _transform,float _x)
	{
		if(!_transform)
		{
			LogTag.System.E("Transform is null.");

			return;
		}

		_transform.localScale = _transform.localScale.SetX(_x);
	}

	public static void SetLocalScaleY(this Transform _transform,float _y)
	{
		if(!_transform)
		{
			LogTag.System.E("Transform is null.");

			return;
		}

		_transform.localScale = _transform.localScale.SetY(_y);
	}

	public static void SetLocalScaleZ(this Transform _transform,float _z)
	{
		if(!_transform)
		{
			LogTag.System.E("Transform is null.");

			return;
		}

		_transform.localScale = _transform.localScale.SetZ(_z);
	}
	#endregion Set Local Scale

	public static void SetLossyScale(this Transform _transform,Vector3 _scale)
	{
		if(!_transform)
		{
			LogTag.System.E("Transform is null.");

			return;
		}

		_transform.localScale = Vector3.one;

		var lossyScale = _transform.lossyScale;

		_transform.localScale = new Vector3(_scale.x/lossyScale.x,_scale.y/lossyScale.y,_scale.z/lossyScale.z);
	}
}