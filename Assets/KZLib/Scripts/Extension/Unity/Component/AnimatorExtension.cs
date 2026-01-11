using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Generic;

#if UNITY_EDITOR

using UnityEditor.Animations;

#endif

public static class AnimatorExtension
{
	public static bool IsState(this Animator animator,string animationName,int layer = 0)
	{
		if(!_IsValid(animator))
		{
			return false;
		}

		return IsState(animator,Animator.StringToHash(animationName),layer);
	}

	public static bool IsState(this Animator animator,int animationHashName,int layer = 0)
	{
		if(!_IsValid(animator))
		{
			return false;
		}

		return animator.GetCurrentAnimatorStateInfo(layer).shortNameHash == animationHashName;
	}

	/// <summary>
	/// Set the anim to a specific frame ( speed 0 to freeze ).
	/// </summary>
	public static void SetAnimationStopAtFrame(this Animator animator,string animationName,float normalizedTime,int layer = 0,float speed = 1.0f)
	{
		if(!_IsValid(animator))
		{
			return;
		}

		SetAnimationStopAtFrame(animator,Animator.StringToHash(animationName),normalizedTime,layer,speed);
	}

	/// <summary>
	/// Set the anim to a specific frame ( speed 0 to freeze ).
	/// </summary>
	public static void SetAnimationStopAtFrame(this Animator animator,int animationHashName,float normalizedTime,int layer = 0,float speed = 1.0f)
	{
		if(!_IsValid(animator))
		{
			return;
		}

		animator.speed = speed;
		animator.Play(animationHashName,layer,normalizedTime);
	}

	public static AnimatorStateInfo GetCurrentState(this Animator animator,int layer = 0)
	{
		if(!_IsValid(animator))
		{
			return default;
		}

		return animator.GetNextAnimatorStateInfo(layer).shortNameHash == 0 ? animator.GetCurrentAnimatorStateInfo(layer) : animator.GetNextAnimatorStateInfo(layer);
	}

	public static async UniTask PlayActionInAnimationAsync(this Animator animator,string animationName,int layer,float exitTime,Action onChange,Action onPlay,Action onComplete)
	{
		await PlayActionInAnimationAsync(animator,Animator.StringToHash(animationName),layer,exitTime,onChange,onPlay,onComplete);
	}

	public static async UniTask PlayActionInAnimationAsync(this Animator animator,int animationHashName,int layer,float exitTime,Action onChange,Action onPlay,Action onComplete)
	{
		var stateInfo = animator.GetCurrentAnimatorStateInfo(layer);

		while(animator.GetCurrentAnimatorStateInfo(layer).shortNameHash != animationHashName)
		{
			onChange?.Invoke();

			await UniTask.Yield();
		}

		while(animator.GetCurrentAnimatorStateInfo(layer).normalizedTime < exitTime)
		{
			onPlay?.Invoke();

			await UniTask.Yield();
		}

		onComplete?.Invoke();
	}

	public static async UniTask PlayAndWaitAsync(this Animator animator,string animationName,int layer = 0,CancellationToken token = default)
	{
		if(!_IsValid(animator))
		{
			return;
		}

		await PlayAndWaitAsync(animator,Animator.StringToHash(animationName),layer,token);
	}

	public static async UniTask PlayAndWaitAsync(this Animator animator,int animationHashName,int layer = 0,CancellationToken token = default)
	{
		if(!_IsValid(animator))
		{
			return;
		}

		animator.Play(animationHashName,layer);

		await WaitForAnimationFinishAsync(animator,animationHashName,layer,token);
	}

	public static async UniTask WaitForAnimationFinishAsync(this Animator animator,string animationName,int layer = 0,CancellationToken token = default)
	{
		if(!_IsValid(animator))
		{
			return;
		}

		await WaitForAnimationFinishAsync(animator,Animator.StringToHash(animationName),layer,token);
	}

