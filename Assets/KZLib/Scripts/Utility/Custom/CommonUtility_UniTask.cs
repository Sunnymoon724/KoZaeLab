using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static partial class CommonUtility
{
	public static void KillTokenSource(ref CancellationTokenSource tokenSource)
	{
		if(tokenSource == null)
		{
			return;
		}

		tokenSource.Cancel();
		tokenSource.Dispose();
		tokenSource = null;
	}

	public static void RecycleTokenSource(ref CancellationTokenSource tokenSource)
	{
		KillTokenSource(ref tokenSource);

		tokenSource = new CancellationTokenSource();
	}

	public static async UniTask MergeUniTaskAsync(Func<UniTask>[] onPlayTaskArray,CancellationToken token)
	{
		foreach(var onPlayTask in onPlayTaskArray)
		{
			if(token.IsCancellationRequested)
			{
				return;
			}

			if(onPlayTask != null)
			{
				await onPlayTask();
			}
		}
	}

	public static async UniTask LoopUniTaskAsync(Func<UniTask> onPlayTask,int count,CancellationToken token)
	{
		await _LoopPlayAsync(onPlayTask,count,token);
	}

	public static async UniTask LoopActionNWaitForSecondAsync(Action onAction,float second,bool ignoreTimeScale,int count,CancellationToken token)
	{
		await _LoopPlayAsync(async ()=>
		{
			onAction();
			await UniTask.Delay(TimeSpan.FromSeconds(second),ignoreTimeScale,cancellationToken : token);

		},count,token);
	}

	public static async UniTask LoopActionNWaitForFrameAsync(Action onAction,int count,CancellationToken token)
	{
		await _LoopPlayAsync(async ()=>
		{
			onAction();
			await UniTask.Yield(token);

		},count,token);
	}

	private static async UniTask _LoopPlayAsync(Func<UniTask> onPlayTask,int count,CancellationToken token)
	{
		if(count == 0)
		{
			return;
		}

		var totalCount = count;

		while(totalCount == -1 || totalCount-- > 0)
		{
			if(token.IsCancellationRequested)
			{
				return;
			}

			if(onPlayTask != null)
			{
				await onPlayTask();
			}
		}
	}

	/// <summary>
	/// Wait time or condition
	/// </summary>
	public static async UniTask WaitForSecondsOrConditionAsync(float duration,Func<bool> onCondition,bool ignoreTimescale = false,CancellationToken token = default)
	{
		await UniTask.WhenAny(UniTask.Delay(TimeSpan.FromSeconds(duration),ignoreTimescale,cancellationToken : token),UniTask.WaitUntil(onCondition,cancellationToken : token));
	}

	public static async UniTask WaitForConditionAsync(Func<bool> onCondition,Action<float> onTimeUpdate,bool ignoreTimescale = false,CancellationToken token = default)
	{
		if(onCondition == null)
		{
			return;
		}

		var elapsedTime = 0.0f;

		var condition = onCondition();

		while(!condition)
		{
			if(token.IsCancellationRequested)
			{
				return;
			}

			onTimeUpdate?.Invoke(elapsedTime);

			await UniTask.Yield();

			var deltaTime = _GetDeltaTime(ignoreTimescale);

			if(deltaTime > 0.0f)
			{
				elapsedTime += deltaTime;
			}
		}

		onTimeUpdate?.Invoke(elapsedTime);
	}

	public static async UniTask ExecuteProgressAsync(float start,float finish,float duration,Action<float> onProgress,bool ignoreTimescale = false,AnimationCurve _curve = null,CancellationToken token = default)
	{
		if(duration == 0.0f)
		{
			return;
		}

		var curve = _curve ?? GetEaseCurve(EaseType.Linear);
		var elapsedTime = 0.0f;

		while(duration < 0.0f || elapsedTime < duration)
		{
			if(token.IsCancellationRequested)
			{
				return;
			}

			var progress = duration > 0.0f ? Mathf.Clamp01(elapsedTime/duration) : 1.0f;

			onProgress?.Invoke((finish-start)*curve.Evaluate(progress)+start);

			await UniTask.Yield(token);

			var deltaTime = _GetDeltaTime(ignoreTimescale);

			if(deltaTime > 0.0f)
			{
				elapsedTime += deltaTime;
			}
		}

		onProgress?.Invoke(finish);
	}

	public static async UniTask ExecutePeriodAsync(float duration,float period,Action<float> onUpdate,Action onComplete,bool ignoreTimescale = false,CancellationToken token = default)
	{
		if(duration == 0.0f)
		{
			return;
		}

		var elapsedTime = 0.0f;

		while(duration < 0.0f || elapsedTime < duration)
		{
			if(token.IsCancellationRequested)
			{
				return;
			}

			onUpdate?.Invoke(elapsedTime);

			await UniTask.WaitForSeconds(period,ignoreTimescale);

			elapsedTime += period;
		}

		onComplete?.Invoke();
	}

	private static float _GetDeltaTime(bool ignoreTimescale)
	{
		return ignoreTimescale ? Time.unscaledDeltaTime : Time.deltaTime;
	}
}