using KZLib;
using KZLib.KZAttribute;
using KZLib.KZFiles;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MetaData
{
	#region Color Data
	[Serializable]
	public class ColorData : IMetaData
	{
		#region Field
		[SerializeField,HideInInspector] private int m_MetaId;
		[SerializeField,HideInInspector] private string m_Version;
		[SerializeField,HideInInspector] private string m_Name;
		[SerializeField,HideInInspector] private string[] m_Color_0Array;
		[SerializeField,HideInInspector] private string[] m_Color_1Array;
		[SerializeField,HideInInspector] private string[] m_Color_2Array;
		[SerializeField,HideInInspector] private string[] m_Color_3Array;
		[SerializeField,HideInInspector] private string[] m_Color_4Array;
		[SerializeField,HideInInspector] private string[] m_Color_5Array;
		[SerializeField,HideInInspector] private string[] m_Color_6Array;
		[SerializeField,HideInInspector] private bool m_Exist = false;
		#endregion Field

		#region Property
		[BoxGroup("정보")]
		[HorizontalGroup("정보/0"),LabelText("MetaId"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip("$MetaId")]
		public int MetaId { get => m_MetaId; private set => m_MetaId = value; }

		[HorizontalGroup("정보/0"),LabelText("Version"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip("$Version")]
		public string Version { get => m_Version; private set => m_Version = value; }

		[HorizontalGroup("정보/0"),LabelText("Name"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip("$Name")]
		public string Name { get => m_Name; private set => m_Name = value; }

		[HorizontalGroup("정보/0"),LabelText("Color_0"),LabelWidth(100),ShowInInspector,PropertyTooltip("$Color_0_ToolTip"),KZRichText]
		private string Color_0_Display => Color_0.IsNullOrEmpty() ? "NULL" : string.Join(" | ",Color_0);
		private string Color_0_ToolTip => Color_0_Display.RemoveRichText();

		private string[] Color_0 { get => m_Color_0Array; set => m_Color_0Array = value; }
		public IEnumerable<string> Color_0Group => m_Color_0Array;

		[HorizontalGroup("정보/0"),LabelText("Color_1"),LabelWidth(100),ShowInInspector,PropertyTooltip("$Color_1_ToolTip"),KZRichText]
		private string Color_1_Display { get => Color_1.IsNullOrEmpty() ? "NULL" : string.Join(" | ",Color_1); set { } }
		protected string Color_1_ToolTip => Color_1_Display.RemoveRichText();

		private string[] Color_1 { get => m_Color_1Array; set => m_Color_1Array = value; }
		public IEnumerable<string> Color_1Group => m_Color_1Array;

		[HorizontalGroup("정보/0"),LabelText("Color_2"),LabelWidth(100),ShowInInspector,PropertyTooltip("$Color_2_ToolTip"),KZRichText]
		private string Color_2_Display { get => Color_2.IsNullOrEmpty() ? "NULL" : string.Join(" | ",Color_2); set { } }
		protected string Color_2_ToolTip => Color_2_Display.RemoveRichText();

		private string[] Color_2 { get => m_Color_2Array; set => m_Color_2Array = value; }
		public IEnumerable<string> Color_2Group => m_Color_2Array;

		[HorizontalGroup("정보/0"),LabelText("Color_3"),LabelWidth(100),ShowInInspector,PropertyTooltip("$Color_3_ToolTip"),KZRichText]
		private string Color_3_Display { get => Color_3.IsNullOrEmpty() ? "NULL" : string.Join(" | ",Color_3); set { } }
		protected string Color_3_ToolTip => Color_3_Display.RemoveRichText();

		private string[] Color_3 { get => m_Color_3Array; set => m_Color_3Array = value; }
		public IEnumerable<string> Color_3Group => m_Color_3Array;

		[HorizontalGroup("정보/0"),LabelText("Color_4"),LabelWidth(100),ShowInInspector,PropertyTooltip("$Color_4_ToolTip"),KZRichText]
		private string Color_4_Display { get => Color_4.IsNullOrEmpty() ? "NULL" : string.Join(" | ",Color_4); set { } }
		protected string Color_4_ToolTip => Color_4_Display.RemoveRichText();

		private string[] Color_4 { get => m_Color_4Array; set => m_Color_4Array = value; }
		public IEnumerable<string> Color_4Group => m_Color_4Array;

		[HorizontalGroup("정보/0"),LabelText("Color_5"),LabelWidth(100),ShowInInspector,PropertyTooltip("$Color_5_ToolTip"),KZRichText]
		private string Color_5_Display { get => Color_5.IsNullOrEmpty() ? "NULL" : string.Join(" | ",Color_5); set { } }
		protected string Color_5_ToolTip => Color_5_Display.RemoveRichText();

		private string[] Color_5 { get => m_Color_5Array; set => m_Color_5Array = value; }
		public IEnumerable<string> Color_5Group => m_Color_5Array;

		[HorizontalGroup("정보/0"),LabelText("Color_6"),LabelWidth(100),ShowInInspector,PropertyTooltip("$Color_6_ToolTip"),KZRichText]
		private string Color_6_Display { get => Color_6.IsNullOrEmpty() ? "NULL" : string.Join(" | ",Color_6); set { } }
		protected string Color_6_ToolTip => Color_6_Display.RemoveRichText();

		private string[] Color_6 { get => m_Color_6Array; set => m_Color_6Array = value; }
		public IEnumerable<string> Color_6Group => m_Color_6Array;
		[HorizontalGroup("정보/0"),LabelText("사용 여부"),LabelWidth(100),ShowInInspector,KZIsValid("사용 가능","오류 있음")]
		public bool IsExist => m_Exist;
		#endregion Property

		#region Initialize
		public ColorData() { }

#if UNITY_EDITOR
		public ColorData Initialize()
		{
			m_Exist = true;

			return this;
		}
#endif
		#endregion Initialize
	}
	#endregion Color Data

	public class ColorTable : MetaDataTable
	{
		[SerializeField,HideInInspector]
		private List<ColorData> m_ColorList = new();

		public override IEnumerable<IMetaData> DataGroup => m_ColorList;

#if UNITY_EDITOR
		[BoxGroup("테이블",ShowLabel = false,Order = 1)]
		[VerticalGroup("테이블/1",Order = 1),LabelText("데이터 리스트"),TableList(IsReadOnly = true,AlwaysExpanded = true,ShowPaging = true,NumberOfItemsPerPage = 10),ShowInInspector]
		private readonly List<ColorData> m_MetaDataList = new();

		protected override string TableName => "Color";

		public override void OnRefresh()
		{
			m_ColorList.Clear();

			var file = new ExcelFile(CurrentPath);
			var dataList = new List<ColorData>();

			foreach(var data in file.Deserialize<ColorData>(TableName))
			{
				dataList.Add(data.Initialize());
			}

			m_ColorList.AddRange(dataList.OrderBy(x=>x.MetaId));

			base.OnRefresh();
		}

		protected override void SetMetaDataList(IEnumerable<IMetaData> _dataGroup)
		{
			m_MetaDataList.Clear();

			foreach(var data in _dataGroup)
			{
				m_MetaDataList.Add(data as ColorData);
			}
		}
#endif
	}
}