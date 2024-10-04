#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEngine;
using KZLib.KZAttribute;
using System.Collections.Generic;
using System;
using KZLib.KZFiles;

/// <summary>
/// 엑셀 파일 사용 세팅
/// </summary>
public abstract partial class ExcelSettings<TObject> : OuterBaseSettings<TObject> where TObject : SerializedScriptableObject
{
	[Serializable]
	protected abstract class ExcelSheetData
	{
		[SerializeField,HideInInspector]
		protected string m_LocalFilePath = null;

		[SerializeField,HideInInspector]
		protected string m_SheetName = null;

		[SerializeField,HideInInspector]
		protected List<string> m_SheetNameList = new();

		protected string m_ErrorLog = null;

		protected bool IsExistPath => FileUtility.IsExist(AbsoluteFilePath);

		protected bool IsExistSheetName => !m_SheetName.IsEmpty();

		[VerticalGroup(" ")]
		[HorizontalGroup(" /0",Order = 0)]
		[HorizontalGroup(" /0/0",Order = 0),ShowInInspector,LabelText("경로"),LabelWidth(100),KZDocumentPath,PropertyTooltip("$AbsoluteFilePath")]
		public string AbsoluteFilePath => FileUtility.GetAbsolutePath(m_LocalFilePath,false);

		public ExcelSheetData(string _path)
		{
			m_LocalFilePath = _path;

			OnRefreshSheet();
		}

		protected abstract string[] TitleArray { get; }

		protected abstract bool IsShowCreateButton { get; }
		protected abstract bool IsCreateAble { get; }

		[HorizontalGroup(" /0/2",Order = 2),Button("시트 갱신하기"),EnableIf(nameof(IsExistPath))]
		protected virtual void OnRefreshSheet()
		{
			m_SheetNameList.Clear();

			var excelFile = GetExcelFile();

			foreach(var sheetName in excelFile.SheetNameGroup)
			{
				var titleGroup = excelFile.GetTitleGroup(sheetName);

				if(titleGroup.IsNullOrEmpty() || !IsValidSheetName(titleGroup,TitleArray))
				{
					continue;
				}

				m_SheetNameList.Add(sheetName);
			}

			if(m_SheetNameList.Count <= 0)
			{
				UnityUtility.DisplayError("조건에 맞는 메타 시트가 없습니다.");
			}
		}

		[HorizontalGroup(" /1/0",Order = 1),Button("생성 하기",ButtonSizes.Large),ShowIf(nameof(IsShowCreateButton)),EnableIf(nameof(IsCreateAble)),PropertyTooltip("$m_ErrorLog")]
		protected abstract void OnCreateData();

		private bool IsValidSheetName(IEnumerable<(string,int)> _titleGroup,string[] _titleArray)
		{
			var count = 0;

			foreach(var title in _titleArray)
			{
				var index = _titleGroup.FindIndex(x=>x.Item1.IsEqual(title));

				if(index != -1)
				{
					count++;
				}
			}

			return _titleArray.Length == count;
		}

		protected ExcelFile GetExcelFile()
		{
			return new(AbsoluteFilePath);
		}
	}
}
#endif