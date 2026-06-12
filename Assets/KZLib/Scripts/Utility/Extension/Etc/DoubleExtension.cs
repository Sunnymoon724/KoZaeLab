using System;

/// <summary>
/// Extension methods for double.
/// Provides formatting, angle wrapping, truncation, and approximate comparison helpers.
/// </summary>
public static class DoubleExtension
{
	public static string ToStringComma(this double number)
	{
		return $"{number:n0}";
	}

	public static string ToStringPercent(this double number,int decimalPoint)
	{
		return string.Format(string.Concat("{0:f",decimalPoint,"}%"),number);
	}

	public static string ToStringSign(this double number)
	{
		return $"{(number > 0.0d ? "+" : "")}{number}";
	}

	/// <summary>
	/// Truncates toward zero to the given number of decimal places.
	/// </summary>
	public static double ToLimit(this double number,int decimalPoint)
	{
		var factor = Math.Pow(10.0d,decimalPoint);

		return Math.Sign(number)*Math.Floor(Math.Abs(number)*factor)/factor;
	}

	/// <summary>
	/// Wraps an angle into the range -180.0d ~ +180.0d.
	/// </summary>
	public static double ToWrapAngle(this double angle)
	{
		var fullAngle = (double) Global.FullAngle;

		angle = ((angle%fullAngle)+fullAngle)%fullAngle;

		return angle > fullAngle ? angle-fullAngle : angle;
	}

	public static long ToMilliseconds(this double second)
	{
		return (long) second*Global.MillisecondsPerSecond;
	}

	public static bool Approximately(this double number1,double number2,double delta = 1e-15d)
	{
		return Math.Abs(number1-number2) <= delta;
	}

	public static bool ApproximatelyZero(this double number,double delta = 1e-15d)
	{
		return Approximately(number,0.0d,delta);
	}

	/// <summary>
	/// Splits a value into its integer and absolute fractional parts.
	/// </summary>
	public static void SeparateDecimal(this double number,out int integer,out double fraction)
	{
		integer = (int) Math.Truncate(number);
        fraction = Math.Abs(number-integer);
	}
}
