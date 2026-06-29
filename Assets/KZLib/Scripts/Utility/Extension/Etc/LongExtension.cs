using System;

/// <summary>
/// Extension methods for long.
/// Provides formatting, timestamp conversion, bit flags, and byte-size helpers.
/// </summary>
public static class LongExtension
{
	/// <summary>
	/// Returns -1, 0, or 1 for negative, zero, or positive values.
	/// </summary>
	public static long Sign(this long number)
	{
		return number < 0L ? -1L : number > 0L ? 1L : 0L;
	}

	/// <summary>
	/// Formats the long with thousands separators.
	/// </summary>
	public static string ToStringComma(this long number)
	{
		return $"{number:n0}";
	}

	/// <summary>
	/// Formats the long with an explicit sign prefix.
	/// </summary>
	public static string ToStringSign(this long number)
	{
		return $"{number:+#;-#;0}";
	}

	/// <summary>
	/// Converts milliseconds to seconds.
	/// </summary>
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

	/// <summary>
	/// Returns whether any bits in <paramref name="target"/> are set in <paramref name="pivot"/>.
	/// </summary>
	public static bool HasAnyFlag(this long pivot,long target)
	{
		return (pivot & target) != 0L;
	}

	/// <summary>
	/// Returns whether all bits in <paramref name="target"/> are set in <paramref name="pivot"/>.
	/// </summary>
	public static bool HasAllFlags(this long pivot,long target)
	{
		return target == 0L || (pivot & target) == target;
	}

	/// <summary>
	/// ORs <paramref name="target"/> bit flags into <paramref name="pivot"/>.
	/// </summary>
	public static long AddFlag(this long pivot,long target)
	{
		return pivot | target;
	}

	/// <summary>
	/// Clears <paramref name="target"/> bit flags from <paramref name="pivot"/>.
	/// </summary>
	public static long RemoveFlag(this long pivot,long target)
	{
		return pivot & ~target;
	}

	/// <summary>
	/// Adds and removes bit flags in one operation.
	/// </summary>
	public static long ChangeFlag(this long pivot,long add,long remove)
	{
		return pivot.AddFlag(add).RemoveFlag(remove);
	}

	/// <summary>
	/// Returns whether the value is a single power-of-two flag.
	/// </summary>
	public static bool IsSingleBitFlag(this long flag)
	{
		return flag > 0L && (flag & (flag-1L)) == 0L;
	}

	/// <summary>
	/// Returns the zero-based bit index for a single-bit flag, or -1 when invalid.
	/// </summary>
	public static int ToFlagOrder(this long flag)
	{
		if(!flag.IsSingleBitFlag())
		{
			return -1;
		}

		var order = 0;

		while(flag > 1L)
		{
			flag >>= 1;
			order++;
		}

		return order;
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

	/// <summary>
	/// Converts a byte count to whole kilobytes.
	/// </summary>
	public static long ToKilobytes(this long value)
	{
		return value/Global.BytesPerKilobyte;
	}

	/// <summary>
	/// Converts a byte count to kilobytes as a double.
	/// </summary>
	public static double ToKilobytesDouble(this long value)
	{
		return value/Global.BytesPerKilobyte;
	}
}
