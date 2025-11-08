using System;
using Cysharp.Threading.Tasks;
using KZLib.KZUtility;

namespace KZLib
{
	public partial class UIManager : LoadSingletonMB<UIManager>
	{
		public async UniTask PlayLoadingAsync(Func<UniTask> onPlayTask)
		{
			var loadingPanelUI = Open<LoadingPanelUI>(Global.LOADING_PANEL_UI);

			await onPlayTask();

			Close(loadingPanelUI);
		}

		public async UniTask PlayLoadingIncludeTransitionAsync(string transitionName,Func<UniTask> onPlayTask)
		{
			await _PlayTransitionOutInAsync(transitionName,async ()=> { await PlayLoadingAsync(onPlayTask); });
		}
	}
}