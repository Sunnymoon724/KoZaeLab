using UnityEngine;
using TMPro;

namespace HudPanel
{
	public class FrameMonitorUI : BaseComponentUI
	{
		[SerializeField]
		private TMP_Text m_frameRateText = null;
		[SerializeField]
		private TMP_Text m_frameTimeText = null;
		
		public void SetFrameData(int frameRate,float frameTime)
		{
			m_frameRateText.SetSafeTextMeshPro($"{frameRate:d2}");
			m_frameTimeText.SetSafeTextMeshPro($"{frameTime:f2}");
		}
	}
}