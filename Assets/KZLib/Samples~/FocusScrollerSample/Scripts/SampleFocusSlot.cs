using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace KZLib.KZSample.UI
{
    public class SampleFocusSlot : FocusSlot
	{
		protected override bool UseImage => false;
		protected override bool UseButton => false;

		[BoxGroup("Animator",Order = 10),SerializeField]
		private Animator m_animator = null;

		public override void RefreshLocation(float location)
		{
			base.RefreshLocation(location);

			if(m_animator.isActiveAndEnabled)
			{
				m_animator.Play("Scroll",0,location);
			}

			m_animator.speed = 0.0f;
		}
	}
}