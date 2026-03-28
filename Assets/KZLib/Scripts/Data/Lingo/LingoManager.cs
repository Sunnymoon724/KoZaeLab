using KZLib.Utilities;
using UnityEngine;
using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;
using UnityEngine.Localization;
using System.Threading;
using R3;

#if UNITY_EDITOR

using UnityEditor.Localization;

#endif

using Object = UnityEngine.Object;

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

#if UNITY_EDITOR
			if(!LocalizationEditorSettings.ActiveLocalizationSettings)
			{
				return false;
			}
#endif

			if(!LocalizationSettings.Instance)
			{
				LogChannel.Data.W("Localization Settings asset not found in Project Settings. Skipping LingoManager.");

				return false;
			}

			await LocalizationSettings.InitializationOperation;

			TuneManager.In.FetchTune<LanguageTune>().OnChangedLanguage.Subscribe(_OnChangeLanguage).AddTo(m_disposable);

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
					LogChannel.Data.E("Localization is not include English locale");

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
				LogChannel.Data.W($"{key} is not exist in localization.");

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

			return await KZExternalKit.LoadHandleSafeAsync(LocalizationSettings.AssetDatabase.GetLocalizedAssetAsync<TAsset>(tableName,key));
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