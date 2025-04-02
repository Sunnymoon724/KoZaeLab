using System.Collections.Generic;
using KZLib;
using KZLib.KZUtility;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 키 값을 세팅하면 해당 현지화 언어를 보여줌
/// </summary>
public class LocalizeTextUI : BaseTextUI
{
	[SerializeField,HideInInspector]
	private string m_localizeKey = null;

	[VerticalGroup("1",Order = 1),ShowInInspector,LabelText("Localize Key")]
	public string LocalizeKey
	{
		get => m_localizeKey;
		private set
		{
			SetLocalizeKey(value);

#if UNITY_EDITOR
			m_localizeTextList.Clear();

			m_localizeTextList.AddRange(LocalizationMgr.In.GetLanguageGroup(m_localizeKey));
#endif
		}
	}

#if UNITY_EDITOR
	[VerticalGroup("3",Order = 3),SerializeField,LabelText("Show LanguagePack")]
	private bool m_showLanguagePack = false;

	[VerticalGroup("3",Order = 3),SerializeField,LabelText("Language List"),ListDrawerSettings(ShowFoldout = false),ReadOnly,DisplayAsString,TextArea,ShowIf(nameof(ShowList))]
	private List<string> m_localizeTextList = new();

	private bool ShowList => m_showLanguagePack && m_localizeTextList.Count > 0;
#endif

	protected override void Initialize()
	{
		base.Initialize();

		if(!m_localizeKey.IsEmpty())
		{
			_OnChangeLocalization();
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		LocalizationMgr.In.OnLocalizationChange += _OnChangeLocalization;
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		LocalizationMgr.In.OnLocalizationChange -= _OnChangeLocalization;
	}

	private void _OnChangeLocalization()
	{
		var localize = m_localizeKey.ToLocalize();

		m_textMesh.SetSafeTextMeshPro(localize);
	}

	public void SetLocalizeKey(string key)
	{
		if(m_localizeKey.IsEqual(key))
		{
			return;
		}
		
		m_localizeKey = key;

		_OnChangeLocalization();
	}
}