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
	public class LingoManager : Singleton<LingoManager>
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
				if(ConfigManager.HasInstance)
				{
					var optionCfg = ConfigManager.In.Access<OptionConfig>();

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

			var optionCfg = ConfigManager.In.Access<OptionConfig>();

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
			if(!_SplitLingoFormat(key,out var tableName))
			{
				return key;
			}

			var localizedText = LocalizationSettings.StringDatabase.GetLocalizedString(tableName,key);

			if(localizedText == key)
			{
				LogSvc.System.W($"{key} is not exist in localization.");

				return key;
			}

			return localizedText;
		}

		public async UniTask<TAsset> FindAssetAsync<TAsset>(string key) where TAsset : Object
		{
			if(!_SplitLingoFormat(key,out var tableName))
			{
				return null;
			}

			return await CommonUtility.LoadHandleSafeAsync(LocalizationSettings.AssetDatabase.GetLocalizedAssetAsync<TAsset>(tableName,key));
		}

		private bool _SplitLingoFormat(string key,out string tableName)
		{
			var index = key.IndexOf('_');

			if(index <= 0 || index >= key.Length - 1)
			{
				tableName = string.Empty;

				return false;
			}

			tableName = key[..index];

			return true;
		}
	}
}