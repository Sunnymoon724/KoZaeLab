using UnityEngine;
using System;
using System.Linq;
using KZLib;
using System.Collections.Generic;

namespace GameData
{
	/// <summary>
	/// 시간 관련
	/// </summary>
	public class GameTime : IGameData
	{
		private long m_ServerTime = 0L;
		private float m_SyncTime = 0.0f;

		public long LastServerTime { get; private set; }
		public float LastClientTime { get; private set; }
		public float NowClientTime { get; private set; }
		public long NowServerTime => m_ServerTime + (long)(TimeSpan.TicksPerSecond*(Time.realtimeSinceStartup-m_SyncTime));

		private readonly Dictionary<string,long> m_TimerDict = new();

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

			LastClientTime = NowClientTime;
			NowClientTime = m_SyncTime;
		}

		public DateTime GetCurrentTime(bool _isLocal = true)
		{
			var result = new DateTime(NowServerTime);

			return _isLocal ? result.ToLocalTime() : result;
		}

		/// <summary>
		/// 현재시간 목표시간 비교후(UTC) 현재시간이 목표시간 지난경우 : ExtraTime // 남은시간 있으면 : RemainingTime
		/// </summary>
		public void CheckDestinationTime(long _milliSeconds,Action _onExtraTime,Action _onRemainingTime = null)
		{
			if (NowServerTime >= _milliSeconds)
			{
				_onExtraTime?.Invoke();
			}
			else
			{
				_onRemainingTime?.Invoke();
			}
		}

		/// <summary>
		/// 현재시간 목표시간 비교후 현재시간이 목표시간 지난경우 : ExtraTime // 남은시간 있으면 : RemainingTime
		/// </summary>
		public void CheckAndDoAction(DateTime _distance,bool _isLocal,Action _onExtraTime,Action _onRemainingTime = null)
		{
			if(GetCurrentTime(_isLocal) >= _distance)
			{
				_onExtraTime?.Invoke();
			}
			else
			{
				_onRemainingTime?.Invoke();
			}
		}

		/// <summary>
		/// 현재 시간 이후로 해당 요일, 해당 시간을 알아오고자 할때 사용.
		/// </summary>
		public DateTime GetTargetDate(DayOfWeek[] _dayOfWeek,int _hour,bool _isLocal = false)
		{
			var dateTime = GetCurrentTime(_isLocal);

			//현재 요일인 경우.
			if(_dayOfWeek.Any(x => x == dateTime.DayOfWeek) && dateTime.Hour < _hour)
			{
				while(dateTime.Hour < _hour)
				{
					dateTime = dateTime.AddSeconds(1.0f);
				}

				return dateTime;
			}

			dateTime = dateTime.GetNextDay();

			//일단 요일 맞춰줌.
			while(!_dayOfWeek.Any(x => x == dateTime.DayOfWeek))
			{
				dateTime = dateTime.GetNextDay();
			}

			//시간 맞춰줌.
			while(dateTime.Hour < _hour)
			{
				dateTime = dateTime.AddSeconds(1.0f);
			}

			return dateTime;
		}

		public void StartTimer(string _key)
		{
			m_TimerDict.AddOrUpdate(_key,DateTime.Now.Ticks);
		}

		public bool GetTime(string _key,out float _seconds)
		{
			if(m_TimerDict.TryGetValue(_key,out var data))
			{
				_seconds = (DateTime.Now.Ticks-data)/(float)TimeSpan.TicksPerSecond;

				return true;
			}

			_seconds = 0.0f;

			return false;
		}
	}
}