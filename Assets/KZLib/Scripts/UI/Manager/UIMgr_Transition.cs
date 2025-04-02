using System;
using Cysharp.Threading.Tasks;
using KZLib.KZUtility;
using TransitionPanel;

namespace KZLib
{
	public partial class UIMgr : LoadSingletonMB<UIMgr>
	{
		public void PlayTransitionOutIn(TransitionInfo info,Func<UniTask> onPlayTask)
		{
			_PlayTransitionOutInAsync(info,onPlayTask).Forget();
		}

		public void PlayTransitionOutIn(TransitionInfo info,Action onPlayAction)
		{
			_PlayTransitionOutInAsync(info,onPlayAction).Forget();
		}

		private async UniTask _PlayTransitionOutInAsync(TransitionInfo info,Func<UniTask> onPlayTask)
		{
			// darker
			await PlayTransitionOutAsync(info,false);

			await onPlayTask.Invoke();

			// brighter
			await PlayTransitionInAsync(info,true);
		}

		private async UniTask _PlayTransitionOutInAsync(TransitionInfo info,Action onPlayAction)
		{
			// darker
			await PlayTransitionOutAsync(info,false);

			onPlayAction.Invoke();

			// brighter
			await PlayTransitionInAsync(info,true);
		}

		public async UniTask PlayTransitionInAsync(TransitionInfo info,bool isAutoHide = true)
		{
			await _PlayTransitionAsync(info,isAutoHide,true);
		}

		public async UniTask PlayTransitionOutAsync(TransitionInfo info,bool isAutoHide = true)
		{
			await _PlayTransitionAsync(info,isAutoHide,false);
		}

		private async UniTask _PlayTransitionAsync(TransitionInfo info,bool isAutoHide,bool isReverse)
		{
			if(info == null)
			{
				return;
			}

			var panel = Open<TransitionPanelUI>(UITag.TransitionPanelUI);

			await panel.PlayTransitionAsync(info,isAutoHide,isReverse);
		}
	}
}