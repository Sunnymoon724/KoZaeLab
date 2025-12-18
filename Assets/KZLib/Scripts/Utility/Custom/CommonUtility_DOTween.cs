using System;
using DG.Tweening;

// https://dotween.demigiant.com/
public static partial class CommonUtility
{
	public static Tween SetTweenProgress(float start,float finish,float duration,Action<float> onProgress,Action onComplete = null)
	{
		return _SetTween(start,finish,duration,onProgress,onComplete).SetId(_GetTag("Progress"));
	}

	private static Tween _SetTween(float start,float finish,float duration,Action<float> onUpdate,Action onComplete = null)
	{
		if(onUpdate == null)
		{
			throw new NullReferenceException("Tween update is null.");
		}

		float _Initialize()
		{
			return start;
		}

		void _Update(float x)
		{
			onUpdate(x);
		}

		var tween = DOTween.To(_Initialize,_Update,finish,duration);

		if(onComplete != null)
		{
			void _Complete()
			{
				onComplete();
			}

			tween.OnComplete(_Complete);
		}

		return tween;
	}

	/// <summary>
	/// Play Delay
	/// </summary>
	public static Tween PlayTimer(float duration,int count,TweenCallback onComplete)
	{
		if(count == 0)
		{
			return null;
		}

		return DOVirtual.DelayedCall(duration,onComplete).SetLoops(count).SetId(_GetTag("Timer"));
	}

	public static bool IsTweenPlaying(Tween tween)
	{
		return tween != null && tween.IsPlaying();
	}

	private static string _GetTag(string prefix)
	{
		return $"{prefix}_{DateTime.Now:mm:ss}";
	}

	public static void KillTween(Tween tween,bool complete = false)
	{
		if(tween != null && tween.IsActive())
		{
			tween.Kill(complete);
		}
	}
}