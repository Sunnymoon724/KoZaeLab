using System;
using KZLib;

/// <summary>
/// Utility methods for time-span calculations based on ServerClockManager.
/// </summary>
public static class KZTimeKit
{
	/// <summary>
	/// Returns the remaining time until tomorrow's midnight.
	/// </summary>
	public static TimeSpan GetRemainTime(bool isLocal)
	{
		return CalculateTomorrowMidnight(isLocal)-ServerClockManager.In.GetNow(isLocal);
	}

	/// <summary>
	/// Returns the remaining time until the timestamp represented by the given Unix time.
	/// </summary>
	public static TimeSpan GetRemainTime(long timeStamp,bool isLocal)
	{
		return GetRemainTime(timeStamp.ToDateTime(isLocal),isLocal);
	}

	/// <summary>
	/// Returns the remaining time until the given end time.
	/// </summary>
	public static TimeSpan GetRemainTime(DateTime endTime,bool isLocal)
	{
		var currentTime = ServerClockManager.In.GetNow(isLocal);
		var normalizedEndTime = isLocal ? endTime.Kind == DateTimeKind.Local ? endTime : endTime.ToLocalTime() : endTime.Kind == DateTimeKind.Utc ? endTime : endTime.ToUniversalTime();

		return normalizedEndTime-currentTime;
	}

	/// <summary>
	/// Returns whether the current local time falls within the given start/end range, inclusive.
	/// </summary>
	public static bool IsWithinPeriod(DateTime startTime,DateTime endTime)
	{
		var currentTime = ServerClockManager.In.GetNow(true);

		return currentTime >= startTime && currentTime <= endTime;
	}

	/// <summary>
	/// Returns the DateTime of tomorrow's midnight for the given clock mode.
	/// </summary>
	public static DateTime CalculateTomorrowMidnight(bool isLocal)
	{
		var currentTime = ServerClockManager.In.GetNow(isLocal);

		return currentTime.Date.AddDays(1);
	}

	/// <summary>
	/// Returns the next occurrence of the target day of week at midnight local time.
	/// If today is the target day, the result is the same day next week.
	/// </summary>
	public static DateTime CalculateNextDay(DayOfWeek targetDay)
	{
		var currentTime = ServerClockManager.In.GetNow(true);
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

	/// <summary>
	/// Returns whether the given hour falls within the start/end hour range.
	/// Supports ranges that wrap past midnight when startHour is greater than endHour.
	/// </summary>
	public static bool IsTimeInRange(DateTime dateTime,int startHour,int endHour)
	{
		var hour = dateTime.Hour;

		return (startHour <= endHour) ? (hour >= startHour && hour < endHour) : (hour >= startHour || hour < endHour);
	}
}
