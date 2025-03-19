using Cysharp.Threading.Tasks;
using TransitionPanel;
using UnityEngine;

public class TransitionPanelUI : WindowUI2D
{
	[SerializeField]
	private TransitionProgressTask m_progressTask = null;

	public override UITag Tag => UITag.TransitionPanelUI;

	public async UniTask PlayTransitionAsync(TransitionInfo info,bool isAutoHide,bool isReverse)
	{
		Show();

		await m_progressTask.PlayProgressAsync(new TransitionProgressTask.TransitionProgressParam(info,isReverse));

		if(isAutoHide)
		{
			Hide();
		}
	}
}