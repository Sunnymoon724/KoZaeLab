using System;
using Cysharp.Threading.Tasks;
using KZLib.KZUtility;

namespace KZLib
{
	public partial class UIMgr : LoadSingletonMB<UIMgr>
	{
		public void PlayTransitionOutIn(TransitionData _data,Func<UniTask> _onPlayTask)
		{
			PlayTransitionOutInAsync(_data,_onPlayTask).Forget();
		}

		public void PlayTransitionOutIn(TransitionData _data,Action _onPlay)
		{
			PlayTransitionOutInAsync(_data,_onPlay).Forget();
		}

		private async UniTask PlayTransitionOutInAsync(TransitionData _data,Func<UniTask> _onPlayTask)
		{
			// darker
			await PlayTransitionOutAsync(_data,false);

			await _onPlayTask.Invoke();

			// brighter
			await PlayTransitionInAsync(_data,true);
		}

		private async UniTask PlayTransitionOutInAsync(TransitionData _data,Action _onPlay)
		{
			// darker
			await PlayTransitionOutAsync(_data,false);

			_onPlay.Invoke();

			// brighter
			await PlayTransitionInAsync(_data,true);
		}

		public async UniTask PlayTransitionInAsync(TransitionData _data,bool _autoHide = true)
		{
			await PlayTransitionAsync(_data,_autoHide,true);
		}

		public async UniTask PlayTransitionOutAsync(TransitionData _data,bool _autoHide = true)
		{
			await PlayTransitionAsync(_data,_autoHide,false);
		}

		private async UniTask PlayTransitionAsync(TransitionData _data,bool _autoHide,bool _reverse)
		{
			if(_data == null)
			{
				return;
			}

			var transitionPanel = Open<TransitionPanelUI>(UITag.TransitionPanelUI);

			await transitionPanel.PlayTransitionAsync(_data,_autoHide,_reverse);
		}
	}
}