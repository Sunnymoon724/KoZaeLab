using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotUI : BaseComponentUI
{
	protected override bool UseGizmos => true;

	protected const int c_image_order = 0;
	protected const int c_text_order = 1;
	protected const int c_button_order = 2;

	#region IMAGE
	[BoxGroup("Image",Order = c_image_order),SerializeField,ShowIf(nameof(UseImage))]
	protected Image m_image = null;

	protected virtual bool UseImage => true;
	#endregion IMAGE

	#region  TEXT
	[BoxGroup("Text",Order = c_text_order),SerializeField,ShowIf(nameof(UseText))]
	protected TMP_Text m_nameText = null;

	[BoxGroup("Text",Order = c_text_order),SerializeField,ShowIf(nameof(UseText))]
	protected TMP_Text m_descriptionText = null;

	protected virtual bool UseText => true;
	#endregion TEXT

	#region BUTTON
	[BoxGroup("Button",Order = c_button_order),SerializeField,ShowIf(nameof(UseButton))]
	protected Button m_button = null;

	private Action m_onClicked = null;

	protected virtual bool UseButton => true;
	#endregion BUTTON

	protected override void OnEnable()
	{
		base.OnEnable();

		if(m_button)
		{
			m_button.onClick.AddAction(OnClicked);
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		if(m_button)
		{
			m_button.onClick.RemoveAction(OnClicked);
		}
	}

	private void OnClicked()
	{
		m_onClicked?.Invoke();
	}

    public virtual void SetCell(ICellData cellData)
	{
		if(UseText)
		{
			SetName(cellData.Name);
			SetDescription(cellData.Description);
		}

		if(UseImage)
		{
			SetImage(cellData.Sprite);
		}

		if(UseButton)
		{
			if(cellData.AudioClip)
			{
				var button = gameObject.GetOrAddComponent<AudioButtonUI>();

				button.SetAudio(cellData.AudioClip);
			}

			SetButton(cellData.OnClicked);
		}
	}

	protected virtual void SetName(string text)
	{
		if(m_nameText)
		{
			m_nameText.SetLocalizeText(text);
		}
	}

	protected virtual void SetDescription(string text)
	{
		if(m_descriptionText)
		{
			m_descriptionText.SetLocalizeText(text);
		}
	}

	protected virtual void SetImage(Sprite sprite)
	{
		if(m_image)
		{
			m_image.SetSafeImage(sprite);
		}
	}

	protected virtual void SetButton(Action onClicked)
	{
		if(m_button)
		{
			m_onClicked = onClicked;
		}
	}
}

public interface IFocusSlotUI
{
	public void UpdateLocation(float location);
}

public interface ICellData
{
	string Name { get; }
	string Description { get; }
	Sprite Sprite { get; }
	AudioClip AudioClip { get; }
	Action OnClicked { get; }
}

public record CellData(string Name,string Description,Sprite Sprite,AudioClip AudioClip,Action OnClicked) : ICellData;