using System;
using System.Collections.Generic;
using KZLib;
using KZLib.KZDevelop;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 숫자를 세팅하면 해당 현지화 언어를 보여줌
/// </summary>
public class LocalizeTextUI : BaseTextUI
{
	[SerializeField,HideInInspector]
	protected string m_LocalizeKey = null;

	[ShowInInspector,LabelText("언어팩 키 값")]
	public string LocalizeKey { get => m_LocalizeKey; private set => SetLocalizeKey(value,true); }

#if UNITY_EDITOR
	[SerializeField,ReadOnly,DisplayAsString,TextArea]
	private List<string> m_LocalizeTextList = new();
#endif

	private object[] m_ArgumentArray = null;

	private string m_Format = null;

	protected override void Awake()
	{
		base.Awake();

		if(!m_LocalizeKey.IsEmpty())
		{
			OnSetLocalizeText();
		}
	}

	private void OnEnable()
	{
		Broadcaster.EnableListener(EventTag.ChangeLanguageOption,OnSetLocalizeText);
	}

	private void OnDisable()
	{
		Broadcaster.DisableListener(EventTag.ChangeLanguageOption,OnSetLocalizeText);
	}

	private void OnSetLocalizeText()
	{
		var localize = m_LocalizeKey.ToTranslate();

		if(m_Format.IsEmpty())
		{
			if(m_ArgumentArray.IsNullOrEmpty())
			{
				m_Text.SetSafeTextMeshPro(localize);
			}
			else
			{
				m_Text.SetSafeTextMeshPro(string.Format(localize,m_ArgumentArray));
			}
		}
		else
		{
			m_Text.SetSafeTextMeshPro(string.Format(m_Format,localize));
		}
	}

	public void SetLocalizeKey(string _key,bool _loadText = false)
	{
		if(m_LocalizeKey.IsEqual(_key))
		{
			return;
		}
		
		m_LocalizeKey = _key;

		m_Format = null;
		m_ArgumentArray = null;

		OnSetLocalizeText();

		if(!_loadText)
		{
			return;
		}

#if UNITY_EDITOR
		m_LocalizeTextList.Clear();

		m_LocalizeTextList.AddRange(GameDataMgr.In.Access<GameData.Option>().GetLanguageGroup(m_LocalizeKey));
#endif
	}

	public void SetLocalizeKeyInArgument(string _key,params object[] _argumentArray)
	{
		if(m_LocalizeKey.IsEqual(_key) && m_ArgumentArray.Equals(_argumentArray))
		{
			return;
		}

		m_LocalizeKey = _key;
		m_Format = null;

		if(!_argumentArray.IsNullOrEmpty())
		{
			m_ArgumentArray = new object[_argumentArray.Length];

			Array.Copy(_argumentArray,0,m_ArgumentArray,0,_argumentArray.Length);
		}

		OnSetLocalizeText();
	}

	public void SetLocalizeKeyInFormat(string _key,string _format)
	{
		if(m_LocalizeKey.IsEqual(_key) && m_Format.IsEqual(_format))
		{
			return;
		}

		m_LocalizeKey = _key;
		m_ArgumentArray = null;

		m_Format = _format;

		OnSetLocalizeText();
	}

	public void ClearText()
	{
		m_Text.SetSafeTextMeshPro(string.Empty);
	}
}