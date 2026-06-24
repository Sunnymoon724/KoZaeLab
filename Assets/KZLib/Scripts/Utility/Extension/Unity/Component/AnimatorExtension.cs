using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Generic;

#if UNITY_EDITOR

using UnityEditor.Animations;

#endif

/// <summary>
/// Extension methods for <see cref="Animator"/> state queries and async playback helpers.
/// </summary>
public static class AnimatorExtension
{
	/// <summary>
	/// Returns whether the animator is currently in the named state on the given layer.
	/// </summary>
	public static bool IsState(this Animator animator,string animationName,int layer = 0)
	{
		if(!_IsValid(animator))
		{
			return false;
		}

		return IsState(animator,Animator.StringToHash(animationName),layer);
	}

	/// <summary>
	/// Returns whether the animator is currently in the state identified by <paramref name="animationHashName"/> on the given layer.
	/// </summary>
	public static bool IsState(this Animator animator,int animationHashName,int layer = 0)
	{
		if(!_IsValid(animator))
		{
			return false;
		}

		return animator.GetCurrentAnimatorStateInfo(layer).shortNameHash == animationHashName;
	}

	/// <summary>
	/// Plays the animation at the given normalized time. Set <paramref name="speed"/> to 0 to freeze on that frame.
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
	/// Plays the animation at the given normalized time. Set <paramref name="speed"/> to 0 to freeze on that frame.
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

	/// <summary>
	/// Returns the current state, or the next state when a transition is in progress.
	/// </summary>
	public static AnimatorStateInfo GetCurrentState(this Animator animator,int layer = 0)
	{
		if(!_IsValid(animator))
		{
			return default;
		}

		return animator.GetNextAnimatorStateInfo(layer).shortNameHash == 0 ? animator.GetCurrentAnimatorStateInfo(layer) : animator.GetNextAnimatorStateInfo(layer);
	}

	/// <summary>
	/// Waits until the named state is active, invokes callbacks during playback, then fires on complete at exit time.
	/// </summary>
	public static async UniTask PlayActionInAnimationAsync(this Animator animator,string animationName,int layer,float exitTime,Action onChange,Action onPlay,Action onComplete)
	{
		await PlayActionInAnimationAsync(animator,Animator.StringToHash(animationName),layer,exitTime,onChange,onPlay,onComplete);
	}

	/// <summary>
	/// Waits until the state identified by <paramref name="animationHashName"/> is active, invokes callbacks during playback, then fires on complete at exit time.
	/// </summary>
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

	/// <summary>
	/// Plays the animation and awaits until it finishes or a transition begins.
	/// </summary>
	public static async UniTask PlayAndWaitAsync(this Animator animator,string animationName,int layer = 0,CancellationToken token = default)
	{
		if(!_IsValid(animator))
		{
			return;
		}

		await PlayAndWaitAsync(animator,Animator.StringToHash(animationName),layer,token);
	}

	/// <summary>
	/// Plays the animation identified by <paramref name="animationHashName"/> and awaits until it finishes or a transition begins.
	/// </summary>
	public static async UniTask PlayAndWaitAsync(this Animator animator,int animationHashName,int layer = 0,CancellationToken token = default)
	{
		if(!_IsValid(animator))
		{
			return;
		}

		animator.Play(animationHashName,layer);

		await WaitForAnimationFinishAsync(animator,animationHashName,layer,token);
	}

	/// <summary>
	/// Awaits until the named animation reaches its finish threshold or begins transitioning out.
	/// </summary>
	public static async UniTask WaitForAnimationFinishAsync(this Animator animator,string animationName,int layer = 0,CancellationToken token = default)
	{
		if(!_IsValid(animator))
		{
			return;
		}

		await WaitForAnimationFinishAsync(animator,Animator.StringToHash(animationName),layer,token);
	}

	/// <summary>
	/// Awaits until the animation identified by <paramref name="animationHashName"/> reaches its finish threshold or begins transitioning out.
	/// </summary>
	public static async UniTask WaitForAnimationFinishAsync(this Animator animator,int animationHashName,int layer = 0,CancellationToken token = default)
	{
		if(!_IsValid(animator))
		{
			return;
		}

		bool _IsAnimationFinished()
		{
			var stateInfo = animator.GetCurrentAnimatorStateInfo(layer);

			if(stateInfo.shortNameHash != animationHashName)
			{
				return false;
			}

			var normalizedTime = stateInfo.loop ? stateInfo.normalizedTime%1.0f : stateInfo.normalizedTime;

			if(animator.IsInTransition(layer))
			{
				return true;
			}

			return normalizedTime >= Global.AnimationFinishThreshold;
		}

		await UniTask.WaitUntil(_IsAnimationFinished,cancellationToken : token).SuppressCancellationThrow();

		while(animator && animator.IsInTransition(layer))
		{
			await UniTask.Yield(cancellationToken : token);
		}
	}

	/// <summary>
	/// Returns whether the named animation has reached its finish threshold on the given layer.
	/// </summary>
	public static bool IsAnimationFinished(this Animator animator,string animationName,int layer = 0)
	{
		if(!_IsValid(animator))
		{
			return false;
		}

		return IsAnimationFinished(animator,Animator.StringToHash(animationName),layer);
	}

