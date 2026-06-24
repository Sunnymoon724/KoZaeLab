using System;

/// <summary>
/// Extension methods for DateTime.
/// Provides array conversion, Unix timestamps, and comparison helpers.
/// </summary>
public static class DateTimeExtension
{
	/// <summary>
	/// Returns year, month, day, hour, minute, and second as an int array.
	/// </summary>
	public static int[] ToArray(this DateTime dateTime)
	{
		return new int[] { dateTime.Year,dateTime.Month,dateTime.Day,dateTime.Hour,dateTime.Minute,dateTime.Second, };
	}

	/// <summary>
	/// Converts the date/time to Unix epoch milliseconds in UTC.
	/// </summary>
	public static long ToLong(this DateTime dateTime)
	{
		return new DateTimeOffset(dateTime.ToUniversalTime()).ToUnixTimeMilliseconds();
	}

	/// <summary>
	/// Returns midnight at the start of the next calendar day.
	/// </summary>
	public static DateTime GetNextDay(this DateTime dateTime)
	{
		return dateTime.Date.AddDays(1);
	}

	/// <summary>
	/// Compares two date/time values with a tolerance of 100 ticks.
	/// </summary>
	public static bool IsEqual(this DateTime dateTime1,DateTime dateTime2)
	{
		return dateTime1 == dateTime2 || Math.Abs(dateTime1.Ticks-dateTime2.Ticks) < 100L;
	}
}
