using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static partial class CommonUtility
{
	public static void KillTokenSource(ref CancellationTokenSource _tokenSource)
	{
		if(_tokenSource == null)
		{
			return;
		}

		_tokenSource.Cancel();
		_tokenSource.Dispose();
		_tokenSource = null;
	}

	public static void RecycleTokenSource(ref CancellationTokenSource _tokenSource)
	{
		KillTokenSource(ref _tokenSource);

		_tokenSource = new CancellationTokenSource();
	}

	public static async UniTask MergeUniTaskAsync(Func<UniTask>[] _onPlayTaskArray,CancellationToken _token)
	{
		foreach(var onPlayTask in _onPlayTaskArray)
		{
			if(_token.IsCancellationRequested)
			{
				return;
			}

			if(onPlayTask != null)
			{
				await onPlayTask();
			}
		}
	}

	public static async UniTask LoopUniTaskAsync(Func<UniTask> _onPlayTask,int _count,CancellationToken _token)
	{
		await LoopPlayAsync(_onPlayTask,_count,_token);
	}

	public static async UniTask LoopActionNWaitForSecondAsync(Action _onAction,float _second,bool _ignoreTimeScale,int _count,CancellationToken _token)
	{
		await LoopPlayAsync(async ()=>
		{
			_onAction();
			await UniTask.Delay(TimeSpan.FromSeconds(_second),_ignoreTimeScale,cancellationToken : _token);

		},_count,_token);
	}

	public static async UniTask LoopActionNWaitForFrameAsync(Action _onAction,int _count,CancellationToken _token)
	{
		await LoopPlayAsync(async ()=>
		{
			_onAction();
			await UniTask.Yield(_token);

		},_count,_token);
	}

	private static async UniTask LoopPlayAsync(Func<UniTask> _onPlayTask,int _count,CancellationToken _token)
	{
		if(_count == 0)
		{
			return;
		}

		var count = _count;

		while(count == -1 || count-- > 0)
		{
			if(_token.IsCancellationRequested)
			{
				return;
			}

			if(_onPlayTask != null)
			{
				await _onPlayTask();
			}
		}
	}

	/// <summary>
	/// Wait time or condition
	/// </summary>
	public static async UniTask WaitForSecondsOrConditionAsync(float _duration,Func<bool> _onCondition,bool _ignoreTimescale = false,CancellationToken _token = default)
	{
		await UniTask.WhenAny(UniTask.Delay(TimeSpan.FromSeconds(_duration),_ignoreTimescale,cancellationToken : _token),UniTask.WaitUntil(_onCondition,cancellationToken : _token));
	}

	public static async UniTask WaitForConditionAsync(Func<bool> _onCondition,Action<float> _onTimer,bool _ignoreTimescale = false,CancellationToken _token = default)
	{
		if(_onCondition == null)
		{
			return;
		}

		var elapsedTime = 0.0f;

		while(!_onCondition())
		{
			if(_token.IsCancellationRequested)
			{
				return;
			}

			_onTimer?.Invoke(elapsedTime);

			await UniTask.Yield(_token);

			var deltaTime = GetDeltaTime(_ignoreTimescale);

			if(deltaTime > 0.0f)
			{
				elapsedTime += deltaTime;
			}
		}

		_onTimer?.Invoke(elapsedTime);
	}

	public static async UniTask ExecuteOverTimeAsync(float _start,float _finish,float _duration,Action<float> _onProgress,bool _ignoreTimescale = false,AnimationCurve _curve = null,CancellationToken _token = default)
	{
		if(_duration == 0.0f)
		{
			return;
		}

		var curve = _curve ?? CommonUtility.GetEaseCurve(EaseType.Linear);
		var elapsedTime = 0.0f;

		while(_duration < 0.0f || elapsedTime < _duration)
		{
			if(_token.IsCancellationRequested)
			{
				return;
			}

			var progress = _duration > 0.0f ? Mathf.Clamp01(elapsedTime/_duration) : 1.0f;

			_onProgress?.Invoke((_finish-_start)*curve.Evaluate(progress)+_start);

			await UniTask.Yield(_token);

			var deltaTime = GetDeltaTime(_ignoreTimescale);

			if(deltaTime > 0.0f)
			{
				elapsedTime += deltaTime;
			}
		}

		_onProgress?.Invoke(_finish);
	}

	private static float GetDeltaTime(bool _ignoreTimescale)
	{
		return _ignoreTimescale ? Time.unscaledDeltaTime : Time.deltaTime;
	}
}