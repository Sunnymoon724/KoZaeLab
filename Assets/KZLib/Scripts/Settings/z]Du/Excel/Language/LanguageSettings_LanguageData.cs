#if UNITY_EDITOR
using UnityEngine;
using Sirenix.OdinInspector;
using System;

/// <summary>
/// 언어 현지화 세팅
/// </summary>
public partial class LanguageSettings : ExcelSettings<LanguageSettings>
{
	[Serializable]
	private class LanguageData
	{
		[HorizontalGroup(" "),LabelText("$m_Language"),SerializeField,ToggleLeft]
		private bool m_Include = true;

		[SerializeField,HideInInspector]
		private string m_Language = null;

		public bool IsInclude => m_Include;
		public string Language => m_Language;

		public LanguageData(string _language,bool _include)
		{
			m_Language = _language;
			m_Include = _include;
		}
	}
}
#endif