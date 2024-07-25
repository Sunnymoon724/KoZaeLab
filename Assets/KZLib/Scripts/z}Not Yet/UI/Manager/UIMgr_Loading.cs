using System;
using Cysharp.Threading.Tasks;

namespace KZLib
{
	public partial class UIMgr : LoadSingletonMB<UIMgr>
	{
		public async UniTask PlayLoadingNoFadeAsync(Func<UniTask> _onPlayTask)
		{
			Open<LoadingPanelUI>(UITag.LoadingPanelUI);

			await _onPlayTask();

			Close(UITag.LoadingPanelUI);
		}

		// public async UniTask PlayLoadingAsync(UniTask _task)
		// {
		// 	await OpenFadeAsync<LoadingPanelUI>(UITag.LoadingPanelUI);

		// 	await _task;

		// 	await CloseFadeAsync(UITag.LoadingPanelUI);
		// }
	}
}