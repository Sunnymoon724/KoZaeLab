using System;

public static class EnumExtension
{
	public static int[] ToArray(this DateTime _dateTime)
	{
		return new int[] { _dateTime.Year,_dateTime.Month,_dateTime.Day,_dateTime.Hour,_dateTime.Minute,_dateTime.Second, };
	}

	public static int ToInt<TEnum>(this TEnum _enum) where TEnum : struct,Enum
	{
		return Convert.ToInt32(_enum);
	}

	public static TEnum ConvertToEnum<TEnum,UEnum>(this UEnum _enum) where TEnum : struct,Enum where UEnum : struct,Enum
	{
		return _enum.ToString().ToEnum<TEnum>();
	}

	public static TEnum AddEnum<TEnum>(this TEnum _enum,int _count) where TEnum : struct,Enum
	{
		var valueArray = Enum.GetValues(typeof(TEnum));
		var index = CommonUtility.LoopClamp(_enum.ToInt()+_count,valueArray.Length-1);

		return (TEnum) valueArray.GetValue(index);
	}
}