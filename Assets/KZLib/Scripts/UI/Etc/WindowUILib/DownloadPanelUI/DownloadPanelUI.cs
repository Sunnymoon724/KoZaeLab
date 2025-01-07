using DownloadPanel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DownloadPanelUI : WindowUI2D
{
	public override UITag Tag => UITag.DownloadPanelUI;

	[SerializeField] private Image m_progressImage = null;
	[SerializeField] private TMP_Text m_downloadedSizeText = null;
	[SerializeField] private TMP_Text m_totalSizeText = null;
	[SerializeField] private DownloadUI m_downloadUI = null;

	protected override void Initialize()
	{
		base.Initialize();

		m_downloadUI.Initialize();
	}

	public virtual void SetProgress(float progress,long downloadedSize,long totalSize)
	{
		m_downloadUI.SetProgress(progress,downloadedSize,totalSize);
	}
}