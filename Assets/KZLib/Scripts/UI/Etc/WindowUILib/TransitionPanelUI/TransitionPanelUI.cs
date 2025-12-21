using Cysharp.Threading.Tasks;
using KZLib.KZDevelop;
using TransitionPanel;
using UnityEngine;

public class TransitionPanelUI : WindowUI2D
{
	[SerializeField]
	private TransitionProgressTaskSequence m_progressTask = null;

	public async UniTask PlayTransitionAsync(bool isAutoHide,bool isReverse)
	{
		Show();

		await m_progressTask.PlaySequenceAsync(new ProgressTaskSequence.Param(null,isReverse));

		if(isAutoHide)
		{
			Hide();
		}
	}
}