using System;
using System.Collections.Generic;
using KZLib;
using KZLib.KZUtility;
using Newtonsoft.Json;
using UnityEngine;

namespace GameData
{
	public class LanguageOption : Option
	{
		// https://docs.google.com/document/d/1x3wfhsAR4urxCOhi3jqwoCRwp7DUmZqGPip6rsrGF_k/edit?usp=sharing
		protected override string OptionKey => "Language Option";
		protected override EventTag OptionTag => EventTag.ChangeLanguageOption;

		private class LanguageData : IOptionData
		{
			[JsonProperty("GameLanguage")]
			private SystemLanguage m_language = GameSettings.In.GameLanguage;

			[JsonIgnore]
			public SystemLanguage Language => m_language;

			public bool TrySetLanguage(SystemLanguage language)
			{
				if(m_language == language)
				{
					return false;
				}

				m_language = language;

				return true;
			}
		}

		private LanguageData m_languageData = null;

		private readonly Dictionary<SystemLanguage,Dictionary<string,string>> m_languageTextDict = new();

		public override void Initialize()
		{
			base.Initialize();

			m_languageData = LoadOptionData<LanguageData>();

			m_languageTextDict.Clear();

			foreach(var textAsset in ResMgr.In.GetTextAssetArray(GameSettings.In.LanguageFilePath))
			{
				var languageDict = JsonConvert.DeserializeObject<Dictionary<string,string>>(textAsset.text);

				if(languageDict.IsNullOrEmpty())
				{
					continue;
				}

				m_languageTextDict.Add(textAsset.name.ToEnum<SystemLanguage>(),languageDict);
			}
		}

		public override void Release()
		{
			m_languageTextDict.Clear();

			SaveOptionData(m_languageData,false);
		}

		public SystemLanguage GameLanguage
		{
			get => m_languageData.Language;
			set
			{
				if(!m_languageTextDict.ContainsKey(value))
				{
					LogTag.System.W($"{value} is not exist.");

					return;
				}

				if(!m_languageData.TrySetLanguage(value))
				{
					return;
				}

				LogTag.System.I($"Language is changed. [{value}]");

				SaveOptionData(m_languageData,true);
			}
		}

		public string ToLocalizeText(string key)
		{
			if(m_languageTextDict.IsNullOrEmpty())
			{
				return key;
			}

			if(m_languageTextDict.TryGetValue(m_languageData.Language,out var dataDict))
			{
				if(dataDict.TryGetValue(key,out var value))
				{
					return value;
				}
			}
			else
			{
				if(m_languageTextDict[SystemLanguage.English].TryGetValue(key,out var value))
				{
					return value;
				}
			}

			return key;
		}

#if UNITY_EDITOR
		public IEnumerable<string> GetLanguageGroup(string key)
		{
			foreach(var (language,dataDict) in m_languageTextDict)
			{
				yield return dataDict.TryGetValue(key,out var text) ? $"{language} {text}" : $"Undefined {key}";
			}
		}

		public string FindKey(string text)
		{
			if(text.IsEmpty())
			{
				return string.Empty;
			}

			foreach(var dict in m_languageTextDict.Values)
			{
				foreach(var pair in dict)
				{
					if(pair.Value.IsEqual(text))
					{
						return pair.Key;
					}
				}
			}

			return string.Empty;
		}
#endif
	}
}

public static class LocalizeExtension
{
	public static string ToLocalize(this string key)
	{
		if(!key.IsEmpty())
		{
			return key;
		}

		var option = GameDataMgr.In.Access<GameData.LanguageOption>();

		return option.ToLocalizeText(key);
	}
}