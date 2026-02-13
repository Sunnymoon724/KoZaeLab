using System;
using KZLib.Data;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;

public class LocalizeTextUI : BaseTextUI
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

	protected override void _Initialize()
	{
		base._Initialize();

		_OnChangedLanguage(Unit.Default);
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		m_subscription = LingoManager.In.OnChangedLanguage.Subscribe(_OnChangedLanguage);
	}

	protected override void OnDisable()
	{
		base.OnDisable();

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