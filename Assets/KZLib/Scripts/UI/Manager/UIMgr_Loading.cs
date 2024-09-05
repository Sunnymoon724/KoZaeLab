using System;
using Cysharp.Threading.Tasks;

namespace KZLib
{
	public partial class UIMgr : LoadSingletonMB<UIMgr>
	{
		public async UniTask PlayLoadingAsync(Func<UniTask> _onPlayTask)
		{
			Open<LoadingPanelUI>(UITag.LoadingPanelUI);

			await _onPlayTask();

			Close(UITag.LoadingPanelUI);
		}

		public async UniTask PlayLoadingIncludeTransitionAsync(TransitionData _data,Func<UniTask> _onPlayTask)
		{
			await PlayTransitionOutAsync(_data,false);

			await PlayLoadingAsync(_onPlayTask);

			await PlayTransitionInAsync(_data,true);
		}
	}
}