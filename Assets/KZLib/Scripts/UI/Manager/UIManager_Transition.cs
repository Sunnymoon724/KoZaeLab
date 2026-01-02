using System;
using Cysharp.Threading.Tasks;
using KZLib.KZUtility;

namespace KZLib
{
	public partial class UIManager : LoadSingletonMB<UIManager>
	{
		public void PlayTransitionOutIn(CommonUINameTag transitionNameTag,Func<UniTask> onPlayTask)
		{
			_PlayTransitionOutInAsync(transitionNameTag,onPlayTask).Forget();
		}

		public void PlayTransitionOutIn(CommonUINameTag transitionNameTag,Action onPlayAction)
		{
			_PlayTransitionOutInAsync(transitionNameTag,onPlayAction).Forget();
		}

		private async UniTask _PlayTransitionOutInAsync(CommonUINameTag transitionNameTag,Func<UniTask> onPlayTask)
		{
			// darker
			await PlayTransitionOutAsync(transitionNameTag,false);

			await onPlayTask.Invoke();

			// brighter
			await PlayTransitionInAsync(transitionNameTag,true);
		}

		private async UniTask _PlayTransitionOutInAsync(CommonUINameTag transitionNameTag,Action onPlayAction)
		{
			// darker
			await PlayTransitionOutAsync(transitionNameTag,false);

			onPlayAction.Invoke();

			// brighter
			await PlayTransitionInAsync(transitionNameTag,true);
		}

		public async UniTask PlayTransitionInAsync(CommonUINameTag transitionNameTag,bool isAutoHide = true)
		{
			await _PlayTransitionAsync(transitionNameTag,isAutoHide,true);
		}

		public async UniTask PlayTransitionOutAsync(CommonUINameTag transitionNameTag,bool isAutoHide = true)
		{
			await _PlayTransitionAsync(transitionNameTag,isAutoHide,false);
		}

		private async UniTask _PlayTransitionAsync(CommonUINameTag transitionNameTag,bool isAutoHide,bool isReverse)
		{
			if(transitionNameTag == CommonUINameTag.None)
			{
				return;
			}

			var panel = Open(transitionNameTag) as TransitionPanelUI;

			if(panel == null)
			{
				return;
			}

			await panel.PlayTransitionAsync(isAutoHide,isReverse);
		}
	}
}