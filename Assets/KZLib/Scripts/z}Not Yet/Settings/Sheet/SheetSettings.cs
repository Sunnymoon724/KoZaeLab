// #if UNITY_EDITOR
// using Sirenix.OdinInspector;
// using UnityEngine;
// using KZLib.KZAttribute;
// using System.Collections.Generic;
// using System;
// using KZLib.Utility;

// public abstract class SheetSettings<TObject> : OuterBaseSettings<TObject> where TObject : SerializedScriptableObject
// {
// 	protected enum DataType { String, Short, Int, Long, Float, Double, Enum, Bool, Vector3, Object }

// 	protected abstract bool IsShowAddButton { get; }

// 	[BoxGroup("Sheet",ShowLabel = false)]
// 	[HorizontalGroup("Sheet/Add",Order = 0),Button("Add Sheet",ButtonSizes.Large),PropertyTooltip("File must not be inside the Unity folder."),ShowIf(nameof(IsShowAddButton))]
//     protected void OnAddSheet()
//     {
// 		var filePath = CommonUtility.GetExcelFilePath();

// 		if(filePath.IsEmpty())
// 		{
// 			return;
// 		}

// 		if(CommonUtility.IsIncludeAssetsHeader(filePath))
// 		{
// 			CommonUtility.DisplayError(new NullReferenceException($"{filePath} is included in the Assets folder."));
// 		}

// 		SetSheetData(filePath);
// 	}

// 	protected abstract void SetSheetData(string _filePath);

// 	[Serializable]
// 	protected abstract class SheetData
// 	{
// 		[SerializeField,HideInInspector]
// 		protected string m_LocalFilePath = null;

// 		[SerializeField,HideInInspector]
// 		protected string m_SheetName = null;

// 		[SerializeField,HideInInspector]
// 		protected List<string> m_SheetNameList = new();

// 		protected string m_ErrorLog = null;

// 		protected bool IsExistPath => CommonUtility.IsFileExist(AbsoluteFilePath);
// 		protected bool IsExistSheetName => !m_SheetName.IsEmpty();

// 		[HorizontalGroup("Menu",Order = 0)]
// 		[HorizontalGroup("Menu/Path",Order = 0),ShowInInspector,LabelText("Local File Path"),KZDocumentPath,PropertyTooltip("$AbsoluteFilePath")]
// 		protected string LocalFilePath
// 		{
// 			get => m_LocalFilePath;
// 			private set => m_LocalFilePath = CommonUtility.RemoveTextInPath(value,CommonUtility.GetProjectParentPath());
// 		}

// 		public string AbsoluteFilePath => CommonUtility.GetAbsolutePath(m_LocalFilePath,false);

// 		public SheetData(string _path)
// 		{
// 			LocalFilePath = _path;

// 			OnRefreshSheet();
// 		}

// 		protected abstract string[] TitleArray { get; }
// 		protected abstract bool IsShowCreateButton { get; }
// 		protected abstract bool IsCreateAble { get; }

// 		[HorizontalGroup("Menu/Button",Order = 2),Button("Refresh"),EnableIf(nameof(IsExistPath))]
// 		protected virtual void OnRefreshSheet()
// 		{
// 			m_SheetNameList.Clear();

// 			var reader = new ExcelReader(AbsoluteFilePath);

// 			foreach(var sheetName in reader.SheetNameGroup)
// 			{
// 				var titleGroup = reader.GetTitleGroup(sheetName);

// 				if(titleGroup.IsNullOrEmpty() || !IsValidSheetName(titleGroup,TitleArray))
// 				{
// 					continue;
// 				}

// 				m_SheetNameList.Add(sheetName);
// 			}

// 			if(m_SheetNameList.Count <= 0)
// 			{
// 				CommonUtility.DisplayError(new NullReferenceException("Sheet name is not exist."));
// 			}
// 		}

// 		[VerticalGroup("Button",Order = 2),Button("Create",ButtonSizes.Large),ShowIf(nameof(IsShowCreateButton)),EnableIf(nameof(IsCreateAble)),PropertyTooltip("$m_ErrorLog")]
// 		protected abstract void OnCreateData();

// 		// private static string ConvertSchemeRecursive(string[] schemeArr, ref int idx)
// 		// {
// 		// 	switch (schemeArr[idx])
// 		// 	{
// 		// 		case "enum":
// 		// 			return schemeArr[++idx];
// 		// 		case "arr":
// 		// 			++idx;
// 		// 			return $"{ConvertSchemeRecursive(schemeArr, ref idx)}[]";
// 		// 		case "list":
// 		// 			++idx;
// 		// 			var elemType = ConvertSchemeRecursive(schemeArr, ref idx);
// 		// 			return $"List<{elemType}>";
// 		// 		case "dict":
// 		// 			++idx;
// 		// 			var keyType = ConvertSchemeRecursive(schemeArr, ref idx);
// 		// 			++idx;
// 		// 			var valType = ConvertSchemeRecursive(schemeArr, ref idx);
// 		// 			return $"Dictionary<{keyType}, {valType}>";
// 		// 		default:
// 		// 			return schemeArr[idx];
// 		// 	}
// 		// }

