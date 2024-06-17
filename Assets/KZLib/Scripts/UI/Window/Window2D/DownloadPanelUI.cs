using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DownloadPanelUI : WindowUI2D
{
	public override UITag Tag => UITag.DownloadPanelUI;

	[SerializeField] private Image m_ProgressImage = null;
	[SerializeField] private TMP_Text m_DataSizeText = null;
	[SerializeField] private TMP_Text m_ProgressText = null;

	public override void Initialize()
	{
		base.Initialize();

		m_ProgressImage.fillAmount = 0.0f;
		m_DataSizeText.SetSafeTextMeshPro(string.Empty);
		m_ProgressText.SetSafeTextMeshPro(string.Empty);
	}

	public void SetProgress(float _progress,long _downloaded,long _total)
	{
		m_ProgressImage.fillAmount = _progress;
		m_ProgressText.SetSafeTextMeshPro((_progress*100.0f).ToStringPercent(1));
	}
}