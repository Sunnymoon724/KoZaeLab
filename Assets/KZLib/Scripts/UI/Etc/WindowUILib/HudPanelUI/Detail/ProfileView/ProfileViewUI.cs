using UnityEngine;

namespace HudPanel
{
	public class ProfileViewUI : BaseComponentUI,IHudUI
	{
		[SerializeField]
		private FrameProfileUI m_frameProfileUI = null;
		[SerializeField]
		private MemoryProfileUI m_memoryProfileUI = null;
		[SerializeField]
		private RenderProfileUI m_renderProfileUI = null;
		[SerializeField]
		private AudioProfileUI m_audioProfileUI = null;

		public bool IsActive => gameObject.activeInHierarchy;

		public void Refresh(float deltaTime,int frameCount)
		{
			m_frameProfileUI.Refresh(deltaTime,frameCount);
			m_memoryProfileUI.Refresh();
			m_renderProfileUI.Refresh();
			m_audioProfileUI.Refresh();
		}
	}
}