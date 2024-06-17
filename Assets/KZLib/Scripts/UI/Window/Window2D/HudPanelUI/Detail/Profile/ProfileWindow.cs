using UnityEngine;

namespace HudPanel
{
	public class ProfileWindow : BaseComponentUI,IHudUI
	{
		[SerializeField]
		private FrameProfile m_FrameProfile = null;
		[SerializeField]
		private MemoryProfile m_MemoryProfile = null;
		[SerializeField]
		private RenderProfile m_RenderProfile = null;
		[SerializeField]
		private AudioProfile m_AudioProfile = null;

		public void Refresh(float _deltaTime,int _frameCount)
		{
			if(!gameObject.activeInHierarchy)
			{
				return;
			}

			m_FrameProfile.Refresh(_deltaTime,_frameCount);
			m_MemoryProfile.Refresh();
			m_RenderProfile.Refresh();
			m_AudioProfile.Refresh();
		}
	}
}