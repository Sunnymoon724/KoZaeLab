#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using KZLib.KZAttribute;
using KZLib.KZFiles;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib
{
	public class MetaDataTable
	{
		[SerializeField,HideInInspector]
		private string m_TableName = null;
		[SerializeField,HideInInspector]
		private Type m_DataType = null;

		public MetaDataTable(Type _type)
		{
			m_TableName = _type.Name;

			m_CurrentPath = MetaSettings.In.GetFilePath(_type);
			m_DataType = _type;
			m_MetaDataList.AddRange(MetaDataMgr.In.GetMetaDataGroup(m_DataType));
		}

		[BoxGroup("Option",ShowLabel = false,Order = 0)]
		[HorizontalGroup("Option/Refresh",Order = 0),Button("Refresh",ButtonSizes.Large)]
		public virtual void OnRefresh()
		{
			MetaSettings.In.RefreshMetaData(m_DataType);

			m_MetaDataList.Clear();

			m_MetaDataList.AddRange(MetaDataMgr.In.GetMetaDataGroup(m_DataType));

			OnShowAllList();
		}

		[HorizontalGroup("Option/Menu",Order = 1),SerializeField,LabelText("Current Path"),KZDocumentPath(false),LabelWidth(75)]
		private string m_CurrentPath = null;

		[HorizontalGroup("Option/Menu",Order = 1)]
		[HorizontalGroup("Option/Menu/추출",Order = 0),Button("ToJson",ButtonSizes.Medium),PropertyTooltip("현재 경로로 익스포트 합니다.")]
		public void OnExportToJson()
		{
			var filePath = FileUtility.PathCombine(FileUtility.GetParentAbsolutePath(m_CurrentPath,true),string.Format("{0}.json",m_TableName));
			var text = JsonConvert.SerializeObject(m_MetaDataList);

			FileUtility.WriteTextToFile(filePath,text);
		}

		[BoxGroup("Table",ShowLabel = false,Order = 1)]
		[VerticalGroup("Table/List",Order = 1),LabelText("Data List"),TableList(IsReadOnly = true,AlwaysExpanded = true,ShowPaging = true,NumberOfItemsPerPage = 10),ShowInInspector]
		private readonly List<IMetaData> m_MetaDataList = new();

		protected virtual void IsValidate()
		{
			var indexSet = new HashSet<int>();

			for(var i=0;i<m_MetaDataList.Count;i++)
			{
				var data = m_MetaDataList[i];

				if(data == null)
				{
					UnityUtility.DisplayError(string.Format("{0} 번쨰 데이터가 null 입니다.",i));
				}

				if(data.MetaId == 0)
				{
					UnityUtility.DisplayError(string.Format("{0} 번쨰 메타아이디가 0 입니다.",i));
				}

				if(indexSet.Contains(data.MetaId))
				{
					UnityUtility.DisplayError(string.Format("{0} 번쨰 메타아이디[{1}]가 중복 입니다!",i,data.MetaId));
				}

				if(data.MetaId != -1)
				{
					indexSet.Add(data.MetaId);
				}
			}
		}

		[BoxGroup("Table",ShowLabel = false,Order = 1)]
		[VerticalGroup("Table/버튼",Order = 0)]
		[HorizontalGroup("Table/버튼/0",Order = 0),Button("전체 리스트",ButtonSizes.Large)]
		protected virtual void OnShowAllList()
		{
			SetMetaDataList(m_MetaDataList);
		}

		[BoxGroup("Table",ShowLabel = false,Order = 1)]
		[HorizontalGroup("Table/버튼/0",Order = 0),Button("사용 가능한 리스트",ButtonSizes.Large)]
		protected virtual void OnShowExistList()
		{
			SetMetaDataList(m_MetaDataList.Where(x=>x.IsExist));
		}

		[BoxGroup("Table",ShowLabel = false,Order = 1)]
		[HorizontalGroup("Table/버튼/0",Order = 0),Button("오류가 있는 리스트",ButtonSizes.Large)]
		protected virtual void OnShowNotExistList()
		{
			SetMetaDataList(m_MetaDataList.Where(x=>!x.IsExist));
		}

		protected virtual void SetMetaDataList(IEnumerable<IMetaData> _dataGroup)
		{
			m_MetaDataList.Clear();

			foreach(var data in _dataGroup)
			{
				// m_MetaDataList.Add(data as PartsData);
			}
		}
	}
}
#endif