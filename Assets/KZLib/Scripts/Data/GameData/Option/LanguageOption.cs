﻿using System;
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

		private class Language
		{
			public SystemLanguage CurrentLanguage { get; set; }
		}

		// https://docs.google.com/document/d/1x3wfhsAR4urxCOhi3jqwoCRwp7DUmZqGPip6rsrGF_k/edit?usp=sharing
		private const SystemLanguage DEFAULT_LANGUAGE = SystemLanguage.English;

		private Language m_Language = null;

		private readonly Dictionary<SystemLanguage,Dictionary<string,string>> m_LanguageDict = new();

		public override void Initialize()
		{
			var language = CommonSaveData.GetEnum("[System] GameLanguage",DEFAULT_LANGUAGE);

			m_Language = GetOption(new Language() { CurrentLanguage = language, });

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
			if(m_Language.CurrentLanguage == _language)
			{
				return;
			}

			if(!m_LanguageDict.ContainsKey(_language))
			{
				LogTag.Data.W($"{_language} is not exist.");

				return;
			}

			LogTag.Data.I($"Language is changed. [{_language}]");

			m_Language.CurrentLanguage = _language;

			SaveOption(m_Language);
		}

		public string ToLocalizeText(string _key)
		{
			if(m_LanguageDict.IsNullOrEmpty())
			{
				throw new NullReferenceException("Data is empty.");
			}

			if(m_LanguageDict.TryGetValue(m_Language.CurrentLanguage,out var dataDict))
			{
				if(dataDict.TryGetValue(_key,out var value))
				{
					return value;
				}
			}
			else
			{
				if(m_LanguageDict[DEFAULT_LANGUAGE].TryGetValue(_key,out var value))
				{
					return value;
				}
			}

			return _key;
		}

#if UNITY_EDITOR
		public IEnumerable<string> GetLanguageGroup(string _key)
		{
			foreach(var (key,value) in m_LanguageDict)
			{
				yield return value.TryGetValue(_key,out var language) ? $"{key} {language}" : $"Undefined {_key}";
			}
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
		if(!_key.IsEmpty())
		{
			return _key;
		}

		var option = GameDataMgr.In.Access<GameData.LanguageOption>();

		return option.ToLocalizeText(_key);
	}
}