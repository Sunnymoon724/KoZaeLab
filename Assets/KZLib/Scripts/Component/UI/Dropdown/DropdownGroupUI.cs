using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_Dropdown))]
public class DropdownGroupUI : BaseComponentUI
{
	[SerializeField] private TMP_Dropdown m_dropdown = null;
	[SerializeField] private Button m_prevButton = null;
	[SerializeField] private Button m_nextButton = null;

	public string GetCurrentText => m_dropdown.options[m_dropdown.value].text;

	private Action<string> OnValueChanged;

	protected override void Initialize()
	{
		base.Initialize();

		m_dropdown.onValueChanged.AddListener(_OnValueChangedDropDown);

		if(m_prevButton != null)
		{
			m_prevButton.onClick.SetAction(_OnClickedPrevButton);
		}

		if(m_nextButton != null)
		{
			m_nextButton.onClick.SetAction(_OnClickedNextButton);
		}
	}

	private void _OnValueChangedDropDown(int _)
	{
		OnValueChanged?.Invoke(GetCurrentText);
	}

	private void _OnClickedPrevButton()
	{
		_SetLoopedValue(-1);
	}

	private void _OnClickedNextButton()
	{
		_SetLoopedValue(+1);
	}

	private void _SetLoopedValue(int added)
	{
		if(added != 0)
		{
			var value = CommonUtility.LoopClamp(m_dropdown.value+added,m_dropdown.options.Count);

			m_dropdown.SetValueWithoutNotify(value);
		}

		OnValueChanged?.Invoke(GetCurrentText);
	}

	public void AddOptions(List<TMP_Dropdown.OptionData> optionList,Action<string> onValueChanged)
	{
		m_dropdown.AddOptions(optionList);
		m_dropdown.value = 0;

		OnValueChanged = onValueChanged;

		OnValueChanged?.Invoke(GetCurrentText);
	}
}