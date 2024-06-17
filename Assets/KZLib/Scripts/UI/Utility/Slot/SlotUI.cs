using System;
using Coffee.UIEffects;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotUI : BaseComponentUI
{
	protected override bool UseGizmos => true;

	protected const int IMAGE_ORDER = 0;
	protected const int TEXT_ORDER = 1;
	protected const int BUTTON_ORDER = 2;

	#region IMAGE
	[FoldoutGroup("이미지",Order = IMAGE_ORDER),SerializeField,ShowIf(nameof(UseImage))]
	protected Image m_Image = null;
	[FoldoutGroup("이미지",Order = IMAGE_ORDER),SerializeField,ShowIf(nameof(UseImage))]
	protected UIEffect m_ImageEffect = null;

	[PropertySpace(10)]

	protected virtual bool UseImage => true;
	#endregion IMAGE

	#region  TEXT
	[FoldoutGroup("텍스트",Order = TEXT_ORDER),SerializeField,ShowIf(nameof(UseText))]
	protected TMP_Text m_NameText = null;

	[FoldoutGroup("텍스트",Order = TEXT_ORDER),SerializeField,ShowIf(nameof(UseText))]
	protected TMP_Text m_DescriptionText = null;

	protected virtual bool UseText => true;
	#endregion TEXT

	#region BUTTON
	[FoldoutGroup("버튼",Order = BUTTON_ORDER),SerializeField,ShowIf(nameof(UseButton))]
	protected Button m_Button = null;

	protected virtual bool UseButton => true;
    #endregion BUTTON

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
			if(_cellData.CellAudioClip && gameObject.TryGetComponent<AudioButtonUI>(out var audioButton))
			{
				audioButton.SetAudio(_cellData.CellAudioClip);
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
		if(!m_Button)
		{
			return;
		}

		m_Button.SetOnClickListener(()=>
		{
			_onClicked?.Invoke();
		});
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