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
			m_FrameRateText.SetSafeTextMeshPro(string.Format("{0:d2}",_frameRate));
			m_FrameTimeText.SetSafeTextMeshPro(string.Format("{0:f2}",_frameTime));
		}
	}
}