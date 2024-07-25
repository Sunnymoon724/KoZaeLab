using System;
using DG.Tweening;

public static partial class CommonUtility
{
	public static Tween SetProgress(float _start,float _finish,float _duration,Action<float> _onProgress,Action _onComplete = null)
	{
		var tag = string.Format("Progress_{0}",DateTime.Now.ToString("HH:mm:ss"));

		return SetTween(tag,_start,_finish,_duration,_onProgress,_onComplete);
	}

	private static Tween SetTween(string _tag,float _start,float _finish,float _duration,Action<float> _onUpdate,Action _onComplete = null)
	{
		if(_onUpdate == null)
		{
			throw new ArgumentNullException("트윈에 업데이트가 없습니다.");
		}

		var tween = DOTween.To(()=>_start,x=>_onUpdate(x),_finish,_duration);

		tween.id = _tag;

		if(_onComplete != null)
		{
			tween.OnComplete(()=> { _onComplete(); });
		}

		return tween;
	}

	/// <summary>
	/// 일정 시간 뒤에 실행 (반복도 가능)
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
		}).SetLoops(_count).SetId(string.Format("Timer_{0}",DateTime.Now.ToString("HH:mm:ss")));
	}

	public static bool IsTweenPlaying(Tween _tween)
	{
		return _tween != null && _tween.IsPlaying();
	}
}