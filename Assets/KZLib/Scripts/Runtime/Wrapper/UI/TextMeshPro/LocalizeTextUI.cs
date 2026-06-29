using System;
using KZLib.Data;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Binds a localization key to TMP text. Refreshes on <see cref="LingoManager.OnChangedLanguage"/>.
/// </summary>
public class LocalizeTextUI : BaseTextMeshUI
{
	[SerializeField]
	private string m_key = null;

	private IDisposable m_subscription = null;

	[Button("Execute Localize"),EnableIf(nameof(IsValidKey))]
	protected void _ExecuteLocalize()
	{
		_RefreshLocalize();
	}

	private bool IsValidKey => !m_key.IsEmpty();

	protected override void OnEnable()
	{
		base.OnEnable();

		_Unsubscribe();

		// OnEnable may run before BaseMain calls TryLoadAsync. Always touch .In so we subscribe
		// before the first locale apply; HasInstance-only skip would miss that notification.
		m_subscription = LingoManager.In.OnChangedLanguage.Subscribe(_OnChangedLanguage);

		_RefreshLocalize();
	}

	protected override void OnDisable()
	{
		_Unsubscribe();

		base.OnDisable();
	}

	private void _Unsubscribe()
	{
		m_subscription?.Dispose();
		m_subscription = null;
	}

	private void _OnChangedLanguage(SystemLanguage _)
	{
		_RefreshLocalize();
	}

	private void _RefreshLocalize()
	{
		if(!IsValidKey || !m_textMesh)
		{
			return;
		}

		var localize = m_key.ToLocalize();

		m_textMesh.SetSafeTextMeshPro(localize);
	}

	public void SetLocalizeKey(string key)
	{
		if(key.IsEmpty())
		{
			return;
		}

		if(string.Equals(m_key,key))
		{
			return;
		}

		m_key = key;

		_RefreshLocalize();
	}
}