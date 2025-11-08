using System;
using Cysharp.Threading.Tasks;
using KZLib.KZUtility;

namespace KZLib
{
	public partial class UIManager : LoadSingletonMB<UIManager>
	{
		public void PlayTransitionOutIn(string transitionName,Func<UniTask> onPlayTask)
		{
			_PlayTransitionOutInAsync(transitionName,onPlayTask).Forget();
		}

		public void PlayTransitionOutIn(string transitionName,Action onPlayAction)
		{
			_PlayTransitionOutInAsync(transitionName,onPlayAction).Forget();
		}

		private async UniTask _PlayTransitionOutInAsync(string transitionName,Func<UniTask> onPlayTask)
		{
			// darker
			await PlayTransitionOutAsync(transitionName,false);

			await onPlayTask.Invoke();

			// brighter
			await PlayTransitionInAsync(transitionName,true);
		}

		private async UniTask _PlayTransitionOutInAsync(string transitionName,Action onPlayAction)
		{
			// darker
			await PlayTransitionOutAsync(transitionName,false);

			onPlayAction.Invoke();

			// brighter
			await PlayTransitionInAsync(transitionName,true);
		}

		public async UniTask PlayTransitionInAsync(string transitionName,bool isAutoHide = true)
		{
			await _PlayTransitionAsync(transitionName,isAutoHide,true);
		}

		public async UniTask PlayTransitionOutAsync(string transitionName,bool isAutoHide = true)
		{
			await _PlayTransitionAsync(transitionName,isAutoHide,false);
		}

		private async UniTask _PlayTransitionAsync(string transitionName,bool isAutoHide,bool isReverse)
		{
			if(transitionName.IsEmpty())
			{
				return;
			}

			var panel = Open<TransitionPanelUI>(transitionName);

			if(panel == null)
			{
				return;
			}

			await panel.PlayTransitionAsync(isAutoHide,isReverse);
		}
	}
}