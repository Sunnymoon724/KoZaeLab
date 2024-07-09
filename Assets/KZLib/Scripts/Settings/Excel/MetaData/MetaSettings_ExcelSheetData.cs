#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEditor;
using System.Text;

/// <summary>
/// 메타 테이블을 세팅
/// </summary>
public partial class MetaSettings : ExcelSettings<MetaSettings>
{
	[Serializable]
	private class MetaSheetData : ExcelSheetData
	{
		[HorizontalGroup(" /0/1",Order = 1),ShowInInspector,LabelText("시트"),LabelWidth(100),ValueDropdown(nameof(m_SheetNameList))]
		public string SheetName
		{
			get => m_SheetName;
			private set
			{
				m_SheetName = value;

				m_HeaderList.Clear();

				var excelFile = GetExcelFile();

				foreach(var title in excelFile.GetTitleGroup(value))
				{
					m_HeaderList.Add(new ExcelCellData(title.Item1.Replace(" ",""),DataType.String,false));
				}
			}
		}

		[Space(5)]
		[VerticalGroup(" /1",Order = 1),SerializeField,LabelText(" "),ListDrawerSettings(HideAddButton = true,ShowFoldout = false,DraggableItems = false,CustomRemoveElementFunction = nameof(OnRemoveHeader)),ShowIf(nameof(IsExistSheetName))]
		private List<ExcelCellData> m_HeaderList = new();

		private void OnRemoveHeader(ExcelCellData _data)
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
					m_ErrorLog = "시트 이름이 NULL 입니다.";

					return false;
				}

				if(MetaType != null)
				{
					m_ErrorLog = string.Format("{0}Table이 이미 존재 합니다.",SheetName);

					return false;
				}

				return true;
			}
		}

		public Type MetaType => ReflectionUtility.FindType(string.Format("MetaData.{0}Table",SheetName),"Assembly-CSharp");

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
			// 엑셀을 읽어서 스크립트를 생성합니다.
			var excelFile = GetExcelFile();
			var scriptData = new ScriptData(SheetName,SheetName,m_HeaderList);

			if(MetaType == null)
			{
				var scriptPath = CommonUtility.PathCombine(CommonUtility.GetFullPath(GameSettings.In.MetaScriptPath),string.Format("{0}Table.cs",SheetName));

				//? script 만들기
				scriptData.WriteScript(scriptPath);
			}

			//? enum 만들기
			if(scriptData.IsInEnum())
			{
				var enumDict = excelFile.GetEnumDict(SheetName);
				var removeList = new List<string>(enumDict.Count);

				foreach(var key in enumDict.Keys)
				{
					var enumType = Type.GetType(string.Format("MetaData.{0}",key));

					if(enumType != null)
					{
						removeList.Add(key);
					}
				}

				foreach(var remove in removeList)
				{
					Log.Data.W("{0}는 이미 존재하여서 생략합니다.",remove);

					enumDict.Remove(remove);
				}

				var builder = new StringBuilder();

				foreach(var pair in enumDict)
				{
					builder.Clear();

					foreach(var data in pair.Value)
					{
						builder.AppendFormat("\t\t{0},{1}",data.Replace("\"",""),Environment.NewLine);
					}

					var enumText = string.Format("{0}\tpublic enum {1}{0}\t{{{0}{2}\t}}",Environment.NewLine,pair.Key,builder.ToString());

					CommonUtility.AddOrUpdateTemplateText(GameSettings.In.MetaScriptPath,"MetaDataEnum.txt","MetaDataEnum.cs",enumText,(text)=>
					{
						var footer = text[..text.LastIndexOf("}")];

						return string.Concat(footer,enumText,Environment.NewLine,"}");
					});
				}
			}

			AssetDatabase.Refresh();
		}
	}
}
#endif