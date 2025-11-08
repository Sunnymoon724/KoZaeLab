using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_Dropdown))]
public class DropdownGroupUI : BaseDropdownUI
{
	[SerializeField] private Button m_prevButton = null;
	[SerializeField] private Button m_nextButton = null;

	public string GetCurrentText => GetCurrentOptionData.text;
	public Sprite GetCurrentSprite => GetCurrentOptionData.image;
	public Color GetCurrentColor => GetCurrentOptionData.color;

	private TMP_Dropdown.OptionData GetCurrentOptionData => m_dropdown.options.TryGetValueByIndex(m_dropdown.value,out var option) ? option : null;

	private Action<string> OnValueChanged;

	protected override void OnEnable()
	{
		base.OnEnable();

		if(m_prevButton != null)
		{
			m_prevButton.onClick.AddAction(_OnClickedPrevButton);
		}

		if(m_nextButton != null)
		{
			m_nextButton.onClick.AddAction(_OnClickedNextButton);
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		if(m_prevButton != null)
		{
			m_prevButton.onClick.RemoveAction(_OnClickedPrevButton);
		}

		if(m_nextButton != null)
		{
			m_nextButton.onClick.RemoveAction(_OnClickedNextButton);
		}
	}

	protected override void _OnValueChanged(int _)
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

		_AddOptions(onValueChanged);
	}

	public void AddOptions(List<string> textList,Action<string> onValueChanged)
	{
		m_dropdown.AddOptions(textList);

		_AddOptions(onValueChanged);
	}

	public void AddOptions(List<Sprite> spriteList,Action<string> onValueChanged)
	{
		m_dropdown.AddOptions(spriteList);

		_AddOptions(onValueChanged);
	}

	private void _AddOptions(Action<string> onValueChanged)
	{
		m_dropdown.value = 0;

		OnValueChanged = onValueChanged;

		OnValueChanged?.Invoke(GetCurrentText);
	}
}