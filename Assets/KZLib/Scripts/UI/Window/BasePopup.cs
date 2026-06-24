using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace KZLib.UI
{
	/// <summary>
	/// Base class for pop-up windows with an open scale animation and an optional close button.
	/// </summary>
	public abstract class BasePopup : Window2D
	{
		public override WindowPrefabType WindowType => WindowPrefabType.PopUp;

		[VerticalGroup("UI/General",Order = 0),SerializeField]
		private Transform m_popUpTransform = null;

		[VerticalGroup("UI/General",Order = 0),SerializeField]
		private Button m_closeButton = null;

		protected override void _Initialize()
		{
			base._Initialize();

			if(m_popUpTransform)
			{
				AddSequence(ref m_openSequence,m_popUpTransform.DOScale(0.5f,0.2f).From().SetEase(Ease.OutBack,2.0f),_OnOpenSequenceComplete);
			}
		}

		/// <summary>Routes the optional popup close button through <see cref="_SelfClose"/>; subclasses may override.</summary>
		protected virtual void _OnCloseButtonClicked()
		{
			_SelfClose();
		}

		protected override void _OnEnable()
		{
			base._OnEnable();

			if(m_closeButton)
			{
				m_closeButton.onClick.AddAction(_OnCloseButtonClicked);
			}
		}

		protected override void _OnDisable()
		{
			base._OnDisable();

			if(m_closeButton)
			{
				m_closeButton.onClick.RemoveAction(_OnCloseButtonClicked);
			}
		}
	}
}