using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class ScheduleGroup : Schedule
{
	[SerializeField,LabelText("스케쥴 그룹 리스트")]
	private List<ScheduleData> m_ScheduleList = new();

	[Serializable]
	private class ScheduleData
	{
		[SerializeField,LabelText("딜레이 시간")]
		private float m_DelayTime = 0.0f;

		[SerializeField,LabelText("스케쥴 리스트"),ListDrawerSettings(ShowFoldout = false,DraggableItems = false)]
		private List<Schedule> m_ScheduleList = new();

		public async UniTask PlayScheduleAsync(ScheduleParam _param)
		{
			if(m_ScheduleList.IsNullOrEmpty())
			{
				return;
			}

			if(m_DelayTime > 0.0f)
			{
				await UniTask.WaitForSeconds(m_DelayTime);
			}

			if(m_ScheduleList.Count == 1)
			{
				await m_ScheduleList[0].PlayScheduleAsync(_param);
			}
			else
			{
				var taskList = new List<UniTask>();

				for(var i=0;i<m_ScheduleList.Count;i++)
				{
					var schedule = m_ScheduleList[i];

					if(schedule == null)
					{
						continue;
					}

					taskList.Add(schedule.PlayScheduleAsync(_param));
				}

				await UniTask.WhenAll(taskList);
			}
		}

		public void ResetSchedule()
		{
			for(var i=0;i<m_ScheduleList.Count;i++)
			{
				m_ScheduleList[i].ResetSchedule();
			}
		}
	}

	protected async override UniTask DoPlayScheduleAsync(ScheduleParam _param)
	{
		for(var i=0;i<m_ScheduleList.Count;i++)
		{
			await m_ScheduleList[i].PlayScheduleAsync(_param);
		}
	}

	public override void ResetSchedule()
	{
		base.ResetSchedule();

		for(var i=0;i<m_ScheduleList.Count;i++)
		{
			m_ScheduleList[i].ResetSchedule();
		}
	}
}