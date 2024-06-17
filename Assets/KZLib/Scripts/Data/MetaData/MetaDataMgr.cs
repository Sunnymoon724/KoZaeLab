using System;
using System.Collections.Generic;
using System.Linq;

namespace KZLib
{
	/// <summary>
	/// 테이블 핸들러로 데이터 테이블을 받아서 MetaID를 Key로 저장한다.
	/// </summary>
	public class MetaDataMgr : DataSingleton<MetaDataMgr>
	{
		private bool m_Loaded = false;

		//key는 타입 / Value [Key는 Metadata Id / Value는 데이터]
		private readonly Dictionary<Type,Dictionary<int,IMetaData>> m_MetaDataContainer = new();

		public void LoadAll()
		{
			if(m_Loaded)
			{
				return;
			}

			Log.Data.I("메타 데이터 로드 시작");

			var dataList = new List<(string,int)>();

			foreach(var data in ResMgr.In.GetScriptableObjectArray(GameSettings.In.MetaAssetPath))
			{
				var table = data as IMetaDataTable ?? throw new NullReferenceException("메타 테이블이 NUll입니다.");

				var count = AddMetaData(table.Initialize());

				if(count != 0)
				{
					dataList.Add((data.name,count));
				}
			}

			if(!m_MetaDataContainer.Any())
			{
				Log.Data.W("메타 데이터 매니져가 비어 있습니다.");
			}
			else
			{
				Log.Data.I("메타 데이터 로드 완료 [{0}]",dataList);
			}

			m_Loaded = true;
		}

		private int AddMetaData(IMetaDataTable _table)
		{
			if(_table.DataGroup.IsNullOrEmpty())
			{
				return 0;
			}

			var dataDict = new Dictionary<int,IMetaData>();
			var iterator = _table.DataGroup.GetEnumerator();

			while(iterator.MoveNext())
			{
				var data = iterator.Current;

				if(!CommonUtility.CheckVersion(data.Version) || !data.IsExist)
				{
					continue;
				}

				if(data.MetaId != -1 && dataDict.ContainsKey(data.MetaId))
				{
					continue;
				}

				dataDict.Add(data.MetaId,data);
			}

			if(dataDict.IsNullOrEmpty())
			{
				return 0;
			}

			var type = _table.DataGroup.First().GetType();

			m_MetaDataContainer.Add(type,dataDict);

			return dataDict.Count;
		}

		public TData Get<TData>(int _metaId) where TData : class,IMetaData
		{
			if(_metaId <= 0)
			{
				throw new ArgumentException(string.Format("{0}은 유효한 메타아이디가 아닙니다.",_metaId));
			}

			var type = typeof(TData);

			if(m_MetaDataContainer.TryGetValue(type,out var dataDict))
			{
				return dataDict.TryGetValue(_metaId,out var data) ? data as TData : null;
			}
			else
			{
				throw new ArgumentException(string.Format("{0}은 존재하지 않습니다.",type));
			}
		}

		public bool IsValidate<TData>(int _metaId) where TData : class,IMetaData
		{
			return _metaId > 0 && m_MetaDataContainer.TryGetValue(typeof(TData),out var dataDict) && dataDict.ContainsKey(_metaId);
		}

		public IEnumerable<TData> GetContainer<TData>() where TData : class,IMetaData
		{
			return m_MetaDataContainer.TryGetValue(typeof(TData),out var data) ? data.Values.Cast<TData>() : null;
		}

		public int GetCount<TData>() where TData : class,IMetaData
		{
			return m_MetaDataContainer.TryGetValue(typeof(TData),out var data) ? data.Count : 0;
		}

		public override void ClearAll()
		{
			m_MetaDataContainer.Clear();

			m_Loaded = false;
		}
		
		public void Reload()
		{
			ClearAll();

			LoadAll();
		}
	}
}