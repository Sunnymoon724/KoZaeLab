#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// 메타 테이블을 세팅
/// </summary>
public partial class MetaSettings : ExcelSettings<MetaSettings>
{
	private enum DataType { String, Int, Long, Float, Enum, Bool, Vector3, }

	private const string META_ID = "MetaId";
	private const string VERSION = "Version";

	[PropertySpace(10)]
	[HorizontalGroup("엑셀 설정/리스트",Order = 1),SerializeField,LabelText(" "),ListDrawerSettings(ShowFoldout = false,HideAddButton = true,DraggableItems = false,NumberOfItemsPerPage = 1),ShowIf(nameof(IsShowCreateButton))]
	private List<MetaSheetData> m_SheetList = new();

	protected override bool IsShowAddButton => true;
    protected override bool IsShowCreateButton => m_SheetList.Count != 0;
    protected override bool IsCreateAble
	{
		get
		{
			m_ErrorLog = string.Empty;

			if(!IsShowCreateButton)
			{
				m_ErrorLog = "시트가 없습니다.";

				return false;
			}

			var builder = new StringBuilder();

			for(var i=0;i<m_SheetList.Count;i++)
			{
				var sheet = m_SheetList[i];

				if(!sheet.IsCreateAble(out var error))
				{
					builder.AppendFormat("{0} 번째 시트 데이터가 오류 입니다. [{1}]\n",i+1,error);
				}
			}

			m_ErrorLog = builder.ToString();

			return builder.Length <= 0;
		}
	}

	protected override void OnSetSheetData(string _filePath)
	{
		m_SheetList.Add(new MetaSheetData(_filePath));
	}

	protected override void OnCreateButton()
	{
		if(!IsShowCreateButton)
		{
			return;
		}

		for(var i=0;i<m_SheetList.Count;i++)
		{
			m_SheetList[i].CreateData();
		}
	}

	public string GetSheetPath(string _sheetName)
	{
		var sheet = m_SheetList.Find(x=>x.SheetName.IsEqual(_sheetName));

		return sheet != null ? sheet.AbsoluteFilePath : string.Empty;
	}

	public IEnumerable<string> GetMetaTableNameGroup()
	{
		var tableList = new List<string>();

		foreach(var sheet in m_SheetList)
		{
			tableList.Add(string.Format("{0}Table",sheet.SheetName));
		}

		return tableList;
	}
}
#endif