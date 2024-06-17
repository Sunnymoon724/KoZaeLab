using Cysharp.Threading.Tasks;
using UnityEngine;

public class ScheduleEffectClip : EffectClip
{
	[SerializeField]
	private Schedule m_Schedule = null;

	protected async override UniTask PlayTaskAsync()
	{
		m_Schedule.PlaySchedule();

		await base.PlayTaskAsync();
	}
}