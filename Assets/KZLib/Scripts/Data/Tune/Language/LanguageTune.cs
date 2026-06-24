using System;
using R3;
using UnityEngine;

namespace KZLib.Data
{
	/// <summary>
	/// User locale preference. <see cref="LingoManager"/> subscribes to <see cref="OnChangedLanguage"/>.
	/// </summary>
	public class LanguageTune : Tune
	{
		protected LanguageTune() { }

		private SystemLanguage m_language;
		public SystemLanguage CurrentLanguage => m_language;

		public Observable<SystemLanguage> OnChangedLanguage => OnChangedWithStart(nameof(m_language)).Select(_GetLanguage);

		private SystemLanguage _GetLanguage(Unit _)
		{
			return CurrentLanguage;
		}

		protected override void _LoadAll()
		{
#if UNITY_EDITOR
			// Editor always defaults to English so locale does not depend on the developer OS language.
			var defaultValue = SystemLanguage.English;
#else
			// Player build defaults to the device language when PlayerPrefs has no saved value.
			var defaultValue = Application.systemLanguage;
#endif
			m_language	= _LoadValue(nameof(m_language),Enum.TryParse,defaultValue);
		}

		public void SetLanguage(SystemLanguage newLanguage)
		{
			_SetValue(ref m_language,newLanguage,nameof(m_language),null);
		}
	}
}
