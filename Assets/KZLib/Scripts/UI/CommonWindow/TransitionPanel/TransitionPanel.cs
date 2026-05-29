using Cysharp.Threading.Tasks;
using KZLib.Utilities;
using UnityEngine;

public class TransitionPanel : BasePanel
{
	[SerializeField]
	private TransitionStanzaLerp m_progressTask = null;

	public async UniTask PlayTransitionAsync(bool isAutoHide,bool isReverse)
	{
		Hide(false);

		await m_progressTask.PlayAsync(new StanzaLerp.Param(null,isReverse));

		if(isAutoHide)
		{
			Hide(true);
		}
	}
}