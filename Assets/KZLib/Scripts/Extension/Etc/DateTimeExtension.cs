using System;

public static class DateTimeExtension
{
	public static int[] ToArray(this DateTime _dateTime)
	{
		return new int[] { _dateTime.Year,_dateTime.Month,_dateTime.Day,_dateTime.Hour,_dateTime.Minute,_dateTime.Second, };
	}

	/// <summary>
	/// 다음날 자정 DateTime 가져올때 사용.
	/// </summary>
	public static DateTime GetNextDay(this DateTime _dateTime)
	{
		return _dateTime.Date.AddDays(1);
	}
}