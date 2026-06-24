using UnityEngine;

namespace KZLib.UI.Widgets.Debug
{
	public class FrameProfile : PeriodicOverlayBehaviour
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

		private long m_sampleCount = 0;
		private long m_frameRateSum = 0;
		private double m_frameTimeSum = 0.0;

		private void OnEnable()
		{
			_ResetStatistics();
		}

		public override void Refresh(int frameRate,float frameTime)
		{
			m_minFrameRate = Mathf.Min(m_minFrameRate,frameRate);
			m_maxFrameRate = Mathf.Max(m_maxFrameRate,frameRate);

			m_minFrameTime = Mathf.Min(m_minFrameTime,frameTime);
			m_maxFrameTime = Mathf.Max(m_maxFrameTime,frameTime);

			m_sampleCount++;
			m_frameRateSum += frameRate;
			m_frameTimeSum += frameTime;

			var averageFrameRate = (int) (m_frameRateSum/m_sampleCount);
			var averageFrameTime = (float) (m_frameTimeSum/m_sampleCount);

			m_currentFrameMonitor.SetFrame(frameRate,frameTime);
			m_maximumFrameMonitor.SetFrame(m_maxFrameRate,m_maxFrameTime);
			m_minimumFrameMonitor.SetFrame(m_minFrameRate,m_minFrameTime);
			m_averageFrameMonitor.SetFrame(averageFrameRate,averageFrameTime);

			m_frameGraph.RefreshGraph(frameRate);
		}

		private void _ResetStatistics()
		{
			m_minFrameRate = int.MaxValue;
			m_maxFrameRate = 0;

			m_minFrameTime = float.MaxValue;
			m_maxFrameTime = 0.0f;

			m_sampleCount = 0;
			m_frameRateSum = 0;
			m_frameTimeSum = 0.0;
		}
	}
}
