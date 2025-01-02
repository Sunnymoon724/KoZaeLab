using Cysharp.Threading.Tasks;
using KZLib.KZUtility;

namespace KZLib
{
	public partial class UIMgr : LoadSingletonMB<UIMgr>
	{
		/// <summary>
		/// fade out -> prepare video -> fade in -> play video
		/// </summary>
		public async UniTask WatchVideoAsync(VideoPanelUI.VideoParam _param)
		{
			if(_param == null)
			{
				LogTag.System.E("VideoParam is null");

				return;
			}

			await WatchVideoInnerAsync(_param,false);
		}

		/// <summary>
		/// fade out -> show loading -> fade in -> play video
		/// </summary>
		public async UniTask WatchVideoIncludeLoadingAsync(VideoPanelUI.VideoParam _param)
		{
			if(_param == null)
			{
				LogTag.System.E("VideoParam is null");

				return;
			}

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