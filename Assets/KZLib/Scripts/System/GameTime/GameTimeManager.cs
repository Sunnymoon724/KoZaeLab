using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using KZLib.Utilities;
using UnityEngine;

namespace KZLib
{
	public class GameTimeManager : Singleton<GameTimeManager>
	{
		#region GameTime
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

				for(var i = m_timerInfoList.Count-1;i>=0;i--)
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
				m_pendingTimerInfoList.Add(new TimerInfo(ElapsedTime+Mathf.Max(0.0f,delayTime),callback));
			}

			public void ClearTimers()
			{
				m_timerInfoList.Clear();
				m_pendingTimerInfoList.Clear();
			}
		}
		#endregion GameTime

		private readonly Dictionary<string,GameTime> m_gameTimeDict = new();
		private readonly List<GameTime> m_gameTimeUpdateList = new();
		private CancellationTokenSource m_tokenSource = null;

		private GameTimeManager() { }

		protected override void _Initialize()
		{
			base._Initialize();

			KZExternalKit.RecycleTokenSource(ref m_tokenSource);

			_UpdateAsync(m_tokenSource.Token).Forget();
		}

		protected override void _Release(bool disposing)
		{
			if(disposing)
			{
				m_gameTimeDict.Clear();
				m_gameTimeUpdateList.Clear();

				KZExternalKit.KillTokenSource(ref m_tokenSource);
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
				var gameTime = new GameTime();

				m_gameTimeDict.Add(name,gameTime);
				m_gameTimeUpdateList.Add(gameTime);

				return true;
			}

			return false;
		}

		public bool RemoveTime(string name)
		{
			if(m_gameTimeDict.TryGetValue(name,out var gameTime))
			{
				m_gameTimeDict.Remove(name);
				m_gameTimeUpdateList.Remove(gameTime);

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
				for(var i=0;i<m_gameTimeUpdateList.Count;i++)
				{
					m_gameTimeUpdateList[i].Update();
				}

				await UniTask.Yield(cancellationToken: token).SuppressCancellationThrow();
			}
		}
	}
}