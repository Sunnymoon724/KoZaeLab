#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEditor;
using System.Linq;
using KZLib;
using KZLib.KZAttribute;
using Newtonsoft.Json;

/// <summary>
/// 언어 현지화 세팅
/// </summary>
public partial class LanguageSettings : ExcelSettings<LanguageSettings>
{
	[Serializable]
	private class LanguageSheetData : ExcelSheetData
	{
		[HorizontalGroup(" /0/1",Order = 1),ShowInInspector,LabelText("시트"),LabelWidth(100),KZRichText]
		private string SheetName => m_SheetName;

		[Space(5)]
		[VerticalGroup(" /1",Order = 1),SerializeField,LabelText("언어 리스트"),TableList(IsReadOnly = true,AlwaysExpanded = true)]
		private List<LanguageData> m_LanguageDataList = new();

		public LanguageSheetData(string _path) : base(_path) { }

		protected override string[] TitleArray => new[] { LANGUAGE_KEY, ENGLISH };

		protected override bool IsShowCreateButton => m_LanguageDataList.Count != 0;

		protected override bool IsCreateAble
		{
			get
			{
				m_ErrorLog = string.Empty;

				var count = m_LanguageDataList.Count(x=>x.IsInclude);

				if(count == 0)
				{
					m_ErrorLog = "생성할 언어가 없습니다.";

					return false;
				}

				return true;
			}
		}

		protected override void OnRefreshSheet()
		{
			base.OnRefreshSheet();

			m_SheetName = m_SheetNameList.IsNullOrEmpty() ? null : m_SheetNameList[0];

			if(IsExistSheetName)
			{
				m_LanguageDataList.Clear();

				var excelFile = GetExcelFile();

				foreach(var title in excelFile.GetTitleGroup(m_SheetName))
				{
					if(title.Item1.IsEnumDefined<SystemLanguage>())
					{
						m_LanguageDataList.Add(new LanguageData(title.Item1,true));
					}
				}
			}
		}

		[VerticalGroup(" /1",Order = 1),SerializeField,LabelText("언어 리스트"),TableList(IsReadOnly = true,AlwaysExpanded = true)]
		protected override void OnCreateData()
		{
			var excelFile = GetExcelFile();
			var titleGroup = excelFile.GetTitleGroup(SheetName);
            var keyArray = excelFile.GetColumnGroup(SheetName,0).ToArray();
            var languageGroupArray = excelFile.GetColumnGroupArray(SheetName,titleGroup.Select(x=>x.Item2).ToArray());

			var languageDict = new Dictionary<string,string>();
			var languageList = new List<string>();

			for(var i=1;i<languageGroupArray.Length;i++)
			{
				var languageGroup = languageGroupArray[i];
				var language = languageGroup.First();

				if(!language.IsEnumDefined<SystemLanguage>())
				{
					continue;
				}

				var data = m_LanguageDataList.Find(x=>x.Language == language);

				if(data == null || !data.IsInclude)
				{
					continue;
				}

				languageDict.Clear();

				var index = 1;

				foreach(var language2 in languageGroup.Skip(1))
				{
					var text = language2;

					if(text.Contains("\\n"))
					{
						text = text.NormalizeNewLines();
					}

					var key = keyArray[index++];

					if(languageDict.ContainsKey(key))
					{
						LogTag.Editor.W("{0}가 중복되어 생략했습니다.",key);

						continue;
					}

					languageDict.Add(key,text);
				}

				if(languageDict.IsNullOrEmpty())
				{
					continue;
				}

				var result = JsonConvert.SerializeObject(languageDict,Formatting.Indented);
				var filePath = string.Format("{0}/{1}.json",GameSettings.In.LanguagePath,language);

				FileUtility.WriteTextToFile(FileUtility.GetAbsolutePath(filePath,false),result);

				languageList.Add(language);
			}

			AssetDatabase.Refresh();

			if(GameDataMgr.HasInstance)
			{
				GameDataMgr.In.Clear<GameData.Option>();
			}

			UnityUtility.DisplayInfo(string.Format("언어팩이 완성 되었습니다. [{0}]",string.Join(",",languageList)));
		}
	}
}
#endif