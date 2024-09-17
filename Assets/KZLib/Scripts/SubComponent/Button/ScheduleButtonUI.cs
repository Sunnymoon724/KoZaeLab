using System.Threading;
using Cysharp.Threading.Tasks;
using KZLib;
using KZLib.KZSchedule;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class ScheduleButtonUI : BaseButtonUI
{
	[SerializeField,LabelText("클릭 전 스케쥴")]
	private Schedule m_BeforeSchedule = null;

	[SerializeField,LabelText("클릭 이벤트")]
	private UnityEvent m_OnClickedEvent = null;

	[SerializeField,LabelText("클릭 후 스케쥴")]
	private Schedule m_AfterSchedule = null;

	[SerializeField,LabelText("사용 후 리셋")]
	private bool m_UseReset = false;

	private CancellationTokenSource m_TokenSource = null;

	public MoreAction OnClicked { get; set; }

	protected override void Initialize()
	{
		m_OnClickedEvent.AddListener(()=> { OnClicked?.Invoke(); });

		base.Initialize();
	}

	protected override void Release()
	{
		base.Release();

		UniTaskUtility.KillTokenSource(ref m_TokenSource);
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		UniTaskUtility.KillTokenSource(ref m_TokenSource);
	}

	protected override void OnClickedButton()
	{
		UniTaskUtility.RecycleTokenSource(ref m_TokenSource);

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