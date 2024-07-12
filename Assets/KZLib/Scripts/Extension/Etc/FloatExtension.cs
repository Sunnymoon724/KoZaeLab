using UnityEngine;

public static class FloatExtension
{
	public static string ToStringComma(this float _single)
	{
		return string.Format("{0:n0}",_single);
	}

	public static string ToStringPercent(this float _single,int _decimals)
	{
		return string.Format(string.Concat("{0:f",_decimals,"}%"),_single);
	}

	public static string ToStringSign(this float _single)
	{
		return string.Format("{0}{1}",_single > 0.0 ? "+" : "",_single);
	}

	/// <summary>
	/// 소수점 n번쨰 까지만 표시
	/// </summary>
	public static float ToLimit(this float _single,int _decimals)
	{
		var factor = Mathf.Pow(10.0f,_decimals);

		return Mathf.Floor(_single*factor)/factor;
	}

	public static float ToWrapAngle(this float _angle)
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

	public static Vector3 ToVector(this float _radius,float _degree)
	{
		return new Vector3(_radius*Mathf.Cos(_degree*Mathf.Deg2Rad),0,_radius*Mathf.Sin(_degree*Mathf.Deg2Rad));
	}

	public static bool Approximately(this float _single,float _number)
	{
		return Mathf.Approximately(_single,_number);
	}

	public static bool ApproximatelyZero(this float _single)
	{
		return Approximately(_single,0.0f);
	}

	/// <summary>
	/// 정수와 소수 분리
	/// </summary>
	public static void SeparateDecimal(this float _single,out int _integer,out float _fraction)
	{
		_integer = Mathf.FloorToInt(_single);
		_fraction = Mathf.Abs(_single-_integer);
	}
}