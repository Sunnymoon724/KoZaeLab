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

			if(MetaType != null)
			{
				var fileName = $"{GameSettings.In.MetaDataScriptPath}/{SheetName}Data.cs";
				var scriptPath = FileUtility.GetAbsolutePath(fileName,true);

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

					FileUtility.AddOrUpdateTemplateText(GameSettings.In.MetaDataScriptPath,"MetaDataEnum.txt","MetaDataEnum.cs",enumText,(text)=>
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
			if(FileUtility.IsExist(_scriptPath))
			{
				LogTag.Editor.W($"Script is exist. [Path : {_scriptPath}]");

				return;
			}

			var data = FileUtility.GetTemplateText("MetaData.txt");

			data = data.Replace("$ClassName",SheetName);
			data = data.Replace("$MemberFields",MemberFields);
			data = data.Replace("$MemberProperties",MemberProperties);
			data = data.Replace("$SheetName",SheetName);

			FileUtility.WriteTextToFile(_scriptPath,data);
		}

		private bool IsExistEnum()
		{
			foreach(var data in m_HeaderList.Where(x=>x.IsEnumType))
			{
				return true;
			}

			return false;
		}

		private string MemberFields => MergeMemberData((member)=>{ return member.ToFieldText(); },$"{Environment.NewLine}\t\t");
		private string MemberProperties => MergeMemberData((member)=>{ return member.ToPropertyText(); },$"{Environment.NewLine}{Environment.NewLine}\t\t");

		private string MergeMemberData(Func<MetaCellData,string> _onGetData,string _spaceText)
		{
			var builder = new StringBuilder();

			builder.Append(_onGetData(m_HeaderList[0]));

			for(var i=1;i<m_HeaderList.Count;i++)
			{
				builder.Append(_spaceText);
				builder.Append(_onGetData(m_HeaderList[i]));
			}

			return builder.ToString();
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

		public string ToFieldText()
		{
			return $"[SerializeField,HideInInspector] private {(string.Concat(DataTypeToString(),IsArray ? "[]" : ""))} m_{Name};";
		}

		public string ToPropertyText()
		{
			var builder = new StringBuilder();
			var type = DataTypeToString();

			if(IsArray)
			{
				// Add Header
				builder.Append($"[HorizontalGroup(\"General/0\"),LabelText(\"{Name}\"),ShowInInspector,PropertyTooltip(\"${Name}_ToolTip\"),KZRichText]{Environment.NewLine}\t\t");

				// Add Display
				builder.Append($"private string {Name}_Display => {Name}.IsNullOrEmpty() ? \"NULL\" : string.Join(\" | \",{Name});{Environment.NewLine}\t\t");
				// Add ToolTip
				builder.Append($"private string {Name}_ToolTip => {Name}_Display.RemoveRichText();{Environment.NewLine}{Environment.NewLine}\t\t");

				// Add Property
				builder.Append($"private {type}[] {Name} {{ get => m_{Name}; set => m_{Name} = value; }}{Environment.NewLine}\t\t");

				// Add IEnumerable
				builder.Append($"public IEnumerable<{type}> {Name}Group => m_{Name};");
			}
			else
			{
				// Add Header
				builder.Append($"[HorizontalGroup(\"General/0\"),LabelText(\"{Name}\"),ShowInInspector,DisplayAsString,PropertyTooltip(\"${Name}\")]{Environment.NewLine}\t\t");

				// Add Property
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
		var sheet = m_SheetList.Find(x => x.MetaType.Equals(_type)) ?? throw new NullReferenceException($"{_type} sheet is not exist.");
		var excelFile = new ExcelFile(sheet.AbsoluteFilePath);
		var dataList = Activator.CreateInstance(typeof(List<>).MakeGenericType(_type)) as IList;

		foreach(var data in excelFile.Deserialize(sheet.SheetName,sheet.MetaType))
		{
			if(data == null || data is not IMetaData metaData)
			{
				continue;
			}

			dataList.Add(metaData);
		}

		if(dataList.Count < 1)
		{
			throw new ArgumentException($"{_type} data count is 0.");
		}

		var bytes = MessagePackSerializer.Serialize(dataList);

		FileUtility.WriteByteToFile(FileUtility.GetAbsolutePath($"{GameSettings.In.MetaDataFilePath}/{_type.Name}.bytes",true),bytes);

		AssetDatabase.Refresh();

		if(MetaDataMgr.HasInstance)
		{
			MetaDataMgr.In.Reload();
		}
	}
}
#endif