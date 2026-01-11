using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using KZLib.KZUtility;
using UnityEngine;

namespace KZLib
{
	public class GameTimeManager : Singleton<GameTimeManager>
	{
		private class GameTime
		{
			private record TimerInfo(float Trigger,Action CallBack);

			public float ElapsedTime { get; private set; }
			public float DeltaTime => TimeScale*Time.deltaTime;
			public float TimeScale { get; private set; }

			public bool IsPaused => TimeScale == 0f;

			private readonly List<TimerInfo> m_timerInfoList = new();
			private readonly List<TimerInfo> m_pendingTimerInfoList = new();

			public GameTime()
			{
				ElapsedTime = 0.0f;
				TimeScale = 1.0f;
			}

			public void Update()
			{
				if(IsPaused)
				{
					return;
				}

				ElapsedTime += DeltaTime;

				for(int i = m_timerInfoList.Count-1;i>=0;i--)
				{
					if(ElapsedTime >= m_timerInfoList[i].Trigger)
					{
						m_timerInfoList[i].CallBack?.Invoke();
						m_timerInfoList.RemoveAt(i);
					}
				}

				if(m_pendingTimerInfoList.Count > 0)
				{
					m_timerInfoList.AddRange(m_pendingTimerInfoList);
					m_pendingTimerInfoList.Clear();
				}
			}

			public void Reset()
			{
				ElapsedTime = 0.0f;
				ClearTimers();
			}

			public void SetTimeScale(float timeScale)
			{
				TimeScale = Mathf.Max(0.0f,timeScale);
			}
			
			public void AddTimer(float delayTime,Action callback)
			{
				m_pendingTimerInfoList.Add(new TimerInfo(ElapsedTime+delayTime,callback));
			}
			
			public void ClearTimers()
			{
				m_timerInfoList.Clear();
				m_pendingTimerInfoList.Clear();
			}
		}

		private readonly Dictionary<string,GameTime> m_gameTimeDict = new();
		private CancellationTokenSource m_tokenSource = null;

		public DateTime ServerTime { get; private set; }
		private TimeSpan m_timeDifference = TimeSpan.Zero;

		private GameTimeManager() { }

		protected override void _Initialize()
		{
			base._Initialize();

			ServerTime = DateTime.UtcNow;
			m_timeDifference = TimeSpan.Zero;

			CommonUtility.RecycleTokenSource(ref m_tokenSource);

			_UpdateAsync(m_tokenSource.Token).Forget();
		}

		protected override void _Release(bool disposing)
		{
			if(disposing)
			{
				m_gameTimeDict.Clear();

				CommonUtility.KillTokenSource(ref m_tokenSource);
			}

			base._Release(disposing);
		}

		public bool HasTime(string name)
		{
			return m_gameTimeDict.ContainsKey(name);
		}

		public bool AddTime(string name)
		{
			if(!HasTime(name))
			{
				m_gameTimeDict.Add(name,new GameTime());

				return true;
			}

			return false;
		}

		public bool RemoveTime(string name)
		{
			if(HasTime(name))
			{
				m_gameTimeDict.Remove(name);

				return true;
			}

			return false;
		}

		public void Pause(string name)
		{
			SetTimeScale(name,0.0f);
		}

		public void Resume(string name)
		{
			SetTimeScale(name,1.0f);
		}

		public void SetTimeScale(string name,float timeScale)
		{
			var gameTime = _GetGameTime(name);

			gameTime?.SetTimeScale(timeScale);
		}

		public void AddTimer(string name,float delayTime,Action callback)
		{
			var gameTime = _GetGameTime(name);

			gameTime?.AddTimer(delayTime,callback);
		}

		public void ClearTimers(string name)
		{
			var gameTime = _GetGameTime(name);

			gameTime?.ClearTimers();
		}

		public float GetElapsedTime(string name)
		{
			var gameTime = _GetGameTime(name);

			return gameTime == null ? 0.0f : gameTime.ElapsedTime;
		}

		public void ResetElapsedTime(string name)
		{
			var gameTime = _GetGameTime(name);

			gameTime?.Reset();
		}

		private GameTime _GetGameTime(string name)
		{
			return m_gameTimeDict.TryGetValue(name,out var gameTime) ? gameTime : null;
		}

		private async UniTaskVoid _UpdateAsync(CancellationToken token)
		{
			while(!token.IsCancellationRequested)
			{
				foreach(var pair in m_gameTimeDict)
				{
					pair.Value.Update();
				}

				await UniTask.Yield(cancellationToken: token).SuppressCancellationThrow();
			}
		}

		public void SyncServerTime(long newServerTimestamp)
		{
			var newServerTime = newServerTimestamp.ToDateTime(false);

			SyncServerTime(newServerTime);
		}

		public void SyncServerTime(DateTime newServerTime)
		{
			m_timeDifference = DateTime.UtcNow-newServerTime;

			ServerTime = newServerTime;
		}

		public DateTime GetCurrentTime(bool isLocal = true)
		{
			var currentTime = DateTime.UtcNow-m_timeDifference;

			return isLocal ? currentTime.ToLocalTime() : currentTime;
		}

		// /// <summary>
		// /// Compare the current time with the target time. Passed : onTimePassed / Remaining : onTimeRemaining
		// /// </summary>
		// public void CheckTimeCondition(DateTime dateTime,bool isLocal,Action onTimePassed,Action onTimeRemaining)
		// {
		// 	if(GetCurrentTime(isLocal) >= dateTime)
		// 	{
		// 		onTimePassed?.Invoke();
		// 	}
		// 	else
		// 	{
		// 		onTimeRemaining?.Invoke();
		// 	}
		// }

		// /// <summary>
		// /// Gets the next day of the week and time after the current time.
		// /// </summary>
		// public DateTime GetTargetDate(HashSet<DayOfWeek> dayHashSet,int hour,bool isLocal)
		// {
		// 	var dateTime = GetCurrentTime(isLocal);

		// 	if(dayHashSet.Contains(dateTime.DayOfWeek) && dateTime.Hour < hour)
		// 	{
		// 		dateTime = dateTime.AddHours(hour-dateTime.Hour);

		// 		return dateTime;
		// 	}

		// 	dateTime = dateTime.AddDays(1);

		// 	while(!dayHashSet.Contains(dateTime.DayOfWeek))
		// 	{
		// 		dateTime = dateTime.AddDays(1);
		// 	}

		// 	dateTime = dateTime.AddHours(hour-dateTime.Hour);

		// 	return dateTime;
		// }
	}
}