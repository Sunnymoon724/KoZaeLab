using KZLib;
using UnityEngine;

/// <summary>
/// Extension methods for <see cref="Transform"/> position, rotation, scale, and camera-space conversion.
/// </summary>
public static partial class TransformExtension
{
	#region Set Position
	/// <summary>
	/// Sets the world position X and Y from <paramref name="point"/>.
	/// </summary>
	public static void SetPositionXY(this Transform transform,Vector2 point)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.position = transform.position.SetXY(point);
	}

	/// <summary>
	/// Sets the world position X and Z from <paramref name="point"/>.
	/// </summary>
	public static void SetPositionXZ(this Transform transform,Vector2 point)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.position = transform.position.SetXZ(point);
	}

	/// <summary>
	/// Sets the world position Y and Z from <paramref name="point"/>.
	/// </summary>
	public static void SetPositionYZ(this Transform transform,Vector2 point)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.position = transform.position.SetYZ(point);
	}

	/// <summary>
	/// Sets the world position X component to <paramref name="x"/>.
	/// </summary>
	public static void SetPositionX(this Transform transform,float x)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.position = transform.position.SetX(x);
	}
	
	/// <summary>
	/// Sets the world position Y component to <paramref name="y"/>.
	/// </summary>
	public static void SetPositionY(this Transform transform,float y)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.position = transform.position.SetY(y);
	}

	/// <summary>
	/// Sets the world position Z component to <paramref name="z"/>.
	/// </summary>
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
	/// <summary>
	/// Sets the local position X and Y from <paramref name="point"/>.
	/// </summary>
	public static void SetLocalPositionXY(this Transform transform,Vector2 point)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.localPosition = transform.localPosition.SetXY(point);
	}

	/// <summary>
	/// Sets the local position X and Z from <paramref name="point"/>.
	/// </summary>
	public static void SetLocalPositionXZ(this Transform transform,Vector2 point)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.localPosition = transform.localPosition.SetXZ(point);
	}

	/// <summary>
	/// Sets the local position Y and Z from <paramref name="point"/>.
	/// </summary>
	public static void SetLocalPositionYZ(this Transform transform,Vector2 point)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.localPosition = transform.localPosition.SetYZ(point);
	}

	/// <summary>
	/// Sets the local position X component to <paramref name="x"/>.
	/// </summary>
	public static void SetLocalPositionX(this Transform transform,float x)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.localPosition = transform.localPosition.SetX(x);
	}
	
	/// <summary>
	/// Sets the local position Y component to <paramref name="y"/>.
	/// </summary>
	public static void SetLocalPositionY(this Transform transform,float y)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.localPosition = transform.localPosition.SetY(y);
	}

	/// <summary>
	/// Sets the local position Z component to <paramref name="z"/>.
	/// </summary>
	public static void SetLocalPositionZ(this Transform transform,float z)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.localPosition = transform.localPosition.SetZ(z);
	}
	#endregion Set Local Position

	/// <summary>
	/// Returns screen-space pixel coordinates using <see cref="CameraManager.In.CurrentCamera"/> or <see cref="Camera.main"/>.
	/// </summary>
	public static Vector2 ScreenPosition(this Transform transform)
	{
		if(!_IsValid(transform))
		{
			return Vector2.zero;
		}

		return ScreenPosition(transform,CameraManager.HasInstance ? CameraManager.In.CurrentCamera : Camera.main);
	}
	
	/// <summary>
	/// Returns screen-space pixel coordinates using the specified <paramref name="camera"/>.
	/// </summary>
	public static Vector2 ScreenPosition(this Transform transform,Camera camera)
	{
		if(!_IsValid(transform))
		{
			return Vector2.zero;
		}

		if(!_IsValidCamera(camera))
		{
			return Vector3.zero;
		}

		return RectTransformUtility.WorldToScreenPoint(camera,transform.position);
	}

	/// <summary>
	/// Returns normalized viewport coordinates (0–1) using <see cref="CameraManager.In.CurrentCamera"/> or <see cref="Camera.main"/>.
	/// </summary>
	public static Vector3 ViewportPosition(this Transform transform)
	{
		if(!_IsValid(transform))
		{
			return Vector3.zero;
		}

		return ViewportPosition(transform,CameraManager.HasInstance ? CameraManager.In.CurrentCamera : Camera.main);
	}
	
	/// <summary>
	/// Returns normalized viewport coordinates (0–1) using the specified <paramref name="camera"/>.
	/// </summary>
	public static Vector3 ViewportPosition(this Transform transform,Camera camera)
	{
		if(!_IsValid(transform))
		{
			return Vector3.zero;
		}

		if(!_IsValidCamera(camera))
		{
			return Vector3.zero;
		}

		return camera.WorldToViewportPoint(transform.position);
	}
	
	private static bool _IsValidCamera(Camera camera)
	{
		if(!camera)
		{
			LogChannel.Kit.E("Camera is null.");

			return false;
		}

		return true;
	}

	#region Set Rotation
	/// <summary>
	/// Sets the world rotation X and Y Euler angles from <paramref name="angle"/>.
	/// </summary>
	public static void SetRotationXY(this Transform transform,Vector2 angle)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.SetXY(angle));
	}

	/// <summary>
	/// Sets the world rotation X and Z Euler angles from <paramref name="angle"/>.
	/// </summary>
	public static void SetRotationXZ(this Transform transform,Vector2 angle)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.SetXZ(angle));
	}

	/// <summary>
	/// Sets the world rotation Y and Z Euler angles from <paramref name="angle"/>.
	/// </summary>
	public static void SetRotationYZ(this Transform transform,Vector2 angle)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.SetYZ(angle));
	}

	/// <summary>
	/// Sets the world rotation X Euler angle to <paramref name="x"/>.
	/// </summary>
	public static void SetRotationX(this Transform transform,float x)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.SetX(x));
	}
	
	/// <summary>
	/// Sets the world rotation Y Euler angle to <paramref name="y"/>.
	/// </summary>
	public static void SetRotationY(this Transform transform,float y)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.SetY(y));
	}

	/// <summary>
	/// Sets the world rotation Z Euler angle to <paramref name="z"/>.
	/// </summary>
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
	/// <summary>
	/// Sets the local rotation X and Y Euler angles from <paramref name="angle"/>.
	/// </summary>
	public static void SetLocalRotationXY(this Transform transform,Vector2 angle)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.SetXY(angle));
	}

	/// <summary>
	/// Sets the local rotation X and Z Euler angles from <paramref name="angle"/>.
	/// </summary>
	public static void SetLocalRotationXZ(this Transform transform,Vector2 angle)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.SetXZ(angle));
	}

	/// <summary>
	/// Sets the local rotation Y and Z Euler angles from <paramref name="angle"/>.
	/// </summary>
	public static void SetLocalRotationYZ(this Transform transform,Vector2 angle)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.SetYZ(angle));
	}

	/// <summary>
	/// Sets the local rotation X Euler angle to <paramref name="x"/>.
	/// </summary>
	public static void SetLocalRotationX(this Transform transform,float x)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.SetX(x));
	}
	
	/// <summary>
	/// Sets the local rotation Y Euler angle to <paramref name="y"/>.
	/// </summary>
	public static void SetLocalRotationY(this Transform transform,float y)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.SetY(y));
	}

	/// <summary>
	/// Sets the local rotation Z Euler angle to <paramref name="z"/>.
	/// </summary>
	public static void SetLocalRotationZ(this Transform transform,float z)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.SetZ(z));
	}
	#endregion Set Local Rotation

	/// <summary>
	/// Orbits around <paramref name="target"/> at <paramref name="speed"/> degrees per call; optionally aligns rotation to face the orbit path.
	/// </summary>
	public static void RotateAroundTarget(this Transform transform,Vector3 target,Vector3 axis,float speed,bool isLook)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		var delta = Quaternion.AngleAxis(speed,isLook ? Vector3.up : axis);
        var offset = delta*(transform.position-target);

        transform.position = target+offset;

        if(isLook)
        {
            transform.rotation = Quaternion.LookRotation(-offset,axis);
        }
	}

	#region Set Local Scale
	/// <summary>
	/// Sets the local scale X and Y from <paramref name="size"/>.
	/// </summary>
	public static void SetLocalScaleXY(this Transform transform,Vector2 size)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.localScale = transform.localScale.SetXY(size);
	}

	/// <summary>
	/// Sets the local scale X and Z from <paramref name="size"/>.
	/// </summary>
	public static void SetLocalScaleXZ(this Transform transform,Vector2 size)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.localScale = transform.localScale.SetXZ(size);
	}

	/// <summary>
	/// Sets the local scale Y and Z from <paramref name="size"/>.
	/// </summary>
	public static void SetLocalScaleYZ(this Transform transform,Vector2 size)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.localScale = transform.localScale.SetYZ(size);
	}

	/// <summary>
	/// Sets the local scale X component to <paramref name="x"/>.
	/// </summary>
	public static void SetLocalScaleX(this Transform transform,float x)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.localScale = transform.localScale.SetX(x);
	}

	/// <summary>
	/// Sets the local scale Y component to <paramref name="y"/>.
	/// </summary>
	public static void SetLocalScaleY(this Transform transform,float y)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.localScale = transform.localScale.SetY(y);
	}

	/// <summary>
	/// Sets the local scale Z component to <paramref name="z"/>.
	/// </summary>
	public static void SetLocalScaleZ(this Transform transform,float z)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.localScale = transform.localScale.SetZ(z);
	}
	#endregion Set Local Scale

	/// <summary>
	/// Sets world (<see cref="Transform.lossyScale"/>) scale by adjusting <see cref="Transform.localScale"/> relative to the current lossy scale.
	/// </summary>
	public static void SetLossyScale(this Transform transform,Vector3 scale)
	{
		if(!_IsValid(transform))
		{
			return;
		}

		transform.localScale = Vector3.one;

		var lossyScale = transform.lossyScale;

		var x = lossyScale.x.ApproximatelyZero() ? 1.0f : scale.x/lossyScale.x;
		var y = lossyScale.y.ApproximatelyZero() ? 1.0f : scale.y/lossyScale.y;
		var z = lossyScale.z.ApproximatelyZero() ? 1.0f : scale.z/lossyScale.z;

		transform.localScale = new Vector3(x,y,z);
	}
}
