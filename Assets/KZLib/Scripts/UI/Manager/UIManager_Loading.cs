using System;
using Cysharp.Threading.Tasks;
using KZLib.Data;
using KZLib.Utilities;

namespace KZLib
{
	public partial class UIManager : SingletonMB<UIManager>
	{
		public async UniTask PlayLoadingAsync(Func<UniTask> onPlayTask)
		{
			var loadingPanelUI = Open(CommonUINameTag.LoadingPanel) as LoadingPanel;

			await onPlayTask();

			Close(loadingPanelUI);
		}

		public async UniTask PlayLoadingIncludeTransitionAsync(CommonUINameTag transitionNameTag,Func<UniTask> onPlayTask)
		{
			async UniTask _PlayTaskAsync()
			{
				await PlayLoadingAsync(onPlayTask);
			}

			await _PlayTransitionOutInAsync(transitionNameTag,_PlayTaskAsync);
		}
	}
}