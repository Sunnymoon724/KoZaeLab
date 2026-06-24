using System;
using Cysharp.Threading.Tasks;
using KZLib.Sounds;
using KZLib.UI;
using KZLib.Utilities;

namespace KZLib
{
	public partial class UIManager : SingletonMB<UIManager>
	{
		/// <summary>
		/// Fade out, prepare video, fade in, then play to completion.
		/// Background music is paused for the duration and resumed afterward.
		/// </summary>
		public async UniTask WatchVideoAsync(CommonUINameTag transitionNameTag,VideoInfo videoInfo)
		{
			await _WatchVideoAsync(transitionNameTag,videoInfo,false);
		}

		/// <summary>
		/// Fade out, show loading while preparing video, fade in, then play to completion.
		/// Background music is paused for the duration and resumed afterward.
		/// </summary>
		public async UniTask WatchVideoIncludeLoadingAsync(CommonUINameTag transitionNameTag,VideoInfo videoInfo)
		{
			await _WatchVideoAsync(transitionNameTag,videoInfo,true);
		}

		private async UniTask _WatchVideoAsync(CommonUINameTag transitionNameTag,VideoInfo videoInfo,bool includeLoading)
		{
			if(videoInfo == null)
			{
				throw new InvalidOperationException("VideoInfo is null.");
			}

			SoundManager.In.PauseMusic();

			VideoPanel videoPanel = null;

			try
			{
				videoPanel = Open(CommonUINameTag.VideoPanel) as VideoPanel;

				if(videoPanel == null)
				{
					throw new NullReferenceException("VideoPanel is null.");
				}

				videoPanel.Hide(true);

				async UniTask _PrepareTaskAsync()
				{
					await _PrepareVideoAsync(videoPanel,videoInfo);
				}

				async UniTask _PrepareWithLoadingAsync(Action<float> onUpdateProgress)
				{
					await _PrepareVideoAsync(videoPanel,videoInfo);
				}

				if(includeLoading)
				{
					await PlayLoadingIncludeTransitionAsync(transitionNameTag,_PrepareWithLoadingAsync);
				}
				else
				{
					await _PlayTransitionOutInAsync(transitionNameTag,_PrepareTaskAsync);
				}

				videoPanel.PlayVideo();

				await videoPanel.WaitUntilPlaybackEndsAsync();
			}
			finally
			{
				if(videoPanel != null)
				{
					Close(videoPanel);
				}

				SoundManager.In.ResumeMusic();
			}
		}

		/// <summary>
		/// Reveals the video panel and prepares the clip without starting playback.
		/// </summary>
		private async UniTask _PrepareVideoAsync(VideoPanel videoPanel,VideoInfo videoInfo)
		{
			videoPanel.Hide(false);

			await videoPanel.PrepareVideoAsync(videoInfo);
		}
	}
}
