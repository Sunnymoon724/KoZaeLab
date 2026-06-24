using Cysharp.Threading.Tasks;
using KZLib.Utilities;
using R3;
using UnityEngine;

/// <summary>
/// Button that plays optional before/after <see cref="Stanza"/> sequences, then emits <see cref="OnClicked"/>.
/// </summary>
public class StanzaButtonUI : BaseButtonUI
{
	[SerializeField]
	private Stanza m_beforeStanza = null;
	[SerializeField]
	private Stanza m_afterStanza = null;

	[SerializeField]
	private bool m_useReset = false;

	private readonly Subject<Unit> m_sequenceSubject = new();
	public Observable<Unit> OnClicked => m_sequenceSubject;

	private bool m_isPlaying = false;

	protected override void _Release()
	{
		m_sequenceSubject.Dispose();

		base._Release();
	}

	protected override void _OnClickedButton()
	{
		ClickedAsync().Forget();
	}

	private async UniTaskVoid ClickedAsync()
	{
		if(m_isPlaying)
		{
			return;
		}

		m_isPlaying = true;

		try
		{
			if(m_beforeStanza)
			{
				await m_beforeStanza.PlayAsync();
			}

			m_sequenceSubject.OnNext(Unit.Default);

			if(m_afterStanza)
			{
				await m_afterStanza.PlayAsync();
			}

			if(m_useReset)
			{
				if(m_beforeStanza)
				{
					m_beforeStanza.ResetAll();
				}

				if(m_afterStanza)
				{
					m_afterStanza.ResetAll();
				}
			}
		}
		finally
		{
			m_isPlaying = false;
		}
	}
}
