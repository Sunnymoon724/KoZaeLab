#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEditor;
using System.Text;
using System.Linq;
using KZLib;
using KZLib.KZFiles;
using MessagePack;
using System.Collections;
using KZLib.KZResolver;

public class MetaSettings : SheetSettings<MetaSettings>
{
	private enum DataType { String, Short, Int, Long, Float, Double, Enum, Bool, Vector3, }

	private const string META_ID = "MetaId";
	private const string VERSION = "Version";

	[PropertySpace(10)]
	[HorizontalGroup("Sheet/List",Order = 1),SerializeField,LabelText(" "),ListDrawerSettings(ShowFoldout = false,HideAddButton = true,DraggableItems = false,NumberOfItemsPerPage = 1),ShowIf(nameof(IsShowSheet))]
	private List<MetaSheetData> m_SheetList = new();

	private bool IsShowSheet => m_SheetList.Count != 0;
	protected override bool IsShowAddButton => true;

	protected override void SetSheetData(string _filePath)
	{
		m_SheetList.Add(new MetaSheetData(_filePath));
	}

	#region Meta Sheet Data
	[Serializable]
	private class MetaSheetData : SheetData
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
				if(m_SheetName.IsEqual(value))
				{
					return;
				}

				m_SheetName = value;

				m_HeaderList.Clear();

				var excelFile = new ExcelFile(AbsoluteFilePath);

