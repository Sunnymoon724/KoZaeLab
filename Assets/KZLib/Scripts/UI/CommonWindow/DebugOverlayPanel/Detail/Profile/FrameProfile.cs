using UnityEngine;

namespace KZLib.UI.Widgets.Debug
{
	public class FrameProfile : BaseComponent,IPeriodicOverlay
	{
		[SerializeField]
		private FrameMonitor m_currentFrameMonitor = null;
		[SerializeField]
		private FrameMonitor m_maximumFrameMonitor = null;
		[SerializeField]
		private FrameMonitor m_minimumFrameMonitor = null;
		[SerializeField]
		private FrameMonitor m_averageFrameMonitor = null;

		[SerializeField]
		private ShiftGraphImage m_frameGraph = null;

		private int m_minFrameRate = int.MaxValue;
		private int m_maxFrameRate = 0;

		private float m_minFrameTime = float.MaxValue;
		private float m_maxFrameTime = 0.0f;

		public bool IsActive => gameObject.activeInHierarchy;

		public void Refresh(int frameRate,float frameTime)
		{
			m_minFrameRate = Mathf.Min(m_minFrameRate,frameRate);
			m_maxFrameRate = Mathf.Max(m_maxFrameRate,frameRate);

			m_minFrameTime = Mathf.Min(m_minFrameTime,frameTime);
			m_maxFrameTime = Mathf.Max(m_maxFrameTime,frameTime);

			m_currentFrameMonitor.SetFrame(frameRate,frameTime);
			m_maximumFrameMonitor.SetFrame(m_maxFrameRate,m_maxFrameTime);
			m_minimumFrameMonitor.SetFrame(m_minFrameRate,m_minFrameTime);
			m_averageFrameMonitor.SetFrame((m_maxFrameRate-m_minFrameRate)/2,(m_maxFrameTime-m_minFrameTime)/2.0f);

			m_frameGraph.RefreshGraph(frameRate);
		}
	}
}