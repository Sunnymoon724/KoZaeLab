using System;
using UnityEngine;

public static class DateTimeExtension
{
	public static int[] ToArray(this DateTime dateTime)
	{
		return new int[] { dateTime.Year,dateTime.Month,dateTime.Day,dateTime.Hour,dateTime.Minute,dateTime.Second, };
	}

	/// <summary>
	/// -> 00:00
	/// </summary>
	public static DateTime GetNextDay(this DateTime dateTime)
	{
		return dateTime.Date.AddDays(1);
	}

	public static bool IsEqual(this DateTime dateTime1,DateTime dateTime2)
	{
		return dateTime1 == dateTime2 || Mathf.Abs(dateTime1.Ticks-dateTime2.Ticks) < 100L;
	}
}