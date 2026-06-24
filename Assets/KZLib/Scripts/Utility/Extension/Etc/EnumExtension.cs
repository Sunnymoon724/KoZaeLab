using System;

/// <summary>
/// Extension methods for enum values.
/// Provides conversion and cyclic navigation helpers.
/// </summary>
public static class EnumExtension
{
	/// <summary>
	/// Converts an enum value to its underlying int.
	/// </summary>
	public static int ToInt<TEnum>(this TEnum value) where TEnum : struct,Enum
	{
		return Convert.ToInt32(value);
	}

	/// <summary>
	/// Converts one enum type to another via its integer value.
	/// </summary>
	public static TEnum ConvertToEnum<TEnum,UEnum>(this UEnum value) where TEnum : struct,Enum where UEnum : struct,Enum
	{
		return (TEnum) Enum.ToObject(typeof(TEnum),Convert.ToInt32(value));
	}

	/// <summary>
	/// Moves to another enum value by offset, wrapping at the ends of the declared values.
	/// </summary>
	/// <param name="count">Steps to move; negative values move backward.</param>
	public static TEnum AddEnum<TEnum>(this TEnum value,int count) where TEnum : struct,Enum
	{
		var valueArray = Enum.GetValues(typeof(TEnum));
		var currentIndex = Array.IndexOf(valueArray,value);
		var newIndex = KZMathKit.LoopClamp(currentIndex+count,valueArray.Length);

		return (TEnum) valueArray.GetValue(newIndex);
	}
}
