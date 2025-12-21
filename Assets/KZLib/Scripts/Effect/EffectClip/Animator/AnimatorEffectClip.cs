using UnityEngine;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(Animator))]
public class AnimatorEffectClip : EffectClip
{
	[SerializeField,HideInInspector]
	private string m_animationName = null;

	public new record Param(string Name,Action<bool> OnComplete = null) : EffectClip.Param(OnComplete);

	protected override bool IsEnableDuration => false;

	[VerticalGroup("General/2",Order = 2),SerializeField]
	private Animator m_animator = null;

	[VerticalGroup("General/3",Order = 3),ShowInInspector,ValueDropdown(nameof(StateNameGroup))]
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

	public override void SetEffect(EffectClip.Param effectParam)
	{
		base.SetEffect(effectParam);

		if(effectParam is not Param param)
		{
			return;
		}

		AnimationName = param.Name;
	}

	protected async override UniTask _ExecuteEffectAsync()
	{
		if(AnimationName.IsEmpty())
		{
			return;
		}

		m_animator.Play(AnimationName);

		await UniTask.Yield();

		bool _WaitForAnimation()
		{
			return m_animator.IsAnimationFinished(AnimationName);
		}

		await CommonUtility.WaitForConditionAsync(_WaitForAnimation,SetTime,m_ignoreTimeScale,m_tokenSource.Token);
	}

#if UNITY_EDITOR
	private readonly List<string> m_stateNameList = new();

	private IEnumerable<string> StateNameGroup
	{
		get
		{
			if(m_animator && m_stateNameList.IsNullOrEmpty())
			{
				m_stateNameList.AddRange(m_animator.FindStateNameGroup());
			}

			return m_stateNameList;
		}
	}
#endif
}