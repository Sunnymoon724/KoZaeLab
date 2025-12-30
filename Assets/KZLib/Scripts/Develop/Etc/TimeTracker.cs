using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace KZLib.KZDevelop
{
	public sealed class TimeTracker : IDisposable
	{
		private CancellationTokenSource m_tokenSource = null;
		private bool m_disposed = false;

		~TimeTracker()
		{
			_Dispose(false);
		}

		public void Dispose()
		{
			_Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void _Dispose(bool disposing)
		{
			if(m_disposed)
			{
				return;
			}

			if(disposing)
			{
				StopTimer();
			}

			m_disposed = true;
		}

		public void StopTimer()
		{
			CommonUtility.KillTokenSource(ref m_tokenSource);
		}

		public void SetTimer(long timestamp,Action<TimeSpan> onRefresh,Action onComplete)
		{
			SetTimerAsync(timestamp,onRefresh,onComplete).Forget();
		}

		public async UniTask SetTimerAsync(long timestamp,Action<TimeSpan> onRefresh,Action onComplete)
		{
			var dateTime = timestamp <= 0L ? DateTime.MinValue : timestamp.ToDateTime(false);

			await SetTimerAsync(dateTime,onRefresh,onComplete);
		}

		public void SetTimer(DateTime dateTime,Action<TimeSpan> onRefresh,Action onComplete)
		{
			SetTimerAsync(dateTime,onRefresh,onComplete).Forget();
		}

		public async UniTask SetTimerAsync(DateTime dateTime,Action<TimeSpan> onRefresh,Action onComplete)
		{
			if(dateTime == DateTime.MinValue)
			{
				// not use timer
				return;
			}

			var endTime = dateTime.Kind == DateTimeKind.Utc ? dateTime : dateTime.ToUniversalTime();
			var remainingTime = _GetRemainingTime(endTime);

			if( remainingTime.TotalMilliseconds <= 0L )
			{
				// timer is over
				onComplete?.Invoke();

				return;
			}
			else
			{
				await _PlayTimerAsync(dateTime,onRefresh,onComplete);
			}
		}

		private async UniTask _PlayTimerAsync(DateTime dateTime,Action<TimeSpan> onRefresh,Action onComplete)
		{
			CommonUtility.RecycleTokenSource(ref m_tokenSource);

			TimeSpan remainingTime;

			while(true) 
			{
				remainingTime = _GetRemainingTime(dateTime);

				if(remainingTime.TotalSeconds <= 0.0d)
				{
					break;
				}

				onRefresh?.Invoke(remainingTime);

				try
				{
					await UniTask.Delay(1000,true,cancellationToken: m_tokenSource.Token);
				}
				catch(OperationCanceledException)
				{
					return;
				}
			}

			remainingTime = _GetRemainingTime(dateTime);

			onRefresh?.Invoke(remainingTime);
			onComplete?.Invoke();
		}

		private TimeSpan _GetRemainingTime(DateTime dateTime)
		{
			return dateTime - GameTimeManager.In.GetCurrentTime(true);
		}
	}
}