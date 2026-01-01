using Cysharp.Threading.Tasks;
using KZLib.KZDevelop;
using TransitionPanel;
using UnityEngine;

public class TransitionPanelUI : BasePanelUI
{
	[SerializeField]
	private TransitionProgressTaskSequence m_progressTask = null;

	public async UniTask PlayTransitionAsync(bool isAutoHide,bool isReverse)
	{
		Hide(false);

		await m_progressTask.PlaySequenceAsync(new ProgressTaskSequence.Param(null,isReverse));

		if(isAutoHide)
		{
			Hide(true);
		}
	}
}