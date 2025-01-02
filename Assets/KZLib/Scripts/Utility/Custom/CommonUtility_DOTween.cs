using System;
using DG.Tweening;

// https://dotween.demigiant.com/
public static partial class CommonUtility
{
	public static Tween SetTweenProgress(float start,float finish,float duration,Action<float> onProgress,Action onComplete = null)
	{
		return SetTween(start,finish,duration,onProgress,onComplete).SetId(GetTag("Progress"));
	}

	private static Tween SetTween(float start,float finish,float duration,Action<float> onUpdate,Action onComplete = null)
	{
		if(onUpdate == null)
		{
			throw new NullReferenceException("Tween update is null.");
		}

		var tween = DOTween.To(()=>start,x=>onUpdate(x),finish,duration);

		if(onComplete != null)
		{
			tween.OnComplete(()=>{ onComplete(); });
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

		return DOVirtual.DelayedCall(duration,onComplete).SetLoops(count).SetId(GetTag("Timer"));
	}

	public static bool IsTweenPlaying(Tween tween)
	{
		return tween != null && tween.IsPlaying();
	}

	private static string GetTag(string prefix)
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