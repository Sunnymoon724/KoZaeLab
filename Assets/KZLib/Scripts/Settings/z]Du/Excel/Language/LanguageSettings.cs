#if UNITY_EDITOR
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 언어 현지화 세팅
/// </summary>
public partial class LanguageSettings : ExcelSettings<LanguageSettings>
{
	private const string LANGUAGE_KEY = "Key";
	private const string ENGLISH = "English";

	//? 리스트 쓰는 이유 -> class 로 하면 안이쁨
	[PropertySpace(10)]
	[HorizontalGroup("엑셀 설정/리스트",Order = 1),SerializeField,LabelText(" "),ListDrawerSettings(ShowFoldout = false,HideAddButton = true,DraggableItems = false,NumberOfItemsPerPage = 1),HideIf(nameof(IsShowAddButton))]
	private List<LanguageSheetData> m_SheetList = new();

	protected override bool IsShowAddButton => m_SheetList.Count == 0;

	protected override void OnSetSheetData(string _filePath)
	{
		m_SheetList.Clear();

		m_SheetList.Add(new LanguageSheetData(_filePath));
	}
}
#endif