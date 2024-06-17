using System;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class ScheduleButtonUI : BaseButtonUI
{
	[SerializeField,LabelText("클릭 전 스케쥴")]
	private Schedule m_BeforeSchedule = null;
	[SerializeField,LabelText("클릭 후 스케쥴")]
	private Schedule m_AfterSchedule = null;

	[SerializeField,LabelText("사용 후 리셋")]
	private bool m_UseReset = false;

	private Action m_OnClicked = null;

	public event Action OnClicked
	{
		add { m_OnClicked -= value; m_OnClicked += value; }
		remove { m_OnClicked -= value; }
	}

	protected override void Awake()
	{
		base.Awake();

		m_Button.SetOnClickListener(()=>
		{
			ClickedAsync().Forget();
		});
	}

	private async UniTaskVoid ClickedAsync()
	{
		if(m_BeforeSchedule)
		{
			await m_BeforeSchedule.PlayScheduleAsync();
		}

		m_OnClicked?.Invoke();

		if(m_AfterSchedule)
		{
			await m_AfterSchedule.PlayScheduleAsync();
		}

		if(m_UseReset)
		{
			if(m_BeforeSchedule)
			{
				m_BeforeSchedule.ResetSchedule();
			}

			if(m_AfterSchedule)
			{
				m_AfterSchedule.ResetSchedule();
			}
		}
	}
}