using System;
using Cysharp.Threading.Tasks;
using KZLib.UI;
using KZLib.Utilities;

namespace KZLib
{
	public partial class UIManager : SingletonMB<UIManager>
	{
		/// <summary>
		/// Opens the download panel, runs <see cref="AddressablesManager.DownloadAssetAsync"/>, then closes the panel.
		/// The download panel is always closed, even when the download throws.
		/// </summary>
		public async UniTask<bool> PlayDownloadAsync(string label)
		{
			var downloadPanel = Open(CommonUINameTag.DownloadPanel) as DownloadPanel;

			if(downloadPanel == null)
			{
				throw new NullReferenceException("DownloadPanel is null.");
			}

			try
			{
				void _UpdateProgress(float progress,long downloadedSize,long totalSize)
				{
					downloadPanel.SetProgress(progress,downloadedSize,totalSize);
				}

				return await AddressablesManager.In.DownloadAssetAsync(label,_UpdateProgress);
			}
			finally
			{
				Close(downloadPanel);
			}
		}
	}
}