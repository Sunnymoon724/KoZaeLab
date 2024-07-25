using System;
using Cysharp.Threading.Tasks;

namespace KZLib
{
	public partial class UIMgr : LoadSingletonMB<UIMgr>
	{
		private TransitionPanelUI m_TransitionPanel = null;

		private TransitionPanelUI TransitionPanel
		{
			get
			{
				if(!m_TransitionPanel)
				{
					m_TransitionPanel = Open<TransitionPanelUI>(UITag.TransitionPanelUI);
				}

				return m_TransitionPanel;
			}
		}

		/// <summary>
		/// 패널을 알파값을 내려서 트렌지션 한다.
		/// </summary>
		public async UniTask PlayTransitionInAsync(TransitionData _data,bool _autoHide = true)
		{
			if(_data == null)
			{
				return;
			}

			await TransitionPanel.PlayTransitionInAsync(_data,_autoHide);
		}

		/// <summary>
		/// 패널을 알파값을 올려서 트렌지션 한다.
		/// </summary>
		public async UniTask PlayTransitionOutAsync(TransitionData _data,bool _autoHide = true)
		{
			if(_data == null)
			{
				return;
			}

			await TransitionPanel.PlayTransitionOutAsync(_data,_autoHide);
		}

		public void PlayTransitionOutIn(TransitionData _data,Func<UniTask> _onPlayTask)
		{
			PlayTransitionOutInAsync(_data,_onPlayTask).Forget();
		}

		public void PlayTransitionOutIn(TransitionData _data,Action _onPlay)
		{
			PlayTransitionOutInAsync(_data,_onPlay).Forget();
		}

		public async UniTask PlayTransitionOutInAsync(TransitionData _data,Func<UniTask> _onPlayTask)
		{
			// 점점 어두워짐
			await PlayTransitionOutAsync(_data,false);

			await _onPlayTask.Invoke();

			// 점점 밝아짐
			await PlayTransitionInAsync(_data,true);
		}

		public async UniTask PlayTransitionOutInAsync(TransitionData _data,Action _onPlay)
		{
			// 점점 어두워짐
			await PlayTransitionOutAsync(_data,false);

			_onPlay.Invoke();

			// 점점 밝아짐
			await PlayTransitionInAsync(_data,true);
		}
	}
}