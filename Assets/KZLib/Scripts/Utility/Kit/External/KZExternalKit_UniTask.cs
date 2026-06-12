using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public static partial class KZExternalKit
{
	/// <summary>
	/// Cancels and disposes the given token source, then clears the reference.
	/// </summary>
	public static void KillTokenSource(ref CancellationTokenSource tokenSource)
	{
		if(tokenSource == null)
		{
			return;
		}

		if(!tokenSource.IsCancellationRequested)
		{
			tokenSource.Cancel(); 
		}

		tokenSource.Dispose();
		tokenSource = null;
	}

	/// <summary>
	/// Recreates the token source linked to the MonoBehaviour destroy cancellation token.
	/// </summary>
	public static void RecycleTokenSourceInMono(ref CancellationTokenSource tokenSource,MonoBehaviour monoBehaviour)
	{
		if(!monoBehaviour)
		{
			throw new ArgumentNullException($"{nameof(monoBehaviour)} is null. MonoBehaviour must be assigned.");
		}

		RecycleTokenSource(ref tokenSource,monoBehaviour.GetCancellationTokenOnDestroy());
	}

	/// <summary>
	/// Recreates the token source linked to the given parent cancellation token.
	/// </summary>
	public static void RecycleTokenSource(ref CancellationTokenSource tokenSource,CancellationToken parentToken)
	{
		KillTokenSource(ref tokenSource);

		tokenSource = CancellationTokenSource.CreateLinkedTokenSource(parentToken);
	}

	/// <summary>
	/// Disposes the current token source and creates a fresh standalone one.
	/// </summary>
	public static void RecycleTokenSource(ref CancellationTokenSource tokenSource)
	{
		KillTokenSource(ref tokenSource);

		tokenSource = new CancellationTokenSource();
	}

	/// <summary>
	/// Runs the given UniTask delegates sequentially, stopping early on cancellation.
	/// </summary>
	public static async UniTask MergeUniTaskAsync(Func<UniTask>[] onPlayTaskArray,CancellationToken token)
	{
		if(onPlayTaskArray == null)
		{
			return;
		}

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

	/// <summary>
	/// Repeats the given UniTask delegate for the specified count.
	/// A count of -1 repeats indefinitely until cancelled.
	/// </summary>
	public static async UniTask LoopUniTaskAsync(Func<UniTask> onPlayTask,int count,CancellationToken token)
	{
		await _LoopPlayAsync(onPlayTask,count,token);
	}

	/// <summary>
	/// Invokes an action and waits for the given seconds, repeating for the specified count.
	/// </summary>
	public static async UniTask LoopActionAndWaitForSecondAsync(Action onAction,float second,bool ignoreTimeScale,int count,CancellationToken token)
	{
		async UniTask _PlayAndDelayAsync()
		{
			onAction?.Invoke();

			await UniTask.Delay(TimeSpan.FromSeconds(second),ignoreTimeScale,cancellationToken : token).SuppressCancellationThrow();
		}

		await _LoopPlayAsync(_PlayAndDelayAsync,count,token);
	}

	/// <summary>
	/// Invokes an action and waits one frame, repeating for the specified count.
	/// </summary>
	public static async UniTask LoopActionAndWaitForFrameAsync(Action onAction,int count,CancellationToken token)
	{
		async UniTask _PlayAndWaitAsync()
		{
			onAction?.Invoke();

			await UniTask.Yield(token).SuppressCancellationThrow();
		}

		await _LoopPlayAsync(_PlayAndWaitAsync,count,token);
	}

	/// <summary>
	/// Invokes an update callback with delta time each frame, repeating for the specified count.
	/// </summary>
	public static async UniTask LoopUpdateAndWaitForFrameAsync(Action<float> onUpdate,bool ignoreTimescale,int count,CancellationToken token)
	{
		async UniTask _PlayAndWaitAsync()
		{
			var deltaTime = _GetDeltaTime(ignoreTimescale);

			onUpdate?.Invoke(deltaTime);

			await UniTask.Yield(token).SuppressCancellationThrow();
		}

		await _LoopPlayAsync(_PlayAndWaitAsync,count,token);
	}

	/// <summary>
	/// Repeats the delegate until count reaches zero, or indefinitely when count is negative.
	/// </summary>
	private static async UniTask _LoopPlayAsync(Func<UniTask> onPlayTask,int count,CancellationToken token)
	{
		if(count == 0)
		{
			return;
		}

		var totalCount = count;

		while(totalCount < 0 || totalCount-- > 0)
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
	/// Waits until either the duration elapses or the condition becomes true, whichever comes first.
	/// </summary>
	public static async UniTask WaitForSecondsOrConditionAsync(float duration,Func<bool> onCondition,bool ignoreTimescale,CancellationToken token)
	{
		if(onCondition == null)
		{
			await UniTask.Delay(TimeSpan.FromSeconds(duration),ignoreTimescale,cancellationToken : token).SuppressCancellationThrow();

			return;
		}

		await UniTask.WhenAny(UniTask.Delay(TimeSpan.FromSeconds(duration),ignoreTimescale,cancellationToken : token),UniTask.WaitUntil(onCondition,cancellationToken : token)).SuppressCancellationThrow();
	}

	/// <summary>
	/// Waits until the condition becomes true, invoking onUpdate each frame with elapsed time.
	/// </summary>
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

	/// <summary>
	/// Interpolates from start to finish over duration using the given animation curve, invoking onProgress each frame.
	/// </summary>
	public static async UniTask ExecuteProgressAsync(float start,float finish,float duration,Action<float> onProgress,bool ignoreTimescale,AnimationCurve animationCurve,CancellationToken token)
	{
		if(duration <= 0.0f)
		{
			return;
		}

		var curve = animationCurve ?? KZMathKit.GetEaseCurve(EaseType.Linear);
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

	/// <summary>
	/// Invokes onUpdate at fixed period intervals until the total duration elapses.
	/// </summary>
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

	/// <summary>
	/// Awaits an Addressables handle, throws on failure, releases the handle, and returns the result.
	/// </summary>
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
