using System;
using Coffee.UIEffects;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotUI : BaseComponentUI
{
	protected override bool UseGizmos => true;

	protected static class SlotOrder
	{
		public const float IMAGE = 0.0f;
		public const float TEXT = 1.0f;
		public const float BUTTON = 2.0f;
	}

	#region IMAGE
	[FoldoutGroup("Image",Order = SlotOrder.IMAGE),SerializeField,ShowIf(nameof(UseImage))]
	protected Image m_Image = null;
	[FoldoutGroup("Image",Order = SlotOrder.IMAGE),SerializeField,ShowIf(nameof(UseImage))]
	protected UIEffect m_ImageEffect = null;

	protected virtual bool UseImage => true;
	#endregion IMAGE

	#region  TEXT
	[FoldoutGroup("Text",Order = SlotOrder.TEXT),SerializeField,ShowIf(nameof(UseText))]
	protected TMP_Text m_NameText = null;

	[FoldoutGroup("Text",Order = SlotOrder.TEXT),SerializeField,ShowIf(nameof(UseText))]
	protected TMP_Text m_DescriptionText = null;

	protected virtual bool UseText => true;
	#endregion TEXT

	#region BUTTON
	[FoldoutGroup("Button",Order = SlotOrder.BUTTON),SerializeField,ShowIf(nameof(UseButton))]
	protected Button m_Button = null;

	private Action m_OnClicked = null;

	protected virtual bool UseButton => true;
	#endregion BUTTON

	protected override void OnEnable()
	{
		base.OnEnable();

		m_Button.onClick.AddAction(OnClicked);
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		m_Button.onClick.RemoveAction(OnClicked);
	}

	private void OnClicked()
	{
		m_OnClicked?.Invoke();
	}

    public virtual void SetCell(ICellData _cellData)
	{
		if(UseText)
		{
			SetName(_cellData.CellName);
			SetDescription(_cellData.CellDescription);
		}

		if(UseImage)
		{
			SetImage(_cellData.CellSprite);
		}

		if(UseButton)
		{
			if(_cellData.CellAudioClip)
			{
				var button = gameObject.GetOrAddComponent<AudioButtonUI>();

				button.SetAudio(_cellData.CellAudioClip);
			}

			SetButton(_cellData.OnClicked);
		}
	}

	protected virtual void SetName(string _text)
	{
		m_NameText.SetLocalizeText(_text);
	}

	protected virtual void SetDescription(string _text)
	{
		m_DescriptionText.SetLocalizeText(_text);
	}

	protected virtual void SetImage(Sprite _sprite)
	{
		m_Image.SetSafeImage(_sprite);
	}

	protected virtual void SetButton(Action _onClicked)
	{
		m_OnClicked = _onClicked;
	}
}

public interface IFocusSlotUI
{
	public void UpdateLocation(float _location);
}

public interface ICellData
{
	string CellName { get; }
	string CellDescription { get; }
	Sprite CellSprite { get; }
	AudioClip CellAudioClip { get; }
	Action OnClicked { get; }
}

public record CellData(string CellName,string CellDescription,Sprite CellSprite,AudioClip CellAudioClip,Action OnClicked) : ICellData;