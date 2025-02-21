using System;
using Cysharp.Threading.Tasks;
using KZLib.KZUtility;
using TransitionPanel;

namespace KZLib
{
	public partial class UIManager : LoadSingletonMB<UIManager>
	{
		public async UniTask PlayLoadingAsync(Func<UniTask> onPlayTask)
		{
			var loadingPanelUI = Open<LoadingPanelUI>(UITag.LoadingPanelUI);

			await onPlayTask();

			Close(loadingPanelUI);
		}

		public async UniTask PlayLoadingIncludeTransitionAsync(TransitionData transitionData,Func<UniTask> onPlayTask)
		{
			await PlayTransitionOutInAsync(transitionData,async ()=> { await PlayLoadingAsync(onPlayTask); });
		}
	}
}