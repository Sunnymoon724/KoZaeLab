using TMPro;
using UnityEngine;

namespace HudPanel
{
	public class SimpleFpsUI : BaseComponentUI,IHudUI
	{
		[SerializeField]
		private TMP_Text m_FrameText = null;

		private string GetFrameRate(int _value)
		{
			return _value > 0 ? string.Format("{0:000}",_value) : "----";
		}

		private string GetFrameTime(float _value)
		{
			return _value > 0.0f ? string.Format("{0:00.00}",_value) : "----";
		}

		public void Refresh(float _deltaTime,int _frameCount)
		{
			var frameRate = Mathf.RoundToInt(_frameCount/_deltaTime);
			var frameTime = _deltaTime/_frameCount*1000.0f;

			m_FrameText.SetSafeTextMeshPro(string.Format("[{0}fps / {1}ms]",GetFrameRate(frameRate),GetFrameTime(frameTime)));
		}
	}
}