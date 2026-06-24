using UnityEngine;

/// <summary>
/// Extension methods for <see cref="Quaternion"/> euler editing, rotation composition, and comparisons.
/// </summary>
public static class QuaternionExtension
{
	#region SetEuler
	/// <summary>
	/// Replaces the X euler angle with the given value.
	/// </summary>
	public static Quaternion SetEulerX(this Quaternion quaternion,float x = 0.0f)
	{
		var angle = quaternion.eulerAngles;

		return Quaternion.Euler(x,angle.y,angle.z);
	}

	/// <summary>
	/// Replaces the Y euler angle with the given value.
	/// </summary>
	public static Quaternion SetEulerY(this Quaternion quaternion,float y = 0.0f)
	{
		var angle = quaternion.eulerAngles;

		return Quaternion.Euler(angle.x,y,angle.z);
	}

	/// <summary>
	/// Replaces the Z euler angle with the given value.
	/// </summary>
	public static Quaternion SetEulerZ(this Quaternion quaternion,float z = 0.0f)
	{
		var angle = quaternion.eulerAngles;

		return Quaternion.Euler(angle.x,angle.y,z);
	}

	/// <summary>
	/// Replaces the X and Y euler angles with the given values.
	/// </summary>
	public static Quaternion SetEulerXY(this Quaternion quaternion,float x = 0.0f,float y = 0.0f) 
	{
		var angle = quaternion.eulerAngles;

		return Quaternion.Euler(x,y,angle.z);
	}
	
	/// <summary>
	/// Replaces the X and Z euler angles with the given values.
	/// </summary>
	public static Quaternion SetEulerXZ(this Quaternion quaternion,float x = 0.0f,float z = 0.0f) 
	{
		var angle = quaternion.eulerAngles;

		return Quaternion.Euler(x,angle.y,z);
	}
	
	/// <summary>
	/// Replaces the Y and Z euler angles with the given values.
	/// </summary>
	public static Quaternion SetEulerYZ(this Quaternion quaternion,float y = 0.0f,float z = 0.0f) 
	{
		var angle = quaternion.eulerAngles;

		return Quaternion.Euler(angle.x,y,z);
	}
	#endregion SetEuler

	#region OffsetEuler
	/// <summary>
	/// Adds an offset to the euler angles.
	/// </summary>
	public static Quaternion OffsetEuler(this Quaternion quaternion,Vector3 offset)
	{
		var angle = quaternion.eulerAngles;

		return Quaternion.Euler(angle.x+offset.x,angle.y+offset.y,angle.z+offset.z);
	}

	/// <summary>
	/// Adds an offset to the X and Y euler angles.
	/// </summary>
	public static Quaternion OffsetEulerXY(this Quaternion quaternion,Vector2 offset)
	{
		return quaternion.OffsetEuler(new(offset.x,offset.y,0.0f));
	}

	/// <summary>
	/// Adds an offset to the X and Z euler angles.
	/// </summary>
	public static Quaternion OffsetEulerXZ(this Quaternion quaternion,Vector2 offset)
	{
		return quaternion.OffsetEuler(new(offset.x,0.0f,offset.y));
	}

	/// <summary>
	/// Adds an offset to the Y and Z euler angles.
	/// </summary>
	public static Quaternion OffsetEulerYZ(this Quaternion quaternion,Vector2 offset)
	{
		return quaternion.OffsetEuler(new(0.0f,offset.x,offset.y));
	}

	/// <summary>
	/// Adds an offset to the X euler angle.
	/// </summary>
	public static Quaternion OffsetEulerX(this Quaternion quaternion,float x)
	{
		return quaternion.OffsetEuler(new(x,0.0f,0.0f));
	}

	/// <summary>
	/// Adds an offset to the Y euler angle.
	/// </summary>
	public static Quaternion OffsetEulerY(this Quaternion quaternion,float y)
	{
		return quaternion.OffsetEuler(new(0.0f,y,0.0f));
	}

