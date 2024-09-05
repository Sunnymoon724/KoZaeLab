using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DownloadPanelUI : WindowUI2D
{
	public override UITag Tag => UITag.DownloadPanelUI;

	[SerializeField] private Image m_ProgressImage = null;
	[SerializeField] private TMP_Text m_DownloadedSizeText = null;
	[SerializeField] private TMP_Text m_TotalSizeText = null;
	[SerializeField] private TMP_Text m_ProgressText = null;

	protected override void Initialize()
	{
		base.Initialize();

		m_ProgressImage.fillAmount = 0.0f;

		m_DownloadedSizeText.SetSafeTextMeshPro(string.Empty);
		m_TotalSizeText.SetSafeTextMeshPro(string.Empty);

		m_ProgressText.SetSafeTextMeshPro(string.Empty);
	}

	public virtual void SetProgress(float _progress,long _downloaded,long _total)
	{
		m_ProgressImage.fillAmount = _progress;
		m_ProgressText.SetSafeTextMeshPro((_progress*100.0f).ToStringPercent(1));

		m_DownloadedSizeText.SetSafeTextMeshPro(_downloaded.ByteToString());
		m_TotalSizeText.SetSafeTextMeshPro(_total.ByteToString());
	}
}