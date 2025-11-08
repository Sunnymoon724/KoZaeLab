using KZLib.KZData;
using Sirenix.OdinInspector;
using UnityEngine;

public class LocalizeTextUI : BaseTextUI
{
	[SerializeField]
	private string m_key = null;

	[Button("Execute Localize"),EnableIf(nameof(IsValidKey))]
	protected void _ExecuteLocalize()
	{
		_OnChangedLanguage();
	}

	private bool IsValidKey => !m_key.IsEmpty();

	protected override void Initialize()
	{
		base.Initialize();

		_OnChangedLanguage();
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		LingoManager.In.OnLanguageChange += _OnChangedLanguage;
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		LingoManager.In.OnLanguageChange -= _OnChangedLanguage;
	}

	private void _OnChangedLanguage()
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

		_OnChangedLanguage();
	}
}