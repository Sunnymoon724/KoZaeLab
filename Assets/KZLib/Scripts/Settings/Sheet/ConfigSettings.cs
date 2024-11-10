#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEditor;
using System.Text;
using KZLib;
using MessagePack;
using System.Collections;
using KZLib.KZResolver;
using KZLib.KZReader;

public class ConfigSettings : SheetSettings<ConfigSettings>
{
	private const string NAME = "Name";
	private const string TYPE = "Type";
	private const string WRITEABLE = "Writable";
	private const string DEFAULT = "Default";

	[PropertySpace(10)]
	[HorizontalGroup("Sheet/List",Order = 1),SerializeField,LabelText(" "),ListDrawerSettings(ShowFoldout = false,HideAddButton = true,DraggableItems = false,NumberOfItemsPerPage = 1),ShowIf(nameof(IsShowSheet))]
	private List<ConfigSheetData> m_SheetList = new();

	private bool IsShowSheet => m_SheetList.Count != 0;
	protected override bool IsShowAddButton => true;

	protected override void SetSheetData(string _filePath)
	{
		m_SheetList.Add(new ConfigSheetData(_filePath));
	}

	#region Config Sheet Data
	[Serializable]
	private class ConfigSheetData : SheetData
	{
		public static readonly string NewLineTapTap = $"{Environment.NewLine}\t\t";
		public static readonly string NewLineTapTapTap = $"{Environment.NewLine}\t\t\t";

		public static readonly string NewLineNewLineTapTap = $"{Environment.NewLine}{Environment.NewLine}\t\t";
		public static readonly string NewLineNewLineTapTapTap = $"{Environment.NewLine}{Environment.NewLine}\t\t\t";
		public static readonly string DoubleNewLine = $"{Environment.NewLine}{Environment.NewLine}";

		[HorizontalGroup("Menu/Name",Order = 1),ShowInInspector,LabelText("Sheet Name"),LabelWidth(100),ValueDropdown(nameof(m_SheetNameList))]
		public string SheetName
		{
			get => m_SheetName;
			private set
			{
				m_SheetName = value;

				m_ColumnList.Clear();

				var reader = new ExcelReader(AbsoluteFilePath);

				var columnJaggedArray = reader.GetColumnJaggedArray(m_SheetName,0,1);

				for(var i=1;i<columnJaggedArray[0].Length;i++)
				{
					m_ColumnList.Add(new ConfigCellData(columnJaggedArray[0][i].Replace(" ",""),DataType.String,false,columnJaggedArray[1][i]));
				}
			}
		}

		[Space(10)]
		[VerticalGroup("RowList",Order = 1),SerializeField,LabelText("Row List"),ListDrawerSettings(HideAddButton = true,ShowFoldout = false,DraggableItems = false),ShowIf(nameof(IsExistSheetName))]
		private List<ConfigCellData> m_ColumnList = new();
		private List<ConfigCellData> m_RowList = new();

		public ConfigSheetData(string _path) : base(_path) { }

		protected override string[] TitleArray => new[] { NAME, DEFAULT };
		protected override bool IsShowCreateButton => m_RowList.Count != 0;

		protected override bool IsCreateAble
		{
			get
			{
				m_ErrorLog = string.Empty;

				if(!IsExistSheetName)
				{
					m_ErrorLog = "Sheet Name is null.";

					return false;
				}

				if(ConfigType != null)
				{
					m_ErrorLog = $"{SheetName}Data is exist.";

					return false;
				}

				return true;
			}
		}

		public Type ConfigType => CommonUtility.FindType($"{SheetName}Data","ConfigData");

		protected override void OnRefreshSheet()
		{
			base.OnRefreshSheet();

			if(!m_SheetNameList.IsNullOrEmpty())
			{
				SheetName = m_SheetNameList[0];
			}
		}

		protected override void OnCreateData()
		{
			if(ConfigType == null)
			{
				// var fileName = $"{GameSettings.In.ConfigDataScriptPath}/{SheetName}Data.cs";
				// var scriptPath = CommonUtility.GetAbsolutePath(fileName,true);

				// //? Create script
				// WriteScript(scriptPath);
			}

			AssetDatabase.Refresh();
		}

