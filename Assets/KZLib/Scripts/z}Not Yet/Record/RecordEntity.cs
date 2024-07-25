
namespace KZLib
{
	public class RecordEntity : BaseComponent,IRecordEntity
	{
		private RecordData m_RecordData = new();

		public void AddEntity()
		{
			RecordMgr.In.Add(this);
		}

		public void RemoveEntity()
		{
			RecordMgr.In.Remove(this);
		}

		public void SetData(RecordData _data)
		{
			m_RecordData = _data;
			m_RecordData.SetTransform(0.0f,transform);
		}

		public void Record(float _time)
		{
			m_RecordData.AddTransform(_time,transform);
		}
	}
}