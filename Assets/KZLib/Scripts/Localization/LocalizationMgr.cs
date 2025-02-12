using System.Collections.Generic;
using UnityEngine;
using KZLib.KZUtility;
using Newtonsoft.Json;
using UnityEngine.Events;

namespace KZLib
{
	public class LocalizationMgr : Singleton<LocalizationMgr>
	{
		private bool m_disposed = false;

		private SystemLanguage m_language = SystemLanguage.English;

		private readonly Dictionary<SystemLanguage,Dictionary<string,string>> m_languageTextDict = new();

		public event UnityAction OnLocalizationChange = null;

		protected override void Initialize()
		{
			m_languageTextDict.Clear();

			// TODO 유니티 로컬라이즈 찾아보기.... -> 언어는 구글 시트 이용? & 루아 써서 실시간도 생각해보기
			foreach(var textAsset in ResourceManager.In.GetTextAssetArray(ConfigManager.In.Access<ConfigData.GameConfig>().LanguageFolderPath))
			{
				var languageDict = JsonConvert.DeserializeObject<Dictionary<string,string>>(textAsset.text);

				if(languageDict.IsNullOrEmpty())
				{
					continue;
				}

				m_languageTextDict.Add(textAsset.name.ToEnum<SystemLanguage>(),languageDict);
			}

			var optionConfig = ConfigManager.In.Access<ConfigData.OptionConfig>();

			optionConfig.OnLanguageChange += OnChangeLanguage;

			OnChangeLanguage(optionConfig.Language);
		}

		protected override void Release(bool disposing)
		{
			if(m_disposed)
			{
				return;
			}

			if(disposing)
			{
				m_languageTextDict.Clear();

				var optionConfig = ConfigManager.In.Access<ConfigData.OptionConfig>();

				optionConfig.OnLanguageChange -= OnChangeLanguage;
			}

			m_disposed = true;

			base.Release(disposing);
		}

		private void OnChangeLanguage(SystemLanguage newLanguage)
		{
			if(!m_languageTextDict.ContainsKey(newLanguage))
			{
				LogTag.System.W($"{newLanguage} is not exist.");

				return;
			}

			m_language = newLanguage;

			OnLocalizationChange?.Invoke();
		}

		public string ToLocalizeText(string key)
		{
			if(m_languageTextDict.IsNullOrEmpty())
			{
				return key;
			}

			if(m_languageTextDict.TryGetValue(m_language,out var dataDict))
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

		return KZLib.LocalizationMgr.In.ToLocalizeText(key);
	}
}