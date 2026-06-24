using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KZLib.UI.Widgets
{
	/// <summary>
	/// Bar and/or percent label driven by normalized loading progress.
	/// Expects <see cref="m_progressImage"/> to use <see cref="Image.Type.Filled"/> when assigned.
	/// </summary>
	public class LoadingProgressWidget : MonoBehaviour
	{
		[SerializeField]
		private Image m_progressImage = null;
		[SerializeField]
		private TMP_Text m_progressText = null;

		private void Awake()
		{
			SetLoadingProgress(0.0f);
		}

		/// <summary>Expects <paramref name="progress"/> in [0, 1]; clamped by <see cref="LoadingPanel"/> when forwarded.</summary>
		public void SetLoadingProgress(float progress)
		{
			if(m_progressImage)
			{
				m_progressImage.fillAmount = progress;
			}

			if(m_progressText)
			{
				m_progressText.SetSafeTextMeshPro((progress*100.0f).ToStringPercent(1));
			}
		}
	}
}