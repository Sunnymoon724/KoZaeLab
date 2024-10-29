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
	private string m_AnimationName = null;

	public record AnimatorEffectParam(string Name,Action<bool> OnComplete = null) : EffectParam(OnComplete);

	protected override bool IsEnableDuration => false;

	[VerticalGroup("General/2",Order = 2),SerializeField]
	private Animator m_Animator = null;

	[VerticalGroup("General/3",Order = 3),ShowInInspector,ValueDropdown(nameof(AnimatorNameList))]
	private string AnimationName
	{
		get => m_AnimationName;
		set
		{
			if(value.IsEmpty())
			{
				m_AnimationName = null;

				return;
			}

			m_AnimationName = value;

			Duration = m_Animator.GetAnimationClipLength(value);
		}
	}

	protected override void Reset()
	{
		base.Reset();

		if(!m_Animator)
		{
			m_Animator = GetComponent<Animator>();
		}

		AnimationName = null;

		m_IgnoreTimeScale = m_Animator.updateMode == AnimatorUpdateMode.UnscaledTime;
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

		m_Animator.Play(AnimationName);

		await UniTask.Yield();

		await UniTaskUtility.WaitForConditionAsync(()=>m_Animator.IsAnimationFinish(AnimationName),SetTime,m_IgnoreTimeScale,m_TokenSource.Token);
	}

	private List<string> AnimatorNameList
	{
		get
		{
			var nameList = new List<string>();

			if(!m_Animator)
			{
				return nameList;
			}

#if UNITY_EDITOR
			var controller = m_Animator.runtimeAnimatorController as AnimatorController;

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