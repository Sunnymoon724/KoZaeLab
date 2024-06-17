using UnityEngine;

public static class QuaternionExtension
{
	#region Mask
	public static Quaternion MaskX(this Quaternion _quaternion,float _x = 0.0f)
	{
		var angle = _quaternion.eulerAngles;

		return Quaternion.Euler(_x,angle.y,angle.z);
	}

	public static Quaternion MaskY(this Quaternion _quaternion,float _y = 0.0f)
	{
		var angle = _quaternion.eulerAngles;

		return Quaternion.Euler(angle.x,_y,angle.z);
	}

	public static Quaternion MaskZ(this Quaternion _quaternion,float _z = 0.0f)
	{
		var angle = _quaternion.eulerAngles;

		return Quaternion.Euler(angle.x,angle.y,_z);
	}

	public static Quaternion MaskXY(this Quaternion _quaternion,float _x = 0.0f,float _y = 0.0f) 
	{
		var angle = _quaternion.eulerAngles;

		return Quaternion.Euler(_x,_y,angle.z);
	}
	
	public static Quaternion MaskXZ(this Quaternion _quaternion,float _x = 0.0f,float _z = 0.0f) 
	{
		var angle = _quaternion.eulerAngles;

		return Quaternion.Euler(_x,angle.y,_z);
	}
	
	public static Quaternion MaskYZ(this Quaternion _quaternion,float _y = 0.0f,float _z = 0.0f) 
	{
		var angle = _quaternion.eulerAngles;

		return Quaternion.Euler(angle.x,_y,_z);
	}
	#endregion Mask

	#region Offset
	public static Quaternion Offset(this Quaternion _quaternion,Vector3 _offset)
	{
		var angle = _quaternion.eulerAngles;

		return Quaternion.Euler(angle.x+_offset.x,angle.y+_offset.y,angle.z+_offset.z);
	}

	public static Quaternion OffsetXY(this Quaternion _quaternion,Vector2 _offset)
	{
		return _quaternion.Offset(new(_offset.x,_offset.y,0.0f));
	}

	public static Quaternion OffsetXZ(this Quaternion _quaternion,Vector2 _offset)
	{
		return _quaternion.Offset(new(_offset.x,0.0f,_offset.y));
	}

	public static Quaternion OffsetYZ(this Quaternion _quaternion,Vector2 _offset)
	{
		return _quaternion.Offset(new(0.0f,_offset.x,_offset.y));
	}

	public static Quaternion OffsetX(this Quaternion _quaternion,float _x)
	{
		return _quaternion.Offset(new(_x,0.0f,0.0f));
	}

	public static Quaternion OffsetY(this Quaternion _quaternion,float _y)
	{
		return _quaternion.Offset(new(0.0f,_y,0.0f));
	}

	public static Quaternion OffsetZ(this Quaternion _quaternion,float _z)
	{
		return _quaternion.Offset(new(0.0f,0.0f,_z));
	}
	#endregion Offset

	#region Invert
	public static Quaternion Invert(this Quaternion _quaternion)
	{
		var angle = _quaternion.eulerAngles;

		return Quaternion.Euler(-angle.x,-angle.y,-angle.z);
	}

	public static Quaternion InvertX(this Quaternion _quaternion)
	{
		var angle = _quaternion.eulerAngles;

		return Quaternion.Euler(-angle.x,angle.y,angle.z);
	}

	public static Quaternion InvertY(this Quaternion _quaternion)
	{
		var angle = _quaternion.eulerAngles;

		return Quaternion.Euler(angle.x,-angle.y,angle.z);
	}

	public static Quaternion InvertZ(this Quaternion _quaternion)
	{
		var angle = _quaternion.eulerAngles;

		return Quaternion.Euler(angle.x,angle.y,-angle.z);
	}

	public static Quaternion InvertXY(this Quaternion _quaternion)
	{
		var angle = _quaternion.eulerAngles;

		return Quaternion.Euler(-angle.x,-angle.y,angle.z);
	}

	public static Quaternion InvertXZ(this Quaternion _quaternion)
	{
		var angle = _quaternion.eulerAngles;

		return Quaternion.Euler(-angle.x,angle.y,-angle.z);
	}

	public static Quaternion InvertYZ(this Quaternion _quaternion)
	{
		var angle = _quaternion.eulerAngles;

		return Quaternion.Euler(angle.x,-angle.y,-angle.z);
	}
	#endregion Invert

	public static bool IsEquals(this Quaternion _quaternion1,Quaternion _quaternion2)
	{
		return _quaternion1.x.Approximately(_quaternion2.x) && _quaternion1.y.Approximately(_quaternion2.y) && _quaternion1.z.Approximately(_quaternion2.z) && _quaternion1.w.Approximately(_quaternion2.w);
	}

	public static bool IsIdentity(this Quaternion _quaternion)
	{
		return _quaternion.x.ApproximatelyZero() && _quaternion.y.ApproximatelyZero() && _quaternion.z.ApproximatelyZero() && _quaternion.w.Approximately(1.0f);
	}

	public static Quaternion Round(this Quaternion _quaternion)
	{
		var angle = _quaternion.eulerAngles;

		return Quaternion.Euler(Mathf.Round(angle.x),Mathf.Round(angle.y),Mathf.Round(angle.z));
	}

	public static Quaternion Ceil(this Quaternion _quaternion)
	{
		var angle = _quaternion.eulerAngles;

		return Quaternion.Euler(Mathf.Ceil(angle.x),Mathf.Ceil(angle.y),Mathf.Ceil(angle.z));
	}

	public static Quaternion Floor(this Quaternion _quaternion)
	{
		var angle = _quaternion.eulerAngles;

		return Quaternion.Euler(Mathf.Floor(angle.x),Mathf.Floor(angle.y),Mathf.Floor(angle.z));
	}

	public static Quaternion Abs(this Quaternion _quaternion)
	{
		var angle = _quaternion.eulerAngles;

		return Quaternion.Euler(Mathf.Abs(angle.x),Mathf.Abs(angle.y),Mathf.Abs(angle.z));
	}

	public static Quaternion Clamp(this Quaternion _quaternion,float _min,float _max)
	{
		var angle = _quaternion.eulerAngles;

		return Quaternion.Euler(Mathf.Clamp(angle.x,_min,_max),Mathf.Clamp(angle.y,_min,_max),Mathf.Clamp(angle.z,_min,_max));
	}

	public static Quaternion Clamp01(this Quaternion _quaternion)
	{
		var angle = _quaternion.eulerAngles;

		return Quaternion.Euler(Mathf.Clamp01(angle.x),Mathf.Clamp01(angle.y),Mathf.Clamp01(angle.z));
	}

	public static Quaternion MiddleVector(this Quaternion _quaternion1,Quaternion _quaternion2)
	{
		var angle1 = _quaternion1.eulerAngles;
		var angle2 = _quaternion2.eulerAngles;

		return Quaternion.Euler((angle2.x-angle1.x)/2.0f,(angle2.y-angle1.y)/2.0f,(angle2.z-angle1.z)/2.0f);
	}

	public static string ToQuaternionString(this Quaternion _quaternion,int _dot = 2)
	{
		var format = string.Format("({1}{0}}}, {2}{0}}}, {3}{0}}}, {4}{0}}})",_dot,"{0:f","{1:f","{2:f","{3:f");

		return string.Format(format,_quaternion.x,_quaternion.y,_quaternion.z,_quaternion.w);
	}
}