				foreach(var title in excelFile.GetTitleGroup(value))
				{
					m_HeaderList.Add(new MetaCellData(title.Title.Replace(" ",""),DataType.String,false));
				}
			}
		}

		[Space(10)]
		[VerticalGroup("HeaderList",Order = 1),SerializeField,LabelText("Header List"),ListDrawerSettings(HideAddButton = true,ShowFoldout = false,DraggableItems = false,CustomRemoveElementFunction = nameof(OnRemoveHeader)),ShowIf(nameof(IsExistSheetName))]
		private List<MetaCellData> m_HeaderList = new();

		private void OnRemoveHeader(MetaCellData _data)
		{
			var index = TitleArray.FindIndex(x=>x.IsEqual(_data.Name));

			if(index != -1)
			{
				return;
			}

			m_HeaderList.Remove(_data);
		}

		public MetaSheetData(string _path) : base(_path) { }

		protected override string[] TitleArray => new[] { META_ID, VERSION };
		protected override bool IsShowCreateButton => m_HeaderList.Count != 0;

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

				if(MetaType != null)
				{
					m_ErrorLog = $"{SheetName}Data is exist.";

					return false;
				}

				return true;
			}
		}

		public Type MetaType => ReflectionUtility.FindType($"{SheetName}Data","MetaData");

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
			var excelFile = new ExcelFile(AbsoluteFilePath);

			if(MetaType == null)
			{
				var fileName = $"{GameSettings.In.MetaDataScriptPath}/{SheetName}Data.cs";
				var scriptPath = CommonUtility.GetAbsolutePath(fileName,true);

				//? Create script
				WriteScript(scriptPath);
			}

			//? Create enum
			if(IsExistEnum())
			{
				var enumDict = excelFile.GetEnumDict(SheetName);
				var removeList = new List<string>(enumDict.Count);

				foreach(var key in enumDict.Keys)
				{
					var enumType = Type.GetType($"MetaData.{key}");

					if(enumType != null)
					{
						removeList.Add(key);
					}
				}

				foreach(var remove in removeList)
				{
					LogTag.Editor.W($"{remove} is exist.");

					enumDict.Remove(remove);
				}

				var builder = new StringBuilder();

				foreach(var pair in enumDict)
				{
					builder.Clear();

					foreach(var data in pair.Value)
					{
						builder.Append($"\t\t{data.Replace("\"","")},{Environment.NewLine}");
					}

					var enumText = $"{Environment.NewLine}\tpublic enum {pair.Key}{Environment.NewLine}\t{{{Environment.NewLine}{builder}\t}}";

					CommonUtility.AddOrUpdateTemplateText(GameSettings.In.MetaDataScriptPath,"MetaDataEnum.txt","MetaDataEnum.cs",enumText,(text)=>
					{
						var footer = text[..text.LastIndexOf("}")];

						return string.Concat(footer,enumText,Environment.NewLine,"}");
					});
				}
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

			var data = CommonUtility.GetTemplateText("MetaData.txt");

			data = data.Replace("$ClassName",SheetName);
			data = data.Replace("$MemberFields",MemberFields);
			data = data.Replace("$GeneralMemberProperties",GeneralMemberProperties);
			data = data.Replace("$InfoMemberProperties",InfoMemberProperties);
			data = data.Replace("$ClassConstructor",ClassConstructor);

			CommonUtility.WriteTextToFile(_scriptPath,data);
		}

		private bool IsExistEnum()
		{
			foreach(var data in m_HeaderList.Where(x=>x.IsEnumType))
			{
				return true;
			}

			return false;
		}

		private string MemberFields
		{
			get
			{
				var builder = new StringBuilder();

				builder.Append(m_HeaderList[0].ToFieldText());
				builder.Append(NewLineTapTap);
				builder.Append(m_HeaderList[1].ToFieldText());
				builder.Append(NewLineNewLineTapTap);

				for(var i=2;i<m_HeaderList.Count;i++)
				{
					builder.Append(m_HeaderList[i].ToFieldText());
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

				builder.Append(m_HeaderList[0].ToPropertyText("General",1));
				builder.Append(NewLineTapTap);
				builder.Append(m_HeaderList[1].ToPropertyText("General",2));

				return builder.ToString();
			}
		}

		private string InfoMemberProperties
		{
			get
			{
				var builder = new StringBuilder();

				for(var i=2;i<m_HeaderList.Count;i++)
				{
					builder.Append(m_HeaderList[i].ToPropertyText("Info",i+1));
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

				for(var i=0;i<m_HeaderList.Count;i++)
				{
					builder.Append($",{m_HeaderList[i].ToConstructorArgument()}");
				}

				builder.Append($"){NewLineTapTap}{{");
				builder.Append($"{NewLineTapTapTap}");
				builder.Append($"{m_HeaderList[0].ToConstructorInitialize()}");
				builder.Append($"{NewLineTapTapTap}");
				builder.Append($"{m_HeaderList[1].ToConstructorInitialize()}");
				builder.Append($"{Environment.NewLine}");

				for(var i=2;i<m_HeaderList.Count;i++)
				{
					builder.Append($"{NewLineTapTapTap}");
					builder.Append($"{m_HeaderList[i].ToConstructorInitialize()}");
				}

				builder.Append($"{Environment.NewLine}");
				builder.Append($"{NewLineTapTapTap}");

				builder.Append($"m_Exist = _exist;{NewLineTapTap}}}");

				return builder.ToString();
			}
		}
	}
	#endregion Meta Sheet Data

	#region Meta Cell Data
	[Serializable]
	private class MetaCellData
	{
		[SerializeField,HideInInspector] private string m_Name = null;
		[SerializeField,HideInInspector] private DataType m_Type = DataType.String;
		[SerializeField,HideInInspector] private bool m_IsArray = false;

		[HorizontalGroup("0"),HideLabel,ShowInInspector,DisplayAsString]
		public string Name { get => m_Name; private set => m_Name = value; }

		[HorizontalGroup("0"),HideLabel,ShowInInspector]
		private DataType Type
		{
			get => m_Type;
			set => m_Type = Name.IsEqual(META_ID) ? DataType.Int : Name == VERSION ? DataType.String : value;
		}
        public bool IsEnumType => Type == DataType.Enum;

		[HorizontalGroup("0"),HideLabel,ShowInInspector,LabelText("IsArray"),ToggleLeft]
		private bool IsArray
		{
			get => m_IsArray;
			set => m_IsArray = Name != META_ID && Name != VERSION && value;
		}

		public MetaCellData(string _name,DataType _type,bool _isArray)
		{
			Name = _name;
			Type = _type;
			IsArray = _isArray;
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
				builder.Append(MetaSheetData.NewLineTapTap);
				builder.Append($"private string {Name}_Display => {Name}.IsNullOrEmpty() ? \"NULL\" : string.Join(\" | \",{Name});");
				builder.Append(MetaSheetData.NewLineTapTap);
				builder.Append($"private string {Name}_ToolTip => {Name}_Display.RemoveRichText();");
				builder.Append(MetaSheetData.NewLineNewLineTapTap);

				builder.Append($"[Key({_key})]");
				builder.Append(MetaSheetData.NewLineTapTap);
				builder.Append($"public {type}[] {Name} {{ get => m_{Name}; private set => m_{Name} = value; }}");
			}
			else
			{
				builder.Append($"[HorizontalGroup(\"{_group}/0\"),LabelText(\"{Name}\"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip(\"${Name}\"),Key({_key})]");
				builder.Append(MetaSheetData.NewLineTapTap);
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
	#endregion Meta Cell Data

	public IEnumerable<Type> GetMetaTypeGroup()
	{
		foreach(var sheet in m_SheetList)
		{
			if(sheet.MetaType != null)
			{
				yield return sheet.MetaType;
			}
		}
	}

	public string GetFilePath(Type _type)
	{
		var sheet = m_SheetList.Find(x => x.MetaType.Equals(_type));

		return sheet?.AbsoluteFilePath;
	}

	public void RefreshMetaData(Type _type)
	{
		var sheet = m_SheetList.Find(x => x.MetaType.Equals(_type));

		if(sheet == null)
		{
			CommonUtility.DisplayError(new NullReferenceException($"{_type} sheet is not exist."));
		}

		var excelFile = new ExcelFile(sheet.AbsoluteFilePath);
		var dataList = Activator.CreateInstance(typeof(List<>).MakeGenericType(_type)) as IList;

		foreach(var data in excelFile.Deserialize(sheet.SheetName,sheet.MetaType))
		{
			if(data == null || data is not IMetaData metaData)
			{
				continue;
			}

			dataList.Add(metaData.Initialize());
		}

		if(dataList.Count < 1)
		{
			CommonUtility.DisplayError(new ArgumentException($"{_type} data count is 0."));
		}

		var bytes = MessagePackSerializer.Serialize(dataList,MessagePackSerializerOptions.Standard.WithResolver(MessagePackResolver.In));

		CommonUtility.WriteByteToFile(CommonUtility.GetAbsolutePath($"{GameSettings.In.MetaDataFilePath}/{_type.Name}.bytes",true),bytes);

		AssetDatabase.Refresh();

		if(MetaDataMgr.HasInstance)
		{
			MetaDataMgr.In.Reload();
		}
	}
}
#endif