using UnityEngine;
using TMPro;

namespace HudPanel
{
	public class FrameMonitor : BaseComponentUI
	{
		[SerializeField]
		private TMP_Text m_FrameRateText = null;
		[SerializeField]
		private TMP_Text m_FrameTimeText = null;
		
		public void SetFrameData(int _frameRate,float _frameTime)
		{
			m_FrameRateText.SetSafeTextMeshPro($"{_frameRate:d2}");
			m_FrameTimeText.SetSafeTextMeshPro($"{_frameTime:f2}");
		}
	}
}