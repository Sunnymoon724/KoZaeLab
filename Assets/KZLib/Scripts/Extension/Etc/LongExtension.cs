using System;

public static class LongExtension
{
	private static readonly DateTime s_epochTime = new(1970,1,1,0,0,0,0,DateTimeKind.Utc);

	public static long Sign(this long number)
	{
		return number < 0L ? -1L : number > 0L ? 1L : 0L;
	}

	public static string ToStringComma(this long number)
	{
		return $"{number:n0}";
	}

	public static string ToStringSign(this long number)
	{
		return $"{number:+#;-#;0}";
	}

	public static double ToSeconds(this long milliSecond)
	{
		return milliSecond/1000.0d;
	}

	public static DateTime ToDateTime(this long timeStamp,bool isLocal)
	{
		var dateTime = s_epochTime.AddMilliseconds(timeStamp);

		return isLocal ? dateTime.ToLocalTime() : dateTime;
	}

	public static bool HasFlag(this long pivot,long target)
	{
		return (pivot & target) != 0;
	}

	public static long AddFlag(this long pivot,long target)
	{
		return pivot |= target;
	}

	public static long RemoveFlag(this long pivot,long target)
	{
		return pivot &= ~target;
	}

	public static long ChangeFlag(this long pivot,long add,long remove)
	{
		return pivot.AddFlag(add).RemoveFlag(remove);
	}

	public static string ByteToString(this long value)
	{
		value.ByteToString(out var size, out var unit);

		return $"{size:N2} {unit}";
	}

	public static void ByteToString(this long value,out double size,out string unit)
	{
		var index = 0;
		size = value;

		while(size >= 1024.0d)
		{
			size /= 1024.0d;
			index++;
		}

		unit = Global.BYTE_UNIT_ARRAY[index];
	}

	public static long ByteScaleUpUnit(this long value)
	{
		return value/1024L;
	}

	public static double ByteScaleUpUnitToDouble(this long value)
	{
		return value/1024.0d;
	}
}