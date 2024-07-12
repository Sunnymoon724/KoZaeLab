using System;

public static class TimeSpanExtension
{
	public static string ToString(this TimeSpan _timeSpan,bool _legacy)
	{
		return _legacy ? _timeSpan.ToString() : CommonUtility.TimeToText(_timeSpan.Hours,_timeSpan.Minutes,_timeSpan.Seconds);
	}
}