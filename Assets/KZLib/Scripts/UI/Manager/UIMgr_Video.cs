using Cysharp.Threading.Tasks;

namespace KZLib
{
	public partial class UIMgr : LoadSingletonMB<UIMgr>
	{
		/// <summary>
		/// 페이드 아웃 -> 비디오 로딩 -> 페이드 인 -> 비디오 실행 
		/// </summary>
		public async UniTask WatchVideoAsync(VideoPanelUI.VideoParam _param)
		{
			await WatchVideoInnerAsync(_param,false);
		}

		/// <summary>
		/// 페이드 아웃 -> 비디오 로딩 -> 페이드 인 -> 비디오 실행 
		/// </summary>
		public async UniTask WatchVideoIncludeLoadingAsync(VideoPanelUI.VideoParam _param)
		{
			await WatchVideoInnerAsync(_param,true);
		}

		private async UniTask WatchVideoInnerAsync(VideoPanelUI.VideoParam _param,bool _useLoading)
		{
			SoundMgr.In.StopBGM(true);

			await PlayTransitionOutAsync(new TransitionData(),false);

			var videoPanel = Open<VideoPanelUI>(UITag.VideoPanelUI,_param);

			if(_useLoading)
			{
				await PlayLoadingAsync(videoPanel.WaitForPreparedAsync);
			}
			else
			{
				await videoPanel.WaitForPreparedAsync();
			}

			videoPanel.PlayVideo();

			await videoPanel.WaitForPlayingAsync();

			Close(UITag.VideoPanelUI);

			await PlayTransitionInAsync(new TransitionData(),true);
		}
	}
}