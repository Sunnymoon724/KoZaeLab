using System;

public static class EnumExtension
{
	public static int ToInt<TEnum>(this TEnum value) where TEnum : struct,Enum
	{
		return Convert.ToInt32(value);
	}

	public static TEnum ConvertToEnum<TEnum,UEnum>(this UEnum value) where TEnum : struct,Enum where UEnum : struct,Enum
	{
		return value.ToString().ToEnum<TEnum>();
	}

	public static TEnum AddEnum<TEnum>(this TEnum value,int count) where TEnum : struct,Enum
	{
		var valueArray = Enum.GetValues(typeof(TEnum));
		var index = CommonUtility.LoopClamp(value.ToInt()+count,valueArray.Length-1);

		return (TEnum) valueArray.GetValue(index);
	}
}