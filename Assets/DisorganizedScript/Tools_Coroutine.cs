using System;
using System.Collections;
using UnityEngine;

public static partial class Tools
{
	public static IEnumerator WaitForFrames(int _count,Action _onComplete)
	{
		if(_count <= 0)
		{
			_onComplete?.Invoke();

			yield break;
		}

		for(var i=0;i<_count;i++)
		{
			yield return null;
		}

		_onComplete?.Invoke();
	}

	public static IEnumerator RepeatForever(Action _onRepeat,float _delay)
	{
		yield return RepeatUntilInnerCoroutine(() => true,_onRepeat,null,_delay);
	}

	public static IEnumerator RepeatUntil(Func<bool> _onCondition,Action _onRepeat,Action _onComplete,float _delay)
	{
		yield return RepeatUntilInnerCoroutine(_onCondition,_onRepeat,_onComplete,_delay);
	}

	private static IEnumerator RepeatUntilInnerCoroutine(Func<bool> _onCondition,Action _onRepeat,Action _onComplete,float _delay)
	{
		while(_onCondition())
		{
			_onRepeat?.Invoke();

			if(_delay > 0.0f)
			{
				yield return YieldCache.WaitForSeconds(_delay);
			}
		}

		_onComplete?.Invoke();
	}
	
	public static IEnumerator WaitForSecondOrAction(float _waitTime,Func<bool> _onCondition)
	{
		var startTime = Time.timeSinceLevelLoad;

		while(true)
		{
			if((Time.timeSinceLevelLoad-startTime >= _waitTime) || _onCondition())
			{
				yield break;
			}

			yield return null;
		}
	}

	public static IEnumerator WaitForAnimationStart(Animator _animator,string _animationName,int _layerIndex = 0)
	{
		yield return WaitForAnimationStart(_animator,Animator.StringToHash(_animationName),_layerIndex);
	}

	public static IEnumerator WaitForAnimationStart(Animator _animator,int _animationNameHash,int _layerIndex = 0)
	{
		var stateInfo = _animator.GetCurrentState(_layerIndex);
		var isPlaying = stateInfo.shortNameHash.Equals(_animationNameHash);

		yield return WaitForAnimationInner(_animator,_animationNameHash,_layerIndex,()=> !isPlaying || !(stateInfo.normalizedTime > 0.0f));
	}

	public static IEnumerator WaitForAnimationToFinish(Animator _animator,string _animationName,int _layerIndex = 0)
	{
		yield return WaitForAnimationToFinish(_animator,Animator.StringToHash(_animationName),_layerIndex);
	}

	public static IEnumerator WaitForAnimationToFinish(Animator _animator,int _animationNameHash,int _layerIndex = 0)
	{
		var stateInfo = _animator.GetCurrentState(_layerIndex);
		var isPlaying = stateInfo.shortNameHash.Equals(_animationNameHash);

		yield return WaitForAnimationInner(_animator,_animationNameHash,_layerIndex,()=> !isPlaying || !(stateInfo.normalizedTime >= 1.0f));
	}

	private static IEnumerator WaitForAnimationInner(Animator _animator,int _nameHash,int _layerIndex,Func<bool> _onCondition)
	{
		var stateInfo = _animator.GetCurrentState(_layerIndex);
		var isPlaying = stateInfo.shortNameHash.Equals(_nameHash);

		yield return new WaitUntil( ()=> !isPlaying || _onCondition() );
	}

	public static IEnumerator WaitForParticleSystem(ParticleSystem _particleSystem)
	{
		_particleSystem.Play();

		yield return new WaitWhile(()=>_particleSystem.isPlaying);
	}
}