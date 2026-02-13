using Cysharp.Threading.Tasks;
using KZLib.Development;
using UnityEngine;

public class TransitionPanel : BasePanel
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