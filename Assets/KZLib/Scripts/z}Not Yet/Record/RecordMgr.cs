using System.Collections.Generic;
using UnityEngine;

namespace KZLib
{
	public interface IRecordEntity
	{
		void Record(float _time);
	}

	public class RecordMgr : AutoSingletonMB<RecordMgr>
	{
		private float m_DeltaTime = 0.0f;
		private bool m_Play = false;
		private float m_UpdatePeriod = 0.0f;

		private readonly List<IRecordEntity> m_EntityList = new();

		protected override void Release()
		{
			m_EntityList.Clear();
		}

		public void Add(IRecordEntity _entity)
		{
			m_EntityList.Add(_entity);
		}

		public void Remove(IRecordEntity _entity)
		{
			m_EntityList.Remove(_entity);
		}

		public void StartRecord(float _period = Global.FRAME_UPDATE_PERIOD)
		{
			m_Play = true;
			m_DeltaTime = 0.0f;
			m_UpdatePeriod = _period;
		}

		public void StopRecord()
		{
			m_Play = false;
		}

		private void Update()
		{
			if(!m_Play)
			{
				return;
			}

			m_DeltaTime += Time.deltaTime;

			if(m_DeltaTime > m_UpdatePeriod)
			{
				for(var i=0;i<m_EntityList.Count;i++)
				{
					m_EntityList[i].Record(m_DeltaTime);
				}

				m_DeltaTime = 0.0f;
			}
		}

		public void ClearRecord()
		{

		}

		public void RemoveRecord(float _start,float _end)
		{

		}
	}
}