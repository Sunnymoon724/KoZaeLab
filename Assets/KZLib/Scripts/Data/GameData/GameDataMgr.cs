using System.Collections.Generic;

namespace KZLib
{
	public interface IGameData
	{
		void Initialize();
		void Release();
	}

	public class GameDataMgr : DataSingleton<GameDataMgr>
	{
		private readonly Dictionary<string,IGameData> m_GameDataDict = new();

		/// <summary>
		/// 없으면 데이터를 자체적으로 생성한다.
		/// </summary>
		public TData Access<TData>() where TData : class,IGameData,new()
		{
			var key = typeof(TData).Name;

			if(!m_GameDataDict.ContainsKey(key))
			{
				var data = new TData();

				data.Initialize();

				m_GameDataDict.AddOrUpdate(key,data);
			}

			return m_GameDataDict[key] as TData;
		}

		public void Clear<TData>()
		{
			var key = typeof(TData).Name;

			if(m_GameDataDict.ContainsKey(key))
			{
				m_GameDataDict[key].Release();
				m_GameDataDict.Remove(key);
			}
		}

		public override void ClearAll()
		{
			foreach(var value in m_GameDataDict.Values)
			{
				value.Release();
			}

			m_GameDataDict.Clear();
		}
	}
}