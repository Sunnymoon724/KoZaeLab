using System.Threading;
using Cysharp.Threading.Tasks;
using KZLib.Utilities;
using R3;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

using Object = UnityEngine.Object;

namespace KZLib.Data
{
	/// <summary>
	/// Unity Localization runtime bridge for <see cref="LanguageTune"/>.
	/// Same role as <see cref="GraphicManager"/> for <see cref="GraphicTune"/> / <see cref="KZLib.Sounds.SoundManager"/> for <see cref="SoundTune"/>.
	/// Applies <see cref="LocalizationSettings.SelectedLocale"/> and notifies UI when locale is ready.
	/// </summary>
	public class LingoManager : Singleton<LingoManager>
	{
		private bool m_isLoaded = false;
		private readonly CompositeDisposable m_disposable = new();
		private readonly Subject<Unit> m_lingoSubject = new();

		private LanguageTune m_languageTune = null;
		private SystemLanguage m_currentLanguage = SystemLanguage.English;

		/// <summary>Locale apply finished; refresh localized UI (not the same as <see cref="LanguageTune.OnChangedLanguage"/>).</summary>
		public Observable<Unit> OnChangedLanguage => m_lingoSubject;

		/// <summary>Last locale applied to <see cref="LocalizationSettings.SelectedLocale"/>.</summary>
		public SystemLanguage CurrentLanguage => m_currentLanguage;

		/// <summary>True after <see cref="TryLoadAsync"/> succeeds; required before string/asset lookup.</summary>
		public bool IsLoaded => m_isLoaded;

		protected override void _Release(bool disposing)
		{
			if(disposing)
			{
				m_disposable.Dispose();
				m_lingoSubject.Dispose();

				m_languageTune = null;
				m_isLoaded = false;
			}

			base._Release(disposing);
		}

		/// <summary>
		/// Initializes Unity Localization, subscribes to <see cref="LanguageTune"/>, and applies saved language.
		/// Call from <see cref="KZLib.BaseMain"/> before localized UI is expected to resolve strings.
		/// </summary>
		public async UniTask<bool> TryLoadAsync(CancellationToken token)
		{
			if(m_isLoaded)
			{
				return true;
			}

			if(!LocalizationSettings.Instance)
			{
				LogChannel.Data.W("Localization Settings asset not found in Project Settings. Skipping LingoManager.");

				return false;
			}

			await LocalizationSettings.InitializationOperation.ToUniTask(cancellationToken: token);

			m_languageTune = TuneManager.In.Fetch<LanguageTune>();
			m_isLoaded = true;

			// OnChangedWithStart on LanguageTune runs synchronously on Subscribe; m_isLoaded must be true first.
			m_languageTune.OnChangedLanguage.Subscribe(_OnChangeLanguage).AddTo(m_disposable);

			return true;
		}

		/// <summary>Resolves a string table entry. Key format: {TableName}_{EntryKey}.</summary>
		public string FindString(string key)
		{
			if(key.IsEmpty())
			{
				return key;
			}

			if(!_IsReadyForQuery())
			{
				return key;
			}

			if(!_SplitFormat(key,out var tableName))
			{
				return key;
			}

			var localizedText = LocalizationSettings.StringDatabase.GetLocalizedString(tableName,key);

			if(localizedText == key)
			{
				LogChannel.Data.W($"{key} does not exist in localization.");

				return key;
			}

			return localizedText;
		}

		/// <summary>Loads a localized asset. Key format: {TableName}_{EntryKey}.</summary>
		public async UniTask<TAsset> FindAssetAsync<TAsset>(string key,CancellationToken token = default) where TAsset : Object
		{
			if(key.IsEmpty() || !_IsReadyForQuery())
			{
				return null;
			}

			if(!_SplitFormat(key,out var tableName))
			{
				return null;
			}

			var handle = LocalizationSettings.AssetDatabase.GetLocalizedAssetAsync<TAsset>(tableName,key);

			await handle.ToUniTask(cancellationToken: token);

			if(handle.Status == AsyncOperationStatus.Failed)
			{
				throw handle.OperationException;
			}

			var result = handle.Result;

			handle.Release();

			return result;
		}

		private void _OnChangeLanguage(SystemLanguage lan)
		{
			if(m_languageTune == null)
			{
				return;
			}

			if(!_TryResolveLocale(lan,out var locale,out var resolvedLanguage))
			{
				LogChannel.Data.E("Localization does not include an English locale.");

				return;
			}

			// Unsupported locale: persist English on LanguageTune, then re-enter via OnChangedWithStart.
			if(resolvedLanguage != lan)
			{
				m_languageTune.SetLanguage(resolvedLanguage);

				return;
			}

			LocalizationSettings.SelectedLocale = locale;
			m_currentLanguage = resolvedLanguage;

			m_lingoSubject.OnNext(Unit.Default);
		}

		private bool _IsReadyForQuery()
		{
			return m_isLoaded && LocalizationSettings.Instance;
		}

		/// <summary>Maps <see cref="SystemLanguage"/> to a project locale; falls back to English.</summary>
		private bool _TryResolveLocale(SystemLanguage lan,out Locale locale,out SystemLanguage resolvedLan)
		{
			locale = LocalizationSettings.AvailableLocales.GetLocale(new LocaleIdentifier(lan));

			if(locale)
			{
				resolvedLan = lan;

				return true;
			}

			resolvedLan = SystemLanguage.English;
			locale = LocalizationSettings.AvailableLocales.GetLocale(new LocaleIdentifier(resolvedLan));

			return locale;
		}

		/// <summary>Key format: {TableName}_{EntryKey}</summary>
		private bool _SplitFormat(string key,out string tableName)
		{
			if(key.IsEmpty())
			{
				tableName = string.Empty;

				return false;
			}

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