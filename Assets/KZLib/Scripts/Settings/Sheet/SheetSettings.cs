#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEngine;
using KZLib.KZAttribute;
using System.Collections.Generic;
using System;
using KZLib.KZFiles;

public abstract class SheetSettings<TObject> : OuterBaseSettings<TObject> where TObject : SerializedScriptableObject
{
	protected abstract bool IsShowAddButton { get; }

	[BoxGroup("Sheet",ShowLabel = false)]
	[HorizontalGroup("Sheet/Add",Order = 0),Button("Add Sheet",ButtonSizes.Large),PropertyTooltip("File must not be inside the Unity folder."),ShowIf(nameof(IsShowAddButton))]
    protected void OnAddSheet()
    {
		var filePath = FileUtility.GetExcelFilePath();

		if(filePath.IsEmpty())
		{
			return;
		}

		if(FileUtility.IsIncludeAssetsHeader(filePath))
		{
			UnityUtility.DisplayError($"{filePath} is included in the Assets folder.");
		}

		var localPath = filePath[(FileUtility.GetProjectParentPath().Length+1)..];

		SetSheetData(localPath);
	}

	protected abstract void SetSheetData(string _filePath);

	[Serializable]
	protected abstract class SheetData
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

		[HorizontalGroup("Menu",Order = 0)]
		[HorizontalGroup("Menu/Path",Order = 0),ShowInInspector,LabelText("Local File Path"),KZDocumentPath,PropertyTooltip("$AbsoluteFilePath")]
		protected string LocalFilePath
		{
			get => m_LocalFilePath;
			set => m_LocalFilePath = FileUtility.RemoveHeaderDirectory(value,FileUtility.GetProjectParentPath());
		}

		public string AbsoluteFilePath => FileUtility.GetAbsolutePath(m_LocalFilePath,false);

		public SheetData(string _path)
		{
			m_LocalFilePath = _path;

			OnRefreshSheet();
		}

		protected abstract string[] TitleArray { get; }
		protected abstract bool IsShowCreateButton { get; }
		protected abstract bool IsCreateAble { get; }

		[HorizontalGroup("Menu/Button",Order = 2),Button("Refresh"),EnableIf(nameof(IsExistPath))]
		protected virtual void OnRefreshSheet()
		{
			m_SheetNameList.Clear();

			var excelFile = new ExcelFile(AbsoluteFilePath);

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
				UnityUtility.DisplayError("Sheet name is not exist.");
			}
		}

		[VerticalGroup("Button",Order = 2),Button("Create",ButtonSizes.Large),ShowIf(nameof(IsShowCreateButton)),EnableIf(nameof(IsCreateAble)),PropertyTooltip("$m_ErrorLog")]
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
	}
}
#endif