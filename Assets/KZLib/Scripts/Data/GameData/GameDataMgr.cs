using System.Collections.Generic;
using KZLib.KZUtility;

namespace KZLib
{
	public interface IGameData
	{
		void Initialize();
		void Release();
	}

	public class GameDataMgr : Singleton<GameDataMgr>
	{
		private readonly Dictionary<string,IGameData> m_gameDataDict = new();

		/// <summary>
		/// If data is not exist, create new data.
		/// </summary>
		public TData Access<TData>() where TData : class,IGameData,new()
		{
			var key = typeof(TData).Name;

			if(!m_gameDataDict.TryGetValue(key,out var data))
			{
				data = new TData();

				data.Initialize();

				m_gameDataDict.Add(key,data);
			}

			return data as TData;
		}

		public void Clear<TData>()
		{
			var key = typeof(TData).Name;

			if(m_gameDataDict.TryGetValue(key,out var data))
			{
				data.Release();

				m_gameDataDict.Remove(key);
			}
		}

		protected void Clear()
		{
			foreach(var value in m_gameDataDict.Values)
			{
				value.Release();
			}

			m_gameDataDict.Clear();
		}
	}
}