#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEditor;
using System.Text;
using KZLib.KZFiles;

/// <summary>
/// 메타 테이블을 세팅
/// </summary>
public partial class MetaSettings : ExcelSettings<MetaSettings>
{
	[Serializable]
	private class MetaSheetData : ExcelSheetData
	{
		[HorizontalGroup(" /0/2",Order = 2),ShowInInspector,LabelText("시트"),LabelWidth(100),ValueDropdown(nameof(m_SheetNameList))]
		public string SheetName
		{
			get => m_SheetName;
			private set
			{
				m_SheetName = value;

				m_HeaderList.Clear();

				var excelFile = new ExcelFile(m_LocalFilePath);

				foreach(var title in excelFile.GetTitleGroup(value))
				{
					m_HeaderList.Add(new CellData(title.Item1.Replace(" ",""),DataType.String,false,title.Item2));
				}
			}
		}

		[Space(5)]
		[VerticalGroup(" /1",Order = 1),SerializeField,LabelText(" "),ListDrawerSettings(HideAddButton = true,HideRemoveButton = false,ShowFoldout = false,DraggableItems = false),ShowIf(nameof(IsExistSheetName))]
		private List<CellData> m_HeaderList = new();

		public MetaSheetData(string _path) : base(_path) { }

		protected override string[] TitleArray => new[] { META_ID, VERSION };

		protected override void OnRefreshSheet()
		{
			base.OnRefreshSheet();

			if(IsExistSheetName && !m_SheetNameList.Contains(m_SheetName))
			{
				m_SheetName = null;
				m_HeaderList.Clear();
			}
		}

		public override bool IsCreateAble(out string _errorLog)
		{
			_errorLog = string.Empty;

			if(!IsExistSheetName)
			{
				_errorLog = "시트 이름이 NULL 입니다.";

				return false;
			}

			if(Type.GetType(string.Format("MetaData.{0}Table",SheetName)) != null)
			{
				_errorLog = string.Format("{0}Table이 이미 존재 합니다.",SheetName);

				return false;
			}

			return true;
		}

		public override void CreateData()
		{
			var excelFile = new ExcelFile(m_LocalFilePath);
			var classType = Type.GetType(string.Format("MetaData.{0}Table",SheetName));
			var scriptData = new ScriptData(SheetName,SheetName,m_HeaderList);

			if(classType == null)
			{
				//? script 만들기
				var scriptPath = CommonUtility.PathCombine(CommonUtility.GetFullPath(GameSettings.In.MetaScriptPath),string.Format("{0}Table.cs",SheetName));

				scriptData.WriteScript(scriptPath);
			}

			//? enum 만들기
			if(scriptData.IsInEnum(out var orderList))
			{
				var enumDict = excelFile.GetEnumDict(SheetName,orderList);
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

				CreateEnum(enumDict);
			}

			AssetDatabase.Refresh();
		}

		private void CreateEnum(Dictionary<string,string[]> _dataDict)
		{
			var builder = new StringBuilder();

			foreach(var pair in _dataDict)
			{
				builder.Clear();

				foreach(var data in pair.Value)
				{
					builder.AppendFormat("\t\t{0},{1}",data.Replace("\"",""),Environment.NewLine);
				}

				var enumHeader = pair.Key;
				var enumText = string.Format("{0}\tpublic enum {1}{0}{2}{0}",Environment.NewLine,enumHeader,builder.ToString());

				CommonUtility.AddOrUpdateTemplateText(GameSettings.In.MetaScriptPath,"MetaDataEnum.txt","MetaDataEnum.cs",enumText,(text)=>
				{
					var footer = text[..text.LastIndexOf("}")];

					return string.Concat(footer,enumText,Environment.NewLine,"}");
				});
			}
		}
	}
}
#endif