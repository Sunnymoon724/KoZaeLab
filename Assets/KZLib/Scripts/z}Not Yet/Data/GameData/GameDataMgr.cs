using System.Collections.Generic;
using KZLib.KZUtility;

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
		/// If data is not exist, create new data.
		/// </summary>
		public TData Access<TData>() where TData : class,IGameData,new()
		{
			var key = typeof(TData).Name;

			if(!m_GameDataDict.TryGetValue(key,out var data))
			{
				data = new TData();

				data.Initialize();

				m_GameDataDict.Add(key,data);
			}

			return data as TData;
		}

		public void Clear<TData>()
		{
			var key = typeof(TData).Name;

			if(m_GameDataDict.TryGetValue(key,out var data))
			{
				data.Release();

				m_GameDataDict.Remove(key);
			}
		}

		protected override void ClearAll()
		{
			foreach(var value in m_GameDataDict.Values)
			{
				value.Release();
			}

			m_GameDataDict.Clear();
		}
	}
}