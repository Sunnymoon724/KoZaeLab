using Cysharp.Threading.Tasks;
using KZLib.KZUtility;
using VideoPanel;

namespace KZLib
{
	public partial class UIManager : LoadSingletonMB<UIManager>
	{
		/// <summary>
		/// fade out -> prepare video -> fade in -> play video
		/// </summary>
		public async UniTask WatchVideoAsync(VideoData videoData)
		{
			await _WatchVideoAsync(videoData,false);
		}

		/// <summary>
		/// fade out -> show loading -> fade in -> play video
		/// </summary>
		public async UniTask WatchVideoIncludeLoadingAsync(VideoData videoData)
		{
			await _WatchVideoAsync(videoData,true);
		}

		private async UniTask _WatchVideoAsync(VideoData videoData,bool includeLoading)
		{
			if(videoData == null)
			{
				LogSvc.System.E("VideoData is null");

				return;
			}

			SoundManager.In.PauseBGMSound();

			var videoPanel = Open<VideoPanelUI>(Global.VIDEO_PANEL_UI);

			videoPanel.Hide();

			if(includeLoading)
			{
				await PlayLoadingIncludeTransitionAsync(Global.TRANSITION_PANEL_UI,async ()=> { await _PrepareVideoAsync(videoPanel,videoData); });
			}
			else
			{
				await _PlayTransitionOutInAsync(Global.TRANSITION_PANEL_UI,async ()=> { await _PrepareVideoAsync(videoPanel,videoData); });
			}

			videoPanel.PlayVideo();

			await videoPanel.WaitForPlayingAsync();

			Close(videoPanel);

			SoundManager.In.ResumeBGMSound();
		}

		private async UniTask _PrepareVideoAsync(VideoPanelUI videoPanel,VideoData videoData)
		{
			videoPanel.Show();

			await videoPanel.PrepareVideoAsync(videoData);
		}
	}
}