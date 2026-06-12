using System;

/// <summary>
/// Extension methods for long.
/// Provides formatting, timestamp conversion, bit flags, and byte-size helpers.
/// </summary>
public static class LongExtension
{
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
		return milliSecond/(double) Global.MillisecondsPerSecond;
	}

	/// <summary>
	/// Converts Unix epoch milliseconds to a DateTime, optionally in local time.
	/// </summary>
	public static DateTime ToDateTime(this long timeStamp,bool isLocal)
	{
		var dateTime = DateTime.UnixEpoch.AddMilliseconds(timeStamp);

		return isLocal ? dateTime.ToLocalTime() : dateTime;
	}

	public static bool HasFlag(this long pivot,long target)
	{
		return (pivot & target) != 0;
	}

	public static long AddFlag(this long pivot,long target)
	{
		return pivot | target;
	}

	public static long RemoveFlag(this long pivot,long target)
	{
		return pivot & ~target;
	}

	public static long ChangeFlag(this long pivot,long add,long remove)
	{
		return pivot.AddFlag(add).RemoveFlag(remove);
	}

	/// <summary>
	/// Formats a byte count as a human-readable size with unit suffix.
	/// </summary>
	public static string ByteToString(this long value)
	{
		value.ByteToString(out var size, out var unit);

		return $"{size:N2} {unit}";
	}

	/// <summary>
	/// Scales a byte count down through KB units until it fits the largest applicable suffix.
	/// </summary>
	public static void ByteToString(this long value,out double size,out string unit)
	{
		var index = 0;
		size = value;

		while(size >= Global.BytesPerKilobyte)
		{
			size /= Global.BytesPerKilobyte;
			index++;
		}

		unit = Global.ByteUnitArray[Math.Min(index, Global.ByteUnitArray.Length-1)];
	}

	public static long ByteScaleUpUnit(this long value)
	{
		return value/Global.BytesPerKilobyte;
	}

	public static double ByteScaleUpUnitToDouble(this long value)
	{
		return value/Global.BytesPerKilobyte;
	}
}
