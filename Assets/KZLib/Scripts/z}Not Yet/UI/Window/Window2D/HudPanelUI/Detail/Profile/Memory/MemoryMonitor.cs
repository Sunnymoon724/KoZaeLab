using TMPro;
using UnityEngine;

namespace HudPanel
{
	public class MemoryMonitor : BaseComponentUI
	{
		[SerializeField]
		private TMP_Text m_SizeText = null;
		[SerializeField]
		private TMP_Text m_UnitText = null;

		public void SetMemory(long _size)
		{
			_size.ByteToString(out var size,out var unit);

			m_SizeText.SetSafeTextMeshPro(string.Format("{0:N2}",size));
			m_UnitText.SetSafeTextMeshPro(unit);
		}
	}
}