using Cysharp.Threading.Tasks;
using KZLib;
using KZLib.KZSchedule;
using UnityEngine;

public class SchedulePanelUI : WindowUI2D
{
	[SerializeField,HideInInspector]
	private UITag m_Tag = null;

	[SerializeField]
	protected Schedule m_Schedule = null;

    public override UITag Tag => m_Tag ??= Enumeration.Parse<UITag>(gameObject.name);

	public override void Open(object _param)
	{
		base.Open(_param);

		m_Schedule.ResetSchedule();
	}

	public async UniTask PlayScheduleAsync()
	{
		await m_Schedule.PlayScheduleAsync();

		SelfClose();
	}
}