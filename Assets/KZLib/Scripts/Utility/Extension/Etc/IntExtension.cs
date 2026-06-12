using System;
using UnityEngine;

/// <summary>
/// Extension methods for int.
/// Provides formatting, hex/color conversion, bit flags, and numeric helpers.
/// </summary>
public static class IntExtension
{
	public static int Sign(this int number)
	{
		return number < 0 ? -1 : number > 0 ? 1 : 0;
	}

	/// <summary>
	/// Returns the number of decimal digits, including the sign bit for <see cref="int.MinValue"/>.
	/// </summary>
	public static int CalculateDigitCount(this int number)
	{
		if(number == int.MinValue)
		{
			return 10;
		}

		return number == 0 ? 1 : (int) Math.Floor(Math.Log10(Math.Abs(number)))+1;
	}

	public static string ToStringComma(this int number)
	{
		return $"{number:n0}";
	}

	public static string ToStringSign(this int number)
	{
		return $"{number:+#;-#;0}";
	}

	/// <summary>
	/// Converts a nibble (0-15) to its uppercase hex character.
	/// Logs and clamps when the value is out of range.
	/// </summary>
	public static char ToHexChar(this int number)
	{
		if(number < 0 || number > 15)
		{
			LogChannel.Kit.E($"{number} is out of hex range (0-15).");

			return number < 0 ? '0' : 'F';
		}

		return number < Global.HexLetterOffset ? (char) ('0'+number) : (char) ('A'+number-Global.HexLetterOffset);
	}

	public static string ToHex(this int number)
	{
		return $"{number.ToHexChar()}";
	}

	public static string ToHex8(this int number)
	{
		return $"{number & 0xFF:x2}";
	}

	public static string ToHex24(this int number)
	{
		return $"{number & 0xFFFFFF:x6}";
	}

	public static string ToHex32(this int number)
	{
		return $"{number & 0xFFFFFFFF:x8}";
	}

	public static bool IsEnumDefined<TNumber>(this int number)
	{
		return Enum.IsDefined(typeof(TNumber),number);
	}

	/// <summary>
	/// Converts an ARGB hex integer to a normalized Unity color.
	/// </summary>
	public static Color ToColor(this int hexNumber)
	{
		var pivot = 1.0f/Global.ColorMaxValue;

		return new Color
		(
			pivot*((hexNumber >> 24)	& 0xFF),
			pivot*((hexNumber >> 16)	& 0xFF),
			pivot*((hexNumber >> 8 )	& 0xFF),
			pivot*( hexNumber			& 0xFF)
		);
	}

	/// <summary>
	/// Converts an ARGB hex integer to a byte-based Unity color.
	/// </summary>
	public static Color32 ToColor32(this int hexNumber)
	{
		return new Color32
		(
			Convert.ToByte((hexNumber >> 24)	& 0xFF),
			Convert.ToByte((hexNumber >> 16)	& 0xFF),
			Convert.ToByte((hexNumber >> 8)		& 0xFF),
			Convert.ToByte( hexNumber			& 0xFF)
		);
	}

	public static bool HasFlag(this int pivot,int target)
	{
		return (pivot & target) != 0;
	}

	public static int AddFlag(this int pivot,int target)
	{
		return pivot | target;
	}

	public static int RemoveFlag(this int pivot,int target)
	{
		return pivot & ~target;
	}

	public static int ChangeFlag(this int pivot,int add,int remove)
	{
		return pivot.AddFlag(add).RemoveFlag(remove);
	}

	/// <summary>
	/// Returns whether the value is a prime number using a 6k±1 trial division check.
	/// </summary>
	public static bool IsPrimeNumber(this int number)
	{
		if(number <= 1)
		{
			return false;
		}
		if(number <= 3)
		{
			return true;
		}
		if(number % 2 == 0 || number % 3 == 0)
		{
			return false;
		}

		var i = 5;

		while(i*i <= number)
		{
			if(number%i == 0 || number % (i+2) == 0)
			{
				return false;
			}

			i += 6;
		}

		return true;
	}

	/// <summary>
	/// Converts a value in the range 1-3999 to Roman numerals.
	/// </summary>
	/// <returns>The Roman numeral string, or null when out of range.</returns>
	public static string ToRoman(this int number)
	{
		if(number <= 0 || number >= 4000)
		{
			return null;
		}

		var romanArray = new string[] { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };
		var valueArray = new int[]{ 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };

		var roman = string.Empty;

		for(var i=0;i<romanArray.Length;i++)
		{
			while(number >= valueArray[i])
			{
				roman += romanArray[i];
				number -= valueArray[i];
			}
		}

		return roman;
	}
}
