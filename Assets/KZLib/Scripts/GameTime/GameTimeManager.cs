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
		
		private readonly Dictionary<string,GameTime> m_timeDict = new();
		private CancellationTokenSource m_tokenSource = null;
		private bool m_disposed = false;

		/// <summary>
		/// 초기화
		/// </summary>
		protected override void Initialize()
		{
			m_tokenSource = new CancellationTokenSource();

			UpdateAsync().Forget();
		}

		protected override void Release(bool disposing)
		{
			if(m_disposed)
			{
				return;
			}

			if(disposing)
			{
				m_timeDict.Clear();
				
				CommonUtility.KillTokenSource(ref m_tokenSource);
			}

			m_disposed = true;

			base.Release(disposing);
		}
		
		public bool HasTime(string name)
		{
			return m_timeDict.ContainsKey(name);
		}

		public void AddTime(string name)
		{
			if(!HasTime(name))
			{
				m_timeDict.Add(name,new GameTime());
			}
		}
		
		public void RemoveTime(string name)
		{
			if(HasTime(name))
			{
				m_timeDict.Remove(name);
			}
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
			if(m_timeDict.TryGetValue(name,out var gameTime))
			{
				return gameTime;
			}

			return null;
		}

		private async UniTaskVoid UpdateAsync()
		{
			try
			{
				while(true)
				{
					foreach(var time in m_timeDict.Values)
					{
						time.Update();
					}

					await UniTask.Yield(cancellationToken: m_tokenSource.Token);
				}
			}
			catch(OperationCanceledException) { }
		}
	}
}