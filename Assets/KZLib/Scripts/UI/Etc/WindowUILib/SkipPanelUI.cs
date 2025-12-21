using System;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using KZLib;

public class SkipPanelUI : WindowUI2D
{
	public record Param(Action OnClicked);

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
	
	private Action m_onClickedTrigger = null;

	public override void Open(object param)
	{
		base.Open(param);

		if(param is Param skipParam)
		{
			m_onClickedTrigger = skipParam.OnClicked;
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

		m_triggerButton.onClick.AddAction(_OnClickedTrigger);
		m_skipButton.onClick.AddAction(_OnClickedSkip);
	}

	public override void Close()
	{
		m_triggerButton.onClick.RemoveAction(_OnClickedTrigger);
		m_skipButton.onClick.RemoveAction(_OnClickedSkip);

		base.Close();
	}

	private void _SetButtonsState(bool isSkipActive,bool isTriggerActive)
	{
		m_skipButton.gameObject.EnsureActive(isSkipActive);
		m_triggerButton.gameObject.EnsureActive(isTriggerActive);
	}

	private void _OnClickedTrigger()
	{
		_ShowSkipButton(0.0f);
	}

	private void _OnClickedSkip()
	{
		m_onClickedTrigger?.Invoke();

		if(m_clickSoundClip)
		{
			SoundManager.In.PlaySFX(m_clickSoundClip);
		}
	}

	private void _ShowSkipButton(float delayTime)
	{
		void _PlayAction()
		{
			_SetButtonsState(isSkipActive : true,isTriggerActive : false);

			CommonUtility.DelayAction(_HideSkipButton,m_skipHideDuration);
		}

		CommonUtility.DelayAction(_PlayAction,delayTime);
	}

	private void _HideSkipButton()
	{
		_SetButtonsState(isSkipActive : false,isTriggerActive : true);
	}
}