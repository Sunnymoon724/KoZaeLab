using System;
using Cysharp.Threading.Tasks;
using KZLib.KZData;
using KZLib.KZUtility;

namespace KZLib
{
	public partial class UIManager : LoadSingletonMB<UIManager>
	{
		public void PlayTransitionOutIn(UINameType transitionNameType,Func<UniTask> onPlayTask)
		{
			_PlayTransitionOutInAsync(transitionNameType,onPlayTask).Forget();
		}

		public void PlayTransitionOutIn(UINameType transitionNameType,Action onPlayAction)
		{
			_PlayTransitionOutInAsync(transitionNameType,onPlayAction).Forget();
		}

		private async UniTask _PlayTransitionOutInAsync(UINameType transitionNameType,Func<UniTask> onPlayTask)
		{
			// darker
			await PlayTransitionOutAsync(transitionNameType,false);

			await onPlayTask.Invoke();

			// brighter
			await PlayTransitionInAsync(transitionNameType,true);
		}

		private async UniTask _PlayTransitionOutInAsync(UINameType transitionNameType,Action onPlayAction)
		{
			// darker
			await PlayTransitionOutAsync(transitionNameType,false);

			onPlayAction.Invoke();

			// brighter
			await PlayTransitionInAsync(transitionNameType,true);
		}

		public async UniTask PlayTransitionInAsync(UINameType transitionNameType,bool isAutoHide = true)
		{
			await _PlayTransitionAsync(transitionNameType,isAutoHide,true);
		}

		public async UniTask PlayTransitionOutAsync(UINameType transitionNameType,bool isAutoHide = true)
		{
			await _PlayTransitionAsync(transitionNameType,isAutoHide,false);
		}

		private async UniTask _PlayTransitionAsync(UINameType transitionNameType,bool isAutoHide,bool isReverse)
		{
			if(transitionNameType == UINameType.None)
			{
				return;
			}

			var panel = Open(transitionNameType) as TransitionPanelUI;

			if(panel == null)
			{
				return;
			}

			await panel.PlayTransitionAsync(isAutoHide,isReverse);
		}
	}
}