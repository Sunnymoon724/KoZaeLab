using System.Collections.Generic;
using KZLib;

namespace GameData
{
	/// <summary>
	/// 저장하지 않는 임시 저장소
	/// </summary>
	public class Temporary : IGameData
	{
		private readonly Dictionary<string,object> m_TemporaryDict = new();

		public void Initialize() { }

		public void Release()
		{
			m_TemporaryDict.Clear();
		}

		public void SetTemporaryData(string _key,object _data)
		{
			m_TemporaryDict.AddOrUpdate(_key,_data);
		}

		public object GetTemporaryData(string _key)
		{
			return m_TemporaryDict.TryGetValue(_key,out var data) ? data : null;
		}
	}
}