	/// <summary>
	/// Adds an offset to the Z euler angle.
	/// </summary>
	public static Quaternion OffsetEulerZ(this Quaternion quaternion,float z)
	{
		return quaternion.OffsetEuler(new(0.0f,0.0f,z));
	}
	#endregion OffsetEuler

	#region Rotate
	/// <summary>
	/// Applies a rotation around the local axes.
	/// </summary>
	public static Quaternion RotateLocal(this Quaternion quaternion,Vector3 eulerOffset)
	{
		return quaternion*Quaternion.Euler(eulerOffset);
	}

	/// <summary>
	/// Applies a rotation around the world axes.
	/// </summary>
	public static Quaternion RotateWorld(this Quaternion quaternion,Vector3 eulerOffset)
	{
		return Quaternion.Euler(eulerOffset)*quaternion;
	}

	/// <summary>
	/// Applies a rotation around a local axis.
	/// </summary>
	public static Quaternion RotateLocal(this Quaternion quaternion,float angle,Vector3 axis)
	{
		return quaternion*Quaternion.AngleAxis(angle,axis);
	}

	/// <summary>
	/// Applies a rotation around a world axis.
	/// </summary>
	public static Quaternion RotateWorld(this Quaternion quaternion,float angle,Vector3 axis)
	{
		return Quaternion.AngleAxis(angle,axis)*quaternion;
	}

	/// <summary>
	/// Keeps only the yaw rotation on the XZ plane.
	/// </summary>
	public static Quaternion WithYawOnly(this Quaternion quaternion)
	{
		var forward = quaternion*Vector3.forward;
		forward.y = 0.0f;

		if(forward.sqrMagnitude < Mathf.Epsilon)
		{
			return Quaternion.identity;
		}

		return Quaternion.LookRotation(forward.normalized);
	}
	#endregion Rotate

	#region Inverse
	/// <summary>
	/// Returns the quaternion inverse via <see cref="Quaternion.Inverse"/>.
	/// </summary>
	public static Quaternion Inverse(this Quaternion quaternion)
	{
		return Quaternion.Inverse(quaternion);
	}
	#endregion Inverse

	#region NegateEuler
	/// <summary>
	/// Negates the X euler angle while preserving Y and Z.
	/// </summary>
	public static Quaternion NegateEulerX(this Quaternion quaternion)
	{
		var angle = quaternion.eulerAngles;

		return Quaternion.Euler(-angle.x,angle.y,angle.z);
	}

	/// <summary>
	/// Negates the Y euler angle while preserving X and Z.
	/// </summary>
	public static Quaternion NegateEulerY(this Quaternion quaternion)
	{
		var angle = quaternion.eulerAngles;

		return Quaternion.Euler(angle.x,-angle.y,angle.z);
	}

	/// <summary>
	/// Negates the Z euler angle while preserving X and Y.
	/// </summary>
	public static Quaternion NegateEulerZ(this Quaternion quaternion)
	{
		var angle = quaternion.eulerAngles;

		return Quaternion.Euler(angle.x,angle.y,-angle.z);
	}

	/// <summary>
	/// Negates the X and Y euler angles while preserving Z.
	/// </summary>
	public static Quaternion NegateEulerXY(this Quaternion quaternion)
	{
		var angle = quaternion.eulerAngles;

		return Quaternion.Euler(-angle.x,-angle.y,angle.z);
	}

	/// <summary>
	/// Negates the X and Z euler angles while preserving Y.
	/// </summary>
	public static Quaternion NegateEulerXZ(this Quaternion quaternion)
	{
		var angle = quaternion.eulerAngles;

		return Quaternion.Euler(-angle.x,angle.y,-angle.z);
	}

	/// <summary>
	/// Negates the Y and Z euler angles while preserving X.
	/// </summary>
	public static Quaternion NegateEulerYZ(this Quaternion quaternion)
	{
		var angle = quaternion.eulerAngles;

		return Quaternion.Euler(angle.x,-angle.y,-angle.z);
	}
	#endregion NegateEuler

