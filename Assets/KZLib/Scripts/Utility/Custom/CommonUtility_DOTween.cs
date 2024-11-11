using System;
using DG.Tweening;

// https://dotween.demigiant.com/
public static partial class CommonUtility
{
	public static Tween SetTweenProgress(float _start,float _finish,float _duration,Action<float> _onProgress,Action _onComplete = null)
	{
		return SetTween(_start,_finish,_duration,_onProgress,_onComplete);
	}

	private static Tween SetTween(float _start,float _finish,float _duration,Action<float> _onUpdate,Action _onComplete = null)
	{
		if(_onUpdate == null)
		{
			LogTag.System.E("Update is null.");

			return null;
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

	public static void KillTween(Tween _tween,bool _complete = false)
	{
		if(_tween != null && _tween.IsActive())
		{
			_tween.Kill(_complete);
		}
	}
}