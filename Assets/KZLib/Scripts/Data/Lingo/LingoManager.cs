using KZLib.Utilities;
using UnityEngine;
using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;
using UnityEngine.Localization;
using System.Threading;
using R3;

using Object = UnityEngine.Object;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;

namespace KZLib.Data
{
	public class LingoManager : Singleton<LingoManager>
	{
		private readonly CompositeDisposable m_disposable = new();

		private bool m_isLoaded = false;

		private SystemLanguage m_currentLanguage = SystemLanguage.English;

		private readonly Subject<Unit> m_lingoSubject = new();
		public Observable<Unit> OnChangedLanguage => m_lingoSubject;

		public SystemLanguage CurrentLanguage => m_currentLanguage;

		protected override void _Release(bool disposing)
		{
			if(disposing)
			{
				m_disposable?.Dispose();
				m_lingoSubject.Dispose();

				m_isLoaded = false;
			}

			base._Release(disposing);
		}
		
		public async UniTask<bool> TryLoadAsync(CancellationToken _)
		{
			if(m_isLoaded)
			{
				return true;
			}

			try 
			{
				var handle = LocalizationSettings.InitializationOperation;
	
				await handle;

				if(handle.Status == AsyncOperationStatus.Failed)
				{
					LogChannel.System.W("Localization initialization failed via Handle.");

					return false;
				}
			}
			catch(Exception exception)
			{
				LogChannel.System.W($"Localization system is not ready or missing: {exception.Message}");

				return false;
			}

			await LocalizationSettings.InitializationOperation;

			if(!LocalizationSettings.Instance)
			{
				LogChannel.System.W("LocalizationSettings is null. Skip LingoManager initialization.");

				return false;
			}

			var optionCfg = ConfigManager.In.Access<OptionConfig>();

			optionCfg.OnChangedLanguage.Subscribe(_OnChangeLanguage).AddTo(m_disposable);

			_OnChangeLanguage(optionCfg.CurrentLanguage);

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
					LogChannel.System.E("Localization is not include English locale");

					return;
				}
			}

			LocalizationSettings.SelectedLocale = newLocal;
			m_currentLanguage = language;

			m_lingoSubject.OnNext(Unit.Default);
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
				LogChannel.System.W($"{key} is not exist in localization.");

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