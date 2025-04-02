using KZLib;
using UnityEngine;

public static partial class TransformExtension
{
	#region Set Position
	public static void SetPositionXY(this Transform transform,Vector2 point)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.position = transform.position.SetXY(point);
	}

	public static void SetPositionXZ(this Transform transform,Vector2 point)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.position = transform.position.SetXZ(point);
	}

	public static void SetPositionYZ(this Transform transform,Vector2 point)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.position = transform.position.SetYZ(point);
	}

	public static void SetPositionX(this Transform transform,float x)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.position = transform.position.SetX(x);
	}
	
	public static void SetPositionY(this Transform transform,float y)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.position = transform.position.SetY(y);
	}

	public static void SetPositionZ(this Transform transform,float z)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.position = transform.position.SetZ(z);
	}
	#endregion Set Position

	#region Set Local Position
	public static void SetLocalPositionXY(this Transform transform,Vector2 point)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.localPosition = transform.localPosition.SetXY(point);
	}

	public static void SetLocalPositionXZ(this Transform transform,Vector2 point)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.localPosition = transform.localPosition.SetXZ(point);
	}

	public static void SetLocalPositionYZ(this Transform transform,Vector2 point)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.localPosition = transform.localPosition.SetYZ(point);
	}

	public static void SetLocalPositionX(this Transform transform,float x)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.localPosition = transform.localPosition.SetX(x);
	}
	
	public static void SetLocalPositionY(this Transform transform,float y)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.localPosition = transform.localPosition.SetY(y);
	}

	public static void SetLocalPositionZ(this Transform transform,float z)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.localPosition = transform.localPosition.SetZ(z);
	}
	#endregion Set Local Position

	public static Vector2 ScreenPosition(this Transform transform)
	{
		if(!_IsValid(transform))
		{
			return Vector2.zero;
		}

		return ScreenPosition(transform,CameraMgr.HasInstance ? CameraMgr.In.CurrentCamera : Camera.main);
	}
	
	public static Vector2 ScreenPosition(this Transform transform,Camera camera)
	{
		if(!_IsValid(transform))
		{
			return Vector2.zero;
		}

		if(!camera)
		{
			LogTag.System.E("Camera is null.");

			return default;
		}

		return RectTransformUtility.WorldToScreenPoint(camera,transform.position);
	}

	public static Vector3 ViewportPosition(this Transform transform)
	{
		if(!_IsValid(transform))
		{
			return Vector3.zero;
		}

		return ViewportPosition(transform,CameraMgr.HasInstance ? CameraMgr.In.CurrentCamera : Camera.main);
	}
	
	public static Vector3 ViewportPosition(this Transform transform,Camera _camera)
	{
		if(!_IsValid(transform))
		{
			return Vector3.zero;
		}

		if(!_camera)
		{
			LogTag.System.E("Camera is null.");

			return Vector3.zero;
		}

		return RectTransformUtility.WorldToScreenPoint(_camera,transform.position);
	}

	#region Set Rotation
	public static void SetRotationXY(this Transform transform,Vector2 _angle)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.SetXY(_angle));
	}

	public static void SetRotationXZ(this Transform transform,Vector2 _angle)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.SetXZ(_angle));
	}

	public static void SetRotationYZ(this Transform transform,Vector2 _angle)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.SetYZ(_angle));
	}

	public static void SetRotationX(this Transform transform,float x)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.SetX(x));
	}
	
	public static void SetRotationY(this Transform transform,float y)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.SetY(y));
	}

	public static void SetRotationZ(this Transform transform,float z)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.SetZ(z));
	}
	#endregion Set Rotation

	#region Set Local Rotation
	public static void SetLocalRotationXY(this Transform transform,Vector2 _angle)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.SetXY(_angle));
	}

	public static void SetLocalRotationXZ(this Transform transform,Vector2 _angle)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.SetXZ(_angle));
	}

	public static void SetLocalRotationYZ(this Transform transform,Vector2 _angle)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.SetYZ(_angle));
	}

	public static void SetLocalRotationX(this Transform transform,float x)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.SetX(x));
	}
	
	public static void SetLocalRotationY(this Transform transform,float y)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.SetY(y));
	}

	public static void SetLocalRotationZ(this Transform transform,float z)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.SetZ(z));
	}
	#endregion Set Local Rotation

	public static void RotateAroundTarget(this Transform transform,Vector3 _target,Vector3 _axis,float _speed,bool _look)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		var delta = Quaternion.AngleAxis(_speed,_look ? Vector3.up : _axis);
        var offset = delta*(transform.position-_target);

        transform.position = _target+offset;

        if(_look)
        {
            transform.rotation = Quaternion.LookRotation(-offset,_axis);
        }
	}

	#region Set Local Scale
	public static void SetLocalScaleXY(this Transform transform,Vector2 _size)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.localScale = transform.localScale.SetXY(_size);
	}

	public static void SetLocalScaleXZ(this Transform transform,Vector2 _size)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.localScale = transform.localScale.SetXZ(_size);
	}

	public static void SetLocalScaleYZ(this Transform transform,Vector2 _size)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.localScale = transform.localScale.SetYZ(_size);
	}

	public static void SetLocalScaleX(this Transform transform,float x)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.localScale = transform.localScale.SetX(x);
	}

	public static void SetLocalScaleY(this Transform transform,float y)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.localScale = transform.localScale.SetY(y);
	}

	public static void SetLocalScaleZ(this Transform transform,float z)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.localScale = transform.localScale.SetZ(z);
	}
	#endregion Set Local Scale

	public static void SetLossyScale(this Transform transform,Vector3 _scale)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.localScale = Vector3.one;

		var lossyScale = transform.lossyScale;

		transform.localScale = new Vector3(_scale.x/lossyScale.x,_scale.y/lossyScale.y,_scale.z/lossyScale.z);
	}
}