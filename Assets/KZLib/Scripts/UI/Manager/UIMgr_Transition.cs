using System;
using Cysharp.Threading.Tasks;

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
			// 점점 어두워짐
			await PlayTransitionOutAsync(_data,false);

			await _onPlayTask.Invoke();

			// 점점 밝아짐
			await PlayTransitionInAsync(_data,true);
		}

		private async UniTask PlayTransitionOutInAsync(TransitionData _data,Action _onPlay)
		{
			// 점점 어두워짐
			await PlayTransitionOutAsync(_data,false);

			_onPlay.Invoke();

			// 점점 밝아짐
			await PlayTransitionInAsync(_data,true);
		}

		/// <summary>
		/// 패널을 알파값을 내려서 트렌지션인 한다.
		/// </summary>
		public async UniTask PlayTransitionInAsync(TransitionData _data,bool _autoHide = true)
		{
			await PlayTransitionAsync(_data,_autoHide,true);
		}

		/// <summary>
		/// 패널을 알파값을 올려서 트렌지션아웃 한다.
		/// </summary>
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