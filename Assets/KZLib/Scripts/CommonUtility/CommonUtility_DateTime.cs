using System;

public static partial class CommonUtility
{
	private const int ONE_MINUTE = 60;

	public static readonly DateTime s_BaseDate = new(1970,1,1,0,0,0,0,DateTimeKind.Utc);

	public static long GetCurrentTimeToMilliSecond()
	{
		return DateTime.Now.Ticks/TimeSpan.TicksPerMillisecond;
	}

	public static string TimeToText(int _hour,int _minute,int _second,bool _fillZero = false)
	{
		DivideTime(ref _minute,ref _second);
		DivideTime(ref _hour,ref _hour);

		if(_fillZero)
		{
			// 시,분,초, 00:00:07 형식으로 빈공간까지 표시.
			return string.Format("{0:D2}:{1:D2}:{2:D2}",_hour,_minute,_second);
		}

		if(_hour > 0)
		{
			return string.Format("{0:D2}:{1:D2}:{2:D2}",_hour,_minute,_second);
		}
		else if(_minute > 0)
		{
			return string.Format("{0:D2}:{1:D2}",_minute,_second);
		}

		return string.Format("{0:D2}",_second);
	}

	public static string TimeToText(float _second)
	{
		int ss = (int) _second;
		int ms = (int) ((_second-ss)*100.0f);
		
		return string.Format("{0}.{1:D2}",ss,ms);
	}

	private static void DivideTime(ref int _num1,ref int _num2)
	{
		if(_num2 >= ONE_MINUTE)
		{
			var div = _num2/ONE_MINUTE;

			_num1 += div;
			_num2 -= div*ONE_MINUTE;
		}
	}

	/// <summary>
	/// 새로운 주 의 월요일을 리턴한다. 
	/// </summary>
	public static DateTime GetNewWeekMonday()
	{
		// 다음주 월요일을 계산한다. 
		var nextMonday = DateTime.Today;

		for(var i=1;i<8;i++)
		{
			nextMonday = DateTime.Today.AddDays(i);

			if(nextMonday.DayOfWeek == DayOfWeek.Monday)
			{
				break;
			}
		}

		return nextMonday;
	}
	
	/// <summary>
	/// 다음 요일 12:00 정각을 리턴함
	/// </summary>
	public static DateTime GetNextDay()
	{
		return DateTime.Today.AddDays(1);
	}
	
	/// <summary>
	/// 기간 표시용 String반환.
	/// </summary>
	public static string GetStrDateTimePeriod(DateTime _start,DateTime _end)
	{
		if(_start == null)
		{
			return null;
		}

		if(_end == null)
		{
			return "상시 진행";
		}

		return string.Format("{0} ~ {1}",_start.ToString("yyyy-MM-dd hh:mm tt"),_end.ToString("yyyy-MM-dd hh:mm tt"));
	}
	
	/// <summary>
	/// 기간 표시용 String반환.
	/// </summary>
	public static string GetStrDateTimePeriod(long _start,long _end)
	{
		return GetStrDateTimePeriod(new DateTime(_start).ToLocalTime(),new DateTime(_end).ToLocalTime());
	}

	public static string GetLocalizeDay(DayOfWeek _dayOfWeek)
	{
		return _dayOfWeek switch
		{
			DayOfWeek.Monday => "월요일",
			DayOfWeek.Tuesday => "화요일",
			DayOfWeek.Wednesday => "수요일",
			DayOfWeek.Thursday => "목요일",
			DayOfWeek.Friday => "금요일",
			DayOfWeek.Saturday => "토요일",
			DayOfWeek.Sunday => "일요일",
			_ => string.Empty,
		};
	}

	public static DateTime FromUnixTime(long _timeStamp)
	{
		return s_BaseDate.AddSeconds(_timeStamp);
	}

	public static long ToUnixTime(DateTime _date)
	{
		return (long) (_date.ToUniversalTime()-s_BaseDate).TotalSeconds;
	}

	public static TimeSpan GetRemainTime()
	{
		return GetNextDay().Date-DateTime.Now;
	}
}