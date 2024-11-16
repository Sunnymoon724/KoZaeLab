using Sirenix.OdinInspector;
using UnityEngine;

public abstract class FocusSlotUI : SlotUI,IFocusSlotUI
{
	[SerializeField,HideInInspector]
	private float m_CurrentLocation = 0.5f;

	[FoldoutGroup("Focus",Order = 6)]
	[VerticalGroup("Focus/0",Order = 0),SerializeField]
	protected RectTransform m_Slot = null;

	[VerticalGroup("Focus/0",Order = 0),ShowInInspector,PropertyRange(0.0f,1.0f)]
	protected float CurrentLocation
	{
		get => m_CurrentLocation;
		set
		{
			UpdateLocation(value);
		}
	}

	[VerticalGroup("Focus/0",Order = 0),SerializeField,ReadOnly]
	private bool m_Center = false;

	protected bool IsCenter => m_Center;

	public virtual void UpdateLocation(float _location)
	{
		m_CurrentLocation = _location;
		m_Center = _location.Approximately(0.5f);

		if(m_Button)
		{
			m_Button.interactable = IsCenter;
		}
	}

#if UNITY_EDITOR
	protected override void DrawGizmo()
	{
		if(m_Slot)
		{
			DrawGizmoText(m_Slot.position);
		}
	}
#endif
}