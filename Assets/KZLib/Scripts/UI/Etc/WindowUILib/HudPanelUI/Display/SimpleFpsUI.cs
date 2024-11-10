using TMPro;
using UnityEngine;

namespace HudPanel
{
	public class SimpleFpsUI : BaseComponentUI,IHudUI
	{
		[SerializeField]
		private TMP_Text m_FrameText = null;

		public bool IsActive => gameObject.activeSelf;

		private string GetFrameRate(int _value)
		{
			return _value > 0 ? $"{_value:000}" : "----";
		}

		private string GetFrameTime(float _value)
		{
			return _value > 0.0f ? $"{_value:00.00}" : "----";
		}

		public void Refresh(float _deltaTime,int _frameCount)
		{
			var frameRate = Mathf.RoundToInt(_frameCount/_deltaTime);
			var frameTime = _deltaTime/_frameCount*1000.0f;

			m_FrameText.SetSafeTextMeshPro($"[{GetFrameRate(frameRate)}fps / {GetFrameTime(frameTime)}ms]");
		}
	}
}