using System;
using KZLib.Data;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;

public class LocalizeTextUI : BaseTextMeshUI
{
	[SerializeField]
	private string m_key = null;

	private IDisposable m_subscription = null;

	[Button("Execute Localize"),EnableIf(nameof(IsValidKey))]
	protected void _ExecuteLocalize()
	{
		_OnChangedLanguage(Unit.Default);
	}

	private bool IsValidKey => !m_key.IsEmpty();

	private void Awake()
	{
		_OnChangedLanguage(Unit.Default);
	}

	private void OnEnable()
	{
		m_subscription = LingoManager.In.OnChangedLanguage.Subscribe(_OnChangedLanguage);
	}

	private void OnDisable()
	{
		m_subscription?.Dispose();
	}

	private void _OnChangedLanguage(Unit _)
	{
		if(!IsValidKey)
		{
			return;
		}

		var localize = m_key.ToLocalize();

		m_textMesh.SetSafeTextMeshPro(localize);
	}

	public void SetLocalizeKey(string key)
	{
		if(!key.IsEmpty())
		{
			return;
		}

		if(m_key.IsEqual(key))
		{
			return;
		}

		m_key = key;

		_OnChangedLanguage(Unit.Default);
	}
}