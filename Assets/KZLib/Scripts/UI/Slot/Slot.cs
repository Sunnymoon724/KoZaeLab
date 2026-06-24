using KZLib.Sounds;
using System;
using Coffee.UIEffects;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using KZLib.Utilities;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace KZLib.UI
{
	/// <summary>
	/// Base UI cell that binds <see cref="IEntryInfo"/> to optional image, text, and button references.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Override <see cref="UseImage"/>, <see cref="UseText"/>, and <see cref="UseButton"/> to hide inspector groups
	/// and skip the matching <see cref="SetEntryInfo"/> branch. When a branch is off, the corresponding <c>_Clear*</c>
	/// method runs so pooled instances do not keep stale visuals or callbacks.
	/// </para>
	/// <para>
	/// <see cref="SetEntryInfo"/> applies branches in order: text, image, then button. Subclasses that add binding logic
	/// should call <c>base.SetEntryInfo</c> after preparing their own data (or pass <c>null</c> to clear everything).
	/// </para>
	/// <para>
	/// Click flow: <see cref="Button.onClick"/> → <see cref="_OnClicked"/> → stored entry callback →
	/// <see cref="IEntryInfo.Sound"/> effect playback. When <see cref="UseButton"/> is true, the click listener is
	/// registered on enable and removed on disable.
	/// </para>
	/// <para>
	/// Pooling: extends <see cref="GameObjectPawn"/>. <see cref="GameObjectPawnPool{TComponent}.Put"/> invokes
	/// <see cref="_Release"/> → <see cref="SetEntryInfo"/>(<c>null</c>) before deactivation. Used by
	/// <see cref="ReuseScrollRect"/> and <see cref="ReuseGridLayoutGroup"/>.
	/// </para>
	/// </remarks>
	[RequireComponent(typeof(RectTransform))]
	public class Slot : GameObjectPawn
	{
		/// <summary>When false, scene gizmos are not drawn.</summary>
		protected virtual bool UseGizmos => true;

		/// <summary>Scene-view label drawn when <see cref="UseGizmos"/> is true.</summary>
		protected virtual string GizmosText => string.Empty;

		/// <summary>Odin inspector order for the Image group.</summary>
		protected const int c_imageOrder = 0;

		/// <summary>Odin inspector order for the Text group.</summary>
		protected const int c_textOrder = 1;

		/// <summary>Odin inspector order for the Button group.</summary>
		protected const int c_buttonOrder = 2;

		[BoxGroup("Image",Order = c_imageOrder),SerializeField,ShowIf(nameof(UseImage))]
		protected Image m_image = null;

		/// <summary>미사용. Prefab wiring reserved; not referenced by this base class.</summary>
		[BoxGroup("Image",Order = c_imageOrder),SerializeField,ShowIf(nameof(UseImage))]
		protected UIEffect m_imageEffect = null;

		/// <summary>When false, image fields are hidden in the inspector and <see cref="_ClearIcon"/> runs on bind.</summary>
		protected virtual bool UseImage => true;

		[BoxGroup("Text",Order = c_textOrder),SerializeField,ShowIf(nameof(UseText))]
		protected TMP_Text m_nameText = null;

		[BoxGroup("Text",Order = c_textOrder),SerializeField,ShowIf(nameof(UseText))]
		protected TMP_Text m_descriptionText = null;

		/// <summary>When false, text fields are hidden in the inspector and <see cref="_ClearText"/> runs on bind.</summary>
		protected virtual bool UseText => true;

		[BoxGroup("Button",Order = c_buttonOrder),SerializeField,ShowIf(nameof(UseButton))]
		protected Button m_button = null;

		/// <summary>Delegate copied from <see cref="IEntryInfo.OnClicked"/>; invoked by <see cref="_OnClicked"/>.</summary>
		private Action<IEntryInfo> m_onClicked = null;

		/// <summary>
		/// When false, button fields are hidden, the click listener is not registered, and
		/// <see cref="_ClearButton"/> runs on bind.
		/// </summary>
		protected virtual bool UseButton => true;

		/// <summary>
		/// When false, <see cref="_ShouldButtonBeInteractable"/> returns true even without
		/// <see cref="IEntryInfo.OnClicked"/> (e.g. <see cref="ToggleSlot"/>).
		/// </summary>
		protected virtual bool RequiresEntryCallback => true;

		/// <summary>Last entry passed to <see cref="SetEntryInfo"/>; forwarded to click callbacks and sound playback.</summary>
		protected IEntryInfo CurrentEntryInfo { get; private set; }

		private RectTransform m_rootRect = null;

		/// <summary>Cached root <see cref="RectTransform"/> on this GameObject.</summary>
		protected RectTransform RootRect
		{
			get
			{
				if(!m_rootRect)
				{
					m_rootRect = GetComponent<RectTransform>();
				}

				return m_rootRect;
			}
		}

		/// <summary>Shortcut to <see cref="RectTransform.anchoredPosition"/> on <see cref="RootRect"/>.</summary>
		public Vector2 AnchoredPosition
		{
			get => RootRect.anchoredPosition;
			set => RootRect.anchoredPosition = value;
		}

		/// <summary>Registers the click listener when the slot becomes active.</summary>
		private void OnEnable()
		{
			_SyncButtonListener(true);
		}

		/// <summary>Removes the click listener when the slot is deactivated or returned to the pool.</summary>
		private void OnDisable()
		{
			_SyncButtonListener(false);
		}

		/// <summary>
		/// Adds or removes <see cref="_OnClicked"/> on <see cref="m_button"/> when <see cref="UseButton"/> is true and
		/// the reference is assigned.
		/// </summary>
		private void _SyncButtonListener(bool isRegister)
		{
			if(!UseButton || !m_button)
			{
				return;
			}

			if(isRegister)
			{
				m_button.onClick.AddAction(_OnClicked);
			}
			else
			{
				m_button.onClick.RemoveAction(_OnClicked);
			}
		}

		/// <summary>
		/// Default click handler. Override to add behaviour before or after; call <c>base._OnClicked()</c> to keep the
		/// entry callback and effect sound.
		/// </summary>
		protected virtual void _OnClicked()
		{
			m_onClicked?.Invoke(CurrentEntryInfo);

			_PlaySound();
		}

		/// <summary>Plays <see cref="IEntryInfo.Sound"/> when present.</summary>
		private void _PlaySound()
		{
			if(CurrentEntryInfo?.Sound != null)
			{
				SoundManager.In.PlayEffect2D(CurrentEntryInfo.Sound);
			}
		}

		/// <summary>
		/// Binds <paramref name="entryInfo"/> to enabled UI branches. Pass <c>null</c> to clear all bound state.
		/// </summary>
		public virtual void SetEntryInfo(IEntryInfo entryInfo)
		{
			CurrentEntryInfo = entryInfo;

			if(UseText)
			{
				_SetName(entryInfo?.Name);
				_SetDescription(entryInfo?.Description);
			}
			else
			{
				_ClearText();
			}

			if(UseImage)
			{
				_SetIcon(entryInfo?.Icon);
			}
			else
			{
				_ClearIcon();
			}

			if(UseButton)
			{
				_SetButton(entryInfo?.OnClicked);
			}
			else
			{
				_ClearButton();
			}
		}

		/// <summary>Clears name and description (deactivates TMP objects when text is empty).</summary>
		protected virtual void _ClearText()
		{
			_SetName(null);
			_SetDescription(null);
		}

		/// <summary>Clears the icon (deactivates <see cref="m_image"/> when the sprite is null).</summary>
		protected virtual void _ClearIcon()
		{
			_SetIcon(null);
		}

		/// <summary>Clears the stored callback and refreshes <see cref="Button.interactable"/> via <see cref="_SetButton"/>.</summary>
		protected virtual void _ClearButton()
		{
			_SetButton(null);
		}

		/// <summary>Writes <paramref name="text"/> to <see cref="m_nameText"/> when assigned.</summary>
		protected virtual void _SetName(string text)
		{
			if(m_nameText)
			{
				m_nameText.SetSafeTextMeshPro(text);
			}
		}

		/// <summary>Writes <paramref name="text"/> to <see cref="m_descriptionText"/> when assigned.</summary>
		protected virtual void _SetDescription(string text)
		{
			if(m_descriptionText)
			{
				m_descriptionText.SetSafeTextMeshPro(text);
			}
		}

		/// <summary>Assigns <paramref name="icon"/> to <see cref="m_image"/> when assigned.</summary>
		protected virtual void _SetIcon(Sprite icon)
		{
			if(m_image)
			{
				m_image.SetSafeImage(icon);
			}
		}

		/// <summary>Stores the click delegate and refreshes <see cref="Button.interactable"/>.</summary>
		protected virtual void _SetButton(Action<IEntryInfo> onClicked)
		{
			m_onClicked = onClicked;
			_ApplyButtonInteractable();
		}

		/// <summary>Applies <see cref="_ShouldButtonBeInteractable"/> to <see cref="m_button"/>.</summary>
		protected void _ApplyButtonInteractable()
		{
			if(m_button)
			{
				m_button.interactable = UseButton && _ShouldButtonBeInteractable();
			}
		}

		/// <summary>
		/// Default rule: interactable when <see cref="RequiresEntryCallback"/> is false or an entry callback is stored.
		/// Override for layout-driven rules (e.g. <see cref="FocusSlot"/> uses center position).
		/// </summary>
		protected virtual bool _ShouldButtonBeInteractable()
		{
			if(!RequiresEntryCallback)
			{
				return true;
			}

			return m_onClicked != null;
		}

		/// <summary>Pool return hook; clears bound entry state before the instance is deactivated.</summary>
		protected override void _Release()
		{
			SetEntryInfo(null);
		}

		/// <summary>Pool purge hook; no-op in the base slot.</summary>
		protected override void _Destroy() { }

		/// <summary>
		/// Returns cell width or height along the scroll axis. Prefers <see cref="LayoutUtility"/> preferred size;
		/// falls back to <see cref="RectTransform.rect"/> when layout has not produced a value yet. Called once on the
		/// pivot by <see cref="ReuseScrollRect"/> to measure scroll stride.
		/// </summary>
		/// <param name="isVertical"><c>true</c> for height; <c>false</c> for width.</param>
		public float GetSlotSize(bool isVertical)
		{
			var size = isVertical ? LayoutUtility.GetPreferredHeight(RootRect) : LayoutUtility.GetPreferredWidth(RootRect);

			if(size > 0.0f)
			{
				return size;
			}

			var rect = RootRect.rect;

			return isVertical ? rect.height : rect.width;
		}

#if UNITY_EDITOR
		private static GUIStyle s_gizmoLabelStyle = null;

		private void OnDrawGizmos()
		{
			if(!UseGizmos)
			{
				return;
			}

			_DrawGizmo();
		}

		/// <summary>
		/// Override to customize scene gizmos. Default draws <see cref="GizmosText"/> at this transform's position.
		/// </summary>
		protected virtual void _DrawGizmo()
		{
			_DrawGizmoText(transform.position);
		}

		/// <summary>Draws <paramref name="position"/> label text in the Scene view.</summary>
		protected void _DrawGizmoText(Vector3 position)
		{
			s_gizmoLabelStyle ??= new GUIStyle() { normal = { textColor = Color.white } };

			Handles.Label(position,GizmosText,s_gizmoLabelStyle);
		}
#endif
	}
}