	/// <summary>
	/// Returns whether the animation identified by <paramref name="animationHashName"/> has reached its finish threshold on the given layer.
	/// </summary>
	public static bool IsAnimationFinished(this Animator animator,int animationHashName,int layer = 0)
	{
		if(!_IsValid(animator))
		{
			return false;
		}

		var stateInfo = animator.GetCurrentAnimatorStateInfo(layer);

		if(stateInfo.shortNameHash != animationHashName)
		{
			return false;
		}

		var normalizedTime = stateInfo.loop ? stateInfo.normalizedTime%1.0f : stateInfo.normalizedTime;

		return normalizedTime >= Global.AnimationFinishThreshold;
	}

	/// <summary>
	/// Awaits until the named animation has started playing (normalized time greater than zero).
	/// </summary>
	public static async UniTask WaitForAnimationStartAsync(this Animator animator,string animationName,int layer = 0,CancellationToken token = default)
	{
		if(!_IsValid(animator))
		{
			return;
		}

		await WaitForAnimationStartAsync(animator,Animator.StringToHash(animationName),layer,token);
	}

	/// <summary>
	/// Awaits until the animation identified by <paramref name="animationHashName"/> has started playing (normalized time greater than zero).
	/// </summary>
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

	/// <summary>
	/// Returns whether the named animation has started playing on the given layer.
	/// </summary>
	public static bool HasAnimationStarted(this Animator animator,string animationName,int layer = 0)
	{
		if(!_IsValid(animator))
		{
			return false;
		}

		return HasAnimationStarted(animator,Animator.StringToHash(animationName),layer);
	}

	/// <summary>
	/// Returns whether the animation identified by <paramref name="animationHashName"/> has started playing on the given layer.
	/// </summary>
	public static bool HasAnimationStarted(this Animator animator,int animationHashName,int layer = 0)
	{
		if(!_IsValid(animator))
		{
			return false;
		}

		var stateInfo = animator.GetCurrentAnimatorStateInfo(layer);

		return stateInfo.shortNameHash == animationHashName && stateInfo.normalizedTime > 0.0f;
	}

	/// <summary>
	/// Returns all <see cref="AnimationClip"/> assets from the animator's runtime controller.
	/// </summary>
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

	/// <summary>
	/// Returns whether an animation clip named <paramref name="clipName"/> exists on the animator controller.
	/// </summary>
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

	/// <summary>
	/// Returns the length of the animation clip named <paramref name="clipName"/>, or <c>-1</c> when not found.
	/// </summary>
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
	/// <summary>
	/// Returns the motion clip length for the named animator state (layer 0). Use when state name differs from clip name.
	/// </summary>
	public static float FindAnimatorStateMotionLength(this Animator animator,string stateName,int layer = 0)
	{
		if(!_IsValid(animator) || stateName.IsEmpty())
		{
			return -1.0f;
		}

		var controller = animator.GetAnimatorController(out var _);

		if(!controller)
		{
			return -1.0f;
		}

		var layerArray = controller.layers;

		if(layerArray == null || layer < 0 || layer >= layerArray.Length)
		{
			return -1.0f;
		}

		var stateMachine = layerArray[layer].stateMachine;

		if(!stateMachine)
		{
			return -1.0f;
		}

		var animatorStateArray = stateMachine.states;

		if(animatorStateArray == null)
		{
			return -1.0f;
		}

		for(var i=0;i<animatorStateArray.Length;i++)
		{
			var state = animatorStateArray[i].state;

			if(!state || !state.name.IsEqual(stateName))
			{
				continue;
			}

			if(state.motion is AnimationClip clip)
			{
				return clip.length;
			}

			return -1.0f;
		}

		return -1.0f;
	}

	/// <summary>
	/// Resolves the underlying <see cref="AnimatorController"/>, including override controllers.
	/// </summary>
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

		if(!controller2)
		{
			return null;
		}

		return controller2.runtimeAnimatorController as AnimatorController;
	}

	/// <summary>
	/// Enumerates all state names defined in the animator controller across all layers.
	/// </summary>
	public static IEnumerable<string> FindStateNameGroup(this Animator animator)
	{
		if(!_IsValid(animator))
		{
			yield break;
		}

		var controller = animator.GetAnimatorController(out var _);

		if(!controller)
		{
			yield break;
		}

		var layerArray = controller.layers;

		if(layerArray == null)
		{
			yield break;
		}

		for(var i=0;i<layerArray.Length;i++)
		{
			var stateMachine = layerArray[i].stateMachine;

			if(!stateMachine)
			{
				continue;
			}

			var animatorStateArray = stateMachine.states;

			if(animatorStateArray == null)
			{
				continue;
			}

			for(var j=0;j<animatorStateArray.Length;j++)
			{
				var state = animatorStateArray[j].state;

				if(!state)
				{
					continue;
				}

				yield return state.name;
			}
		}
	}
#endif

	private static bool _IsValid(Animator animator)
	{
		if(!animator)
		{
			LogChannel.Kit.E("Animator is null");

			return false;
		}

		return true;
	}
}
