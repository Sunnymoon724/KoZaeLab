using System;
using System.Collections.Generic;
using KZLib;
using Newtonsoft.Json;
using UnityEngine;

namespace GameData
{
	public class LanguageOption : Option
	{
		protected override string OPTION_KEY => "Language Option";
		protected override EventTag Tag => EventTag.ChangeLanguageOption;

		private class LanguageData
		{
			public SystemLanguage Language { get; set; }
		}

		// https://docs.google.com/document/d/1x3wfhsAR4urxCOhi3jqwoCRwp7DUmZqGPip6rsrGF_k/edit?usp=sharing
		private static SystemLanguage DefaultLanguage => GameSettings.In.DefaultLanguage;

		private LanguageData m_LanguageData = null;

		private readonly Dictionary<SystemLanguage,Dictionary<string,string>> m_LanguageDict = new();

		public override void Initialize()
		{
			m_LanguageData = GetOption(new LanguageData()
			{
				Language = DefaultLanguage,
			});

			m_LanguageDict.Clear();

			foreach(var textAsset in ResMgr.In.GetTextAssetArray(GameSettings.In.LanguagePath))
			{
				var languageDict = JsonConvert.DeserializeObject<Dictionary<string,string>>(textAsset.text);

				if(languageDict.IsNullOrEmpty())
				{
					continue;
				}

				m_LanguageDict.Add(textAsset.name.ToEnum<SystemLanguage>(),languageDict);
			}
		}

		public override void Release()
		{
			m_LanguageDict.Clear();
		}

		public void SetGameLanguage(SystemLanguage _language)
		{
			if(m_LanguageData.Language == _language)
			{
				return;
			}

			if(!m_LanguageDict.ContainsKey(_language))
			{
				LogTag.Data.W("해당 언어의 번역본이 없어서 변경하지 않았습니다. [{0}]",_language);

				return;
			}

			LogTag.Data.I("언어가 바꿨습니다. [{0}]",_language);

			m_LanguageData.Language = _language;

			SaveOption(m_LanguageData);
		}

		public string ToLocalizeText(string _key)
		{
			if(m_LanguageDict.IsNullOrEmpty())
			{
				throw new NullReferenceException("번역 데이터가 비어있습니다.");
			}

			if(m_LanguageDict.TryGetValue(m_LanguageData.Language,out var dataDict))
			{
				if(dataDict.TryGetValue(_key,out var value))
				{
					return value;
				}
			}
			else
			{
				if(m_LanguageDict[DefaultLanguage].TryGetValue(_key,out var value))
				{
					return value;
				}
			}

			return _key;
		}

#if UNITY_EDITOR
		public IEnumerable<string> GetLanguageGroup(string _key)
		{
			var textList = new List<string>();

			foreach(var pair in m_LanguageDict)
			{
				if(pair.Value.TryGetValue(_key,out var value))
				{
					textList.Add(string.Format("[{0}] {1}",pair.Key,value));
				}
				else
				{
					textList.Add(string.Format("[Undefined] {0}",_key.ToString()));
				}
			}

			return textList;
		}

		public string FindKey(string _text)
		{
			if(_text.IsEmpty())
			{
				return string.Empty;
			}

			foreach(var dict in m_LanguageDict.Values)
			{
				foreach(var pair in dict)
				{
					if(pair.Value.IsEqual(_text))
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
	public static string ToLocalize(this string _key)
	{
		if(_key.IsEmpty())
		{
			return _key;
		}

		var option = GameDataMgr.In.Access<GameData.LanguageOption>();

		return option.ToLocalizeText(_key);
	}
}