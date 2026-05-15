using UnityEngine;

public static class QuaternionExtension
{
	#region Mask
	public static Quaternion MaskX(this Quaternion quaternion,float x = 0.0f)
	{
		var angle = quaternion.eulerAngles;

		return Quaternion.Euler(x,angle.y,angle.z);
	}

	public static Quaternion MaskY(this Quaternion quaternion,float y = 0.0f)
	{
		var angle = quaternion.eulerAngles;

		return Quaternion.Euler(angle.x,y,angle.z);
	}

	public static Quaternion MaskZ(this Quaternion quaternion,float z = 0.0f)
	{
		var angle = quaternion.eulerAngles;

		return Quaternion.Euler(angle.x,angle.y,z);
	}

	public static Quaternion MaskXY(this Quaternion quaternion,float x = 0.0f,float y = 0.0f) 
	{
		var angle = quaternion.eulerAngles;

		return Quaternion.Euler(x,y,angle.z);
	}
	
	public static Quaternion MaskXZ(this Quaternion quaternion,float x = 0.0f,float z = 0.0f) 
	{
		var angle = quaternion.eulerAngles;

		return Quaternion.Euler(x,angle.y,z);
	}
	
	public static Quaternion MaskYZ(this Quaternion quaternion,float y = 0.0f,float z = 0.0f) 
	{
		var angle = quaternion.eulerAngles;

		return Quaternion.Euler(angle.x,y,z);
	}
	#endregion Mask

	#region Offset
	public static Quaternion Offset(this Quaternion quaternion,Vector3 offset)
	{
		var angle = quaternion.eulerAngles;

		return Quaternion.Euler(angle.x+offset.x,angle.y+offset.y,angle.z+offset.z);
	}

	public static Quaternion OffsetXY(this Quaternion quaternion,Vector2 offset)
	{
		return quaternion.Offset(new(offset.x,offset.y,0.0f));
	}

	public static Quaternion OffsetXZ(this Quaternion quaternion,Vector2 offset)
	{
		return quaternion.Offset(new(offset.x,0.0f,offset.y));
	}

	public static Quaternion OffsetYZ(this Quaternion quaternion,Vector2 offset)
	{
		return quaternion.Offset(new(0.0f,offset.x,offset.y));
	}

	public static Quaternion OffsetX(this Quaternion quaternion,float x)
	{
		return quaternion.Offset(new(x,0.0f,0.0f));
	}

	public static Quaternion OffsetY(this Quaternion quaternion,float y)
	{
		return quaternion.Offset(new(0.0f,y,0.0f));
	}

	public static Quaternion OffsetZ(this Quaternion quaternion,float z)
	{
		return quaternion.Offset(new(0.0f,0.0f,z));
	}
	#endregion Offset

	#region Invert
	public static Quaternion Invert(this Quaternion quaternion)
	{
		var angle = quaternion.eulerAngles;

		return Quaternion.Euler(-angle.x,-angle.y,-angle.z);
	}

	public static Quaternion InvertX(this Quaternion quaternion)
	{
		var angle = quaternion.eulerAngles;

		return Quaternion.Euler(-angle.x,angle.y,angle.z);
	}

	public static Quaternion InvertY(this Quaternion quaternion)
	{
		var angle = quaternion.eulerAngles;

		return Quaternion.Euler(angle.x,-angle.y,angle.z);
	}

	public static Quaternion InvertZ(this Quaternion quaternion)
	{
		var angle = quaternion.eulerAngles;

		return Quaternion.Euler(angle.x,angle.y,-angle.z);
	}

	public static Quaternion InvertXY(this Quaternion quaternion)
	{
		var angle = quaternion.eulerAngles;

		return Quaternion.Euler(-angle.x,-angle.y,angle.z);
	}

	public static Quaternion InvertXZ(this Quaternion quaternion)
	{
		var angle = quaternion.eulerAngles;

		return Quaternion.Euler(-angle.x,angle.y,-angle.z);
	}

	public static Quaternion InvertYZ(this Quaternion quaternion)
	{
		var angle = quaternion.eulerAngles;

		return Quaternion.Euler(angle.x,-angle.y,-angle.z);
	}
	#endregion Invert

	public static bool IsEquals(this Quaternion quaternion1,Quaternion quaternion2)
	{
		return quaternion1.x.Approximately(quaternion2.x) && quaternion1.y.Approximately(quaternion2.y) && quaternion1.z.Approximately(quaternion2.z) && quaternion1.w.Approximately(quaternion2.w);
	}

	public static bool IsIdentity(this Quaternion quaternion)
	{
		return quaternion.x.ApproximatelyZero() && quaternion.y.ApproximatelyZero() && quaternion.z.ApproximatelyZero() && quaternion.w.Approximately(1.0f);
	}

	public static Quaternion Round(this Quaternion quaternion)
	{
		var angle = quaternion.eulerAngles;

		return Quaternion.Euler(Mathf.Round(angle.x),Mathf.Round(angle.y),Mathf.Round(angle.z));
	}

	public static Quaternion Ceil(this Quaternion quaternion)
	{
		var angle = quaternion.eulerAngles;

		return Quaternion.Euler(Mathf.Ceil(angle.x),Mathf.Ceil(angle.y),Mathf.Ceil(angle.z));
	}

	public static Quaternion Floor(this Quaternion quaternion)
	{
		var angle = quaternion.eulerAngles;

		return Quaternion.Euler(Mathf.Floor(angle.x),Mathf.Floor(angle.y),Mathf.Floor(angle.z));
	}

	public static Quaternion Abs(this Quaternion quaternion)
	{
		var angle = quaternion.eulerAngles;

		return Quaternion.Euler(Mathf.Abs(angle.x),Mathf.Abs(angle.y),Mathf.Abs(angle.z));
	}

	public static Quaternion Clamp(this Quaternion quaternion,float _min,float _max)
	{
		var angle = quaternion.eulerAngles;

		return Quaternion.Euler(Mathf.Clamp(angle.x,_min,_max),Mathf.Clamp(angle.y,_min,_max),Mathf.Clamp(angle.z,_min,_max));
	}

	public static Quaternion Clamp01(this Quaternion quaternion)
	{
		var angle = quaternion.eulerAngles;

		return Quaternion.Euler(Mathf.Clamp01(angle.x),Mathf.Clamp01(angle.y),Mathf.Clamp01(angle.z));
	}

	public static Vector3 Normalized(this Quaternion quaternion,float _distance = 1.0f)
	{
		return quaternion*Vector3.forward*_distance;
	}

	public static Quaternion MiddleVector(this Quaternion quaternion1,Quaternion quaternion2)
	{
		var angle1 = quaternion1.eulerAngles;
		var angle2 = quaternion2.eulerAngles;

		return Quaternion.Euler((angle2.x-angle1.x)/2.0f,(angle2.y-angle1.y)/2.0f,(angle2.z-angle1.z)/2.0f);
	}

	public static string ToQuaternionString(this Quaternion quaternion,int decimalPoint = 2)
	{
		var format = $"({"{0:f"}{decimalPoint}}}, {"{1:f"}{decimalPoint}}}, {"{2:f"}{decimalPoint}}}, {"{3:f"}{decimalPoint}}})";

		return string.Format(format,quaternion.x,quaternion.y,quaternion.z,quaternion.w);
	}
}