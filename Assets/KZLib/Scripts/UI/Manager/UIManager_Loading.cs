using System;
using Cysharp.Threading.Tasks;
using KZLib.KZData;
using KZLib.KZUtility;

namespace KZLib
{
	public partial class UIManager : LoadSingletonMB<UIManager>
	{
		public async UniTask PlayLoadingAsync(Func<UniTask> onPlayTask)
		{
			var loadingPanelUI = Open(UINameType.LoadingPanelUI) as LoadingPanelUI;

			await onPlayTask();

			Close(loadingPanelUI);
		}

		public async UniTask PlayLoadingIncludeTransitionAsync(UINameType transitionNameType,Func<UniTask> onPlayTask)
		{
			async UniTask _PlayTaskAsync()
			{
				await PlayLoadingAsync(onPlayTask);
			}

			await _PlayTransitionOutInAsync(transitionNameType,_PlayTaskAsync);
		}
	}
}