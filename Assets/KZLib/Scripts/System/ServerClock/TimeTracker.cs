using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace KZLib.Utilities
{
	public sealed class TimeTracker : IDisposable
	{
		private CancellationTokenSource m_tokenSource = null;
		private int m_timerGeneration = 0;
		private bool m_disposed = false;

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
			KZExternalKit.KillTokenSource(ref m_tokenSource);
		}

		public void SetTimer(long timestamp,Action<TimeSpan> onRefresh,Action onComplete)
		{
			SetTimerAsync(timestamp,onRefresh,onComplete).Forget();
		}

		public async UniTask SetTimerAsync(long timestamp,Action<TimeSpan> onRefresh,Action onComplete)
		{
			var dateTime = timestamp <= 0L ? DateTime.MinValue : timestamp.ToDateTime(true);

			await SetTimerAsync(dateTime,onRefresh,onComplete);
		}

		public void SetTimer(DateTime dateTime,Action<TimeSpan> onRefresh,Action onComplete)
		{
			SetTimerAsync(dateTime,onRefresh,onComplete).Forget();
		}

		public async UniTask SetTimerAsync(DateTime dateTime,Action<TimeSpan> onRefresh,Action onComplete)
		{
			if(m_disposed)
			{
				return;
			}

			if(dateTime == DateTime.MinValue)
			{
				// not use timer
				return;
			}

			StopTimer();

			var generation = ++m_timerGeneration;
			var remainingTime = KZTimeKit.GetRemainTime(dateTime,true);

			if(remainingTime.TotalSeconds <= 0.0d)
			{
				// timer is over
				if(_IsActiveTimer(generation))
				{
					onRefresh?.Invoke(TimeSpan.Zero);
					onComplete?.Invoke();
				}

				return;
			}

			await _PlayTimerAsync(dateTime,onRefresh,onComplete,generation);
		}

		private async UniTask _PlayTimerAsync(DateTime dateTime,Action<TimeSpan> onRefresh,Action onComplete,int generation)
		{
			KZExternalKit.RecycleTokenSource(ref m_tokenSource);

			var token = m_tokenSource.Token;

			TimeSpan remainingTime;

			while(_IsActiveTimer(generation,token))
			{
				remainingTime = KZTimeKit.GetRemainTime(dateTime,true);

				if(remainingTime.TotalSeconds <= 0.0d)
				{
					break;
				}

				onRefresh?.Invoke(remainingTime);

				await UniTask.Delay(TimeSpan.FromSeconds(1.0d),true,cancellationToken: token).SuppressCancellationThrow();
			}

			if(_IsActiveTimer(generation,token))
			{
				onRefresh?.Invoke(_ClampRemainTime(KZTimeKit.GetRemainTime(dateTime,true)));
				onComplete?.Invoke();
			}
		}

		private bool _IsActiveTimer(int generation,CancellationToken token = default)
		{
			return !m_disposed && generation == m_timerGeneration && !token.IsCancellationRequested;
		}

		private static TimeSpan _ClampRemainTime(TimeSpan remainingTime)
		{
			return remainingTime > TimeSpan.Zero ? remainingTime : TimeSpan.Zero;
		}

	}
}