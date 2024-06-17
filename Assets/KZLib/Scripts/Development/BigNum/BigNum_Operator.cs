using System;

namespace KZLib.KZDevelop
{
	public partial struct BigNum : IComparable<BigNum>,IEquatable<BigNum>
	{
		public static BigNum operator -(BigNum _number)
		{
			return new BigNum(-_number.Mantissa,_number.Exponent);
		}

		public static BigNum operator +(BigNum _number1,BigNum _number2)
		{
			return Plus(_number1,_number2);
		}

		public static BigNum operator +(BigNum _number,double _double)
		{
			return _number+new BigNum(_double);
		}

		private static BigNum Plus(BigNum _number1,BigNum _number2)
		{
			var number1 = AdjustNumber(_number1.Mantissa,_number1.Exponent);
			var number2 = AdjustNumber(_number2.Mantissa,_number2.Exponent);

			if(number1.Item2 == number2.Item2)
			{
				return new BigNum(number1.Item1+number2.Item1,number1.Item2);
			}
			else
			{
				var result = AdjustExponent(number1.Item1,number1.Item2,number2.Item1,number2.Item2);

				return new BigNum(AdjustNumber(result.Item1+result.Item3,result.Item2));
			}
		}

		public static BigNum operator -(BigNum _number1,BigNum _number2)
		{
			return Minus(_number1,_number2);
		}

		public static BigNum operator -(BigNum _number,double _double)
		{
			return _number-new BigNum(_double);
		}

		private static BigNum Minus(BigNum _number1,BigNum _number2)
		{
			var number1 = AdjustNumber(_number1.Mantissa,_number1.Exponent);
			var number2 = AdjustNumber(_number2.Mantissa,_number2.Exponent);

			if(number1.Item1 == number2.Item1)
			{
				return new BigNum(number1.Item1-number2.Item1,number1.Item2);
			}
			else
			{
				var result = AdjustExponent(number1.Item1,number1.Item2,number2.Item1,number2.Item2);

				return new BigNum(AdjustNumber(result.Item1-result.Item3,result.Item2));
			}
		}

		public static BigNum operator *(BigNum _number1,BigNum _number2)
		{
			return Multiplication(_number1,_number2);
		}

		public static BigNum operator *(BigNum _number,double _double)
		{
			return _number*new BigNum(_double);
		}

		private static BigNum Multiplication(BigNum _number1,BigNum _number2)
		{
			var number1 = AdjustNumber(_number1.Mantissa,_number1.Exponent);
			var number2 = AdjustNumber(_number2.Mantissa,_number2.Exponent);

			return new BigNum(number1.Item1*number2.Item1,number1.Item2+number2.Item2);
		}

		public static BigNum operator /(BigNum _number1,BigNum _number2)
		{
			return Divide(_number1,_number2);
		}

		public static BigNum operator /(BigNum _num,double _double)
		{
			return _num/new BigNum(_double);
		}

		public static BigNum Divide(BigNum _number1,BigNum _number2)
		{
			var number1 = AdjustNumber(_number1.Mantissa,_number1.Exponent);
			var number2 = AdjustNumber(_number2.Mantissa,_number2.Exponent);

			if(number2.Item2 == 0.0d)
			{
				return double.NaN;
			}

			return new BigNum(number1.Item1/number2.Item1,number1.Item2-number2.Item2);
		}

		public static bool operator >(BigNum _number1,BigNum _number2)
		{
			return Compare(_number1.Mantissa,_number1.Exponent,_number2.Mantissa,_number2.Exponent);
		}

		public static bool operator >(BigNum _number,double _double)
		{
			return _number > new BigNum(_double);
		}

		public static bool operator <(BigNum _number1,BigNum _number2)
		{
			return Compare(_number2.Mantissa,_number2.Exponent,_number1.Mantissa,_number1.Exponent);
		}

		public static bool operator <(BigNum _number,double _double)
		{
			return _number < new BigNum(_double);
		}

		public static bool operator >=(BigNum _number1,BigNum _number2)
		{
			return Compare(_number1.Mantissa,_number1.Exponent,_number2.Mantissa,_number2.Exponent) || Equal(_number1.Mantissa,_number1.Exponent,_number2.Mantissa,_number2.Exponent);
		}

		public static bool operator >=(BigNum _number,double _double)
		{
			return _number >= new BigNum(_double);
		}

		public static bool operator <=(BigNum _number1,BigNum _number2)
		{
			return Compare(_number2.Mantissa,_number2.Exponent,_number1.Mantissa,_number1.Exponent) || Equal(_number2.Mantissa,_number2.Exponent,_number1.Mantissa,_number1.Exponent);
		}

		public static bool operator <=(BigNum _num,double _double)
		{
			return _num <= new BigNum(_double);
		}

		private static bool Compare(double _mantissa1,int _exponent1,double _mantissa2,int _exponent2)
		{
			// 둘다 같은 부호인 경우
			if((_mantissa1*_mantissa2) > 0.0d)
			{
				_mantissa1.CompareTo(0.0d);

				if(_mantissa1 > 0.0d)
				{
					return _exponent1 == _exponent2 ? _mantissa1 > _mantissa2 : _exponent1 > _exponent2;
				}
				else
				{
					return _exponent1 == _exponent2 ? _mantissa1 < _mantissa2 : _exponent1 < _exponent2;
				}
			}
			else
			{
				return _mantissa1 > _mantissa2;
			}
		}

		public static bool operator ==(BigNum _left,BigNum _right)
		{
			return Equal(_left.Mantissa,_left.Exponent,_right.Mantissa,_right.Exponent);
		}

		public static bool operator ==(BigNum _left,double _right)
		{
			return _left == new BigNum(_right);
		}

		public static bool operator !=(BigNum _left,BigNum _right)
		{
			return !(_left == _right);
		}

		public static bool operator !=(BigNum _left,double _right)
		{
			return !(_left == _right);
		}

		private static bool Equal(double _mantissa1,int _exponent1,double _mantissa2,int _exponent2)
		{
			return _mantissa1 == _mantissa2 && _exponent1 == _exponent2;
		}
	}
}