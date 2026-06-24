using System;
using KZLib.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace KZLib.UI
{
	/// <summary>Pooled page-indicator dot used by <see cref="CarouselNavigator"/>.</summary>
	/// <remarks>
	/// <para>
	/// Wire an <see cref="Image"/> for selection tinting and an optional <see cref="Button"/> for click-to-focus.
	/// <see cref="CarouselNavigator"/> calls <see cref="SetNavigatorIndex"/> after each roster resize.
	/// </para>
	/// </remarks>
	public class CarouselDot : GameObjectPawn
	{
		[SerializeField]
		private Image m_image = null;
		[SerializeField]
		private Color m_selectedColor = Color.white;
		[SerializeField]
		private Color m_idleColor = new(1.0f,1.0f,1.0f,0.35f);
		[SerializeField]
		private Button m_button = null;

		private int m_index = -1;
		private Action<int> m_onClicked = null;

		private void Awake()
		{
			if(m_button)
			{
				m_button.onClick.AddListener(_OnClicked);
			}
		}

		private void OnDestroy()
		{
			if(m_button)
			{
				m_button.onClick.RemoveListener(_OnClicked);
			}
		}

		/// <summary>Registers the roster index and click callback used by <see cref="CarouselNavigator"/>.</summary>
		/// <param name="onClicked">When <c>null</c>, the button is disabled.</param>
		public void SetNavigatorIndex(int index,Action<int> onClicked)
		{
			m_index = index;
			m_onClicked = onClicked;

			if(m_button)
			{
				m_button.interactable = onClicked != null;
			}
		}

		/// <summary>Applies the selected or idle visual state.</summary>
		/// <param name="selected"><c>true</c> when this dot matches the carousel focus index.</param>
		public void SetDot(bool selected)
		{
			if(m_image)
			{
				m_image.color = selected ? m_selectedColor : m_idleColor;
			}
		}

		private void _OnClicked()
		{
			if(m_index >= 0)
			{
				m_onClicked?.Invoke(m_index);
			}
		}

		/// <summary>Pool return hook; no extra state to clear because bindings are refreshed on checkout.</summary>
		protected override void _Release() { }

		/// <summary>Pool purge hook; no-op for this dot view.</summary>
		protected override void _Destroy() { }
	}
}
