using UnityEngine;

/// <summary>
/// Extension methods for float.
/// Provides formatting, angle wrapping, vector conversion, and approximate comparison helpers.
/// </summary>
public static class FloatExtension
{
	/// <summary>
	/// Formats the float with thousands separators and no decimal places.
	/// </summary>
	public static string ToStringComma(this float number)
	{
		return $"{number:n0}";
	}

	/// <summary>
	/// Formats the float as a percentage with the given decimal places.
	/// </summary>
	public static string ToStringPercent(this float number,int decimalPoint)
	{
		return string.Format(string.Concat("{0:f",decimalPoint,"}%"),number);
	}

	/// <summary>
	/// Formats the float with an explicit plus sign for positive values.
	/// </summary>
	public static string ToStringSign(this float number)
	{
		return $"{(number > 0.0 ? "+" : "")}{number}";
	}

	/// <summary>
	/// Truncates toward zero to the given number of decimal places.
	/// </summary>
	public static float ToLimit(this float number,int decimalPoint)
	{
		var factor = Mathf.Pow(10.0f,decimalPoint);

		return Mathf.Sign(number)*Mathf.Floor(Mathf.Abs(number)*factor)/factor;
	}

	/// <summary>
	/// Wraps an angle into the range -180.0f ~ +180.0f.
	/// </summary>
	public static float ToWrapAngle(this float angle)
	{
		var fullAngle = Global.FullAngle;
		var halfAngle = fullAngle*0.5f;

		angle = ((angle%fullAngle)+fullAngle)%fullAngle;

		return angle > halfAngle ? angle-fullAngle : angle;
	}

	/// <summary>
	/// Builds an XZ-plane position from polar coordinates.
	/// </summary>
	/// <param name="radius">Distance from the origin.</param>
	/// <param name="degree">Angle in degrees.</param>
	public static Vector3 ToVector(this float radius,float degree)
	{
		return new Vector3(radius*Mathf.Cos(degree*Mathf.Deg2Rad),0,radius*Mathf.Sin(degree*Mathf.Deg2Rad));
	}

	/// <summary>
	/// Returns whether two floats are approximately equal.
	/// </summary>
	public static bool Approximately(this float number1,float number2)
	{
		return Mathf.Approximately(number1,number2);
	}

	/// <summary>
	/// Returns whether the float is approximately zero.
	/// </summary>
	public static bool ApproximatelyZero(this float number)
	{
		return Approximately(number,0.0f);
	}

	/// <summary>
	/// Splits a value into its integer and absolute fractional parts.
	/// </summary>
	public static void SeparateDecimal(this float number,out int integer,out float fraction)
	{
		integer = Mathf.FloorToInt(number);
		fraction = Mathf.Abs(number-integer);
	}
}
