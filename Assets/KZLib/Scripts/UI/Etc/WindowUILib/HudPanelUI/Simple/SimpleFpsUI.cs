using TMPro;
using UnityEngine;

namespace HudPanel
{
	public class SimpleFpsUI : BaseComponentUI,IHudUI
	{
		[SerializeField]
		private TMP_Text m_frameText = null;

		public bool IsActive => gameObject.activeSelf;

		private string _GetFrameRate(int frameRate)
		{
			return frameRate > 0 ? $"{frameRate:000}" : "----";
		}

		private string _GetFrameTime(float frameTime)
		{
			return frameTime > 0.0f ? $"{frameTime:00.00}" : "----";
		}

		public void Refresh(float deltaTime,int frameCount)
		{
			var frameRate = Mathf.RoundToInt(frameCount/deltaTime);
			var frameTime = deltaTime/frameCount*1000.0f;

			m_frameText.SetSafeTextMeshPro($"[{_GetFrameRate(frameRate)}fps / {_GetFrameTime(frameTime)}ms]");
		}
	}
}