		private void WriteScript(string _scriptPath)
		{
			if(CommonUtility.IsFileExist(_scriptPath))
			{
				LogTag.Editor.W($"Script is exist. [Path : {_scriptPath}]");

				return;
			}

			var data = CommonUtility.GetTemplateText("ConfigData.txt");

			data = data.Replace("$ClassName",SheetName);
			data = data.Replace("$MemberFields",MemberFields);
			data = data.Replace("$GeneralMemberProperties",GeneralMemberProperties);
			data = data.Replace("$InfoMemberProperties",InfoMemberProperties);
			data = data.Replace("$ClassConstructor",ClassConstructor);

			CommonUtility.WriteTextToFile(_scriptPath,data);
		}

		private string MemberFields
		{
			get
			{
				var builder = new StringBuilder();

				builder.Append(m_RowList[0].ToFieldText());
				builder.Append(NewLineTapTap);
				builder.Append(m_RowList[1].ToFieldText());
				builder.Append(NewLineNewLineTapTap);

				for(var i=2;i<m_RowList.Count;i++)
				{
					builder.Append(m_RowList[i].ToFieldText());
					builder.Append(NewLineTapTap);
				}

				return builder.ToString();
			}
		}

		private string GeneralMemberProperties
		{
			get
			{
				var builder = new StringBuilder();

				builder.Append(m_RowList[0].ToPropertyText("General",1));
				builder.Append(NewLineTapTap);
				builder.Append(m_RowList[1].ToPropertyText("General",2));

				return builder.ToString();
			}
		}

		private string InfoMemberProperties
		{
			get
			{
				var builder = new StringBuilder();

				for(var i=2;i<m_RowList.Count;i++)
				{
					builder.Append(m_RowList[i].ToPropertyText("Info",i+1));
					builder.Append(NewLineTapTap);
				}

				return builder.ToString();
			}
		}

		private string ClassConstructor
		{
			get
			{
				var builder = new StringBuilder();

				builder.Append($"public {SheetName}Data(bool _exist");

				for(var i=0;i<m_RowList.Count;i++)
				{
					builder.Append($",{m_RowList[i].ToConstructorArgument()}");
				}

				builder.Append($"){NewLineTapTap}{{");
				builder.Append($"{NewLineTapTapTap}");
				builder.Append($"{m_RowList[0].ToConstructorInitialize()}");
				builder.Append($"{NewLineTapTapTap}");
				builder.Append($"{m_RowList[1].ToConstructorInitialize()}");
				builder.Append($"{Environment.NewLine}");

				for(var i=2;i<m_RowList.Count;i++)
				{
					builder.Append($"{NewLineTapTapTap}");
					builder.Append($"{m_RowList[i].ToConstructorInitialize()}");
				}

				builder.Append($"{Environment.NewLine}");
				builder.Append($"{NewLineTapTapTap}");

				builder.Append($"m_Exist = _exist;{NewLineTapTap}}}");

				return builder.ToString();
			}
		}
	}
	#endregion Config Sheet Data

	#region Config Cell Data
	[Serializable]
	private class ConfigCellData
	{
		[SerializeField,HideInInspector] private string m_Name = null;
		[SerializeField,HideInInspector] private DataType m_Type = DataType.String;
		[SerializeField,HideInInspector] private bool m_IsArray = false;
		[SerializeField,HideInInspector] private string m_Default = null;

		[HorizontalGroup("0"),HideLabel,ShowInInspector,DisplayAsString]
		public string Name { get => m_Name; private set => m_Name = value; }

		[HorizontalGroup("0"),HideLabel,ShowInInspector]
		private DataType Type { get => m_Type; set => m_Type = value; }

		[HorizontalGroup("0"),ShowInInspector,LabelText("IsArray"),ToggleLeft]
		private bool IsArray { get => m_IsArray; set => m_IsArray = value; }

		[HorizontalGroup("0"),HideLabel,ShowInInspector,ReadOnly]
		private string Default { get => m_Default; set => m_Default = value; }

