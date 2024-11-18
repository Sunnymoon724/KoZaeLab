// using KZLib;
// using KZLib.KZAttribute;
// using KZLib.KZFiles;
// using Sirenix.OdinInspector;
// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;

// namespace MetaData
// {
// 	#region Parts Data
// 	[Serializable]
// 	public class PartsData : IMetaData
// 	{
// 		#region Field
// 		[SerializeField,HideInInspector] private int m_MetaId;
// 		[SerializeField,HideInInspector] private string m_Version;
// 		[SerializeField,HideInInspector] private string m_ModelPath;
// 		[SerializeField,HideInInspector] private DataType m_DataType;
// 		[SerializeField,HideInInspector] private string m_StatId;
// 		[SerializeField,HideInInspector] private bool m_Exist = false;
// 		#endregion Field

// 		#region Property
// 		[BoxGroup("정보")]
// 		[HorizontalGroup("정보/0"),LabelText("MetaId"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip("$MetaId")]
// 		public int MetaId { get => m_MetaId; private set => m_MetaId = value; }

// 		[HorizontalGroup("정보/0"),LabelText("Version"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip("$Version")]
// 		public string Version { get => m_Version; private set => m_Version = value; }

// 		[HorizontalGroup("정보/0"),LabelText("ModelPath"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip("$ModelPath")]
// 		public string ModelPath { get => m_ModelPath; private set => m_ModelPath = value; }

// 		[HorizontalGroup("정보/0"),LabelText("DataType"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip("$DataType")]
// 		public DataType DataType { get => m_DataType; private set => m_DataType = value; }

// 		[HorizontalGroup("정보/0"),LabelText("StatId"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip("$StatId")]
// 		public string StatId { get => m_StatId; private set => m_StatId = value; }
// 		[HorizontalGroup("정보/0"),LabelText("사용 여부"),LabelWidth(100),ShowInInspector,KZIsValid("사용 가능","오류 있음")]
// 		public bool IsExist => m_Exist;

//         public string Name => throw new NotImplementedException();
//         #endregion Property

//         #region Initialize
//         public PartsData() { }

// #if UNITY_EDITOR
// 		public PartsData Initialize()
// 		{
// 			m_Exist = true;

// 			return this;
// 		}
// #endif
// 		#endregion Initialize
// 	}
// 	#endregion Parts Data

// 	public class PartsTable : MetaDataTable
// 	{
// 		[SerializeField,HideInInspector]
// 		private List<PartsData> m_PartsList = new();

// 		public override IEnumerable<IMetaData> DataGroup => m_PartsList;

// #if UNITY_EDITOR
// 		[BoxGroup("테이블",ShowLabel = false,Order = 1)]
// 		[VerticalGroup("테이블/1",Order = 1),LabelText("데이터 리스트"),TableList(IsReadOnly = true,AlwaysExpanded = true,ShowPaging = true,NumberOfItemsPerPage = 10),ShowInInspector]
// 		private readonly List<PartsData> m_MetaDataList = new();

// 		protected override string TableName => "Parts";

// 		public override void OnRefresh()
// 		{
// 			m_PartsList.Clear();

// 			var file = new ExcelFile(CurrentPath);
// 			var dataList = new List<PartsData>();

// 			foreach(var data in file.Deserialize<PartsData>(TableName))
// 			{
// 				dataList.Add(data.Initialize());
// 			}

// 			m_PartsList.AddRange(dataList.OrderBy(x=>x.MetaId));

// 			base.OnRefresh();
// 		}

// 		protected override void SetMetaDataList(IEnumerable<IMetaData> _dataGroup)
// 		{
// 			m_MetaDataList.Clear();

// 			foreach(var data in _dataGroup)
// 			{
// 				m_MetaDataList.Add(data as PartsData);
// 			}
// 		}
// #endif
// 	}
// }