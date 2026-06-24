using System;
using Cysharp.Threading.Tasks;
using KZLib.Data;
using KZLib.UI;
using KZLib.Utilities;

namespace KZLib
{
	public partial class UIManager : SingletonMB<UIManager>
	{
		/// <summary>
		/// Opens the loading panel, runs <paramref name="onPlayTask"/>, then closes the panel.
		/// Passes a progress callback that updates <see cref="LoadingPanel"/>; ignore it for indeterminate work.
		/// The loading panel is always closed, even when <paramref name="onPlayTask"/> throws.
		/// </summary>
		public async UniTask PlayLoadingAsync(Func<Action<float>,UniTask> onPlayTask)
		{
			var loadingPanelUI = Open(CommonUINameTag.LoadingPanel) as LoadingPanel;

			if(loadingPanelUI == null)
			{
				throw new NullReferenceException("LoadingPanel is null.");
			}

			try
			{
				void _UpdateProgress(float progress)
				{
					loadingPanelUI.SetLoadingProgress(progress);
				}

				await onPlayTask(_UpdateProgress);
			}
			finally
			{
				Close(loadingPanelUI);
			}
		}

		/// <summary>
		/// Runs <see cref="PlayLoadingAsync"/> between a transition fade-out and fade-in.
		/// </summary>
		public async UniTask PlayLoadingIncludeTransitionAsync(CommonUINameTag transitionNameTag,Func<Action<float>,UniTask> onPlayTask)
		{
			async UniTask _PlayTaskAsync()
			{
				await PlayLoadingAsync(onPlayTask);
			}

			await _PlayTransitionOutInAsync(transitionNameTag,_PlayTaskAsync);
		}
	}
}
