using TMPro;
using UnityEngine;

namespace KZLib.KZWidget.Debug
{
	public class SimpleFps : BaseComponent,IPeriodicOverlay
	{
		[SerializeField]
		private TMP_Text m_frameText = null;

		public bool IsActive => gameObject.activeInHierarchy;

		private string _GetFrameRate(int frameRate)
		{
			return frameRate > 0 ? $"{frameRate:000}" : "----";
		}

		private string _GetFrameTime(float frameTime)
		{
			return frameTime > 0.0f ? $"{frameTime:00.00}" : "----";
		}

		public void Refresh(int frameRate,float frameTime)
		{
			m_frameText.SetSafeTextMeshPro($"[{_GetFrameRate(frameRate)}fps / {_GetFrameTime(frameTime)}ms]");
		}
	}
}