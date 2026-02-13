using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.Development
{
	public class TaskSequenceGroup : TaskSequence
	{
		[SerializeField]
		private List<Sequence> m_sequenceList = new();

		[Serializable]
		private class Sequence
		{
			[SerializeField]
			private float m_delayTime = 0.0f;

			[SerializeField,ListDrawerSettings(ShowFoldout = false,DraggableItems = false)]
			private List<TaskSequence> m_sequenceList = new();

			public async UniTask PlaySequenceAsync(Param sequenceParam)
			{
				if(m_sequenceList.IsNullOrEmpty())
				{
					return;
				}

				if(m_delayTime > 0.0f)
				{
					await UniTask.Delay(TimeSpan.FromSeconds(m_delayTime)).SuppressCancellationThrow();
				}

				if(m_sequenceList.Count == 1)
				{
					await m_sequenceList[0].PlaySequenceAsync(sequenceParam);
				}
				else
				{
					var taskList = new List<UniTask>();

					for(var i=0;i<m_sequenceList.Count;i++)
					{
						var sequence = m_sequenceList[i];

						if(sequence == null)
						{
							continue;
						}

						taskList.Add(sequence.PlaySequenceAsync(sequenceParam));
					}

					await UniTask.WhenAll(taskList).SuppressCancellationThrow();
				}
			}

			public void ResetSequence()
			{
				for(var i=0;i<m_sequenceList.Count;i++)
				{
					m_sequenceList[i].ResetSequence();
				}
			}
		}

		protected async override UniTask _DoPlaySequenceAsync(Param sequenceParam)
		{
			for(var i=0;i<m_sequenceList.Count;i++)
			{
				await m_sequenceList[i].PlaySequenceAsync(sequenceParam);
			}
		}

		public override void ResetSequence()
		{
			base.ResetSequence();

			for(var i=0;i<m_sequenceList.Count;i++)
			{
				m_sequenceList[i].ResetSequence();
			}
		}
	}
}