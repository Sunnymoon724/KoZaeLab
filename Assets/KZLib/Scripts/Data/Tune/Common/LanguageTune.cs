using System;
using R3;
using UnityEngine;

namespace KZLib.Data
{
	public class LanguageTune : Tune
	{
#if UNITY_EDITOR
		public SystemLanguage m_language = SystemLanguage.English;
#else
		public SystemLanguage m_language = Application.systemLanguage;
#endif
		public SystemLanguage CurrentLanguage => m_language;

		public Observable<SystemLanguage> OnChangedLanguage => OnChangedWithStart(nameof(m_language)).Select(_GetLanguage);

		private SystemLanguage _GetLanguage(Unit _)
		{
			return CurrentLanguage;
		}

		protected override void _LoadAll()
		{
#if UNITY_EDITOR
			var defaultValue = SystemLanguage.English;
#else
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