using TMPro;
using UnityEngine;

namespace KZLib.KZWidget.Debug
{
	public class MemoryMonitor : BaseComponentUI
	{
		[SerializeField]
		private TMP_Text m_sizeText = null;
		[SerializeField]
		private TMP_Text m_unitText = null;

		public void SetMemory(long memorySize)
		{
			memorySize.ByteToString(out var size,out var unit);

			m_sizeText.SetSafeTextMeshPro($"{size:n2}");
			m_unitText.SetSafeTextMeshPro(unit);
		}
	}
}