using Cysharp.Threading.Tasks;
using KZLib.KZUtility;
using TransitionPanel;
using VideoPanel;

namespace KZLib
{
	public partial class UIMgr : LoadSingletonMB<UIMgr>
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
				LogTag.System.E("VideoData is null");

				return;
			}

			SoundMgr.In.PauseBGMSound();

			var videoPanel = Open<VideoPanelUI>(UITag.VideoPanelUI);

			videoPanel.Hide();

			if(includeLoading)
			{
				await PlayLoadingIncludeTransitionAsync(new TransitionInfo(),async ()=> { await _PrepareVideoAsync(videoPanel,videoData); });
			}
			else
			{
				await _PlayTransitionOutInAsync(new TransitionInfo(),async ()=> { await _PrepareVideoAsync(videoPanel,videoData); });
			}

			videoPanel.PlayVideo();

			await videoPanel.WaitForPlayingAsync();

			Close(videoPanel);

			SoundMgr.In.ResumeBGMSound();
		}

		private async UniTask _PrepareVideoAsync(VideoPanelUI videoPanel,VideoData videoData)
		{
			videoPanel.Show();

			await videoPanel.PrepareVideoAsync(videoData);
		}
	}
}