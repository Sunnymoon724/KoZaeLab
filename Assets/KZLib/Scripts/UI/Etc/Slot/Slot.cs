using System;
using Coffee.UIEffects;
using KZLib;
using Sirenix.OdinInspector;
using TMPro;

namespace UnityEngine.UI
{
	[RequireComponent(typeof(RectTransform))]
	public class Slot : BaseComponent
	{
		protected override bool UseGizmos => true;

		protected const int c_imageOrder = 0;
		protected const int c_textOrder = 1;
		protected const int c_buttonOrder = 2;

		[BoxGroup("Image",Order = c_imageOrder),SerializeField,ShowIf(nameof(UseImage))]
		protected Image m_image = null;
		[BoxGroup("Image",Order = c_imageOrder),SerializeField,ShowIf(nameof(UseImage))]
		protected UIEffect m_imageEffect = null;

		protected virtual bool UseImage => true;

		[BoxGroup("Text",Order = c_textOrder),SerializeField,ShowIf(nameof(UseText))]
		protected TMP_Text m_nameText = null;

		[BoxGroup("Text",Order = c_textOrder),SerializeField,ShowIf(nameof(UseText))]
		protected TMP_Text m_descriptionText = null;

		protected virtual bool UseText => true;

		[BoxGroup("Button",Order = c_buttonOrder),SerializeField,ShowIf(nameof(UseButton))]
		protected Button m_button = null;

		private Action<IEntryInfo> m_onClicked = null;

		protected virtual bool UseButton => true;

		protected IEntryInfo CurrentEntryInfo { get; private set; }

		private RectTransform m_rectTrans = null;

		protected RectTransform RectTrans
		{
			get
			{
				if(!m_rectTrans)
				{
					m_rectTrans = GetComponent<RectTransform>();
				}

				return m_rectTrans;
			}
		}

		public Vector2 AnchoredPosition
		{
			get => RectTrans.anchoredPosition;
			set => RectTrans.anchoredPosition = value;
		}

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

		protected virtual void _OnClicked()
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

		public virtual void SetEntryInfo(IEntryInfo entryInfo)
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

		public float GetSlotSize(bool isVertical)
		{
			var rect = RectTrans.rect;

			return isVertical ? rect.height : rect.width;
		}
	}
}