using UnityEngine;
using UnityEngine.Profiling;

namespace HudPanel
{
	public class MemoryProfileUI : BaseComponentUI
	{
		[SerializeField]
		private MemoryMonitorUI m_reservedMemory = null;
		[SerializeField]
		private MemoryMonitorUI m_allocatedMemory = null;
		[SerializeField]
		private MemoryMonitorUI m_graphics = null;
		[SerializeField]
		private MemoryMonitorUI m_monoHeap = null;
		[SerializeField]
		private MemoryMonitorUI m_monoUsed = null;

		[SerializeField]
		private ShiftGraphImageUI m_reservedGraph = null;
		[SerializeField]
		private ShiftGraphImageUI m_allocatedGraph = null;
		[SerializeField]
		private ShiftGraphImageUI m_monoGraph = null;

		public void Refresh()
		{
			var reserved = Profiler.GetTotalReservedMemoryLong();
			var allocated = Profiler.GetTotalAllocatedMemoryLong();
			var mono = Profiler.GetMonoUsedSizeLong();

			m_reservedMemory.SetMemory(reserved);
			m_allocatedMemory.SetMemory(allocated);

			m_graphics.SetMemory(Profiler.GetAllocatedMemoryForGraphicsDriver());

			m_monoHeap.SetMemory(Profiler.GetMonoHeapSizeLong());
			m_monoUsed.SetMemory(mono);

			m_reservedGraph.UpdateGraph(reserved);
			m_allocatedGraph.UpdateGraph(allocated);
			m_monoGraph.UpdateGraph(mono);
		}
	}
}