// 		// private string DataTypeToString()
// 		// {
// 		// 	return Type switch
// 		// 	{
// 		// 		DataType.Enum => Name,
// 		// 		DataType.Vector3 => Type.ToString(),
// 		// 		_ => Type.ToString().ToLowerInvariant(),
// 		// 	};
// 		// }

// 		private bool IsValidSheetName(IEnumerable<(string,int)> _nameGroup,string[] _titleArray)
// 		{
// 			var cnt = 0;

// 			foreach(var title in _titleArray)
// 			{
// 				var idx = _nameGroup.IndexOf(x=>x.Item1.IsEqual(title));

// 				if(idx != -1)
// 				{
// 					cnt++;
// 				}
// 			}

// 			return _titleArray.Length == cnt;
// 		}
// 	}

// 	#region Sheet Cell Data
// 	[Serializable]
// 	protected abstract class SheetCellData
// 	{
// 		[SerializeField,HideInInspector] private string m_Name = null;
// 		[SerializeField,HideInInspector] private object m_Type = null;

// 		[HorizontalGroup("0"),HideLabel,ShowInInspector,DisplayAsString]
// 		public string Name { get => m_Name; private set => m_Name = value; }

// 		[HorizontalGroup("0"),HideLabel,ShowInInspector,DisplayAsString]
// 		public object Type { get => m_Type; private set => m_Type = value; }

// 		public SheetCellData(string _name,string _type)
// 		{
// 			Name = _name;
// 			Type = _type;
// 		}

// 		// public string ToConstructorArgument()
// 		// {
// 		// 	return $"{(string.Concat(DataTypeToString(),IsArray ? "[]" : ""))} _{Name.ToFirstCharacterToLower()}";
// 		// }

// 		// public string ToConstructorInitialize()
// 		// {
// 		// 	return $"m_{Name} = _{Name.ToFirstCharacterToLower()};";
// 		// }

// 		// public string ToFieldText()
// 		// {
// 		// 	return $"[SerializeField,HideInInspector] private {(string.Concat(DataTypeToString(),IsArray ? "[]" : ""))} m_{Name};";
// 		// }

// 		// public string ToPropertyText(string _group,int _key)
// 		// {
// 		// 	var builder = new StringBuilder();
// 		// 	var type = DataTypeToString();

// 		// 	if(IsArray)
// 		// 	{
// 		// 		builder.Append($"[HorizontalGroup(\"{_group}/0\"),LabelText(\"{Name}\"),LabelWidth(100),ShowInInspector,PropertyTooltip(\"${Name}_ToolTip\"),KZRichText]");
// 		// 		builder.Append(ConfigSheetData.NewLineTapTap);
// 		// 		builder.Append($"private string {Name}_Display => {Name}.IsNullOrEmpty() ? \"NULL\" : string.Join(\" | \",{Name});");
// 		// 		builder.Append(ConfigSheetData.NewLineTapTap);
// 		// 		builder.Append($"private string {Name}_ToolTip => {Name}_Display.RemoveRichText();");
// 		// 		builder.Append(ConfigSheetData.NewLineNewLineTapTap);

// 		// 		builder.Append($"[Key({_key})]");
// 		// 		builder.Append(ConfigSheetData.NewLineTapTap);
// 		// 		builder.Append($"public {type}[] {Name} {{ get => m_{Name}; private set => m_{Name} = value; }}");
// 		// 	}
// 		// 	else
// 		// 	{
// 		// 		builder.Append($"[HorizontalGroup(\"{_group}/0\"),LabelText(\"{Name}\"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip(\"${Name}\"),Key({_key})]");
// 		// 		builder.Append(ConfigSheetData.NewLineTapTap);
// 		// 		builder.Append($"public {type} {Name} {{ get => m_{Name}; private set => m_{Name} = value; }}");
// 		// 	}

// 		// 	return builder.ToString();
// 		// }

// 		// private string DataTypeToString()
// 		// {
// 		// 	return Type switch
// 		// 	{
// 		// 		DataType.Enum => Name,
// 		// 		DataType.Vector3 => Type.ToString(),
// 		// 		_ => Type.ToString().ToLowerInvariant(),
// 		// 	};
// 		// }
// 	}
// 	#endregion Sheet Cell Data
// }
// #endif