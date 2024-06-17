using System;

public static class DateTimeExtension
{
	public static int[] ToArray(this DateTime _dateTime)
	{
		return new int[] { _dateTime.Year,_dateTime.Month,_dateTime.Day,_dateTime.Hour,_dateTime.Minute,_dateTime.Second, };
	}

	public static string ToString(this DateTime _dateTime,bool _legacy)
	{
		return _legacy ? _dateTime.ToString() : CommonUtility.TimeToText(_dateTime.Hour,_dateTime.Minute,_dateTime.Second);
	}

	public static string ToString(this TimeSpan _timeSpan,bool _legacy)
	{
		return _legacy ? _timeSpan.ToString() : CommonUtility.TimeToText(_timeSpan.Hours,_timeSpan.Minutes,_timeSpan.Seconds);
	}

	/// <summary>
	/// 다음날 자정 DateTime 가져올때 사용.
	/// </summary>
	public static DateTime GetNextDay(this DateTime _dateTime)
	{
		var current = _dateTime.Day;

		while(_dateTime.Day == current)
		{
			_dateTime = _dateTime.AddSeconds(1.0f);
		}

		return _dateTime;
	}
}