	public static async UniTask WaitForAnimationFinishAsync(this Animator animator,int animationHashName,int layer = 0,CancellationToken token = default)
	{
		if(!_IsValid(animator))
		{
			return;
		}

		bool _IsAnimationFinished()
		{
			return IsAnimationFinished(animator,animationHashName,layer);
		}

		await UniTask.WaitUntil(_IsAnimationFinished,cancellationToken : token).SuppressCancellationThrow();
	}

	public static bool IsAnimationFinished(this Animator animator,string animationName,int layer = 0)
	{
		if(!_IsValid(animator))
		{
			return false;
		}

		return IsAnimationFinished(animator,Animator.StringToHash(animationName),layer);
	}

	public static bool IsAnimationFinished(this Animator animator,int animationHashName,int layer = 0)
	{
		if(!_IsValid(animator))
		{
			return false;
		}

		var stateInfo = animator.GetCurrentAnimatorStateInfo(layer);

		return stateInfo.shortNameHash == animationHashName && stateInfo.normalizedTime >= 0.99f;
	}

	public static async UniTask WaitForAnimationStartAsync(this Animator animator,string animationName,int layer = 0,CancellationToken token = default)
	{
		if(!_IsValid(animator))
		{
			return;
		}

		await WaitForAnimationStartAsync(animator,Animator.StringToHash(animationName),layer,token);
	}

	public static async UniTask WaitForAnimationStartAsync(this Animator animator,int animationHashName,int layer = 0,CancellationToken token = default)
	{
		if(!_IsValid(animator))
		{
			return;
		}

		bool _HasAnimationStarted()
		{
			return HasAnimationStarted(animator,animationHashName,layer);
		}

		await UniTask.WaitUntil(_HasAnimationStarted,cancellationToken : token).SuppressCancellationThrow();
	}

	public static bool HasAnimationStarted(this Animator animator,string animationName,int layer = 0)
	{
		if(!_IsValid(animator))
		{
			return false;
		}

		return HasAnimationStarted(animator,Animator.StringToHash(animationName),layer);
	}

	public static bool HasAnimationStarted(this Animator animator,int animationHashName,int layer = 0)
	{
		if(!_IsValid(animator))
		{
			return false;
		}

		var stateInfo = animator.GetCurrentAnimatorStateInfo(layer);

		return stateInfo.shortNameHash == animationHashName && stateInfo.normalizedTime <= 0.0f;
	}

	public static AnimationClip[] GetAnimationClipArray(this Animator animator)
	{
		if(_IsValid(animator))
		{
			var controller = animator.runtimeAnimatorController;

			if(controller != null)
			{
				return controller.animationClips;
			}
		}

		return Array.Empty<AnimationClip>();
	}

	public static bool IsExistAnimationClip(this Animator animator,string clipName)
	{
		var clipArray = animator.GetAnimationClipArray();

		for(var i=0;i<clipArray.Length;i++)
		{
			var clip = clipArray[i];

			if(clipName.IsEqual(clip.name))
			{
				return true;
			}
		}

		return false;
	}

	public static float FindAnimationClipLength(this Animator animator,string clipName)
	{
		var clipArray = animator.GetAnimationClipArray();

		for(var i=0;i<clipArray.Length;i++)
		{
			var clip = clipArray[i];

			if(clipName.IsEqual(clip.name))
			{
				return clip.length;
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

	public static IEnumerable<string> FindStateNameGroup(this Animator animator)
	{
		var controller = animator.GetAnimatorController(out var _);

		if(_IsValid(animator))
		{
			for(var i=0;i<controller.layers.Length;i++)
			{
				var animatorStateArray = controller.layers[i].stateMachine.states;

				for(var j=0;j<animatorStateArray.Length;j++)
				{
					yield return animatorStateArray[j].state.name;
				}
			}
		}
	}
#endif

	private static bool _IsValid(Animator animator)
	{
		if(!animator)
		{
			LogChannel.System.E("Animator is null");

			return false;
		}

		return true;
	}
}