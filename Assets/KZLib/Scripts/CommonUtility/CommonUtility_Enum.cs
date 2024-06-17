using System;
using System.Collections.Generic;
using System.Linq;

public static partial class CommonUtility
{
	public static IEnumerable<TEnum> GetAllEnumValues<TEnum>() where TEnum : struct,IConvertible,IComparable,IFormattable
	{
		return Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
	}

	public static string[] GetAllEnumNameArray<TEnum>() where TEnum : struct,IConvertible,IComparable,IFormattable
	{
		return Enum.GetNames(typeof(TEnum));
	}

	public static string[] GetAllEnumNameArray(Type _type)
	{
		return Enum.GetNames(_type);
	}
}