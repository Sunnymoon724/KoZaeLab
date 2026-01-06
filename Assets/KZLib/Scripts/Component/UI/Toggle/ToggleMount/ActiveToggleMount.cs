using UnityEngine;

public class ActiveToggleMount : BaseToggleMount
{
	[SerializeField]
	private GameObject m_gameObject = null;

	protected override void Set()
	{
		m_gameObject.EnsureActive(IsOnNow);
	}
}