using System.Threading;
using Cysharp.Threading.Tasks;
using KZLib;
using KZLib.KZSchedule;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class ScheduleButtonUI : BaseButtonUI
{
	[SerializeField,LabelText("Click Before Schedule")]
	private Schedule m_BeforeSchedule = null;

	[SerializeField,LabelText("Click Event")]
	private UnityEvent m_OnClickedEvent = null;

	[SerializeField,LabelText("Click After Schedule")]
	private Schedule m_AfterSchedule = null;

	[SerializeField,LabelText("Reset After Clicked")]
	private bool m_UseReset = false;

	private CancellationTokenSource m_TokenSource = null;

	public NewAction onClicked = new();

	protected override void Initialize()
	{
		m_OnClickedEvent.AddAction(()=> { onClicked?.Invoke(); });

		base.Initialize();
	}

	protected override void Release()
	{
		base.Release();

		CommonUtility.KillTokenSource(ref m_TokenSource);
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		CommonUtility.KillTokenSource(ref m_TokenSource);
	}

	protected override void OnClickedButton()
	{
		CommonUtility.RecycleTokenSource(ref m_TokenSource);

		ClickedAsync().Forget();
	}

	private async UniTaskVoid ClickedAsync()
	{
		if(m_BeforeSchedule)
		{
			await m_BeforeSchedule.PlayScheduleAsync();
		}

		if(m_TokenSource.IsCancellationRequested)
		{
			return;
		}

		m_OnClickedEvent.Invoke();

		if(m_AfterSchedule)
		{
			await m_AfterSchedule.PlayScheduleAsync();
		}

		if(m_TokenSource.IsCancellationRequested)
		{
			return;
		}

		if(!m_UseReset)
		{
			return;
		}

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