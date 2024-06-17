using System;
using System.Collections.Generic;
using System.Linq;
using KZLib.KZDevelop;

public static partial class CommonUtility
{
	/// <summary>
	/// 가두기
	/// </summary>
	public static BigNum Clamp(BigNum _value,BigNum _min,BigNum _max) => _value <= _min ? _min : _max <= _value ? _max : _value;

	/// <summary>
	/// 가두기
	/// </summary>
	public static BigNum Clamp(BigNum _value,BigNum _min) => Clamp(_value,_min,BigNum.MAX_VALUE);

	/// <summary>
	/// 절댓 값
	/// </summary>
	public static BigNum Abs(BigNum _value) => _value < 0 ? -_value : _value;

	/// <summary>
	/// 합
	/// </summary>
	public static BigNum Sum(IEnumerable<BigNum> _sources)
	{
		var result = new BigNum();

		foreach(var num in _sources)
		{
			result += num;
		}

		return result;
	}

	/// <summary>
	/// 평균
	/// </summary>
	public static BigNum Avg(IEnumerable<BigNum> _sources) => Sum(_sources) / _sources.Count();

	/// <summary>
	/// y 배수
	/// </summary>
	public static BigNum Pow(BigNum _x,double _y)
	{
		var man = Math.Pow(_x.Mantissa,_y);
		var exp = _x.Exponent*_y;

		return new BigNum(man,exp);
	}

	/// <summary>
	/// 제곱근
	/// </summary>
	public static BigNum Sqrt(BigNum _x)
	{
		var man = Math.Sqrt(_x.Mantissa);
		var exp = _x.Exponent/2;

		return new BigNum(man,exp);
	}

	/// <summary>
	/// 소수점 버림
	/// </summary>
	public static BigNum Floor(BigNum _x)
	{
		var value = _x.ToDouble();

		return value != double.MaxValue ? Math.Floor(value) : _x;
	}

	/// <summary>
	/// 소수점 올림
	/// </summary>
	public static BigNum Ceiling(BigNum _x)
	{
		var value = _x.ToDouble();

		return value != double.MaxValue ? Math.Ceiling(value) : _x;
	}
}