	/// <summary>
	/// Returns whether all quaternion components are approximately equal.
	/// </summary>
	public static bool AreEqual(this Quaternion quaternion1,Quaternion quaternion2)
	{
		return quaternion1.x.Approximately(quaternion2.x) && quaternion1.y.Approximately(quaternion2.y) && quaternion1.z.Approximately(quaternion2.z) && quaternion1.w.Approximately(quaternion2.w);
	}

	/// <summary>
	/// Returns whether this quaternion is approximately the identity.
	/// </summary>
	public static bool IsIdentity(this Quaternion quaternion)
	{
		return quaternion.x.ApproximatelyZero() && quaternion.y.ApproximatelyZero() && quaternion.z.ApproximatelyZero() && quaternion.w.Approximately(1.0f);
	}

	#region EulerMath
	/// <summary>
	/// Rounds each euler angle to the nearest integer.
	/// </summary>
	public static Quaternion RoundEuler(this Quaternion quaternion)
	{
		var angle = quaternion.eulerAngles;

		return Quaternion.Euler(Mathf.Round(angle.x),Mathf.Round(angle.y),Mathf.Round(angle.z));
	}

	/// <summary>
	/// Applies ceiling to each euler angle.
	/// </summary>
	public static Quaternion CeilEuler(this Quaternion quaternion)
	{
		var angle = quaternion.eulerAngles;

		return Quaternion.Euler(Mathf.Ceil(angle.x),Mathf.Ceil(angle.y),Mathf.Ceil(angle.z));
	}

	/// <summary>
	/// Applies floor to each euler angle.
	/// </summary>
	public static Quaternion FloorEuler(this Quaternion quaternion)
	{
		var angle = quaternion.eulerAngles;

		return Quaternion.Euler(Mathf.Floor(angle.x),Mathf.Floor(angle.y),Mathf.Floor(angle.z));
	}

	/// <summary>
	/// Returns the absolute value of each euler angle.
	/// </summary>
	public static Quaternion AbsEuler(this Quaternion quaternion)
	{
		var angle = quaternion.eulerAngles;

		return Quaternion.Euler(Mathf.Abs(angle.x),Mathf.Abs(angle.y),Mathf.Abs(angle.z));
	}

	/// <summary>
	/// Clamps each euler angle between the given bounds.
	/// </summary>
	public static Quaternion ClampEuler(this Quaternion quaternion,float min,float max)
	{
		var angle = quaternion.eulerAngles;

		return Quaternion.Euler(Mathf.Clamp(angle.x,min,max),Mathf.Clamp(angle.y,min,max),Mathf.Clamp(angle.z,min,max));
	}
	#endregion EulerMath

	/// <summary>
	/// Returns the forward direction scaled by the given distance.
	/// </summary>
	public static Vector3 ForwardDirection(this Quaternion quaternion,float distance = 1.0f)
	{
		return quaternion*Vector3.forward*distance;
	}

	/// <summary>
	/// Returns the spherical interpolation between two quaternions.
	/// </summary>
	public static Quaternion Slerp(this Quaternion quaternion1,Quaternion quaternion2,float time = 0.5f)
	{
		return Quaternion.Slerp(quaternion1,quaternion2,time);
	}

	/// <summary>
	/// Formats the components as <c>(x, y, z, w)</c> with fixed decimal places.
	/// </summary>
	public static string ToQuaternionString(this Quaternion quaternion,int decimalPoint = 2)
	{
		var format = $"({"{0:f"}{decimalPoint}}}, {"{1:f"}{decimalPoint}}}, {"{2:f"}{decimalPoint}}}, {"{3:f"}{decimalPoint}}})";

		return string.Format(format,quaternion.x,quaternion.y,quaternion.z,quaternion.w);
	}
}
