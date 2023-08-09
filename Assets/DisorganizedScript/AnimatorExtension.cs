using System.Collections;
using UnityEngine;

public static class AnimatorExtension
{
	public static bool IsState(this Animator _animator,int _animationNameHash,int _layerIndex = 0)
	{
		return _animator.GetCurrentAnimatorStateInfo(_layerIndex).shortNameHash == _animationNameHash;
	}

	//? 현재 애니메이션을 특정 프레임의 시점으로 세팅 ( 속도를 0으로 넣으면 고정 ) 0번프레임으로 고정시킬때 주로 사용.
	public static void SetAnimationStopAtFrame(this Animator _animator,string _animationName,float _normalizedTime,int _layerIndex = 0,float _speed = 1.0f)
	{
		if(!_animator.enabled)
		{
			return;
		}

		_animator.speed = _speed;
		_animator.Play(_animationName,_layerIndex,_normalizedTime);
	}

	//? 현재 애니메이션을 특정 프레임의 시점으로 세팅 ( 속도를 0으로 넣으면 고정 ) 0번프레임으로 고정시킬때 주로 사용.
	public static void SetAnimationStopAtFrame(this Animator _animator,int _animationNameHash,float _normalizedTime,int _layerIndex = 0,float _speed = 1.0f)
	{
		if(!_animator.enabled)
		{
			return;
		}

		_animator.speed = _speed;
		_animator.Play(_animationNameHash,_layerIndex,_normalizedTime);
	}

	public static AnimatorStateInfo GetCurrentState(this Animator _animator,int _layerIndex = 0)
	{
		return _animator.GetNextAnimatorStateInfo(_layerIndex).shortNameHash == 0 ? _animator.GetCurrentAnimatorStateInfo(_layerIndex) : _animator.GetNextAnimatorStateInfo(_layerIndex);
	}
}