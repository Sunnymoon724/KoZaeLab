using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TimerTextUI : BaseTextUI
{
	private CancellationTokenSource m_tokenSource = null;

	protected override void OnDisable()
	{
		base.OnDisable();

		StopTimer();
	}

	protected override void Release()
	{
		base.Release();

		StopTimer();
	}

	public void StopTimer()
	{
		CommonUtility.KillTokenSource(ref m_tokenSource);
	}

	public void PlayTimer(long duration,string format,Action<TimeSpan> onUpdate,Action onComplete,bool ignoreTimeScale = false)
	{
		PlayTimerAsync(new TimeSpan(duration),format,onUpdate,onComplete,ignoreTimeScale).Forget();
	}

	public void PlayTimer(TimeSpan duration,string format,Action<TimeSpan> onUpdate,Action onComplete,bool ignoreTimeScale = false)
	{
		PlayTimerAsync(duration,format,onUpdate,onComplete,ignoreTimeScale).Forget();
	}

	public async UniTask PlayTimerAsync(TimeSpan duration,string format,Action<TimeSpan> onUpdate,Action onComplete,bool ignoreTimeScale = false)
	{
		CommonUtility.RecycleTokenSource(ref m_tokenSource);

		var remainingTime = TimeSpan.FromSeconds(duration.TotalSeconds-Time.realtimeSinceStartup);

		if(remainingTime.TotalSeconds < 0.0d)
		{
			onComplete?.Invoke();

			StopTimer();

			return;
		}

		while(remainingTime.TotalSeconds > 0)
		{
			onUpdate?.Invoke(remainingTime);

			_UpdateText(remainingTime,format);

			await UniTask.Delay(1000,ignoreTimeScale,cancellationToken : m_tokenSource.Token);

			remainingTime = TimeSpan.FromSeconds(duration.TotalSeconds-Time.realtimeSinceStartup);
		}

		remainingTime = TimeSpan.FromSeconds(duration.TotalSeconds-Time.realtimeSinceStartup);

		onUpdate?.Invoke(remainingTime);

		_UpdateText(remainingTime,format);

		onComplete?.Invoke();
	}

	private void _UpdateText(TimeSpan timeSpan,string format)
    {
        m_textMesh.SetSafeTextMeshPro(string.IsNullOrEmpty(format) ? timeSpan.ToString() : timeSpan.ToString(format));
    }
}