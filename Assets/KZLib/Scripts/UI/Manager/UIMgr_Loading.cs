using System;
using Cysharp.Threading.Tasks;
using KZLib.KZUtility;
using TransitionPanel;

namespace KZLib
{
	public partial class UIMgr : LoadSingletonMB<UIMgr>
	{
		public async UniTask PlayLoadingAsync(Func<UniTask> onPlayTask)
		{
			var loadingPanelUI = Open<LoadingPanelUI>(UITag.LoadingPanelUI);

			await onPlayTask();

			Close(loadingPanelUI);
		}

		public async UniTask PlayLoadingIncludeTransitionAsync(TransitionInfo info,Func<UniTask> onPlayTask)
		{
			await PlayTransitionOutInAsync(info,async ()=> { await PlayLoadingAsync(onPlayTask); });
		}
	}
}