using UnityEngine;
using System;
using System.Linq;
using KZLib;
using System.Collections.Generic;

namespace GameData
{
	public class GameTime : IGameData
	{
		private long m_ServerTime = 0L;
		private float m_SyncTime = 0.0f;

		public long LastServerTime { get; private set; }
		public float LastClientTime { get; private set; }

		public float CurrentClientTime { get; private set; }
		public long CurrentServerTime => m_ServerTime + (long)(TimeSpan.TicksPerSecond*(Time.realtimeSinceStartup-m_SyncTime));

		private readonly Dictionary<string,float> m_TimerDict = new();

		public void Initialize() { }

		public void Release()
		{
			m_TimerDict.Clear();
		}

		public void SetServerTime(long _serverTime)
		{
			m_SyncTime = Time.realtimeSinceStartup;
            LastServerTime = m_ServerTime;
            m_ServerTime = _serverTime;

            LastClientTime = CurrentClientTime;
			CurrentClientTime = m_SyncTime;
		}

		public DateTime GetCurrentTime(bool _isLocal = true)
		{
			var result = new DateTime(CurrentServerTime);

			return _isLocal ? result.ToLocalTime() : result;
		}

		/// <summary>
		/// Compare the current time with the target time (UTC). Passed : onTimePassed / Remaining : onTimeRemaining
		/// </summary>
		public void CheckTimeCondition(long _time,Action _onTimePassed,Action _onTimeRemaining)
		{
			if(CurrentServerTime >= _time)
			{
				_onTimePassed?.Invoke();
			}
			else
			{
				_onTimeRemaining?.Invoke();
			}
		}

		/// <summary>
		/// Compare the current time with the target time (UTC). Passed : onTimePassed / Remaining : onTimeRemaining
		/// </summary>
		public void CheckTimeCondition(DateTime _time,bool _isLocal,Action _onTimePassed,Action _onTimeRemaining)
		{
			if(GetCurrentTime(_isLocal) >= _time)
			{
				_onTimePassed?.Invoke();
			}
			else
			{
				_onTimeRemaining?.Invoke();
			}
		}

		/// <summary>
		/// Gets the next day of the week and time after the current time.
		/// </summary>
		public DateTime GetTargetDate(DayOfWeek[] _dayOfWeek,int _hour,bool _isLocal = false)
		{
			var dateTime = GetCurrentTime(_isLocal);

			//? Current day
			if(_dayOfWeek.Any(x => x == dateTime.DayOfWeek) && dateTime.Hour < _hour)
			{
				while(dateTime.Hour < _hour)
				{
					dateTime = dateTime.AddSeconds(1.0f);
				}

				return dateTime;
			}

			dateTime = dateTime.GetNextDay();

			//? Adjust Day
			while(!_dayOfWeek.Any(x => x == dateTime.DayOfWeek))
			{
				dateTime = dateTime.GetNextDay();
			}

			//? Adjust Hour.
			while(dateTime.Hour < _hour)
			{
				dateTime = dateTime.AddSeconds(1.0f);
			}

			return dateTime;
		}

		public void StartTimer(string _key)
		{
			m_TimerDict.AddOrUpdate(_key,Time.realtimeSinceStartup);
		}

		public bool GetTime(string _key,out float _seconds)
		{
			if(m_TimerDict.TryGetValue(_key,out var start))
			{
				_seconds = Time.realtimeSinceStartup-start;

				return true;
			}

			_seconds = 0.0f;

			return false;
		}
	}
}