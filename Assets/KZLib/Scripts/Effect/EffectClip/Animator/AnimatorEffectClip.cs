using UnityEngine;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR

using UnityEditor.Animations;

#endif

[RequireComponent(typeof(Animator))]
public class AnimatorEffectClip : EffectClip
{
	[SerializeField,HideInInspector]
	private string m_animationName = null;

	public record AnimatorEffectParam(string Name,Action<bool> OnComplete = null) : EffectParam(OnComplete);

	protected override bool IsEnableDuration => false;

	[VerticalGroup("General/2",Order = 2),SerializeField]
	private Animator m_animator = null;

	[VerticalGroup("General/3",Order = 3),ShowInInspector,ValueDropdown(nameof(AnimatorNameList))]
	private string AnimationName
	{
		get => m_animationName;
		set
		{
			if(value.IsEmpty())
			{
				m_animationName = null;

				return;
			}

			m_animationName = value;

			Duration = m_animator.FindAnimationClipLength(value);
		}
	}

	protected override void Reset()
	{
		base.Reset();

		if(!m_animator)
		{
			m_animator = GetComponent<Animator>();
		}

		AnimationName = null;

		m_ignoreTimeScale = m_animator.updateMode == AnimatorUpdateMode.UnscaledTime;
	}

	public override void SetEffect(EffectParam _param)
	{
		base.SetEffect(_param);

		if(_param is not AnimatorEffectParam param)
		{
			return;
		}

		AnimationName = param.Name;
	}

	protected async override UniTask PlayTaskAsync()
	{
		if(AnimationName.IsEmpty())
		{
			return;
		}

		m_animator.Play(AnimationName);

		await UniTask.Yield();

		await CommonUtility.WaitForConditionAsync(()=>m_animator.IsAnimationFinish(AnimationName),SetTime,m_ignoreTimeScale,m_tokenSource.Token);
	}

	private List<string> AnimatorNameList
	{
		get
		{
			var nameList = new List<string>();

			if(!m_animator)
			{
				return nameList;
			}

#if UNITY_EDITOR
			var controller = m_animator.runtimeAnimatorController as AnimatorController;

			foreach(var layer in controller.layers)
			{
				foreach(var child in layer.stateMachine.states)
				{
					nameList.Add(child.state.name);
				}
			}
#endif
			return nameList;
		}
	}
}