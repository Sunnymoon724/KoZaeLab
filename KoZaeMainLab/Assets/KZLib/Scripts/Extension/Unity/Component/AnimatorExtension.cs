using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

#if UNITY_EDITOR

using UnityEditor.Animations;

#endif

public static class AnimatorExtension
{
	public static bool IsState(this Animator _animator,string _animName,int _layerIdx = 0)
	{
		if(!_animator)
		{
			LogTag.System.E("Animator is null");

			return false;
		}

		return IsState(_animator,Animator.StringToHash(_animName),_layerIdx);
	}

	public static bool IsState(this Animator _animator,int _animHashName,int _layerIdx = 0)
	{
		if(!_animator)
		{
			LogTag.System.E("Animator is null");

			return false;
		}

		return _animator.GetCurrentAnimatorStateInfo(_layerIdx).shortNameHash ==_animHashName;
	}

	/// <summary>
	/// Set the anim to a specific frame ( speed 0 to freeze ).
	/// </summary>
	public static void SetAnimationStopAtFrame(this Animator _animator,string _animName,float _normalizedTime,int _layerIdx = 0,float _speed = 1.0f)
	{
		if(!_animator)
		{
			LogTag.System.E("Animator is null");

			return;
		}

		SetAnimationStopAtFrame(_animator,Animator.StringToHash(_animName),_normalizedTime,_layerIdx,_speed);
	}

	/// <summary>
	/// Set the anim to a specific frame ( speed 0 to freeze ).
	/// </summary>
	public static void SetAnimationStopAtFrame(this Animator _animator,int _animHashName,float _normalizedTime,int _layerIdx = 0,float _speed = 1.0f)
	{
		if(!_animator)
		{
			LogTag.System.E("Animator is null");

			return;
		}

		_animator.speed = _speed;
		_animator.Play(_animHashName,_layerIdx,_normalizedTime);
	}

	public static AnimatorStateInfo GetCurrentState(this Animator _animator,int _layerIdx = 0)
	{
		if(!_animator)
		{
			LogTag.System.E("Animator is null");

			return default;
		}

		return _animator.GetNextAnimatorStateInfo(_layerIdx).shortNameHash == 0 ? _animator.GetCurrentAnimatorStateInfo(_layerIdx) : _animator.GetNextAnimatorStateInfo(_layerIdx);
	}

	public static async UniTask PlayAndWaitAsync(this Animator _animator,string _animName,int _layerIdx = 0,CancellationToken _token = default)
	{
		if(!_animator)
		{
			LogTag.System.E("Animator is null");

			return;
		}

		await PlayAndWaitAsync(_animator,Animator.StringToHash(_animName),_layerIdx,_token);
	}

	public static async UniTask PlayAndWaitAsync(this Animator _animator,int _animHashName,int _layerIdx = 0,CancellationToken _token = default)
	{
		if(!_animator)
		{
			LogTag.System.E("Animator is null");

			return;
		}

		_animator.Play(_animHashName,_layerIdx);

		await WaitForAnimationFinishAsync(_animator,_animHashName,_layerIdx,_token);
	}

	public static async UniTask WaitForAnimationFinishAsync(this Animator _animator,string _animName,int _layerIdx = 0,CancellationToken _token = default)
	{
		if(!_animator)
		{
			LogTag.System.E("Animator is null");

			return;
		}

		await WaitForAnimationFinishAsync(_animator,Animator.StringToHash(_animName),_layerIdx,_token);
	}

	public static async UniTask WaitForAnimationFinishAsync(this Animator _animator,int _animHashName,int _layerIdx = 0,CancellationToken _token = default)
	{
		if(!_animator)
		{
			LogTag.System.E("Animator is null");

			return;
		}

		await UniTask.WaitUntil(()=>IsAnimationFinish(_animator,_animHashName,_layerIdx),cancellationToken : _token);
	}

	public static bool IsAnimationFinish(this Animator _animator,string _animName,int _layerIdx = 0)
	{
		if(!_animator)
		{
			LogTag.System.E("Animator is null");

			return false;
		}

		return IsAnimationFinish(_animator,Animator.StringToHash(_animName),_layerIdx);
	}

	public static bool IsAnimationFinish(this Animator _animator,int _animHashName,int _layerIdx = 0)
	{
		if(!_animator)
		{
			LogTag.System.E("Animator is null");

			return false;
		}

		return _animator.GetCurrentAnimatorStateInfo(_layerIdx).shortNameHash == _animHashName && _animator.GetCurrentAnimatorStateInfo(_layerIdx).normalizedTime >= 1.0f;
	}

	public static async UniTask WaitForAnimationStartAsync(this Animator _animator,string _animName,int _layerIdx = 0,CancellationToken _token = default)
	{
		if(!_animator)
		{
			LogTag.System.E("Animator is null");

			return;
		}

		await WaitForAnimationStartAsync(_animator,Animator.StringToHash(_animName),_layerIdx,_token);
	}

	public static async UniTask WaitForAnimationStartAsync(this Animator _animator,int _animHashName,int _layerIdx = 0,CancellationToken _token = default)
	{
		if(!_animator)
		{
			LogTag.System.E("Animator is null");

			return;
		}

		await UniTask.WaitUntil(()=>IsAnimationStart(_animator,_animHashName,_layerIdx),cancellationToken : _token);
	}

	public static bool IsAnimationStart(this Animator _animator,string _animName,int _layerIdx = 0)
	{
		if(!_animator)
		{
			LogTag.System.E("Animator is null");

			return false;
		}

		return IsAnimationStart(_animator,Animator.StringToHash(_animName),_layerIdx);
	}

	public static bool IsAnimationStart(this Animator _animator,int _animHashName,int _layerIdx = 0)
	{
		if(!_animator)
		{
			LogTag.System.E("Animator is null");

			return false;
		}

		return _animator.GetCurrentAnimatorStateInfo(_layerIdx).shortNameHash == _animHashName && _animator.GetCurrentAnimatorStateInfo(_layerIdx).normalizedTime <= 0.0f;
	}

	public static float GetAnimationClipLength(this Animator _animator,string _animName)
	{
		if(!_animator)
		{
			LogTag.System.E("Animator is null");

			return Global.INVALID_NUMBER;
		}

		var controller = _animator.runtimeAnimatorController;

		if(controller != null)
		{
			foreach(var clip in controller.animationClips)
			{
				if(_animName.IsEqual(clip.name))
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
		if(!_animator)
		{
			LogTag.System.E("Animator is null");

			_isOverride = false;

			return null;
		}

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