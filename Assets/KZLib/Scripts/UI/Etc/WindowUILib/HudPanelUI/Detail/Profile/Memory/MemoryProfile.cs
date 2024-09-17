using UnityEngine;
using UnityEngine.Profiling;

namespace HudPanel
{
	public class MemoryProfile : BaseComponentUI
	{
		[SerializeField]
		private MemoryMonitor m_ReservedMemory = null;
		[SerializeField]
		private MemoryMonitor m_AllocatedMemory = null;
		[SerializeField]
		private MemoryMonitor m_Graphics = null;
		[SerializeField]
		private MemoryMonitor m_MonoHeap = null;
		[SerializeField]
		private MemoryMonitor m_MonoUsed = null;

		[SerializeField]
		private ShiftGraphImageUI m_ReservedGraph = null;
		[SerializeField]
		private ShiftGraphImageUI m_AllocatedGraph = null;
		[SerializeField]
		private ShiftGraphImageUI m_MonoGraph = null;

		public void Refresh()
		{
			var reserved = Profiler.GetTotalReservedMemoryLong();
			var allocated = Profiler.GetTotalAllocatedMemoryLong();
			var mono = Profiler.GetMonoUsedSizeLong();

			m_ReservedMemory.SetMemory(reserved);
			m_AllocatedMemory.SetMemory(allocated);

			m_Graphics.SetMemory(Profiler.GetAllocatedMemoryForGraphicsDriver());

			m_MonoHeap.SetMemory(Profiler.GetMonoHeapSizeLong());
			m_MonoUsed.SetMemory(mono);

			m_ReservedGraph.UpdateGraph(reserved);
			m_AllocatedGraph.UpdateGraph(allocated);
			m_MonoGraph.UpdateGraph(mono);
		}
	}
}