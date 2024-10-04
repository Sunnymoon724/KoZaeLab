#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// 메타 테이블을 세팅
/// </summary>
public partial class MetaSettings : ExcelSettings<MetaSettings>
{
	private enum DataType { String, Short, Int, Long, Float, Double, Enum, Bool, Vector3, }

	private const string META_ID = "MetaId";
	private const string VERSION = "Version";

	[PropertySpace(10)]
	[HorizontalGroup("엑셀 설정/리스트",Order = 1),SerializeField,LabelText(" "),ListDrawerSettings(ShowFoldout = false,HideAddButton = true,DraggableItems = false,NumberOfItemsPerPage = 1),ShowIf(nameof(IsShowSheet))]
	private List<MetaSheetData> m_SheetList = new();

	private bool IsShowSheet => m_SheetList.Count != 0;

	protected override bool IsShowAddButton => true;

	protected override void OnSetSheetData(string _filePath)
	{
		m_SheetList.Add(new MetaSheetData(_filePath));
	}

	public string GetSheetPath(string _sheetName)
	{
		var sheet = m_SheetList.Find(x=>x.SheetName.IsEqual(_sheetName));

		return sheet != null ? sheet.AbsoluteFilePath : string.Empty;
	}

	public IEnumerable<Type> GetMetaTableGroup()
	{
		foreach(var sheet in m_SheetList)
		{
			var type = sheet.MetaType;

			if(type != null)
			{
				yield return type;
			}
		}
	}
}
#endif