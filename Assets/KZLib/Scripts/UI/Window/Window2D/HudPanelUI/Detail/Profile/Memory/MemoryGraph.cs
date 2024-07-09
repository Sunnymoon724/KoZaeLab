using System.Linq;
using UnityEngine;

namespace HudPanel
{
	public class MemoryGraph : BaseComponentUI
	{
		private const int GRAPH_RESOLUTION = 150;

		private readonly float[] m_ReservedArray = new float[GRAPH_RESOLUTION];
		private readonly float[] m_AllocatedArray = new float[GRAPH_RESOLUTION];
		private readonly float[] m_MonoArray = new float[GRAPH_RESOLUTION];

		[SerializeField] private GraphImageUI m_ReservedGraphImage = null;
		[SerializeField] private GraphImageUI m_AllocatedGraphImage = null;
		[SerializeField] private GraphImageUI m_MonoGraphImage = null;

		protected override void OnEnable()
		{
			base.OnEnable();

			m_ReservedGraphImage.SetResolution(GRAPH_RESOLUTION);
			m_AllocatedGraphImage.SetResolution(GRAPH_RESOLUTION);
			m_MonoGraphImage.SetResolution(GRAPH_RESOLUTION);
		}

		public void UpdateGraph(float _reserved,float _allocated,float _mono)
		{
			for(var i=0;i<GRAPH_RESOLUTION;i++)
			{
				m_ReservedArray[i] = i >= GRAPH_RESOLUTION-1 ? _reserved : m_ReservedArray[i+1];
				m_AllocatedArray[i] = i >= GRAPH_RESOLUTION-1 ? _allocated : m_AllocatedArray[i+1];
				m_MonoArray[i] = i >= GRAPH_RESOLUTION-1 ? _mono : m_MonoArray[i+1];
			}

			var reservedMaxValue = m_ReservedArray.Max();

			m_ReservedGraphImage.UpdateGraph(m_ReservedArray.Select(x=>x/reservedMaxValue).ToArray());

			var allocatedMaxValue = m_AllocatedArray.Max();

			m_AllocatedGraphImage.UpdateGraph(m_AllocatedArray.Select(x=>x/allocatedMaxValue).ToArray());

			var monoMaxValue = m_MonoArray.Max();

			m_MonoGraphImage.UpdateGraph(m_MonoArray.Select(x=>x/monoMaxValue).ToArray());
		}
	}
}