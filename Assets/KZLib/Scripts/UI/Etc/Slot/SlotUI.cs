using System;
using KZLib;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotUI : BaseComponentUI
{
	protected override bool UseGizmos => true;

	protected const int c_imageOrder = 0;
	protected const int c_textOrder = 1;
	protected const int c_buttonOrder = 2;

	#region IMAGE
	[BoxGroup("Image",Order = c_imageOrder),SerializeField,ShowIf(nameof(UseImage))]
	protected Image m_image = null;

	protected virtual bool UseImage => true;
	#endregion IMAGE

	#region  TEXT
	[BoxGroup("Text",Order = c_textOrder),SerializeField,ShowIf(nameof(UseText))]
	protected TMP_Text m_nameText = null;

	[BoxGroup("Text",Order = c_textOrder),SerializeField,ShowIf(nameof(UseText))]
	protected TMP_Text m_descriptionText = null;

	protected virtual bool UseText => true;
	#endregion TEXT

	#region BUTTON
	[BoxGroup("Button",Order = c_buttonOrder),SerializeField,ShowIf(nameof(UseButton))]
	protected Button m_button = null;

	private Action<IEntryInfo> m_onClicked = null;

	protected virtual bool UseButton => true;
	#endregion BUTTON

	protected IEntryInfo CurrentEntryInfo { get; private set; }

	protected override void OnEnable()
	{
		base.OnEnable();

		if(m_button)
		{
			m_button.onClick.AddAction(_OnClicked);
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		if(m_button)
		{
			m_button.onClick.RemoveAction(_OnClicked);
		}
	}

	private void _OnClicked()
	{
		m_onClicked?.Invoke(CurrentEntryInfo);

		_PlaySound();
	}

	private void _PlaySound()
	{
		if(CurrentEntryInfo.Sound != null)
		{
			SoundManager.In.PlaySFX(CurrentEntryInfo.Sound);
		}
	}

    public virtual void SetEntry(IEntryInfo entryInfo)
	{
		CurrentEntryInfo = entryInfo;

		if(UseText)
		{
			_SetName(entryInfo.Name);
			_SetDescription(entryInfo.Description);
		}

		if(UseImage)
		{
			_SetIcon(entryInfo.Icon);
		}

		if(UseButton)
		{
			_SetButton(entryInfo.OnClicked);
		}
	}

	protected virtual void _SetName(string text)
	{
		if(m_nameText)
		{
			m_nameText.SetSafeTextMeshPro(text);
		}
	}

	protected virtual void _SetDescription(string text)
	{
		if(m_descriptionText)
		{
			m_descriptionText.SetSafeTextMeshPro(text);
		}
	}

	protected virtual void _SetIcon(Sprite icon)
	{
		if(m_image)
		{
			m_image.SetSafeImage(icon);
		}
	}

	protected virtual void _SetButton(Action<IEntryInfo> onClicked)
	{
		if(m_button)
		{
			m_onClicked = onClicked;
		}
	}
}