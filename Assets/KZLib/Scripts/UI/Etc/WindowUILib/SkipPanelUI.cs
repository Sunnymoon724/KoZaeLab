using System;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using KZLib;

public class SkipPanelUI : WindowUI2D
{
	public record SkipParam(Action OnClicked);

	public override UITag Tag => UITag.SkipPanelUI;

	[SerializeField,LabelText("Skip Show Duration"),ValidateInput(nameof(IsValidShowDuration),"0 is not defined.",InfoMessageType.Error),PropertyTooltip("Negative numbers represent infinity, and zero doesn't work.")]
	private float m_skipShowDuration = 0.0f;
	[SerializeField,LabelText("Skip Hide Duration"),MinValue(0.02f)]
	private float m_skipHideDuration = 0.0f;

	[SerializeField,LabelText("Click Sound")]
	private AudioClip m_clickSoundClip = null;

	[SerializeField,LabelText("Trigger Button")]
	private Button m_triggerButton = null;

	[SerializeField,LabelText("Skip Button")]
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
					SoundMgr.In.PlayUIShot(m_clickSoundClip);
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
			SetButtonsState(isSkipActive : true,isTriggerActive : false);

			return;
		}

		SetButtonsState(isSkipActive : false,isTriggerActive : false);
		ShowSkipButton(m_skipShowDuration);

		m_triggerButton.onClick.AddAction(()=> { ShowSkipButton(0.0f); } );
	}

	private void SetButtonsState(bool isSkipActive,bool isTriggerActive)
	{
		m_skipButton.gameObject.SetActiveIfDifferent(isSkipActive);
		m_triggerButton.gameObject.SetActiveIfDifferent(isTriggerActive);
	}

	private void ShowSkipButton(float delayTime)
	{
		CommonUtility.DelayAction(()=>
		{
			SetButtonsState(isSkipActive : true,isTriggerActive : false);

			CommonUtility.DelayAction(HideSkipButton,m_skipHideDuration);
		},delayTime);
	}

	private void HideSkipButton()
	{
		SetButtonsState(isSkipActive : false,isTriggerActive : true);
	}
}