using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

#if UNITY_EDITOR

using UnityEditor.Animations;

#endif

public static class AnimatorExtension
{
	public static bool IsState(this Animator animator,string animationName,int layerIndex = 0)
	{
		if(!_IsValid(animator))
		{
			return false;
		}

		return IsState(animator,Animator.StringToHash(animationName),layerIndex);
	}

	public static bool IsState(this Animator animator,int animationHashName,int layerIndex = 0)
	{
		if(!_IsValid(animator))
		{
			return false;
		}

		return animator.GetCurrentAnimatorStateInfo(layerIndex).shortNameHash == animationHashName;
	}

	/// <summary>
	/// Set the anim to a specific frame ( speed 0 to freeze ).
	/// </summary>
	public static void SetAnimationStopAtFrame(this Animator animator,string animationName,float normalizedTime,int layerIndex = 0,float speed = 1.0f)
	{
		if(!_IsValid(animator))
		{
			return;
		}

		SetAnimationStopAtFrame(animator,Animator.StringToHash(animationName),normalizedTime,layerIndex,speed);
	}

	/// <summary>
	/// Set the anim to a specific frame ( speed 0 to freeze ).
	/// </summary>
	public static void SetAnimationStopAtFrame(this Animator animator,int animationHashName,float normalizedTime,int layerIndex = 0,float speed = 1.0f)
	{
		if(!_IsValid(animator))
		{
			return;
		}

		animator.speed = speed;
		animator.Play(animationHashName,layerIndex,normalizedTime);
	}

	public static AnimatorStateInfo GetCurrentState(this Animator animator,int layerIndex = 0)
	{
		if(!_IsValid(animator))
		{
			return default;
		}

		return animator.GetNextAnimatorStateInfo(layerIndex).shortNameHash == 0 ? animator.GetCurrentAnimatorStateInfo(layerIndex) : animator.GetNextAnimatorStateInfo(layerIndex);
	}

	public static async UniTask PlayActionInAnimationAsync(this Animator animator,string animationName,int layerIndex,float exitTime,Action onChange,Action onPlay,Action onComplete)
	{
		await PlayActionInAnimationAsync(animator,Animator.StringToHash(animationName),layerIndex,exitTime,onChange,onPlay,onComplete);
	}

	public static async UniTask PlayActionInAnimationAsync(this Animator animator,int animationHashName,int layerIndex,float exitTime,Action onChange,Action onPlay,Action onComplete)
	{
		var stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);

		while(animator.GetCurrentAnimatorStateInfo(layerIndex).shortNameHash != animationHashName)
		{
			onChange?.Invoke();

			await UniTask.Yield();
		}

		while(animator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime < exitTime)
		{
			onPlay?.Invoke();

			await UniTask.Yield();
		}

		onComplete?.Invoke();
	}

	public static async UniTask PlayAndWaitAsync(this Animator animator,string animationName,int layerIndex = 0,CancellationToken cancellationToken = default)
	{
		if(!_IsValid(animator))
		{
			return;
		}

		await PlayAndWaitAsync(animator,Animator.StringToHash(animationName),layerIndex,cancellationToken);
	}

	public static async UniTask PlayAndWaitAsync(this Animator animator,int animationHashName,int layerIndex = 0,CancellationToken cancellationToken = default)
	{
		if(!_IsValid(animator))
		{
			return;
		}

		animator.Play(animationHashName,layerIndex);

		await WaitForAnimationFinishAsync(animator,animationHashName,layerIndex,cancellationToken);
	}

	public static async UniTask WaitForAnimationFinishAsync(this Animator animator,string animationName,int layerIndex = 0,CancellationToken cancellationToken = default)
	{
		if(!_IsValid(animator))
		{
			return;
		}

		await WaitForAnimationFinishAsync(animator,Animator.StringToHash(animationName),layerIndex,cancellationToken);
	}

	public static async UniTask WaitForAnimationFinishAsync(this Animator animator,int animationHashName,int layerIndex = 0,CancellationToken cancellationToken = default)
	{
		if(!_IsValid(animator))
		{
			return;
		}

		await UniTask.WaitUntil(()=>IsAnimationFinish(animator,animationHashName,layerIndex),cancellationToken : cancellationToken);
	}

	public static bool IsAnimationFinish(this Animator animator,string animationName,int layerIndex = 0)
	{
		if(!_IsValid(animator))
		{
			return false;
		}

		return IsAnimationFinish(animator,Animator.StringToHash(animationName),layerIndex);
	}

	public static bool IsAnimationFinish(this Animator animator,int animationHashName,int layerIndex = 0)
	{
		if(!_IsValid(animator))
		{
			return false;
		}

		var stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);

		return stateInfo.shortNameHash == animationHashName && stateInfo.normalizedTime >= 0.99f;
	}

	public static async UniTask WaitForAnimationStartAsync(this Animator animator,string animationName,int layerIndex = 0,CancellationToken cancellationToken = default)
	{
		if(!_IsValid(animator))
		{
			return;
		}

		await WaitForAnimationStartAsync(animator,Animator.StringToHash(animationName),layerIndex,cancellationToken);
	}

	public static async UniTask WaitForAnimationStartAsync(this Animator animator,int animationHashName,int layerIndex = 0,CancellationToken cancellationToken = default)
	{
		if(!_IsValid(animator))
		{
			return;
		}

		await UniTask.WaitUntil(()=>IsAnimationStart(animator,animationHashName,layerIndex),cancellationToken : cancellationToken);
	}

	public static bool IsAnimationStart(this Animator animator,string animationName,int layerIndex = 0)
	{
		if(!_IsValid(animator))
		{
			return false;
		}

		return IsAnimationStart(animator,Animator.StringToHash(animationName),layerIndex);
	}

	public static bool IsAnimationStart(this Animator animator,int animationHashName,int layerIndex = 0)
	{
		if(!_IsValid(animator))
		{
			return false;
		}

		var stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);

		return stateInfo.shortNameHash == animationHashName && stateInfo.normalizedTime <= 0.0f;
	}

	public static float FindAnimationClipLength(this Animator animator,string clipName)
	{
		if(!_IsValid(animator))
		{
			return -1.0f;
		}

		var controller = animator.runtimeAnimatorController;

		if(controller != null)
		{
			foreach(var clip in controller.animationClips)
			{
				if(clipName.IsEqual(clip.name))
				{
					return clip.length;
				}
			}
		}

		return -1.0f;
	}

#if UNITY_EDITOR
	public static AnimatorController GetAnimatorController(this Animator animator,out bool isOverride)
	{
		if(!_IsValid(animator))
		{
			isOverride = false;

			return null;
		}

		if(animator.runtimeAnimatorController is AnimatorController controller)
		{
			isOverride = false;

			return controller;
		}

		isOverride = true;

		var controller2 = animator.runtimeAnimatorController as AnimatorOverrideController;

		return controller2.runtimeAnimatorController as AnimatorController;
	}
#endif

	private static bool _IsValid(Animator animator)
	{
		if(!animator)
		{
			KZLogType.System.E("Animator is null");

			return false;
		}

		return true;
	}
}