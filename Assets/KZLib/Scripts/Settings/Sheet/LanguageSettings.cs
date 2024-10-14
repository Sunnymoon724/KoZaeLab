#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEditor;
using System.Linq;
using KZLib;
using KZLib.KZAttribute;
using Newtonsoft.Json;
using KZLib.KZFiles;

public class LanguageSettings : SheetSettings<LanguageSettings>
{
	private const string LANGUAGE_KEY = "Key";
	private const string ENGLISH = "English";

	[PropertySpace(10)]
	[HorizontalGroup("Sheet/Data",Order = 1),SerializeField,HideLabel,HideIf(nameof(IsShowAddButton))]
	private LanguageSheetData m_SheetData = null;

	protected override bool IsShowAddButton => m_SheetData == null;

	protected override void SetSheetData(string _filePath)
	{
		m_SheetData = new LanguageSheetData(_filePath);
	}

	#region Language Sheet Data
	[Serializable]
	private class LanguageSheetData : SheetData
	{
		[HorizontalGroup("Menu/Name",Order = 1),ShowInInspector,LabelText("Sheet Name"),KZRichText]
		private string SheetName => m_SheetName;

		[Space(10)]
		[VerticalGroup("DataList",Order = 1),SerializeField,LabelText("Language List"),ListDrawerSettings(ShowFoldout = false,HideAddButton = true,HideRemoveButton = true,DraggableItems = false)]
		private List<LanguageData> m_LanguageDataList = new();

		public LanguageSheetData(string _path) : base(_path) { }

		protected override string[] TitleArray => new[] { LANGUAGE_KEY, ENGLISH };
		protected override bool IsShowCreateButton => m_LanguageDataList.Count != 0;

		protected override bool IsCreateAble
		{
			get
			{
				m_ErrorLog = string.Empty;

				if(!m_LanguageDataList.Any(x => x.IsInclude))
				{
					m_ErrorLog = "No Language Data.";

					return false;
				}

				return true;
			}
		}

		protected override void OnRefreshSheet()
		{
			base.OnRefreshSheet();

			m_SheetName = m_SheetNameList.IsNullOrEmpty() ? null : m_SheetNameList[0];

			if(!IsExistSheetName)
			{
				return;
			}

			m_LanguageDataList.Clear();

			var excelFile = new ExcelFile(AbsoluteFilePath);

			foreach(var title in excelFile.GetTitleGroup(m_SheetName))
			{
				if(title.Title.IsEnumDefined<SystemLanguage>())
				{
					m_LanguageDataList.Add(new LanguageData(title.Title));
				}
			}
		}

		protected override void OnCreateData()
		{
			var excelFile = new ExcelFile(AbsoluteFilePath);
			var titleGroup = excelFile.GetTitleGroup(SheetName);
			var keyArray = excelFile.GetColumnGroup(SheetName,0).ToArray();
			var languageGroupArray = excelFile.GetColumnGroupArray(SheetName,titleGroup.Select(x=>x.Index).ToArray());

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

					if(!languageDict.ContainsKey(key))
					{
						languageDict.Add(key, text);
					}
					else
					{
						LogTag.Editor.W($"{key} is duplicated.");
					}
				}

				if(languageDict.IsNullOrEmpty())
				{
					continue;
				}

				var result = JsonConvert.SerializeObject(languageDict,Formatting.Indented);
				var filePath = $"{GameSettings.In.LanguageFilePath}/{language}.json";

				FileUtility.WriteTextToFile(FileUtility.GetAbsolutePath(filePath,false),result);

				languageList.Add(language);
			}

			AssetDatabase.Refresh();

			if(GameDataMgr.HasInstance)
			{
				GameDataMgr.In.Clear<GameData.Option>();
			}

			UnityUtility.DisplayInfo($"Language is created. [{string.Join(",",languageList)}]");
		}
	}
	#endregion Language Sheet Data

	#region Language Data
	[Serializable]
	private class LanguageData
	{
		[LabelText("$m_Language"),SerializeField,ToggleLeft]
		private bool m_Include = true;

		[SerializeField,HideInInspector]
		private string m_Language = null;

		public bool IsInclude => m_Include;
		public string Language => m_Language;

		public LanguageData(string _language)
		{
			m_Language = _language;
			m_Include = true;
		}
	}
	#endregion Language Data
}
#endif