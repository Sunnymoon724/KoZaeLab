using System;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using KZLib;

public class SkipPanelUI : WindowUI2D
{
	public record SkipParam(Action OnClicked);

	public override UITag Tag => UITag.SkipPanelUI;

	[SerializeField,ValidateInput(nameof(IsValidShowDuration),"0 is not defined.",InfoMessageType.Error),PropertyTooltip("Negative numbers represent infinity, and zero doesn't work.")]
	private float m_skipShowDuration = 0.0f;
	[SerializeField,MinValue(0.02f)]
	private float m_skipHideDuration = 0.0f;

	[SerializeField]
	private AudioClip m_clickSoundClip = null;

	[SerializeField]
	private Button m_triggerButton = null;

	[SerializeField]
	private Button m_skipButton = null;

	private bool IsValidShowDuration => m_skipShowDuration != 0.0f;

	public override void Open(object param)
	{
		base.Open(param);

		if(param is SkipParam skipParam)
		{
			m_skipButton.onClick.SetAction(()=>
			{
				skipParam.OnClicked?.Invoke();

				if(m_clickSoundClip)
				{
					SoundMgr.In.TryPlayUISound(m_clickSoundClip);
				}
			});
		}

		// ShowTime이 0초 이므로 작동 X
		if(!IsValidShowDuration || m_skipHideDuration <= 0.02f)
		{
			SelfClose();

			return;
		}

		// 스킵 버튼 계속 보임
		if(m_skipShowDuration < 0.0f)
		{
			_SetButtonsState(isSkipActive : true,isTriggerActive : false);

			return;
		}

		_SetButtonsState(isSkipActive : false,isTriggerActive : false);
		_ShowSkipButton(m_skipShowDuration);

		m_triggerButton.onClick.AddAction(()=> { _ShowSkipButton(0.0f); } );
	}

	private void _SetButtonsState(bool isSkipActive,bool isTriggerActive)
	{
		m_skipButton.gameObject.EnsureActive(isSkipActive);
		m_triggerButton.gameObject.EnsureActive(isTriggerActive);
	}

	private void _ShowSkipButton(float delayTime)
	{
		CommonUtility.DelayAction(()=>
		{
			_SetButtonsState(isSkipActive : true,isTriggerActive : false);

			CommonUtility.DelayAction(_HideSkipButton,m_skipHideDuration);
		},delayTime);
	}

	private void _HideSkipButton()
	{
		_SetButtonsState(isSkipActive : false,isTriggerActive : true);
	}
}