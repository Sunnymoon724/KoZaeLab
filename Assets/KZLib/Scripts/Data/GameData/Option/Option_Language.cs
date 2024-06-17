using System;
using System.Collections.Generic;
using KZLib;
using KZLib.KZDevelop;
using Newtonsoft.Json;
using UnityEngine;

namespace GameData
{
	public partial class Option : IGameData
	{
		// https://docs.google.com/document/d/1x3wfhsAR4urxCOhi3jqwoCRwp7DUmZqGPip6rsrGF_k/edit?usp=sharing

		private static SystemLanguage DefaultLanguage => GameSettings.In.DefaultLanguage;

		private const string LANGUAGE_OPTION = "Language Option";

		public partial class Language
		{
			[SerializeField,JsonProperty("GameLanguage")]
			private string m_GameLanguage = null;
			[JsonIgnore]
			public SystemLanguage GameLanguage
			{
				get => m_GameLanguage.ToEnum<SystemLanguage>();
				set
				{
					var language = value.ToString();

					if(m_GameLanguage.IsEqual(language))
					{
						return;
					}

					m_GameLanguage = language;

					SaveLanguage();
				}
			}

			public Language()
			{
				m_GameLanguage = GameSettings.In.GameLanguage.ToString();
			}

			private void SaveLanguage()
			{
				s_SaveHandler.SetObject(LANGUAGE_OPTION,this);

				Broadcaster.SendEvent(EventTag.ChangeLanguageOption);
			}
		}

		public Language LanguageOption { get; private set; }

		private readonly Dictionary<SystemLanguage,Dictionary<string,string>> m_TranslateDict = new();

		private void InitializeLanguage()
		{
			LanguageOption = s_SaveHandler.GetObject(LANGUAGE_OPTION,new Language());

			m_TranslateDict.Clear();

			foreach(var textAsset in ResMgr.In.GetTextAssetArray(GameSettings.In.TranslatePath))
			{
				var languageDict = JsonConvert.DeserializeObject<Dictionary<string,string>>(textAsset.text);

				if(languageDict.IsNullOrEmpty())
				{
					continue;
				}

				m_TranslateDict.Add(textAsset.name.ToEnum<SystemLanguage>(),languageDict);
			}
		}

		private void ReleaseLanguage()
		{
			m_TranslateDict.Clear();
		}

		public void SetGameLanguage(SystemLanguage _language)
		{
			var option = GameDataMgr.In.Access<Option>().LanguageOption;

			if(option.GameLanguage == _language)
			{
				return;
			}

			if(!m_TranslateDict.ContainsKey(_language))
			{
				Log.System.I("해당 언어의 번역본이 없어서 변경하지 않았습니다. [{0}]",_language);

				return;
			}

			Log.System.I("언어가 바꿨습니다. [{0}]",_language);

			option.GameLanguage = _language;
		}

		public string ToTranslateText(string _key)
		{
			if(m_TranslateDict.IsNullOrEmpty())
			{
				throw new NullReferenceException("번역 데이터가 비어있습니다.");
			}

			if(m_TranslateDict.TryGetValue(LanguageOption.GameLanguage,out var dataDict))
			{
				if(dataDict.TryGetValue(_key,out var value))
				{
					return value;
				}
			}
			else
			{
				if(m_TranslateDict[DefaultLanguage].TryGetValue(_key,out var value))
				{
					return value;
				}
			}

			return _key;
		}

		public IEnumerable<string> GetLanguageGroup(string _key)
		{
#if UNITY_EDITOR
			var textList = new List<string>();

			foreach(var pair in m_TranslateDict)
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
#else
			return null;
#endif
		}

		public string FindKey(string _text)
		{
#if UNITY_EDITOR
			if(!_text.IsEmpty())
			{
				foreach(var dict in m_TranslateDict.Values)
				{
					foreach(var pair in dict)
					{
						if(pair.Value.IsEqual(_text))
						{
							return pair.Key;
						}
					}
				}
			}
#endif
			return string.Empty;
		}
	}
}

public static class LocalizeExtension
{
	public static string ToTranslate(this string _key)
	{
		if(_key.IsEmpty())
		{
			return _key;
		}

		return GameDataMgr.In.Access<GameData.Option>().ToTranslateText(_key);
	}
}