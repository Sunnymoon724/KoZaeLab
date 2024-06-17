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

		protected bool IsExistFilePath => CommonUtility.IsExistFile(AbsoluteFilePath);

		protected bool IsExistSheetName => !m_SheetName.IsEmpty();

		[VerticalGroup(" ")]
		[HorizontalGroup(" /0",Order = 0)]
		[HorizontalGroup(" /0/0",Order = 0),ShowInInspector,LabelText("경로"),LabelWidth(100),KZDocumentPath,PropertyTooltip("$AbsoluteFilePath")]
		public string AbsoluteFilePath => CommonUtility.GetExternalFileAbsolutePath(m_LocalFilePath);

		public ExcelSheetData(string _path)
		{
			m_LocalFilePath = _path;

			OnRefreshSheet();
		}

		protected abstract string[] TitleArray { get; }

		public abstract bool IsCreateAble(out string _errorLog);
		public abstract void CreateData();

		[HorizontalGroup(" /0/1",Order = 1),Button("시트 갱신하기"),EnableIf(nameof(IsExistFilePath))]
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
				CommonUtility.DisplayError("조건에 맞는 메타 시트가 없습니다.");
			}
		}

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