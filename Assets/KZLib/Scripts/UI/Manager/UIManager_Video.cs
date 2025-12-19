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
		public async UniTask WatchVideoAsync(UINameType transitionNameType,VideoInfo videoInfo)
		{
			await _WatchVideoAsync(transitionNameType,videoInfo,false);
		}

		/// <summary>
		/// fade out -> show loading -> fade in -> play video
		/// </summary>
		public async UniTask WatchVideoIncludeLoadingAsync(UINameType transitionNameType,VideoInfo videoInfo)
		{
			await _WatchVideoAsync(transitionNameType,videoInfo,true);
		}

		private async UniTask _WatchVideoAsync(UINameType transitionNameType,VideoInfo videoInfo,bool includeLoading)
		{
			if(videoInfo == null)
			{
				LogSvc.System.E("VideoInfo is null");

				return;
			}

			SoundManager.In.PauseBGMSound();

			var videoPanel = Open(UINameType.VideoPanelUI) as VideoPanelUI;

			videoPanel.Hide();

			async UniTask _PlayTaskAsync()
			{
				await _PrepareVideoAsync(videoPanel,videoInfo);
			}

			if(includeLoading)
			{
				await PlayLoadingIncludeTransitionAsync(transitionNameType,_PlayTaskAsync);
			}
			else
			{
				await _PlayTransitionOutInAsync(transitionNameType,_PlayTaskAsync);
			}

			videoPanel.PlayVideo();

			await videoPanel.WaitForPlayingAsync();

			Close(videoPanel);

			SoundManager.In.ResumeBGMSound();
		}

		private async UniTask _PrepareVideoAsync(VideoPanelUI videoPanel,VideoInfo videoInfo)
		{
			videoPanel.Show();

			await videoPanel.PrepareVideoAsync(videoInfo);
		}
	}
}