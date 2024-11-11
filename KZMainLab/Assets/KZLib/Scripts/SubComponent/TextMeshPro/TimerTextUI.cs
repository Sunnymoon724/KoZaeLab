using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TimerTextUI : BaseTextUI
{
	private CancellationTokenSource m_TokenSource = null;

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
		CommonUtility.KillTokenSource(ref m_TokenSource);
	}

	public void PlayTimer(long _duration,string _format,Action<TimeSpan> _onUpdate,Action _onComplete,bool _ignoreTimeScale = false)
	{
		PlayTimerAsync(new TimeSpan(_duration),_format,_onUpdate,_onComplete,_ignoreTimeScale).Forget();
	}

	public void PlayTimer(TimeSpan _duration,string _format,Action<TimeSpan> _onUpdate,Action _onComplete,bool _ignoreTimeScale = false)
	{
		PlayTimerAsync(_duration,_format,_onUpdate,_onComplete,_ignoreTimeScale).Forget();
	}

	public async UniTask PlayTimerAsync(TimeSpan _duration,string _format,Action<TimeSpan> _onUpdate,Action _onComplete,bool _ignoreTimeScale = false)
	{
		CommonUtility.RecycleTokenSource(ref m_TokenSource);

		var remainingTime = TimeSpan.FromSeconds(_duration.TotalSeconds-Time.realtimeSinceStartup);

		if(remainingTime.TotalSeconds < 0.0d)
		{
			_onComplete?.Invoke();

			StopTimer();

			return;
		}

		while(remainingTime.TotalSeconds > 0)
		{
			_onUpdate?.Invoke(remainingTime);

			UpdateText(remainingTime,_format);

			await UniTask.Delay(1000,_ignoreTimeScale,cancellationToken : m_TokenSource.Token);

			remainingTime = TimeSpan.FromSeconds(_duration.TotalSeconds-Time.realtimeSinceStartup);
		}

		remainingTime = TimeSpan.FromSeconds(_duration.TotalSeconds-Time.realtimeSinceStartup);

		_onUpdate?.Invoke(remainingTime);

		UpdateText(remainingTime,_format);

		_onComplete?.Invoke();
	}

	private void UpdateText(TimeSpan _time,string _format)
    {
        m_TextMesh.SetSafeTextMeshPro(string.IsNullOrEmpty(_format) ? _time.ToString() : _time.ToString(_format));
    }
}