using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static partial class CommonUtility
{
	public static async UniTask DelayActionAsync(Action _onAction,float _second,bool _ignoreTimeScale,CancellationToken _token)
	{
		await UniTask.WaitForSeconds(_second,_ignoreTimeScale,cancellationToken : _token);

		_onAction();
	}

	public static async UniTask MergeUniTaskAsync(Func<UniTask>[] _onPlayTaskArray,CancellationToken _token)
	{
		for(var i=0;i<_onPlayTaskArray.Length;i++)
		{
			if(_token.IsCancellationRequested)
			{
				return;
			}

			var onPlayTask = _onPlayTaskArray[i];

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

	public static async UniTask LoopActionWaitForSecondeAsync(Action _onAction,float _second,bool _ignoreTimeScale,int _count,CancellationToken _token)
	{
		await LoopPlayAsync(async ()=>
		{
			_onAction();
			await UniTask.WaitForSeconds(_second,_ignoreTimeScale,cancellationToken : _token);

		},_count,_token);
	}

	public static async UniTask LoopActionWaitForFrameAsync(Action _onAction,int _count,CancellationToken _token)
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
	/// 지정된 시간 또는 조건이 충족될 때까지 대기하는 코루틴.
	/// </summary>
	public static async UniTask WaitForSecondOrActionAsync(float _duration,Func<bool> _onCondition,bool _ignoreTimescale = false,AnimationCurve _curve = null,CancellationToken _token = default)
	{
		await UniTask.WhenAny(ExecuteDurationAsync(_duration,_curve,_ignoreTimescale,null,_token),UniTask.WaitUntil(_onCondition,cancellationToken : _token));
	}

	public static async UniTask ExecuteLoopAsync(Action<float> _onProgress,bool _ignoreTimescale = false,AnimationCurve _curve = null,CancellationToken _token = default)
	{
		await ExecuteDurationAsync(-1.0f,_curve,_ignoreTimescale,(progress)=>
		{
			_onProgress?.Invoke(progress);
		},_token);
	}

	public static async UniTask ExecuteOverTimeAsync(float _start,float _finish,float _duration,Action<float> _onProgress,bool _ignoreTimescale = false,AnimationCurve _curve = null,CancellationToken _token = default)
	{
		await ExecuteDurationAsync(_duration,_curve,_ignoreTimescale,(progress)=>
		{
			_onProgress?.Invoke((_finish-_start)*progress+_start);
		},_token);
	}

	private static async UniTask ExecuteDurationAsync(float _duration,AnimationCurve _curve,bool _ignoreTimescale,Action<float> _onProgress,CancellationToken _token)
	{
		if(_duration == 0.0f)
		{
			return;
		}

		var curve = _curve ?? GetEaseCurve(EaseType.Linear);
		var current = 0.0f;
		var elapsedTime = 0.0f;

		while(_duration < 0.0f || elapsedTime < _duration)
		{
			if(_token.IsCancellationRequested)
			{
				return;
			}

			_onProgress?.Invoke(curve.Evaluate(elapsedTime/_duration));

			await UniTask.Yield(_token);

			current += GetTime(_ignoreTimescale);

			if(elapsedTime != current)
			{
				elapsedTime = current;
			}
		}

		_onProgress?.Invoke(curve.Evaluate(1.0f));
	}

	private static float GetTime(bool _ignoreTimescale)
	{
		return _ignoreTimescale ? Time.unscaledDeltaTime : Time.deltaTime;
	}
}