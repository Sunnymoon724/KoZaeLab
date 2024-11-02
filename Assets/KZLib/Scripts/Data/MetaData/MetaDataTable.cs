#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using KZLib.KZAttribute;
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

		[SerializeField,HideInInspector]
		private readonly List<IMetaData> m_MetaDataList = new();

		public MetaDataTable(Type _type)
		{
			m_TableName = _type.Name;

			m_CurrentPath = MetaSettings.In.GetFilePath(_type);
			m_DataType = _type;

			ShowList();
		}

		[BoxGroup("Option",ShowLabel = false,Order = 0)]
		[HorizontalGroup("Option/Refresh",Order = 0),Button("Refresh",ButtonSizes.Large)]
		protected void OnRefresh()
		{
			MetaSettings.In.RefreshMetaData(m_DataType);

			ShowList();
		}

		private void ShowList()
		{
			m_MetaDataList.Clear();
			m_MetaDataList.AddRange(MetaDataMgr.In.GetMetaDataGroup(m_DataType));

			OnShowAllList();
		}

		[HorizontalGroup("Option/Menu",Order = 1),SerializeField,LabelText("Current Path"),KZDocumentPath(false)]
		private string m_CurrentPath = null;

		[HorizontalGroup("Option/Menu",Order = 1)]
		[HorizontalGroup("Option/Menu/Export",Order = 0),Button("ToJson",ButtonSizes.Medium)]
		protected void OnExportToJson()
		{
			var filePath = CommonUtility.PathCombine(CommonUtility.GetParentAbsolutePath(m_CurrentPath,true),string.Format("{0}.json",m_TableName));
			var text = JsonConvert.SerializeObject(m_MetaDataList);

			CommonUtility.WriteTextToFile(filePath,text);
		}

		[BoxGroup("Table",ShowLabel = false,Order = 1)]
		[VerticalGroup("Table/List",Order = 1),LabelText("Meta Data List"),TableList(IsReadOnly = true,AlwaysExpanded = true,ShowPaging = true,NumberOfItemsPerPage = 10,ShowIndexLabels = true),ShowInInspector]
		private readonly List<IMetaData> m_ShowList = new();

		[BoxGroup("Table",ShowLabel = false,Order = 1)]
		[VerticalGroup("Table/Button",Order = 0)]
		[HorizontalGroup("Table/Button/0",Order = 0),Button("Full List",ButtonSizes.Large)]
		protected void OnShowAllList()
		{
			SetMetaDataList(m_MetaDataList);
		}

		[BoxGroup("Table",ShowLabel = false,Order = 1)]
		[HorizontalGroup("Table/Button/0",Order = 0),Button("Exist List",ButtonSizes.Large)]
		protected void OnShowExistList()
		{
			SetMetaDataList(m_MetaDataList.Where(x=>x.IsExist));
		}

		[BoxGroup("Table",ShowLabel = false,Order = 1)]
		[HorizontalGroup("Table/Button/0",Order = 0),Button("Error List",ButtonSizes.Large)]
		protected void OnShowNotExistList()
		{
			SetMetaDataList(m_MetaDataList.Where(x=>!x.IsExist));
		}

		private void SetMetaDataList(IEnumerable<IMetaData> _dataGroup)
		{
			m_ShowList.Clear();
			m_ShowList.AddRange(_dataGroup);
		}
	}
}
#endif