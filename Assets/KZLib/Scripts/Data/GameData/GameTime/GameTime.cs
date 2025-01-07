using System;
using System.Linq;
using KZLib;

namespace GameData
{
	public class GameTime : IGameData
	{
		public DateTime ServerTime { get; private set; }
		private TimeSpan m_timeDifference = TimeSpan.Zero;

		public void Initialize()
		{
			ServerTime = DateTime.UtcNow;
			m_timeDifference = TimeSpan.Zero;
		}

		public void Release()
		{
			
		}

		public void SyncServerTime(DateTime newServerTime)
		{
			m_timeDifference = DateTime.UtcNow-newServerTime;
			ServerTime = newServerTime;
		}

		public DateTime GetCurrentTime(bool isLocal = true)
		{
			var gameTime = DateTime.UtcNow-m_timeDifference;

			return isLocal ? gameTime.ToLocalTime() : gameTime;
		}

		/// <summary>
		/// Compare the current time with the target time. Passed : onTimePassed / Remaining : onTimeRemaining
		/// </summary>
		public void CheckTimeCondition(DateTime dateTime,bool isLocal,Action onTimePassed,Action onTimeRemaining)
		{
			if(GetCurrentTime(isLocal) >= dateTime)
			{
				onTimePassed?.Invoke();
			}
			else
			{
				onTimeRemaining?.Invoke();
			}
		}

		/// <summary>
		/// Gets the next day of the week and time after the current time.
		/// </summary>
		public DateTime GetTargetDate(DayOfWeek[] dayOfWeek,int hour,bool isLocal)
		{
			var dateTime = GetCurrentTime(isLocal);

			if(dayOfWeek.Contains(dateTime.DayOfWeek) && dateTime.Hour < hour)
			{
				dateTime = dateTime.AddHours(hour-dateTime.Hour);

				return dateTime;
			}

			dateTime = dateTime.AddDays(1);

			while(!dayOfWeek.Contains(dateTime.DayOfWeek))
			{
				dateTime = dateTime.AddDays(1);
			}

			dateTime = dateTime.AddHours(hour-dateTime.Hour);

			return dateTime;
		}
	}
}