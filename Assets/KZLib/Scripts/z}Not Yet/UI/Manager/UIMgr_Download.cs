// using System;
// using System.Collections;

// namespace KZLib
// {
// 	public partial class UIMgr : LoadSingletonMB<UIMgr>
// 	{
// 		public void DownloadAssetPlayVideo(string _label,string _videoPath,Action<bool> _onResult)
// 		{
// 			StartCoroutine(CoWatchVideo(new VideoPanelUI.ParamData(_videoPath,false,true),false,null));
// 			StartCoroutine(CoPlayDownload(_label,_onResult));
// 		}

// 		private IEnumerator CoPlayDownload(string _label,Action<bool> _onResult)
// 		{
// 			var videoPanel = Get<VideoPanelUI>(UITag.VideoPanelUI);
// 			var downloadPanel = Open<DownloadPanelUI>(UITag.DownloadPanelUI);

// 			downloadPanel.AddLink(videoPanel);

// 			yield return AddressablesMgr.In.CoDownloadAsset(_label,(progress,downloaded,total)=>
// 			{
// 				downloadPanel.SetProgress(progress,downloaded,total);
// 			},(result)=>
// 			{
// 				Close(UITag.DownloadPanelUI);

// 				_onResult?.Invoke(result);
// 			});
// 		}
// 	}
// }