		public ConfigCellData(string _name,DataType _type,bool _isArray,string _default)
		{
			Name = _name;
			Type = _type;
			IsArray = _isArray;
			Default = _default;
		}

		public string ToConstructorArgument()
		{
			return $"{(string.Concat(DataTypeToString(),IsArray ? "[]" : ""))} _{Name.ToFirstCharacterToLower()}";
		}

		public string ToConstructorInitialize()
		{
			return $"m_{Name} = _{Name.ToFirstCharacterToLower()};";
		}

		public string ToFieldText()
		{
			return $"[SerializeField,HideInInspector] private {(string.Concat(DataTypeToString(),IsArray ? "[]" : ""))} m_{Name};";
		}

		public string ToPropertyText(string _group,int _key)
		{
			var builder = new StringBuilder();
			var type = DataTypeToString();

			if(IsArray)
			{
				builder.Append($"[HorizontalGroup(\"{_group}/0\"),LabelText(\"{Name}\"),LabelWidth(100),ShowInInspector,PropertyTooltip(\"${Name}_ToolTip\"),KZRichText]");
				builder.Append(ConfigSheetData.NewLineTapTap);
				builder.Append($"private string {Name}_Display => {Name}.IsNullOrEmpty() ? \"NULL\" : string.Join(\" | \",{Name});");
				builder.Append(ConfigSheetData.NewLineTapTap);
				builder.Append($"private string {Name}_ToolTip => {Name}_Display.RemoveRichText();");
				builder.Append(ConfigSheetData.NewLineNewLineTapTap);

				builder.Append($"[Key({_key})]");
				builder.Append(ConfigSheetData.NewLineTapTap);
				builder.Append($"public {type}[] {Name} {{ get => m_{Name}; private set => m_{Name} = value; }}");
			}
			else
			{
				builder.Append($"[HorizontalGroup(\"{_group}/0\"),LabelText(\"{Name}\"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip(\"${Name}\"),Key({_key})]");
				builder.Append(ConfigSheetData.NewLineTapTap);
				builder.Append($"public {type} {Name} {{ get => m_{Name}; private set => m_{Name} = value; }}");
			}

			return builder.ToString();
		}

		private string DataTypeToString()
		{
			return Type switch
			{
				DataType.Enum => Name,
				DataType.Vector3 => Type.ToString(),
				_ => Type.ToString().ToLowerInvariant(),
			};
		}
	}
	#endregion Config Cell Data

	public IEnumerable<Type> GetConfigTypeGroup()
	{
		foreach(var sheet in m_SheetList)
		{
			if(sheet.ConfigType != null)
			{
				yield return sheet.ConfigType;
			}
		}
	}

	public string GetFilePath(Type _type)
	{
		var sheet = m_SheetList.Find(x => x.ConfigType.Equals(_type));

		return sheet?.AbsoluteFilePath;
	}

	public void RefreshConfigData(Type _type)
	{
		var sheet = m_SheetList.Find(x => x.ConfigType.Equals(_type));

		if(sheet == null)
		{
			CommonUtility.DisplayError(new NullReferenceException($"{_type} sheet is not exist."));
		}

		var reader = new ExcelReader(sheet.AbsoluteFilePath);
		var dataList = Activator.CreateInstance(typeof(List<>).MakeGenericType(_type)) as IList;

		foreach(var data in reader.Deserialize(sheet.SheetName,sheet.ConfigType))
		{
			if(data == null || data is not IConfigData configData)
			{
				continue;
			}

			// dataList.Add(configData.Initialize());
		}

		if(dataList.Count < 1)
		{
			CommonUtility.DisplayError(new ArgumentException($"{_type} data count is 0."));
		}

		var bytes = MessagePackSerializer.Serialize(dataList,MessagePackSerializerOptions.Standard.WithResolver(MessagePackResolver.In));

		CommonUtility.WriteByteToFile(CommonUtility.GetAbsolutePath($"{GameSettings.In.ConfigDataFilePath}/{_type.Name}.bytes",true),bytes);

		AssetDatabase.Refresh();

		if(ConfigDataMgr.HasInstance)
		{
			// ConfigDataMgr.In.Reload();
		}
	}
}
#endif