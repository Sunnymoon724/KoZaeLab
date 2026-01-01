using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_Dropdown))]
public class GroupDropdownUI : BaseDropdownUI
{
	[SerializeField]
	private Button m_prevButton = null;
	[SerializeField]
	private Button m_nextButton = null;

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

	public void SetDropdown(List<TMP_Dropdown.OptionData> optionList,Action<string> onValueChanged)
	{
		SetOptions(optionList);

		_SetAction(onValueChanged);
	}

	public void SetDropdown(List<string> textList,Action<string> onValueChanged)
	{
		SetOptions(textList);

		_SetAction(onValueChanged);
	}

	public void SetDropdown(List<Sprite> spriteList,Action<string> onValueChanged)
	{
		SetOptions(spriteList);

		_SetAction(onValueChanged);
	}

	public void SetOptions(List<TMP_Dropdown.OptionData> optionList)
	{
		m_dropdown.ClearOptions();
		m_dropdown.AddOptions(optionList);
	}

	public void SetOptions(List<string> textList)
	{
		m_dropdown.ClearOptions();
		m_dropdown.AddOptions(textList);
	}

	public void SetOptions(List<Sprite> spriteList)
	{
		m_dropdown.ClearOptions();
		m_dropdown.AddOptions(spriteList);
	}

	public void AddOptions(List<TMP_Dropdown.OptionData> optionList)
	{
		m_dropdown.AddOptions(optionList);
	}

	public void AddOptions(List<string> textList)
	{
		m_dropdown.AddOptions(textList);
	}

	public void AddOptions(List<Sprite> spriteList)
	{
		m_dropdown.AddOptions(spriteList);
	}

	private void _SetAction(Action<string> onValueChanged)
	{
		m_dropdown.value = 0;

		OnValueChanged = onValueChanged;

		OnValueChanged?.Invoke(GetCurrentText);
	}
}