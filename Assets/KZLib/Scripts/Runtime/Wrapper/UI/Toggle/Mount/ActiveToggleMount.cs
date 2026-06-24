using UnityEngine;

/// <summary>Toggle mount that shows or hides a <see cref="GameObject"/>.</summary>
public class ActiveToggleMount : BaseToggleMount
{
	[SerializeField]
	private GameObject m_gameObject = null;

	protected override void Set()
	{
		if(!m_gameObject)
		{
			return;
		}

		m_gameObject.EnsureActive(IsOnNow);
	}
}