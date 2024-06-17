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
using KZLib.KZFiles;

/// <summary>
/// 언어 현지화 세팅
/// </summary>
public partial class LanguageSettings : ExcelSettings<LanguageSettings>
{
	[Serializable]
	private class LanguageSheetData : ExcelSheetData
	{
	#pragma warning disable IDE0051
		[HorizontalGroup(" /0/2",Order = 2),ShowInInspector,LabelText("시트"),LabelWidth(100),KZRichText]
		private string SheetName => m_SheetName;

		[Space(5)]
		[VerticalGroup(" /1",Order = 1),SerializeField,LabelText("언어 리스트"),TableList(IsReadOnly = true,AlwaysExpanded = true)]
		private List<LanguageData> m_LanguageDataList = new();

		public LanguageSheetData(string _path) : base(_path) { }

		protected override string[] TitleArray => new[] { LANGUAGE_KEY, ENGLISH };

		protected override void OnRefreshSheet()
		{
			base.OnRefreshSheet();

			m_SheetName = m_SheetNameList.IsNullOrEmpty() ? null : m_SheetNameList[0];

			if(IsExistSheetName)
			{
				m_LanguageDataList.Clear();

				var excelFile = new ExcelFile(m_LocalFilePath);

				foreach(var title in excelFile.GetTitleGroup(m_SheetName))
				{
					m_LanguageDataList.Add(new LanguageData(title.Item1,true));
				}
			}
		}

		public override bool IsCreateAble(out string _errorLog)
		{
			_errorLog = string.Empty;

			var count = m_LanguageDataList.Count(x=>x.IsInclude);

			if(count == 0)
			{
				_errorLog = "생성할 언어가 없습니다.";

				return false;
			}

			return true;
		}

		public override void CreateData()
		{
			var excelFile = new ExcelFile(m_LocalFilePath);
			var titleGroup = excelFile.GetTitleGroup(m_SheetName);
            var keyArray = excelFile.GetColumnGroup(m_SheetName,0).ToArray();
            var languageGroupArray = excelFile.GetColumnGroupArray(m_SheetName,titleGroup.Select(x=>x.Item2).ToArray());

			var localizeDict = new Dictionary<string,string>();

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

				localizeDict.Clear();

				var index = 1;

				foreach(var language2 in languageGroup.Skip(1))
				{
					var text = language2;

					if(text.Contains("\\n"))
					{
						text = text.NormalizeNewLines();
					}

					var key = keyArray[index++];

					if(localizeDict.ContainsKey(key))
					{
						Log.Editor.W("{0}가 중복되어 생략했습니다.",key);

						continue;
					}

					localizeDict.Add(key,text);
				}

				if(localizeDict.IsNullOrEmpty())
				{
					continue;
				}

				var result = JsonConvert.SerializeObject(localizeDict,Formatting.Indented);

				CommonUtility.WriteDataToFile(CommonUtility.GetFullPath(CommonUtility.PathCombine(GameSettings.In.TranslatePath,string.Format("{0}.json",language))),result);
			}

			AssetDatabase.Refresh();

			if(GameDataMgr.HasInstance)
			{
				GameDataMgr.In.Clear<GameData.Option>();
			}

			CommonUtility.DisplayInfo("언어팩이 완성 되었습니다.");
		}
#pragma warning restore IDE0051
	}
}
#endif