using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.UI
{
	/// <summary>Normalized scroll position for focus-based slot views.</summary>
	public interface IFocusSlot
	{
		/// <summary>Updates scroll position and derived focus state.</summary>
		void RefreshLocation(float location);
	}

	/// <summary>
	/// <see cref="Slot"/> for focus-based scrollers. Button interactability follows scroll position instead of
	/// callback presence: only the centered item (<see cref="IsCenter"/>) is clickable.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="FocusScroller"/> calls <see cref="RefreshLocation"/> while scrolling. After binding entry data,
	/// the parent should keep calling <see cref="RefreshLocation"/> so interactability tracks position.
	/// </para>
	/// <para>
	/// <see cref="_ShouldButtonBeInteractable"/> uses <see cref="IsCenter"/> only; a centered slot may stay
	/// interactable even without <see cref="IEntryInfo.OnClicked"/>.
	/// </para>
	/// <para>
	/// <see cref="SetEntryInfo"/> with <c>null</c> clears base slot state and moves the slot off-center so the
	/// button is disabled (pool-safe). Scene gizmos use the assigned focus UI transform when present.
	/// </para>
	/// </remarks>
	public abstract class FocusSlot : Slot,IFocusSlot
	{
		[SerializeField,HideInInspector]
		private float m_currentLocation = 0.5f;

		[BoxGroup("Focus",Order = 6)]
		[VerticalGroup("Focus/0",Order = 0),SerializeField]
		protected RectTransform m_focusUI = null;

		/// <summary>Normalized position along the scroller axis (0 = start, 0.5 = center, 1 = end).</summary>
		[VerticalGroup("Focus/0",Order = 0),ShowInInspector,PropertyRange(0.0f,1.0f)]
		protected float CurrentLocation
		{
			get => m_currentLocation;
			set => RefreshLocation(value);
		}

		[VerticalGroup("Focus/0",Order = 0),SerializeField,ReadOnly]
		private bool m_center = false;

		/// <summary><c>true</c> when <see cref="CurrentLocation"/> is approximately 0.5.</summary>
		protected bool IsCenter => m_center;

		/// <summary>
		/// Binds entry data, then resets scroll position when <paramref name="entryInfo"/> is <c>null</c>.
		/// </summary>
		public override void SetEntryInfo(IEntryInfo entryInfo)
		{
			base.SetEntryInfo(entryInfo);

			if(entryInfo == null)
			{
				RefreshLocation(0.0f);
			}
		}

		/// <summary>Updates scroll position and refreshes button interactability.</summary>
		public virtual void RefreshLocation(float location)
		{
			m_currentLocation = location;
			m_center = location.Approximately(0.5f);

			_ApplyButtonInteractable();
		}

		/// <summary>Interactable only when <see cref="IsCenter"/>; ignores entry callback presence.</summary>
		protected override bool _ShouldButtonBeInteractable() => IsCenter;

#if UNITY_EDITOR
		/// <summary>Scene gizmo cube size derived from <see cref="Transform.lossyScale"/>.</summary>
		protected Vector3 CubeSize => transform.lossyScale*10.0f;

		/// <summary>Draws gizmo text at the focus UI transform when assigned.</summary>
		protected override void _DrawGizmo()
		{
			if(m_focusUI)
			{
				_DrawGizmoText(m_focusUI.position);
			}
		}
#endif
	}
}