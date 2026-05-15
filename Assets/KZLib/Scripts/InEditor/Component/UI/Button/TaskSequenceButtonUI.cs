using Cysharp.Threading.Tasks;
using KZLib.Development;
using R3;
using UnityEngine;

public class TaskSequenceButtonUI : BaseButtonUI
{
	[SerializeField]
	private TaskSequence m_beforeTaskSequence = null;
	[SerializeField]
	private TaskSequence m_afterTaskSequence = null;

	[SerializeField]
	private bool m_useReset = false;

	private readonly Subject<Unit> m_sequenceSubject = new();
	public Observable<Unit> OnClicked => m_sequenceSubject;

	protected override void _OnClickedButton()
	{
		ClickedAsync().Forget();
	}

	private async UniTaskVoid ClickedAsync()
	{
		if(m_beforeTaskSequence)
		{
			await m_beforeTaskSequence.PlaySequenceAsync();
		}

		m_sequenceSubject.OnNext(Unit.Default);

		if(m_afterTaskSequence)
		{
			await m_afterTaskSequence.PlaySequenceAsync();
		}

		if(m_useReset)
		{
			if(m_beforeTaskSequence)
			{
				m_beforeTaskSequence.ResetSequence();
			}

			if(m_afterTaskSequence)
			{
				m_afterTaskSequence.ResetSequence();
			}
		}
	}
}