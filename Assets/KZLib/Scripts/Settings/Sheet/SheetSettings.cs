#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEngine;
using KZLib.KZAttribute;
using System.Collections.Generic;
using System;
using KZLib.KZReader;

public abstract class SheetSettings<TObject> : OuterBaseSettings<TObject> where TObject : SerializedScriptableObject
{
	protected enum DataType { String, Short, Int, Long, Float, Double, Enum, Bool, Vector3, Object }

	protected abstract bool IsShowAddButton { get; }

	[BoxGroup("Sheet",ShowLabel = false)]
	[HorizontalGroup("Sheet/Add",Order = 0),Button("Add Sheet",ButtonSizes.Large),PropertyTooltip("File must not be inside the Unity folder."),ShowIf(nameof(IsShowAddButton))]
    protected void OnAddSheet()
    {
		var filePath = CommonUtility.GetExcelFilePath();

		if(filePath.IsEmpty())
		{
			return;
		}

		if(CommonUtility.IsIncludeAssetsHeader(filePath))
		{
			CommonUtility.DisplayError(new NullReferenceException($"{filePath} is included in the Assets folder."));
		}

		SetSheetData(filePath);
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

		protected bool IsExistPath => CommonUtility.IsFileExist(AbsoluteFilePath);
		protected bool IsExistSheetName => !m_SheetName.IsEmpty();

		[HorizontalGroup("Menu",Order = 0)]
		[HorizontalGroup("Menu/Path",Order = 0),ShowInInspector,LabelText("Local File Path"),KZDocumentPath,PropertyTooltip("$AbsoluteFilePath")]
		protected string LocalFilePath
		{
			get => m_LocalFilePath;
			private set => m_LocalFilePath = CommonUtility.RemoveHeaderDirectory(value,CommonUtility.GetProjectParentPath());
		}

		public string AbsoluteFilePath => CommonUtility.GetAbsolutePath(m_LocalFilePath,false);

		public SheetData(string _path)
		{
			LocalFilePath = _path;

			OnRefreshSheet();
		}

		protected abstract string[] TitleArray { get; }
		protected abstract bool IsShowCreateButton { get; }
		protected abstract bool IsCreateAble { get; }

		[HorizontalGroup("Menu/Button",Order = 2),Button("Refresh"),EnableIf(nameof(IsExistPath))]
		protected virtual void OnRefreshSheet()
		{
			m_SheetNameList.Clear();

			var reader = new ExcelReader(AbsoluteFilePath);

			foreach(var sheetName in reader.SheetNameGroup)
			{
				var titleGroup = reader.GetTitleGroup(sheetName);

				if(titleGroup.IsNullOrEmpty() || !IsValidSheetName(titleGroup,TitleArray))
				{
					continue;
				}

				m_SheetNameList.Add(sheetName);
			}

			if(m_SheetNameList.Count <= 0)
			{
				CommonUtility.DisplayError(new NullReferenceException("Sheet name is not exist."));
			}
		}

		[VerticalGroup("Button",Order = 2),Button("Create",ButtonSizes.Large),ShowIf(nameof(IsShowCreateButton)),EnableIf(nameof(IsCreateAble)),PropertyTooltip("$m_ErrorLog")]
		protected abstract void OnCreateData();

		private bool IsValidSheetName(IEnumerable<(string,int)> _nameGroup,string[] _titleArray)
		{
			var cnt = 0;

			foreach(var title in _titleArray)
			{
				var idx = _nameGroup.IndexOf(x=>x.Item1.IsEqual(title));

				if(idx != -1)
				{
					cnt++;
				}
			}

			return _titleArray.Length == cnt;
		}
	}
}
#endif