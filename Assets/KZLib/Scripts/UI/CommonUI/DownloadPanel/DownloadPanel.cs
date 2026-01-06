using KZLib.KZWidget;
using UnityEngine;

public class DownloadPanel : BasePanel
{
	[SerializeField]
	private DownloadWidget m_downloadWidget = null;

	public void SetProgress(float progress,long downloadedSize,long totalSize)
	{
		m_downloadWidget.SetProgress(progress,downloadedSize,totalSize);
	}
}