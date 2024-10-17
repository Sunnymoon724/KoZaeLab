using System;
using DG.Tweening;

public static class DOTweenUtility
{
	public static Tween SetProgress(float _start,float _finish,float _duration,Action<float> _onProgress,Action _onComplete = null)
	{
		return SetTween(_start,_finish,_duration,_onProgress,_onComplete).SetId(GetTag("Progress"));
	}

	private static Tween SetTween(float _start,float _finish,float _duration,Action<float> _onUpdate,Action _onComplete = null)
	{
		if(_onUpdate == null)
		{
			throw new ArgumentNullException("Update is null.");
		}

		var tween = DOTween.To(()=>_start,x=>_onUpdate(x),_finish,_duration);

		if(_onComplete != null)
		{
			tween.OnComplete(()=> { _onComplete(); });
		}

		return tween;
	}

	/// <summary>
	/// Play Delay
	/// </summary>
	public static Tween PlayTimer(float _duration,int _count,Action _onComplete)
	{
		if(_count == 0)
		{
			return null;
		}

		return DOVirtual.DelayedCall(_duration,()=>
		{
			_onComplete();
		}).SetLoops(_count).SetId(GetTag("Timer"));
	}

	public static bool IsTweenPlaying(Tween _tween)
	{
		return _tween != null && _tween.IsPlaying();
	}

	private static string GetTag(string _header)
	{
		return $"{_header}_{DateTime.Now:HH:mm:ss}";
	}
}