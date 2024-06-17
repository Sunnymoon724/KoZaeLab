using System;

namespace KZLib.KZDevelop
{
	[Serializable]
	public partial struct BigNum : IComparable<BigNum>,IEquatable<BigNum>
	{
		public const string MAX_VALUE = "INFINITY";

		//! 가수
		private double m_Mantissa;
		public double Mantissa { readonly get => m_Mantissa; private set => m_Mantissa = value; }

		//! 지수
		private int m_Exponent;
		public int Exponent { readonly get => m_Exponent; private set => m_Exponent = value; }

		public BigNum((double,int) _tuple) : this(_tuple.Item1,_tuple.Item2) { }

		public BigNum(double _mantissa,int _exponent)
		{
			var result = AdjustNumber(_mantissa,_exponent);

			m_Mantissa = result.Item1;
			m_Exponent = result.Item2;
		}

		public BigNum(double _mantissa,double _exponent) : this(_mantissa,(int) _exponent) { }

		public BigNum(BigNum _number)
		{
			m_Mantissa = _number.Mantissa;
			m_Exponent = _number.Exponent;
		}

		public BigNum(string _data)
		{
			if(_data.IsEmpty())
			{
				m_Mantissa = 0.0d;
				m_Exponent = 0;

				return;
			}

			if(_data.Contains('e') || _data.Contains('E'))
			{
				ParseNumber(_data,out m_Mantissa,out m_Exponent);

				return;
			}

			if(_data.IsEqual(MAX_VALUE))
            {
                m_Mantissa = 1.0d;
                m_Exponent = NumUnit.GetExpByUnit("zz");

                return;
            }

			var text = _data.ExtractOnlyDigit();

			if(double.TryParse(text,out var mantissa))
			{
				var exponent = NumUnit.GetExpByUnit(_data.Replace(text,string.Empty));

				var result = AdjustNumber(mantissa,exponent);

				m_Mantissa = result.Item1;
				m_Exponent = result.Item2;

				return;
			}

			m_Mantissa = 0.0d;
			m_Exponent = 0;

			throw new ArgumentException(string.Format("{0}가 이렇게 들어와서 처리를 못했습니다.",_data));
		}

		public static implicit operator BigNum(double _number)
		{
			ParseNumber(_number,out var mantissa,out var exponent);

			return new BigNum(mantissa,exponent);
		}

		public static implicit operator BigNum(string _data)
		{
			return new BigNum(_data);
		}
	}
}