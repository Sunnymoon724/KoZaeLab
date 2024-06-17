using System;

namespace KZLib.KZDevelop
{
	public partial struct BigNum : IComparable<BigNum>,IEquatable<BigNum>
	{
		private static (double,int) AdjustNumber(double _mantissa,int _exponent)
		{
			if(_mantissa == 0.0d)
			{
				return (0.0d,0);
			}

			var exponent = (int) Math.Floor(Math.Log10(Math.Abs(_mantissa)));
			var power = Math.Pow(10.0d,-exponent);

			return (_mantissa*power,_exponent+exponent);
		}

		private static (double,int,double,int) AdjustExponent(double _mantissa1,int _exponent1,double _mantissa2,int _exponent2)
		{
			var gap = _exponent1-_exponent2;

			if(gap > 0)
			{
				_mantissa2 *= Math.Pow(10.0d,+gap);

				return (_mantissa1,_exponent1,_mantissa2,_exponent1);
			}
			else if(gap < 0)
			{
				_mantissa1 *= Math.Pow(10.0d,-gap);

				return (_mantissa1,_exponent2,_mantissa2,_exponent2);
			}

			return (_mantissa1,_exponent1,_mantissa2,_exponent2);
		}


		private static void ParseNumber(double _number,out double _mantissa,out int _exponent)
		{
			var result = AdjustNumber(_number,0);

			_mantissa = result.Item1;
			_exponent = result.Item2;
		}

		private static void ParseNumber(string _number,out double _mantissa,out int _exponent)
		{
			var numberArray = _number.ToLowerInvariant().Split('e');

			_mantissa = 0.0d;
			_exponent = 0;

			if(numberArray.Length != 2)
			{
				throw new ArgumentException(string.Format("숫자가 {0}라서 처리를 못했습니다.",_number));
			}

			if(double.TryParse(numberArray[0],out var mantissa) && int.TryParse(numberArray[1],out var exponent))
			{
				_mantissa = mantissa;
				_exponent = exponent;

				return;
			}

			throw new ArgumentException(string.Format("숫자가 {0}라서 처리를 못했습니다.",_number));
		}

		/// <summary>
		/// 음수는 디폴트 (단위가 없으면 0 / 있으면 2)이며 디폴트가 아니면 적힌 값으로 나온다.
		/// 소수가 0일 경우 false면 보여주지 않는다.
		/// </summary>
		public string ToString(int _point = -1,bool _zero = true)
		{
			if(CheckUnit(out var unit))
			{
				return MAX_VALUE;
			}

			var man = Mantissa*Math.Pow(10.0d,Exponent-NumUnit.GetExpByUnit(unit));

			_point = unit.IsEmpty() ? _point <= -1 ? 0 : _point : _point <= -1 ? 2 : _point;

			if(_zero)
			{
				return string.Format(CreateFormat(_point),Math.Round(man,_point),unit);
			}

			var result = Math.Round(man,_point);

			if(result == Math.Floor(result))
			{
				return string.Format(CreateFormat(0),result,unit);
			}
			else
			{
				return string.Format(CreateFormat(_point),result,unit);
			}
		}

		private readonly string CreateFormat(int _count)
		{
			return string.Concat("{0:0.",new string('0',_count),"}{1}");
		}

		public double ToDouble()
		{
			if(CheckUnit(out _))
			{
				return double.MaxValue;
			}

			var pow = Math.Pow(10.0d,Exponent);

			return Mantissa*pow >= double.MaxValue ? double.MaxValue : Mantissa*pow;
		}

		private bool CheckUnit(out string _unit)
		{
			_unit = (Exponent <= 0) ? string.Empty : NumUnit.GetUnit(Exponent);

			return _unit == MAX_VALUE;
		}

		public float ToFloat() => (float) ToDouble();

		public override string ToString() => ToString(-1);

		public string ToExpNumber() => string.Format("{0}e{1}{2}",Mantissa,Exponent>0 ? "+" : "",Exponent);

		public override readonly bool Equals(object _obj) => base.Equals(_obj);

		public override readonly int GetHashCode() => base.GetHashCode();

		public int CompareTo(BigNum _num)
		{
			return Equals(_num) ? 0 : Compare(Mantissa,Exponent,_num.Mantissa,_num.Exponent) ? 1 : -1;
		}

		public bool Equals(BigNum _num)
		{
			return Equal(Mantissa,Exponent,_num.Mantissa,_num.Exponent);
		}

		public bool NotEquals(BigNum _num)
		{
			return !Equal(Mantissa,Exponent,_num.Mantissa,_num.Exponent);
		}
	}
}