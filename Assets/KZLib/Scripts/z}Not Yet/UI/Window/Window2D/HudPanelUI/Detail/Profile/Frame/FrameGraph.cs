using System.Linq;
using UnityEngine;

namespace HudPanel
{
	public class FrameGraph : BaseComponentUI
	{
		private const int GRAPH_RESOLUTION = 150;

		private readonly float[] m_FrameRateArray = new float[GRAPH_RESOLUTION];

		[SerializeField]
		private GraphImageUI m_GraphImage = null;

		protected override void OnEnable()
		{
			base.OnEnable();

			m_GraphImage.SetResolution(GRAPH_RESOLUTION);
		}

		public void UpdateGraph(int _frameRate)
		{
			for(var i=0;i<GRAPH_RESOLUTION;i++)
			{
				m_FrameRateArray[i] = i >= GRAPH_RESOLUTION-1 ? _frameRate : m_FrameRateArray[i+1];
			}

			var frameRateMaxValue = m_FrameRateArray.Max();

			m_GraphImage.UpdateGraph(m_FrameRateArray.Select(x=>x/frameRateMaxValue).ToArray());
		}
	}
}