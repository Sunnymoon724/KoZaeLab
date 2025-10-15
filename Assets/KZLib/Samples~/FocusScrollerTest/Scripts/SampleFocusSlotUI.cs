using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.KZSample
{
    public class SampleFocusSlotUI : FocusSlotUI
	{
		protected override bool UseImage => false;
		protected override bool UseButton => false;

		[BoxGroup("Animator",Order = 10),SerializeField]
		private Animator m_animator = null;

		public override void UpdateLocation(float location)
		{
			base.UpdateLocation(location);

			if(m_animator.isActiveAndEnabled)
			{
				m_animator.Play("Scroll",0,location);
			}

			m_animator.speed = 0.0f;
		}
	}
}