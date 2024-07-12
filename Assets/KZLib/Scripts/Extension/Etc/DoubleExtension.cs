using System;

public static class DoubleExtension
{
	public static string ToStringComma(this double _double)
	{
		return string.Format("{0:n0}",_double);
	}

	public static string ToStringPercent(this double _double,int _decimals)
	{
		return string.Format(string.Concat("{0:f",_decimals,"}%"),_double);
	}

	public static string ToStringSign(this double _double)
	{
		return string.Format("{0}{1}",_double > 0.0d ? "+" : "",_double);
	}

	/// <summary>
	/// 소수점 n번쨰 까지만 표시
	/// </summary>
	public static double ToLimit(this double _double,int _decimals)
	{
		var factor = Math.Pow(10.0d,_decimals);

		return Math.Floor(_double*factor)/factor;
	}

	public static double ToWrapAngle(this double _angle)
	{
		_angle %= Global.FULL_ANGLE;

		while(_angle > +Global.HALF_ANGLE)
		{
			_angle -= Global.FULL_ANGLE;
		}

		while(_angle < -Global.HALF_ANGLE)
		{
			_angle += Global.FULL_ANGLE;
		}

		return _angle;
	}

	public static long ToMilliseconds(this double _seconds)
	{
		return (long) _seconds*1000L;
	}

	public static bool Approximately(this double _double,double _number,double _delta = 1e-15d)
	{
		return Math.Abs(_double-_number) <= _delta;
	}

	public static bool ApproximatelyZero(this double _double,double _delta = 1e-15d)
	{
		return Approximately(_double,0.0d,_delta);
	}

	public static void SeparateDecimal(this double _double,out int _integer,out double _fraction)
	{
		_integer = (int) Math.Truncate(_double);
        _fraction = Math.Abs(_double-_integer);
	}
}