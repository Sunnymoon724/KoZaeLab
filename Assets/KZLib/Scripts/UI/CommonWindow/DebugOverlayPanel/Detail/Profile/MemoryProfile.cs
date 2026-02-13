using UnityEngine;
using UnityEngine.Profiling;

namespace KZLib.UI.Widgets.Debug
{
	public class MemoryProfile : BaseComponent,IImmediateOverlay
	{
		public bool IsActive => gameObject.activeInHierarchy;

		[SerializeField]
		private MemoryMonitor m_reservedMemory = null;
		[SerializeField]
		private MemoryMonitor m_allocatedMemory = null;
		[SerializeField]
		private MemoryMonitor m_graphics = null;
		[SerializeField]
		private MemoryMonitor m_monoHeap = null;
		[SerializeField]
		private MemoryMonitor m_monoUsed = null;

		[SerializeField]
		private ShiftGraphImage m_reservedGraph = null;
		[SerializeField]
		private ShiftGraphImage m_allocatedGraph = null;
		[SerializeField]
		private ShiftGraphImage m_monoGraph = null;

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

			m_reservedGraph.RefreshGraph(reserved);
			m_allocatedGraph.RefreshGraph(allocated);
			m_monoGraph.RefreshGraph(mono);
		}
	}
}