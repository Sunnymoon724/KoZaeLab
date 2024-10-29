using Sirenix.OdinInspector;
using UnityEngine;

public class SampleFocusSlotUI : FocusSlotUI
{
	[FoldoutGroup("Animator",Order = 5),SerializeField]
	private Animator m_Animator = null;

	public override void UpdateLocation(float _location)
	{
		base.UpdateLocation(_location);

		if(m_Animator.isActiveAndEnabled)
		{
			m_Animator.Play("Scroll",0,_location);
		}

		m_Animator.speed = 0.0f;
	}
}