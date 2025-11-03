using System;
using KZLib.KZUtility;
using UnityEngine;
using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;
using UnityEngine.Localization;
using System.Threading;
using Object = UnityEngine.Object;

namespace KZLib.KZData
{
	public class LingoMgr : Singleton<LingoMgr>
	{
		private bool m_disposed = false;
		private bool m_isLoaded = false;

		private SystemLanguage m_currentLanguage = SystemLanguage.English;

		public event Action OnLanguageChange = null;

		public SystemLanguage CurrentLanguage => m_currentLanguage;

		protected override void Release(bool disposing)
		{
			if(m_disposed)
			{
				return;
			}

			if(disposing)
			{
				if(ConfigMgr.HasInstance)
				{
					var optionCfg = ConfigMgr.In.Access<OptionConfig>();

					optionCfg.OnLanguageChanged -= _OnChangeLanguage;
				}

				m_isLoaded = false;
			}

			m_disposed = true;

			base.Release(disposing);
		}
		
		public async UniTask<bool> TryLoadAsync(CancellationToken token)
		{
			if(m_isLoaded)
			{
				return true;
			}

			await LocalizationSettings.InitializationOperation;

			var optionCfg = ConfigMgr.In.Access<OptionConfig>();

			optionCfg.OnLanguageChanged += _OnChangeLanguage;

			_OnChangeLanguage(optionCfg.Language);

			m_isLoaded = true;

			return true;
		}

		private void _OnChangeLanguage(SystemLanguage language)
		{
			var newLocal = LocalizationSettings.AvailableLocales.GetLocale(new LocaleIdentifier(language));

			if(newLocal == null)
			{
				language = SystemLanguage.English;

				newLocal = LocalizationSettings.AvailableLocales.GetLocale(new LocaleIdentifier(language));

				if(newLocal == null)
				{
					LogSvc.System.E("Localization is not include English locale");

					return;
				}
			}

			LocalizationSettings.SelectedLocale = newLocal;
			m_currentLanguage = language;

			OnLanguageChange?.Invoke();
		}

		public string FindString(string key)
		{
			if(!_SplitLingoFormat(key,out var tableName,out var entryKey))
			{
				return null;
			}

			var localizedText = LocalizationSettings.StringDatabase.GetLocalizedString(tableName,entryKey);

			if(localizedText == entryKey)
			{
				LogSvc.System.W($"{entryKey} is not exist in localization.");
				
				return key;
			}

			return localizedText;
		}

		public async UniTask<TAsset> FindAssetAsync<TAsset>(string key) where TAsset : Object
		{
			if(!_SplitLingoFormat(key,out var tableName,out var _))
			{
				return null;
			}

			return await CommonUtility.LoadHandleSafeAsync(LocalizationSettings.AssetDatabase.GetLocalizedAssetAsync<TAsset>(tableName,key));
		}

		private bool _SplitLingoFormat(string key,out string tableName,out string entryKey)
		{
			var index = key.IndexOf('_');

			if(index <= 0 || index >= key.Length - 1)
			{
				LogSvc.System.W($"{key} is not lingo format.");
				
				tableName = string.Empty;
				entryKey = string.Empty;

				return false;
			}

			tableName = key[..index];
			entryKey = key[(index + 1)..];

			return true;
		}
	}
}