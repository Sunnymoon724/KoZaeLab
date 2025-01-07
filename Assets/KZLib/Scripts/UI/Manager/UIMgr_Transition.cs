using System;
using Cysharp.Threading.Tasks;
using KZLib.KZUtility;
using TransitionPanel;

namespace KZLib
{
	public partial class UIMgr : LoadSingletonMB<UIMgr>
	{
		public void PlayTransitionOutIn(TransitionData transitionData,Func<UniTask> onPlayTask)
		{
			PlayTransitionOutInAsync(transitionData,onPlayTask).Forget();
		}

		public void PlayTransitionOutIn(TransitionData transitionData,Action onPlay)
		{
			PlayTransitionOutInAsync(transitionData,onPlay).Forget();
		}

		private async UniTask PlayTransitionOutInAsync(TransitionData transitionData,Func<UniTask> onPlayTask)
		{
			// darker
			await PlayTransitionOutAsync(transitionData,false);

			await onPlayTask.Invoke();

			// brighter
			await PlayTransitionInAsync(transitionData,true);
		}

		private async UniTask PlayTransitionOutInAsync(TransitionData transitionData,Action onPlay)
		{
			// darker
			await PlayTransitionOutAsync(transitionData,false);

			onPlay.Invoke();

			// brighter
			await PlayTransitionInAsync(transitionData,true);
		}

		public async UniTask PlayTransitionInAsync(TransitionData transitionData,bool isAutoHide = true)
		{
			await _PlayTransitionAsync(transitionData,isAutoHide,true);
		}

		public async UniTask PlayTransitionOutAsync(TransitionData transitionData,bool isAutoHide = true)
		{
			await _PlayTransitionAsync(transitionData,isAutoHide,false);
		}

		private async UniTask _PlayTransitionAsync(TransitionData transitionData,bool isAutoHide,bool isReverse)
		{
			if(transitionData == null)
			{
				return;
			}

			var transitionPanel = Open<TransitionPanelUI>(UITag.TransitionPanelUI);

			await transitionPanel.PlayTransitionAsync(transitionData,isAutoHide,isReverse);
		}
	}
}