﻿using TMPro;
using UnityEngine;

namespace HudPanel
{
	public class MemoryMonitorUI : BaseComponentUI
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