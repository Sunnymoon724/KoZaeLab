using Sirenix.OdinInspector;
using UnityEngine;

public abstract class FocusSlotUI : SlotUI //,IFocusSlotUI
{
	[SerializeField,HideInInspector]
	private float m_currentLocation = 0.5f;

	[BoxGroup("Focus",Order = 6)]
	[VerticalGroup("Focus/0",Order = 0),SerializeField]
	protected RectTransform m_focusUI = null;

	[VerticalGroup("Focus/0",Order = 0),ShowInInspector,PropertyRange(0.0f,1.0f)]
	protected float CurrentLocation
	{
		get => m_currentLocation;
		set
		{
			RefreshLocation(value);
		}
	}

	[VerticalGroup("Focus/0",Order = 0),SerializeField,ReadOnly]
	private bool m_center = false;

	protected bool IsCenter => m_center;

	public virtual void RefreshLocation(float location)
	{
		m_currentLocation = location;
		m_center = location.Approximately(0.5f);

		if(m_button)
		{
			m_button.interactable = IsCenter;
		}
	}

#if UNITY_EDITOR
	protected override void _DrawGizmo()
	{
		if(m_focusUI)
		{
			_DrawGizmoText(m_focusUI.position);
		}
	}
#endif
}

// public interface IFocusSlotUI
// {
// 	public void RefreshLocation(float location);
// }