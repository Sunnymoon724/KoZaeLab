using System;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using KZLib;

public class SkipPanelUI : WindowUI2D
{
	public record SkipParam(Action OnClicked);

	public override UITag Tag => UITag.SkipPanelUI;

	[SerializeField,LabelText("스킵 보이는 시간"),ValidateInput("IsValidShowDuration","0초는 설정할 수 없습니다.",InfoMessageType.Error),PropertyTooltip("음수는 무한/0은 작동 안함")]
	private float m_SkipShowDuration = 0.0f;
	[SerializeField,LabelText("스킵 숨기는 시간"),MinValue(0.02f)]
	private float m_SkipHideDuration = 0.0f;

	[SerializeField,LabelText("클릭 시 사운드")]
	private AudioClip m_AudioClip = null;

	[SerializeField,LabelText("스킵 보이게 하는 버튼")]
	private Button m_TriggerButton = null;

	[SerializeField,LabelText("스킵 버튼")]
	private Button m_SkipButton = null;

	private bool IsValidShowDuration => m_SkipShowDuration != 0.0f;

	public override void Open(object _param)
	{
		base.Open(_param);

		if(_param is SkipParam param)
		{
			m_SkipButton.SetListener(()=>
			{
				param.OnClicked?.Invoke();

				if(m_AudioClip)
				{
					SoundMgr.In.PlayUIShot(m_AudioClip);
				}
			});
		}

		// ShowTime이 0초 이므로 작동 X
		if(!IsValidShowDuration || m_SkipHideDuration <= 0.02f)
		{
			SelfClose();

			return;
		}

		// 스킵 버튼 계속 보임
		if(m_SkipShowDuration < 0.0f)
		{
			SetButtonsState(_skipActive : true,_triggerActive : false);

			return;
		}

		SetButtonsState(_skipActive : false,_triggerActive : false);
		ShowSkipButton(m_SkipShowDuration);

		m_TriggerButton.AddListener(()=> { ShowSkipButton(0.0f); } );
	}

	private void SetButtonsState(bool _skipActive,bool _triggerActive)
	{
		m_SkipButton.gameObject.SetActiveSelf(_skipActive);
		m_TriggerButton.gameObject.SetActiveSelf(_triggerActive);
	}

	private void ShowSkipButton(float _delay)
	{
		R3Utility.DelayAction(()=>
		{
			SetButtonsState(_skipActive : true,_triggerActive : false);

			R3Utility.DelayAction(HideSkipButton,m_SkipHideDuration);
		},_delay);
	}

	private void HideSkipButton()
	{
		SetButtonsState(_skipActive : false,_triggerActive : true);
	}
}