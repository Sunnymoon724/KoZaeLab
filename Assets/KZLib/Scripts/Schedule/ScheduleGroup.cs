using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.KZSchedule
{
	public class ScheduleGroup : Schedule
	{
		[SerializeField,LabelText("Schedule Group List")]
		private List<ScheduleData> m_ScheduleList = new();

		[Serializable]
		private class ScheduleData
		{
			[SerializeField,LabelText("Delay Time")]
			private float m_DelayTime = 0.0f;

			[SerializeField,LabelText("Schedule List"),ListDrawerSettings(ShowFoldout = false,DraggableItems = false)]
			private List<Schedule> m_ScheduleList = new();

			public async UniTask PlayScheduleAsync(ScheduleParam _param)
			{
				if(m_ScheduleList.IsNullOrEmpty())
				{
					return;
				}

				if(m_DelayTime > 0.0f)
				{
					await UniTask.Delay(TimeSpan.FromSeconds(m_DelayTime));
				}

				if(m_ScheduleList.Count == 1)
				{
					await m_ScheduleList[0].PlayScheduleAsync(_param);
				}
				else
				{
					var taskList = new List<UniTask>();

					foreach(var schedule in m_ScheduleList)
					{
						if(schedule != null)
						{
							taskList.Add(schedule.PlayScheduleAsync(_param));
						}
					}

					await UniTask.WhenAll(taskList);
				}
			}

			public void ResetSchedule()
			{
				foreach(var schedule in m_ScheduleList)
				{
					schedule.ResetSchedule();
				}
			}
		}

		protected async override UniTask DoPlayScheduleAsync(ScheduleParam _param)
		{
			foreach(var schedule in m_ScheduleList)
			{
				await schedule.PlayScheduleAsync(_param);
			}
		}

		public override void ResetSchedule()
		{
			base.ResetSchedule();

			foreach(var schedule in m_ScheduleList)
			{
				schedule.ResetSchedule();
			}
		}
	}
}