using DownloadPanel;
using UnityEngine;

public class DownloadPanelUI : WindowUI2D
{
	public override UITag Tag => UITag.DownloadPanelUI;

	[SerializeField] private DownloadUI m_downloadUI = null;

	protected override void Initialize()
	{
		base.Initialize();

		m_downloadUI.Initialize();
	}

	public void SetProgress(float progress,long downloadedSize,long totalSize)
	{
		m_downloadUI.SetProgress(progress,downloadedSize,totalSize);
	}
}