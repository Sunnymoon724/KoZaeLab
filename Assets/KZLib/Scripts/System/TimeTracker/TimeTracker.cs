using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace KZLib.Utilities
{
	/// <summary>
	/// Counts down to a target time and invokes callbacks on each tick and at completion.
	/// <see cref="SetTimer"/> / <see cref="SetTimerAsync"/> replace any running timer; <see cref="StopTimer"/> cancels it.
	/// Pass <see cref="DateTime.MinValue"/> or a non-positive timestamp to skip starting a timer.
	/// </summary>
	public sealed class TimeTracker : IDisposable
	{
		private CancellationTokenSource m_tokenSource = null;
		/// <summary>Incremented on each new timer so stale async loops exit without firing callbacks.</summary>
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

		/// <summary>Cancels the active countdown without invoking <c>onComplete</c>.</summary>
		public void StopTimer()
		{
			KZExternalKit.KillTokenSource(ref m_tokenSource);
		}

		/// <summary>Starts a countdown to <paramref name="timestamp"/> (Unix ms). Fire-and-forget wrapper.</summary>
		public void SetTimer(long timestamp,Action<TimeSpan> onRefresh,Action onComplete)
		{
			SetTimerAsync(timestamp,onRefresh,onComplete).Forget();
		}

		/// <summary>Starts a countdown to <paramref name="timestamp"/> (Unix ms). No-op when <paramref name="timestamp"/> is zero or negative.</summary>
		public async UniTask SetTimerAsync(long timestamp,Action<TimeSpan> onRefresh,Action onComplete)
		{
			var dateTime = timestamp <= 0L ? DateTime.MinValue : timestamp.ToDateTime(true);

			await SetTimerAsync(dateTime,onRefresh,onComplete);
		}

		/// <summary>Starts a countdown to <paramref name="dateTime"/>. Fire-and-forget wrapper.</summary>
		public void SetTimer(DateTime dateTime,Action<TimeSpan> onRefresh,Action onComplete)
		{
			SetTimerAsync(dateTime,onRefresh,onComplete).Forget();
		}

		/// <summary>
		/// Starts a countdown to <paramref name="dateTime"/>.
		/// <paramref name="onRefresh"/> is called about once per second with remaining time, then once more with zero at completion.
		/// If the target is already past, <paramref name="onRefresh"/> and <paramref name="onComplete"/> run immediately.
		/// </summary>
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
			var remainingTime = KZTimeKit.GetRemainUntil(dateTime,true);

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

		/// <summary>1-second tick loop until the target time or until superseded / cancelled.</summary>
		private async UniTask _PlayTimerAsync(DateTime dateTime,Action<TimeSpan> onRefresh,Action onComplete,int generation)
		{
			KZExternalKit.RecycleTokenSource(ref m_tokenSource);

			var token = m_tokenSource.Token;

			TimeSpan remainingTime;

			while(_IsActiveTimer(generation,token))
			{
				remainingTime = KZTimeKit.GetRemainUntil(dateTime,true);

				if(remainingTime.TotalSeconds <= 0.0d)
				{
					break;
				}

				onRefresh?.Invoke(remainingTime);

				await UniTask.Delay(TimeSpan.FromSeconds(1.0d),true,cancellationToken: token).SuppressCancellationThrow();
			}

			if(_IsActiveTimer(generation,token))
			{
				onRefresh?.Invoke(_ClampRemainTime(KZTimeKit.GetRemainUntil(dateTime,true)));
				onComplete?.Invoke();
			}
		}

		/// <summary>True when this timer generation is still the current one and not cancelled.</summary>
		private bool _IsActiveTimer(int generation,CancellationToken token = default)
		{
			return !m_disposed && generation == m_timerGeneration && !token.IsCancellationRequested;
		}

		/// <summary>Clamps negative drift after the target time to <see cref="TimeSpan.Zero"/>.</summary>
		private static TimeSpan _ClampRemainTime(TimeSpan remainingTime)
		{
			return remainingTime > TimeSpan.Zero ? remainingTime : TimeSpan.Zero;
		}

	}
}