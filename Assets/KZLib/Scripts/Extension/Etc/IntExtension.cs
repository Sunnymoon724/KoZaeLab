using System;
using UnityEngine;

public static class IntExtension
{
	public static int Sign(this int _int32)
	{
		return _int32 < 0 ? -1 : _int32 > 0 ? 1 : 0;
	}

	public static int GetDigitsSize(this int _int32)
	{
		return _int32 == 0 ? 1 : (int) Mathf.Log10(_int32)+1;
	}

	public static string ToStringComma(this int _int32)
	{
		return string.Format("{0:n0}",_int32);
	}

	public static string ToStringSign(this int _int32)
	{
		return string.Format("{0:+#;-#;0}",_int32);
	}

	public static char ToHexChar(this int _int32)
	{
		return _int32 > 15 ? 'F' : _int32 < 10 ? (char) ('0'+_int32) : (char) ('A'+_int32-10);
	}

	public static string ToHex(this int _decimal)
	{
		return string.Format("{0}",_decimal.ToHexChar());
	}

	public static string ToHex8(this int _decimal)
	{
		return string.Format("{0:x2}",_decimal &= 0xFF);
	}

	public static string ToHex24(this int _decimal)
	{
		return string.Format("{0:x6}",_decimal &= 0xFFFFFF);
	}

	public static string ToHex32(this int _decimal)
	{
		return string.Format("{0:x8}",_decimal &= 0xFFFFFF);
	}

	public static bool IsEnumDefined<TNumber>(this int _int32)
	{
		return Enum.IsDefined(typeof(TNumber),_int32);
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

	public static bool IsPrimeNumber(this int _int32)
	{
		for(var i=2;i*i<=_int32;i++)
		{
			if(_int32%i == 0)
			{
				return false;
			}
		}
		
		return true;
	}

	#region ToRoman
	public static string ToRoman(this int _decimal)
	{
		if(_decimal > 999)
		{
			return string.Format("M{0}",ToRoman(_decimal-1000));
		}

		if(_decimal > 899)
		{
			return string.Format("CM{0}",ToRoman(_decimal-900));
		}

		if(_decimal > 499)
		{
			return string.Format("D{0}",ToRoman(_decimal-500));
		}

		if(_decimal > 399)
		{
			return string.Format("CD{0}",ToRoman(_decimal-400));
		}

		if(_decimal > 99)
		{
			return string.Format("C{0}",ToRoman(_decimal-100));
		}

		if(_decimal > 89)
		{
			return string.Format("XC{0}",ToRoman(_decimal-90));
		}

		if(_decimal > 49)
		{
			return string.Format("L{0}",ToRoman(_decimal-50));
		}

		if(_decimal > 39)
		{
			return string.Format("XL{0}",ToRoman(_decimal-40));
		}

		if(_decimal > 9)
		{
			return string.Format("X{0}",ToRoman(_decimal-10));
		}

		if(_decimal > 8)
		{
			return string.Format("IX{0}",ToRoman(_decimal-9));
		}

		if(_decimal > 4)
		{
			return string.Format("V{0}",ToRoman(_decimal-5));
		}

		if(_decimal > 3)
		{
			return string.Format("IV{0}",ToRoman(_decimal-4));
		}

		if(_decimal > 0)
		{
			return string.Format("I{0}",ToRoman(_decimal-1));
		}

		return "";
	}
	#endregion ToRoman
}