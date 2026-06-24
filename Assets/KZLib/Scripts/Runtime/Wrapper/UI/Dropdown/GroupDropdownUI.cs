using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Dropdown with prev/next buttons for looped navigation. Exposes current option text, sprite, and color.
/// </summary>
[RequireComponent(typeof(TMP_Dropdown))]
public class GroupDropdownUI : BaseDropdownUI
{
	[SerializeField]
	private Button m_prevButton = null;
	[SerializeField]
	private Button m_nextButton = null;

	public string CurrentText => CurrentOptionData?.text;
	public Sprite CurrentSprite => CurrentOptionData?.image;
	public Color CurrentColor => CurrentOptionData?.color ?? Color.white;

	private TMP_Dropdown.OptionData CurrentOptionData => m_dropdown != null && m_dropdown.options.TryGetValueByIndex(m_dropdown.value,out var option) ? option : null;

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
		if(m_prevButton != null)
		{
			m_prevButton.onClick.RemoveAction(_OnClickedPrevButton);
		}

		if(m_nextButton != null)
		{
			m_nextButton.onClick.RemoveAction(_OnClickedNextButton);
		}

		base.OnDisable();
	}

	protected override void _OnValueChanged(int _)
	{
		OnValueChanged?.Invoke(CurrentText);
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
		if(!m_dropdown)
		{
			return;
		}

		if(added != 0)
		{
			var value = KZMathKit.LoopClamp(m_dropdown.value+added,m_dropdown.options.Count);

			m_dropdown.SetValueWithoutNotify(value);
		}

		OnValueChanged?.Invoke(CurrentText);
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
		if(!m_dropdown)
		{
			return;
		}

		m_dropdown.ClearOptions();
		m_dropdown.AddOptions(optionList);
	}

	public void SetOptions(List<string> textList)
	{
		if(!m_dropdown)
		{
			return;
		}

		m_dropdown.ClearOptions();
		m_dropdown.AddOptions(textList);
	}

	public void SetOptions(List<Sprite> spriteList)
	{
		if(!m_dropdown)
		{
			return;
		}

		m_dropdown.ClearOptions();
		m_dropdown.AddOptions(spriteList);
	}

	public void AddOptions(List<TMP_Dropdown.OptionData> optionList)
	{
		if(!m_dropdown)
		{
			return;
		}

		m_dropdown.AddOptions(optionList);
	}

	public void AddOptions(List<string> textList)
	{
		if(!m_dropdown)
		{
			return;
		}

		m_dropdown.AddOptions(textList);
	}

	public void AddOptions(List<Sprite> spriteList)
	{
		if(!m_dropdown)
		{
			return;
		}

		m_dropdown.AddOptions(spriteList);
	}

	private void _SetAction(Action<string> onValueChanged)
	{
		if(!m_dropdown)
		{
			return;
		}

		m_dropdown.value = 0;

		OnValueChanged = onValueChanged;

		OnValueChanged?.Invoke(CurrentText);
	}

	public void MoveNext()
	{
		_OnClickedNextButton();
	}

	public void MovePrev()
	{
		_OnClickedPrevButton();
	}
}