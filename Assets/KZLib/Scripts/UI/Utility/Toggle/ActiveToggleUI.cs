using UnityEngine;
using Sirenix.OdinInspector;

public class ActiveToggleUI : ToggleUI
{
	[VerticalGroup("2",Order = 2),SerializeField,LabelText("오브젝트들")]
	private GameObject m_ActiveObject = null;

	protected override void PlayToggle()
	{
		m_ActiveObject.SetActiveSelf(IsOnNow);
	}

	protected override void Reset()
	{
		base.Reset();

		if(!m_ActiveObject)
		{
			m_ActiveObject = gameObject;
		}
	}
}