using System;
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
	/// Persists language in PlayerPrefs and applies <see cref="LocalizationSettings.SelectedLocale"/>.
	/// Also resolves localized strings and assets.
	/// </summary>
	public class LingoManager : Singleton<LingoManager>
	{
		private LingoManager() { }

		private bool m_isLoaded = false;

		/// <summary>True after <see cref="TryLoadAsync"/> succeeds; required before string/asset lookup and locale apply.</summary>
		public bool IsLoaded => m_isLoaded;

		// Access via LingoManager.In only; Singleton init completes before the instance is returned.
		private ReactivePrefs<SystemLanguage> m_language = null;
		public Observable<SystemLanguage> OnChangedLanguage => m_language.OnChanged;
		public SystemLanguage Language
		{
			get => m_language.Value;
			set => _ApplyLanguage(value);
		}

		private string _PrefsKey(string name) => $"[{nameof(LingoManager)}] {name}";

		protected override void _Initialize()
		{
			base._Initialize();

#if UNITY_EDITOR
			var defaultValue = SystemLanguage.English;
#else
			var defaultValue = Application.systemLanguage;
#endif
			m_language = new ReactivePrefs<SystemLanguage>(_PrefsKey(nameof(m_language)),Enum.TryParse,defaultValue);
		}

		protected override void _Release(bool disposing)
		{
			if(disposing)
			{
				m_language?.Dispose();

				m_isLoaded = false;
			}

			base._Release(disposing);
		}

		/// <summary>
		/// Awaits Unity Localization init, then applies the saved language to <see cref="LocalizationSettings.SelectedLocale"/>.
		/// Call from <see cref="BaseMain"/> before localized UI is expected to resolve strings.
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

			m_isLoaded = true;
			_ApplyLanguage(Language);

			return true;
		}

		/// <summary>Resolves a string table entry. Key format: {TableName}_{EntryKey}.</summary>
		public string FindString(string key)
		{
			if(key.IsEmpty() || !m_isLoaded || !_SplitFormat(key,out var tableName))
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
			if(key.IsEmpty() || !m_isLoaded || !_SplitFormat(key,out var tableName))
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

		private void _ApplyLanguage(SystemLanguage requested)
		{
			if(!_TryResolveLocale(requested,out var locale,out var resolved))
			{
				LogChannel.Data.E("Localization does not include an English locale.");

				return;
			}

			if(resolved != m_language.Value)
			{
				m_language.TrySetValue(resolved);
			}

			if(m_isLoaded)
			{
				LocalizationSettings.SelectedLocale = locale;
			}
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

			return locale != null;
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