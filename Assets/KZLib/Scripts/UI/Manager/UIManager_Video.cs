using Cysharp.Threading.Tasks;
using KZLib.KZData.Video;
using KZLib.KZUtility;

namespace KZLib
{
	public partial class UIManager : SingletonMB<UIManager>
	{
		/// <summary>
		/// fade out -> prepare video -> fade in -> play video
		/// </summary>
		public async UniTask WatchVideoAsync(CommonUINameTag transitionNameTag,VideoInfo videoInfo)
		{
			await _WatchVideoAsync(transitionNameTag,videoInfo,false);
		}

		/// <summary>
		/// fade out -> show loading -> fade in -> play video
		/// </summary>
		public async UniTask WatchVideoIncludeLoadingAsync(CommonUINameTag transitionNameTag,VideoInfo videoInfo)
		{
			await _WatchVideoAsync(transitionNameTag,videoInfo,true);
		}

		private async UniTask _WatchVideoAsync(CommonUINameTag transitionNameTag,VideoInfo videoInfo,bool includeLoading)
		{
			if(videoInfo == null)
			{
				LogSvc.System.E("VideoInfo is null");

				return;
			}

			SoundManager.In.PauseBGMSound();

			var videoPanel = Open(CommonUINameTag.VideoPanel) as VideoPanel;

			videoPanel.Hide(true);

			async UniTask _PlayTaskAsync()
			{
				await _PrepareVideoAsync(videoPanel,videoInfo);
			}

			if(includeLoading)
			{
				await PlayLoadingIncludeTransitionAsync(transitionNameTag,_PlayTaskAsync);
			}
			else
			{
				await _PlayTransitionOutInAsync(transitionNameTag,_PlayTaskAsync);
			}

			videoPanel.PlayVideo();

			await videoPanel.WaitForPlayingAsync();

			Close(videoPanel);

			SoundManager.In.ResumeBGMSound();
		}

		private async UniTask _PrepareVideoAsync(VideoPanel videoPanel,VideoInfo videoInfo)
		{
			videoPanel.Hide(false);

			await videoPanel.PrepareVideoAsync(videoInfo);
		}
	}
}