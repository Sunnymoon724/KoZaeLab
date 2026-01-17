using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

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

	public static async UniTask LoopActionAndWaitForSecondAsync(Action onAction,float second,bool ignoreTimeScale,int count,CancellationToken token)
	{
		async UniTask _PlayAndDelayAsync()
		{
			onAction();

			await UniTask.Delay(TimeSpan.FromSeconds(second),ignoreTimeScale,cancellationToken : token).SuppressCancellationThrow();
		}

		await _LoopPlayAsync(_PlayAndDelayAsync,count,token);
	}

	public static async UniTask LoopActionAndWaitForFrameAsync(Action onAction,int count,CancellationToken token)
	{
		async UniTask _PlayAndWaitAsync()
		{
			onAction();

			await UniTask.Yield(token).SuppressCancellationThrow();
		}

		await _LoopPlayAsync(_PlayAndWaitAsync,count,token);
	}

	public static async UniTask LoopUpdateAndWaitForFrameAsync(Action<float> onUpdate,bool ignoreTimescale,int count,CancellationToken token)
	{
		async UniTask _PlayAndWaitAsync()
		{
			var deltaTime = _GetDeltaTime(ignoreTimescale);

			onUpdate(deltaTime);

			await UniTask.Yield(token).SuppressCancellationThrow();
		}

		await _LoopPlayAsync(_PlayAndWaitAsync,count,token);
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
	public static async UniTask WaitForSecondsOrConditionAsync(float duration,Func<bool> onCondition,bool ignoreTimescale,CancellationToken token)
	{
		await UniTask.WhenAny(UniTask.Delay(TimeSpan.FromSeconds(duration),ignoreTimescale,cancellationToken : token),UniTask.WaitUntil(onCondition,cancellationToken : token)).SuppressCancellationThrow();
	}

	public static async UniTask WaitForConditionAsync(Func<bool> onCondition,Action<float> onUpdate,bool ignoreTimescale,CancellationToken token)
	{
		if(onCondition == null)
		{
			return;
		}

		var elapsedTime = 0.0f;

		if (onCondition())
		{
			onUpdate?.Invoke(elapsedTime);

			return;
		}

		while(!onCondition())
		{
			if(token.IsCancellationRequested)
			{
				return;
			}

			onUpdate?.Invoke(elapsedTime);

			await UniTask.Yield(token).SuppressCancellationThrow();

			var deltaTime = _GetDeltaTime(ignoreTimescale);

			if(deltaTime > 0.0f)
			{
				elapsedTime += deltaTime;
			}
		}

		onUpdate?.Invoke(elapsedTime);
	}

	public static async UniTask ExecuteProgressAsync(float start,float finish,float duration,Action<float> onProgress,bool ignoreTimescale,AnimationCurve animationCurve,CancellationToken token)
	{
		if(duration <= 0.0f)
		{
			return;
		}

		var curve = animationCurve ?? GetEaseCurve(EaseType.Linear);
		var elapsedTime = 0.0f;

		while(elapsedTime < duration)
		{
			if(token.IsCancellationRequested)
			{
				return;
			}

			var progress = elapsedTime / duration;
			var value = Mathf.Lerp(start, finish, curve.Evaluate(progress));

			onProgress?.Invoke(value);

			await UniTask.Yield(token).SuppressCancellationThrow();

			var deltaTime = _GetDeltaTime(ignoreTimescale);
			elapsedTime += deltaTime;
		}

		onProgress?.Invoke(finish);
	}

	public static async UniTask ExecutePeriodAsync(float duration,float period,Action<float> onUpdate,bool ignoreTimescale,CancellationToken token)
	{
		if(duration <= 0.0f || period <= 0.0f)
		{
			return;
		}

		var elapsed = 0.0f;

		while(elapsed < duration)
		{
			if(token.IsCancellationRequested)
			{
				return;
			}

			var nextInterval = Mathf.Min(period,duration-elapsed);

			var canceled = await UniTask.Delay(TimeSpan.FromSeconds(nextInterval), ignoreTimescale, cancellationToken: token).SuppressCancellationThrow();

			if(canceled)
			{
				return;
			}

			elapsed += nextInterval;
			onUpdate?.Invoke(elapsed);

			if(duration.Approximately(elapsed))
			{
				break;
			}
		}
	}

	private static float _GetDeltaTime(bool ignoreTimescale)
	{
		return ignoreTimescale ? Time.unscaledDeltaTime : Time.deltaTime;
	}

	public static async UniTask<T> LoadHandleSafeAsync<T>(AsyncOperationHandle<T> handle)
	{
		await handle;

		if(handle.Status == AsyncOperationStatus.Failed)
		{
			throw handle.OperationException;
		}

		var result = handle.Result;

		handle.Release();

		return result;
	}
}