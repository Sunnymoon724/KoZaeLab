using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

#if UNITY_EDITOR

using UnityEditor.Animations;

#endif

public static class AnimatorExtension
{
	public static bool IsState(this Animator _animator,string _animationName,int _layerIndex = 0)
	{
		return IsState(_animator,Animator.StringToHash(_animationName),_layerIndex);
	}

	public static bool IsState(this Animator _animator,int _animationHashName,int _layerIndex = 0)
	{
		return _animator.GetCurrentAnimatorStateInfo(_layerIndex).shortNameHash ==_animationHashName;
	}

	/// <summary>
	/// 현재 애니메이션을 특정 프레임의 시점으로 세팅 ( 속도를 0으로 넣으면 고정 ) 0번프레임으로 고정시킬때 주로 사용.
	/// </summary>
	public static void SetAnimationStopAtFrame(this Animator _animator,string _animationName,float _normalizedTime,int _layerIndex = 0,float _speed = 1.0f)
	{
		SetAnimationStopAtFrame(_animator,Animator.StringToHash(_animationName),_normalizedTime,_layerIndex,_speed);
	}

	/// <summary>
	/// 현재 애니메이션을 특정 프레임의 시점으로 세팅 ( 속도를 0으로 넣으면 고정 ) 0번프레임으로 고정시킬때 주로 사용.
	/// </summary>
	public static void SetAnimationStopAtFrame(this Animator _animator,int _animationHashName,float _normalizedTime,int _layerIndex = 0,float _speed = 1.0f)
	{
		_animator.speed = _speed;
		_animator.Play(_animationHashName,_layerIndex,_normalizedTime);
	}

	public static AnimatorStateInfo GetCurrentState(this Animator _animator,int _layerIndex = 0)
	{
		return _animator.GetNextAnimatorStateInfo(_layerIndex).shortNameHash == 0 ? _animator.GetCurrentAnimatorStateInfo(_layerIndex) : _animator.GetNextAnimatorStateInfo(_layerIndex);
	}

	public static async UniTask PlayAndWaitAsync(this Animator _animator,string _animationName,int _layerIndex = 0,CancellationToken _token = default)
	{
		await PlayAndWaitAsync(_animator,Animator.StringToHash(_animationName),_layerIndex,_token);
	}

	public static async UniTask PlayAndWaitAsync(this Animator _animator,int _animationHashName,int _layerIndex = 0,CancellationToken _token = default)
	{
		_animator.Play(_animationHashName,_layerIndex);

		await WaitForAnimationFinishAsync(_animator,_animationHashName,_layerIndex,_token);
	}

	public static async UniTask WaitForAnimationFinishAsync(this Animator _animator,string _animationName,int _layerIndex = 0,CancellationToken _token = default)
	{
		await WaitForAnimationFinishAsync(_animator,Animator.StringToHash(_animationName),_layerIndex,_token);
	}

	public static async UniTask WaitForAnimationFinishAsync(this Animator _animator,int _animationHashName,int _layerIndex = 0,CancellationToken _token = default)
	{
		await UniTask.WaitUntil(()=>IsAnimationFinish(_animator,_animationHashName,_layerIndex),cancellationToken : _token);
	}

	public static bool IsAnimationFinish(this Animator _animator,string _animationName,int _layerIndex = 0)
	{
		return IsAnimationFinish(_animator,Animator.StringToHash(_animationName),_layerIndex);
	}

	public static bool IsAnimationFinish(this Animator _animator,int _animationHashName,int _layerIndex = 0)
	{
		return _animator.GetCurrentAnimatorStateInfo(_layerIndex).shortNameHash == _animationHashName && _animator.GetCurrentAnimatorStateInfo(_layerIndex).normalizedTime >= 1.0f;
	}

	public static async UniTask WaitForAnimationStartAsync(this Animator _animator,string _animationName,int _layerIndex = 0,CancellationToken _token = default)
	{
		await WaitForAnimationStartAsync(_animator,Animator.StringToHash(_animationName),_layerIndex,_token);
	}

	public static async UniTask WaitForAnimationStartAsync(this Animator _animator,int _animationHashName,int _layerIndex = 0,CancellationToken _token = default)
	{
		await UniTask.WaitUntil(()=>IsAnimationStart(_animator,_animationHashName,_layerIndex),cancellationToken : _token);
	}

	public static bool IsAnimationStart(this Animator _animator,string _animationName,int _layerIndex = 0)
	{
		return IsAnimationStart(_animator,Animator.StringToHash(_animationName),_layerIndex);
	}

	public static bool IsAnimationStart(this Animator _animator,int _animationHashName,int _layerIndex = 0)
	{
		return _animator.GetCurrentAnimatorStateInfo(_layerIndex).shortNameHash == _animationHashName && _animator.GetCurrentAnimatorStateInfo(_layerIndex).normalizedTime <= 0.0f;
	}

	public static float GetAnimationClipLength(this Animator _animator,string _animationName)
	{
		var controller = _animator.runtimeAnimatorController;

		if(controller != null)
		{
			foreach(var clip in controller.animationClips)
			{
				if(_animationName.IsEqual(clip.name))
				{
					return clip.length;
				}
			}
		}

		return -1.0f;
	}

#if UNITY_EDITOR
	public static AnimatorController GetAnimatorController(this Animator _animator,out bool _isOverride)
	{
		if(_animator.runtimeAnimatorController is AnimatorController controller)
		{
			_isOverride = false;

			return controller;
		}

		_isOverride = true;

		var controller2 = _animator.runtimeAnimatorController as AnimatorOverrideController;

		return controller2.runtimeAnimatorController as AnimatorController;
	}
#endif
}