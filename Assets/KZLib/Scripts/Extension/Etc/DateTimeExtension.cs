using System;
using UnityEngine;

public static class DateTimeExtension
{
	public static int[] ToArray(this DateTime _dateTime)
	{
		return new int[] { _dateTime.Year,_dateTime.Month,_dateTime.Day,_dateTime.Hour,_dateTime.Minute,_dateTime.Second, };
	}

	/// <summary>
	/// -> 00:00
	/// </summary>
	public static DateTime GetNextDay(this DateTime _dateTime)
	{
		return _dateTime.Date.AddDays(1);
	}

	public static bool IsEqual(this DateTime _dateTime1,DateTime _dateTime2)
	{
		return _dateTime1 == _dateTime2 || Mathf.Abs(_dateTime1.Ticks-_dateTime2.Ticks) < 100;
	}
}