using System;
using System.Collections.Generic;
using KZLib;
using Newtonsoft.Json;
using UnityEngine;

namespace GameData
{
	public class LanguageOption : Option
	{
		// https://docs.google.com/document/d/1x3wfhsAR4urxCOhi3jqwoCRwp7DUmZqGPip6rsrGF_k/edit?usp=sharing
		protected override string OPTION_KEY => "Language Option";
		protected override EventTag Tag => EventTag.ChangeLanguageOption;

		private class LanguageData
		{
			[JsonProperty("UseVibration")]
			private SystemLanguage m_CurrentLanguage = GameSettings.In.GameLanguage;

			[JsonIgnore]
			public SystemLanguage CurrentLanguage => m_CurrentLanguage;

			public bool SetCurrentLanguage(SystemLanguage _language)
			{
				if(m_CurrentLanguage == _language)
				{
					return false;
				}

				m_CurrentLanguage = _language;

				return true;
			}
		}

		private LanguageData m_LanguageData = null;

		private readonly Dictionary<SystemLanguage,Dictionary<string,string>> m_LanguageDict = new();

		public override void Initialize()
		{
			base.Initialize();

			LoadOption(ref m_LanguageData);

			m_LanguageDict.Clear();

			foreach(var textAsset in ResMgr.In.GetTextAssetArray(GameSettings.In.LanguageFilePath))
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

			SaveOption(m_LanguageData,false);
		}

		public SystemLanguage GameLanguage
		{
			get => m_LanguageData.CurrentLanguage;
			set
			{
				if(!m_LanguageDict.ContainsKey(value))
				{
					LogTag.System.W($"{value} is not exist.");

					return;
				}

				if(!m_LanguageData.SetCurrentLanguage(value))
				{
					return;
				}

				LogTag.System.I($"Language is changed. [{value}]");

				SaveOption(m_LanguageData,true);
			}
		}

		public string ToLocalizeText(string _key)
		{
			if(m_LanguageDict.IsNullOrEmpty())
			{
				throw new NullReferenceException("Data is empty.");
			}

			if(m_LanguageDict.TryGetValue(m_LanguageData.CurrentLanguage,out var dataDict))
			{
				if(dataDict.TryGetValue(_key,out var value))
				{
					return value;
				}
			}
			else
			{
				if(m_LanguageDict[SystemLanguage.English].TryGetValue(_key,out var value))
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