using Cysharp.Threading.Tasks;
using KZLib.KZData;
using KZLib.KZUtility;
using VideoPanel;

namespace KZLib
{
	public partial class UIManager : LoadSingletonMB<UIManager>
	{
		/// <summary>
		/// fade out -> prepare video -> fade in -> play video
		/// </summary>
		public async UniTask WatchVideoAsync(UINameType transitionNameType,VideoData videoData)
		{
			await _WatchVideoAsync(transitionNameType,videoData,false);
		}

		/// <summary>
		/// fade out -> show loading -> fade in -> play video
		/// </summary>
		public async UniTask WatchVideoIncludeLoadingAsync(UINameType transitionNameType,VideoData videoData)
		{
			await _WatchVideoAsync(transitionNameType,videoData,true);
		}

		private async UniTask _WatchVideoAsync(UINameType transitionNameType,VideoData videoData,bool includeLoading)
		{
			if(videoData == null)
			{
				LogSvc.System.E("VideoData is null");

				return;
			}

			SoundManager.In.PauseBGMSound();

			var videoPanel = Open(UINameType.VideoPanelUI) as VideoPanelUI;

			videoPanel.Hide();

			if(includeLoading)
			{
				await PlayLoadingIncludeTransitionAsync(transitionNameType,async ()=> { await _PrepareVideoAsync(videoPanel,videoData); });
			}
			else
			{
				await _PlayTransitionOutInAsync(transitionNameType,async ()=> { await _PrepareVideoAsync(videoPanel,videoData); });
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