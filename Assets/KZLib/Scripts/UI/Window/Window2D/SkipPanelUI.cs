using System;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using KZLib;
using Cysharp.Threading.Tasks;

public class SkipPanelUI : WindowUI2D
{
	public record SkipParam(Action OnClicked);

	public override UITag Tag => UITag.SkipPanelUI;

	[SerializeField,LabelText("스킵 보이는 시간"),ValidateInput("IsExist","0초는 설정할 수 없습니다.",InfoMessageType.Error),PropertyTooltip("음수는 무한/0은 작동 안함")]
	private float m_SkipShowDuration = 0.0f;
	[SerializeField,LabelText("스킵 숨기는 시간"),MinValue(0.02f)]
	private float m_SkipHideDuration = 0.0f;

	[SerializeField,LabelText("클릭 시 사운드")]
	private AudioClip m_AudioClip = null;

	[SerializeField,LabelText("스킵 보이게 하는 버튼")]
	private Button m_TriggerButton = null;

	[SerializeField,LabelText("스킵 버튼")]
	private Button m_SkipButton = null;

	private bool IsExist => m_SkipShowDuration != 0.0f;

	public override void Open(object _param)
	{
		base.Open(_param);

		if(_param is SkipParam param)
		{
			m_SkipButton.SetOnClickListener(()=>
			{
				param.OnClicked?.Invoke();

				if(!m_AudioClip)
				{
					return;
				}

				SoundMgr.In.PlayEffect(m_AudioClip,true);
			});
		}

		// ShowTime이 0초 이므로 작동 X
		if(!IsExist || m_SkipHideDuration <= 0.02f)
		{
			SelfClose();

			return;
		}

		// 스킵 버튼 계속 보임
		if(m_SkipShowDuration < 0.0f)
		{
			SetActiveButtons(true,false);

			return;
		}

		SetActiveButtons(false,false);

		ShowExitButtonAsync(m_SkipShowDuration).Forget();

		m_TriggerButton.SetOnClickListener(()=>
		{
			ShowExitButtonAsync(0.0f).Forget();
		});
	}

	private void SetActiveButtons(bool _exit,bool _show)
	{
		m_SkipButton.gameObject.SetActiveSelf(_exit);
		m_TriggerButton.gameObject.SetActiveSelf(_show);
	}

	private async UniTask ShowExitButtonAsync(float _waitTime)
	{
		if(_waitTime > 0.0f)
		{
			await UniTask.Delay(TimeSpan.FromSeconds(_waitTime),true);
		}

		SetActiveButtons(true,false);

		await UniTask.Delay(TimeSpan.FromSeconds(m_SkipHideDuration),true);

		SetActiveButtons(false,true);
	}
}