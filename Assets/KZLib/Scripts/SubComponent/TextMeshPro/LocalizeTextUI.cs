using System.Collections.Generic;
using KZLib;
using KZLib.KZDevelop;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 키 값을 세팅하면 해당 현지화 언어를 보여줌
/// </summary>
public class LocalizeTextUI : BaseTextUI
{
	[SerializeField,HideInInspector]
	private string m_LocalizeKey = null;

	[VerticalGroup("1",Order = 1),ShowInInspector,LabelText("Localize Key")]
	public string LocalizeKey
	{
		get => m_LocalizeKey;
		private set
		{
			SetLocalizeKey(value);

#if UNITY_EDITOR
			m_LocalizeTextList.Clear();

			m_LocalizeTextList.AddRange(GameDataMgr.In.Access<GameData.LanguageOption>().GetLanguageGroup(m_LocalizeKey));
#endif
		}
	}

#if UNITY_EDITOR
	[VerticalGroup("3",Order = 3),SerializeField,LabelText("Show LanguagePack")]
	private bool m_ShowLanguagePack = false;

	[VerticalGroup("3",Order = 3),SerializeField,LabelText("Language List"),ListDrawerSettings(ShowFoldout = false),ReadOnly,DisplayAsString,TextArea,ShowIf(nameof(ShowList))]
	private List<string> m_LocalizeTextList = new();

	private bool ShowList => m_ShowLanguagePack && m_LocalizeTextList.Count > 0;
#endif

	protected override void Initialize()
	{
		base.Initialize();

		if(!m_LocalizeKey.IsEmpty())
		{
			OnSetLocalizeText();
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		Broadcaster.EnableListener(EventTag.ChangeLanguageOption,OnSetLocalizeText);
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		Broadcaster.DisableListener(EventTag.ChangeLanguageOption,OnSetLocalizeText);
	}

	private void OnSetLocalizeText()
	{
		var localize = m_LocalizeKey.ToLocalize();

		m_TextMesh.SetSafeTextMeshPro(localize);
	}

	public void SetLocalizeKey(string _key)
	{
		if(m_LocalizeKey.IsEqual(_key))
		{
			return;
		}
		
		m_LocalizeKey = _key;

		OnSetLocalizeText();
	}
}