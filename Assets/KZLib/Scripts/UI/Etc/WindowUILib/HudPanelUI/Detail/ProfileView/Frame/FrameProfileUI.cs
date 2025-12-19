using UnityEngine;

namespace HudPanel
{
	public class FrameProfileUI : BaseComponentUI
	{
		[SerializeField]
		private FrameMonitorUI m_currentFrameMonitor = null;
		[SerializeField]
		private FrameMonitorUI m_maximumFrameMonitor = null;
		[SerializeField]
		private FrameMonitorUI m_minimumFrameMonitor = null;
		[SerializeField]
		private FrameMonitorUI m_averageFrameMonitor = null;

		[SerializeField]
		private ShiftGraphImageUI m_frameGraph = null;
		
		private int m_minFrameRate = int.MaxValue;
		private int m_maxFrameRate = 0;

		private float m_minFrameTime = float.MaxValue;
		private float m_maxFrameTime = 0.0f;

		public void Refresh(float deltaTime,int frameCount)
		{
			var frameRate = Mathf.RoundToInt(frameCount/deltaTime);
			var frameTime = deltaTime/frameCount*1000.0f;

			m_minFrameRate = Mathf.Min(m_minFrameRate,frameRate);
			m_maxFrameRate = Mathf.Max(m_maxFrameRate,frameRate);

			m_minFrameTime = Mathf.Min(m_minFrameTime,frameTime);
			m_maxFrameTime = Mathf.Max(m_maxFrameTime,frameTime);

			m_currentFrameMonitor.SetFrame(frameRate,frameTime);
			m_maximumFrameMonitor.SetFrame(m_maxFrameRate,m_maxFrameTime);
			m_minimumFrameMonitor.SetFrame(m_minFrameRate,m_minFrameTime);
			m_averageFrameMonitor.SetFrame((m_maxFrameRate-m_minFrameRate)/2,(m_maxFrameTime-m_minFrameTime)/2.0f);

			m_frameGraph.UpdateGraph(frameRate);
		}
	}
}