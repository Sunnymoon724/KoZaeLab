using UnityEngine;

namespace HudPanel
{
	public class FrameProfile : BaseComponentUI
	{
		[SerializeField]
		private FrameMonitor m_CurrentMonitor = null;
		[SerializeField]
		private FrameMonitor m_MaximumMonitor = null;
		[SerializeField]
		private FrameMonitor m_MinimumMonitor = null;
		[SerializeField]
		private FrameMonitor m_AverageMonitor = null;

		[SerializeField]
		private FrameGraph m_FrameGraph = null;
		
		private int m_MinFrameRate = int.MaxValue;
		private int m_MaxFrameRate = 0;

		private float m_MinFrameTime = float.MaxValue;
		private float m_MaxFrameTime = 0.0f;

		public void Refresh(float _deltaTime,int _frameCount)
		{
			var frameRate = Mathf.RoundToInt(_frameCount/_deltaTime);
			var frameTime = _deltaTime/_frameCount*1000.0f;

			m_MinFrameRate = Mathf.Min(m_MinFrameRate,frameRate);
			m_MaxFrameRate = Mathf.Max(m_MaxFrameRate,frameRate);

			m_MinFrameTime = Mathf.Min(m_MinFrameTime,frameTime);
			m_MaxFrameTime = Mathf.Max(m_MaxFrameTime,frameTime);

			m_CurrentMonitor.SetFrameData(frameRate,frameTime);
			m_MaximumMonitor.SetFrameData(m_MaxFrameRate,m_MaxFrameTime);
			m_MinimumMonitor.SetFrameData(m_MinFrameRate,m_MinFrameTime);
			m_AverageMonitor.SetFrameData((m_MaxFrameRate-m_MinFrameRate)/2,(m_MaxFrameTime-m_MinFrameTime)/2.0f);

			m_FrameGraph.UpdateGraph(frameRate);
		}
	}
}