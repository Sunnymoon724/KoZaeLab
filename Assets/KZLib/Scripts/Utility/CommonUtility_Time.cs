using System;
using KZLib;

public static partial class CommonUtility
{
	public static TimeSpan GetRemainTime(bool isLocal)
	{
		return CalculateTomorrowMidnight(isLocal)-GameTimeManager.In.GetCurrentTime(isLocal);
	}

	public static TimeSpan GetRemainTime(long timeStamp,bool isLocal)
	{
		var currentTime = GameTimeManager.In.GetCurrentTime(isLocal);

		return timeStamp.ToDateTime(isLocal) - currentTime;
	}

	public static bool IsWithinPeriod(DateTime startTime,DateTime endTime)
	{
		var currentTime = GameTimeManager.In.GetCurrentTime(true);

		return currentTime >= startTime && currentTime <= endTime;
	}

	public static DateTime CalculateTomorrowMidnight(bool isLocal)
	{
		var currentTime = GameTimeManager.In.GetCurrentTime(isLocal);

		return currentTime.Date.AddDays(1);
	}

	public static DateTime CalculateNextDay(DayOfWeek targetDay)
	{
		var currentTime = GameTimeManager.In.GetCurrentTime(true);
		var todayMidnight = currentTime.Date;

		var todayDayOfWeek = (int) todayMidnight.DayOfWeek;
		var targetDayOfWeek = (int) targetDay;

		int daysUntilNextDay = (targetDayOfWeek-todayDayOfWeek+7)%7;

		if(daysUntilNextDay == 0)
		{
			daysUntilNextDay = 7;
		}

		return todayMidnight.AddDays(daysUntilNextDay);
	}
}