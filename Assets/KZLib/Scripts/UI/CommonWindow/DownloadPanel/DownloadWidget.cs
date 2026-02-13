using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KZLib.UI.Widgets
{
	public class DownloadWidget : BaseComponent
	{
		[SerializeField]
		private Image m_progressImage = null;
		[SerializeField]
		private TMP_Text m_downloadedSizeText = null;
		[SerializeField]
		private TMP_Text m_totalSizeText = null;
		[SerializeField]
		private TMP_Text m_progressText = null;

		protected override void _Initialize()
		{
			base._Initialize();

			SetProgressImage(0.0f);
			SetProgressText(string.Empty);
			SetDownloadedSizeText(string.Empty);
			SetTotalSizeText(string.Empty);
		}

		public virtual void SetProgress(float progress,long downloadedSize,long totalSize)
		{
			SetProgressImage(progress);
			SetProgressText((progress*100.0f).ToStringPercent(1));
			SetDownloadedSizeText(downloadedSize.ByteToString());
			SetTotalSizeText(totalSize.ByteToString());
		}

		protected void SetProgressImage(float progress)
		{
			if(m_progressImage)
			{
				m_progressImage.fillAmount = progress;
			}
		}

		protected void SetProgressText(string progress)
		{
			if(m_progressText)
			{
				m_progressText.SetSafeTextMeshPro(progress);
			}
		}

		protected void SetDownloadedSizeText(string downloadedSize)
		{
			if(m_downloadedSizeText)
			{
				m_downloadedSizeText.SetSafeTextMeshPro(downloadedSize);
			}
		}

		protected void SetTotalSizeText(string totalSize)
		{
			if(m_totalSizeText)
			{
				m_totalSizeText.SetSafeTextMeshPro(totalSize);
			}
		}
	}
}