using KZLib.UI.Widgets;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.UI
{
	/// <summary>
	/// Full-screen download panel that forwards progress to <see cref="DownloadWidget"/>.
	/// Updated via <see cref="SetProgress"/> from <see cref="KZLib.UIManager.PlayDownloadAsync"/> or
	/// <see cref="KZLib.AddressablesManager.DownloadAssetAsync"/>.
	/// </summary>
	public class DownloadPanel : BasePanel
	{
		[SerializeField,Required]
		private DownloadWidget m_downloadWidget = null;

		public override void Open(object param)
		{
			SetProgress(0.0f,0,0);

			base.Open(param);

			if(!m_downloadWidget)
			{
				LogChannel.UI.W($"{NameTag} download widget is not assigned.");
			}
		}

		/// <summary>Updates the download widget. <paramref name="progress"/> is clamped to [0, 1].</summary>
		public void SetProgress(float progress,long downloadedSize,long totalSize)
		{
			if(!m_downloadWidget)
			{
				return;
			}

			m_downloadWidget.SetProgress(Mathf.Clamp01(progress),downloadedSize,totalSize);
		}
	}
}