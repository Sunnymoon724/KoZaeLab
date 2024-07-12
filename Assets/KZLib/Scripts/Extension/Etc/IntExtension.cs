using System;
using UnityEngine;

public static class IntExtension
{
	public static int Sign(this int _integer)
	{
		return _integer < 0 ? -1 : _integer > 0 ? 1 : 0;
	}

	public static int GetNumberSize(this int _integer)
	{
		return _integer == 0 ? 1 : (int) Math.Floor(Math.Log10(Math.Abs(_integer)))+1;
	}

	public static string ToStringComma(this int _integer)
	{
		return string.Format("{0:n0}",_integer);
	}

	public static string ToStringSign(this int _integer)
	{
		return string.Format("{0:+#;-#;0}",_integer);
	}

	public static char ToHexChar(this int _integer)
	{
		return _integer > 15 ? 'F' : _integer < 10 ? (char) ('0'+_integer) : (char) ('A'+_integer-10);
	}

	public static string ToHex(this int _decimal)
	{
		return string.Format("{0}",_decimal.ToHexChar());
	}

	public static string ToHex8(this int _decimal)
	{
		return string.Format("{0:x2}",_decimal & 0xFF);
	}

	public static string ToHex24(this int _decimal)
	{
		return string.Format("{0:x6}",_decimal & 0xFFFFFF);
	}

	public static string ToHex32(this int _decimal)
	{
		return string.Format("{0:x8}", _decimal & 0xFFFFFFFF);
	}

	public static bool IsEnumDefined<TNumber>(this int _integer)
	{
		return Enum.IsDefined(typeof(TNumber),_integer);
	}

	public static Color ToColor(this int _hexNum)
	{
		var pivot = 1.0f/255.0f;

		return new Color
		(
			pivot*((_hexNum >> 24)	& 0xFF),
			pivot*((_hexNum >> 16)	& 0xFF),
			pivot*((_hexNum >> 8 )	& 0xFF),
			pivot*( _hexNum			& 0xFF)
		);
	}

	public static Color32 ToColor32(this int _hexNum)
	{
		return new Color32
		(
			Convert.ToByte((_hexNum >> 24)	& 0xFF),
			Convert.ToByte((_hexNum >> 16)	& 0xFF),
			Convert.ToByte((_hexNum >> 8)	& 0xFF),
			Convert.ToByte( _hexNum			& 0xFF)
		);
	}

	public static bool HasFlag(this int _pivot,int _target)
	{
		return (_pivot & _target) != 0;
	}

	public static int AddFlag(this int _pivot,int _target)
	{
		return _pivot |= _target;
	}

	public static int RemoveFlag(this int _pivot,int _target)
	{
		return _pivot &= ~_target;
	}

	public static bool IsPrimeNumber(this int _integer)
	{
		if(_integer <= 1)
		{
			return false;
		}
		if(_integer <= 3)
		{
			return true;
		}
		if(_integer % 2 == 0 || _integer % 3 == 0)
		{
			return false;
		}

		var i = 5;
		while(i*i <= _integer)
		{
			if(_integer%i == 0 || _integer % (i+2) == 0)
			{
				return false;
			}

			i += 6;
		}

		return true;
	}

	public static string ToRoman(this int _decimal)
	{
		if(_decimal <= 0 || _decimal >= 4000)
		{
			return null;
		}

		var romanArray = new string[] { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };
		var valueArray = new int[]{ 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };

		var roman = string.Empty;

		for(var i=0;i<romanArray.Length;i++)
		{
			while(_decimal >= valueArray[i])
			{
				roman += romanArray[i];
				_decimal -= valueArray[i];
			}
		}

		return roman;
	}
}