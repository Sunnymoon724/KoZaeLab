using System;
using Cysharp.Threading.Tasks;
using KZLib.UI;
using KZLib.Utilities;

namespace KZLib
{
	public partial class UIManager : SingletonMB<UIManager>
	{
		/// <summary>
		/// Fades out, runs <paramref name="onPlayTask"/>, then fades in. Fire-and-forget variant.
		/// </summary>
		public void PlayTransitionOutIn(CommonUINameTag transitionNameTag,Func<UniTask> onPlayTask)
		{
			_PlayTransitionOutInAsync(transitionNameTag,onPlayTask).Forget(_OnForgetException);
		}

		/// <summary>
		/// Fades out, runs <paramref name="onPlayAction"/>, then fades in. Fire-and-forget variant.
		/// </summary>
		public void PlayTransitionOutIn(CommonUINameTag transitionNameTag,Action onPlayAction)
		{
			_PlayTransitionOutInAsync(transitionNameTag,onPlayAction).Forget(_OnForgetException);
		}

		private async UniTask _PlayTransitionOutInAsync(CommonUINameTag transitionNameTag,Func<UniTask> onPlayTask)
		{
			await PlayTransitionOutAsync(transitionNameTag,false);

			await onPlayTask.Invoke();

			await PlayTransitionInAsync(transitionNameTag,true);
		}

		private async UniTask _PlayTransitionOutInAsync(CommonUINameTag transitionNameTag,Action onPlayAction)
		{
			await PlayTransitionOutAsync(transitionNameTag,false);

			onPlayAction.Invoke();

			await PlayTransitionInAsync(transitionNameTag,true);
		}

		/// <summary>
		/// Plays the fade-in (brighten) transition on the given panel.
		/// </summary>
		public async UniTask PlayTransitionInAsync(CommonUINameTag transitionNameTag,bool isAutoHide = true)
		{
			await _PlayTransitionAsync(transitionNameTag,isAutoHide,true);
		}

		/// <summary>
		/// Plays the fade-out (darken) transition on the given panel.
		/// </summary>
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

			var panel = Open(transitionNameTag) as TransitionPanel;

			if(panel == null)
			{
				throw new NullReferenceException($"{transitionNameTag} is not TransitionPanel.");
			}

			await panel.PlayTransitionAsync(isAutoHide,isReverse);
		}

		private static void _OnForgetException(Exception exception)
		{
			LogChannel.UI.E(exception);
		}
	}
}
