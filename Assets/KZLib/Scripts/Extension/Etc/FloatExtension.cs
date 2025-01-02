using UnityEngine;

public static class FloatExtension
{
	public static string ToStringComma(this float number)
	{
		return $"{number:n0}";
	}

	public static string ToStringPercent(this float number,int decimalPoint)
	{
		return string.Format(string.Concat("{0:f",decimalPoint,"}%"),number);
	}

	public static string ToStringSign(this float number)
	{
		return $"{(number > 0.0 ? "+" : "")}{number}";
	}

	public static float ToLimit(this float number,int decimalPoint)
	{
		var factor = Mathf.Pow(10.0f,decimalPoint);

		return Mathf.Floor(number*factor)/factor;
	}

	/// <summary>
	/// -180.0f ~ +180.0f
	/// </summary>
	public static float ToWrapAngle(this float angle)
	{
		angle %= Global.FULL_ANGLE;

		while(angle > +Global.HALF_ANGLE)
		{
			angle -= Global.FULL_ANGLE;
		}

		while(angle < -Global.HALF_ANGLE)
		{
			angle += Global.FULL_ANGLE;
		}

		return angle;
	}

	public static Vector3 ToVector(this float radius,float degree)
	{
		return new Vector3(radius*Mathf.Cos(degree*Mathf.Deg2Rad),0,radius*Mathf.Sin(degree*Mathf.Deg2Rad));
	}

	public static bool Approximately(this float number1,float number2)
	{
		return Mathf.Approximately(number1,number2);
	}

	public static bool ApproximatelyZero(this float number)
	{
		return Approximately(number,0.0f);
	}

	public static void SeparateDecimal(this float number,out int integer,out float fraction)
	{
		integer = Mathf.FloorToInt(number);
		fraction = Mathf.Abs(number-integer);
	}
}