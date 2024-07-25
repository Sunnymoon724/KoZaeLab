using Cysharp.Threading.Tasks;

namespace KZLib
{
	public partial class UIMgr : LoadSingletonMB<UIMgr>
	{
		/// <summary>
		/// 페이드 아웃 -> 비디오 로딩 -> 페이드 인 -> 비디오 실행 
		/// </summary>
		public async UniTask WatchVideoAsync(string _videoPath,bool _useSubTitle,bool _isLoop,bool _useSkip)
		{
			await WatchVideoInnerAsync(_videoPath,_useSubTitle,_isLoop,_useSkip,false);
		}

		/// <summary>
		/// 페이드 아웃 -> 비디오 로딩 -> 페이드 인 -> 비디오 실행 
		/// </summary>
		public async UniTask WatchVideoIncludeLoadingAsync(string _videoPath,bool _useSubTitle,bool _isLoop,bool _useSkip)
		{
			await WatchVideoInnerAsync(_videoPath,_useSubTitle,_isLoop,_useSkip,true);
		}

		private async UniTask WatchVideoInnerAsync(string _videoPath,bool _useSubTitle,bool _isLoop,bool _useSkip,bool _useLoading)
		{
			SoundMgr.In.StopBGM(true);

			// await OpenFadeAsync<VideoPanelUI,VideoPanelUI.ParamData>(UITag.VideoPanelUI,new VideoPanelUI.ParamData(_videoPath,_useSubTitle,_isLoop));

			var videoPanel = Get<VideoPanelUI>(UITag.VideoPanelUI);

			if(_useLoading)
			{
				await PlayLoadingNoFadeAsync(videoPanel.WaitForPreparedAsync);
			}
			else
			{
				await videoPanel.WaitForPreparedAsync();
			}

			if(_useSkip)
			{
				var skipPanel = Open<SkipPanelUI>(UITag.SkipPanelUI,new SkipPanelUI.SkipParam(() =>
				{
					videoPanel.Stop();
				}));

				videoPanel.AddLink(skipPanel);
			}

			videoPanel.PlayVideo();

			await videoPanel.WaitForPlayingAsync();

			// await CloseFadeAsync(UITag.VideoPanelUI);
		}
	}
}