#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using KZLib.KZAttribute;
using KZLib.KZFiles;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace KZLib.KZWindow
{
	public partial class MetaDataWindow : OdinEditorWindow
	{
		[Serializable]
		private class ExcelSheetData
		{
			[SerializeField,HideInInspector]
			private string m_LocalFilePath = null;

			[SerializeField,HideInInspector]
			private List<string> m_SheetNameList = new();

			[SerializeField,HideInInspector]
			private string m_SheetName = null;

			[SerializeField,HideInInspector]
			private Action<string> m_OnRemoveSheet = null;

			private ExcelFile File => new(AbsoluteFilePath);

			private bool IsExistFilePath => CommonUtility.IsExistFile(AbsoluteFilePath);
			private bool IsExistSheetName => !m_SheetName.IsEmpty();

			public ExcelSheetData(string _path,Action<string> _onRemoveSheet)
			{
				m_LocalFilePath = _path;

				m_OnRemoveSheet = _onRemoveSheet;

				OnRefreshSheet();
			}

			[VerticalGroup(" ")]
			[HorizontalGroup(" /0",Order = 0)]
			[HorizontalGroup(" /0/0",Order = 0),ShowInInspector,LabelText("경로"),LabelWidth(100),KZDocumentPath,PropertyTooltip("$AbsoluteFilePath")]
			public string AbsoluteFilePath => CommonUtility.GetExternalFileAbsolutePath(m_LocalFilePath);

			[HorizontalGroup(" /0/1",Order = 1),Button("시트 갱신하기"),EnableIf(nameof(IsExistFilePath))]
			private void OnRefreshSheet()
			{
				m_SheetNameList.Clear();

				foreach(var sheetName in File.SheetNameGroup)
				{
					var titleGroup = File.GetTitleGroup(sheetName);

					// if(titleGroup.IsNullOrEmpty() || !titleGroup.Contains(META_ID) || !titleGroup.Contains(VERSION))
					// {
					// 	continue;
					// }

					m_SheetNameList.Add(sheetName);
				}

				if(m_SheetNameList.Count <= 0)
				{
					CommonUtility.DisplayError("조건에 맞는 메타 시트가 없습니다.");
				}

				if(IsExistSheetName && !m_SheetNameList.Contains(m_SheetName))
				{
					m_SheetName = null;
					m_HeaderList.Clear();
				}
			}

			[HorizontalGroup(" /0/2",Order = 2),ShowInInspector,LabelText("시트"),LabelWidth(100),ValueDropdown(nameof(m_SheetNameList))]
			private string SheetName
			{
				get => m_SheetName;
				set
				{
					m_SheetName = value;

					var order = 0;

					m_HeaderList.Clear();

					// foreach(var title in File.GetTitleList(value))
					// {
					// 	m_HeaderList.Add(new ExcelCellData(title.Replace(" ",""),DataType.String,false,order++));
					// }
				}
			}

			[HorizontalGroup(" /0/3",Order = 3),Button("시트 제거하기"),EnableIf(nameof(IsExistFilePath))]
			private void OnRemoveSheet()
			{
				m_OnRemoveSheet?.Invoke(AbsoluteFilePath);
			}

			[Space(5)]
			[VerticalGroup(" /1",Order = 1),SerializeField,LabelText(" "),ListDrawerSettings(HideAddButton = true,HideRemoveButton = false,ShowFoldout = false,DraggableItems = false),ShowIf(nameof(IsShowCreateButton))]
			private List<ExcelCellData> m_HeaderList = new();

			private bool IsShowCreateButton => m_HeaderList.Count > 0;

			public bool IsConvertAble(out string _errorLog)
			{
				_errorLog = string.Empty;

				if(!IsExistSheetName)
				{
					_errorLog = "시트 이름이 NULL 입니다.";

					return false;
				}

				return true;
			}
		}
	}
}
#endif