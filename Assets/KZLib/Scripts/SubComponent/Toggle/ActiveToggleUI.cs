using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;

public class ActiveToggleUI : BaseToggleUI
{
	[Serializable]
	protected class ActiveChild : ToggleChild
	{
		[BoxGroup("0",ShowLabel = false,Order = 0),SerializeField,LabelText("Object")]
		private GameObject m_gameObject = null;

		protected override void Set()
		{
			m_gameObject.SetActiveIfDifferent(IsOnNow);
		}
	}

	[VerticalGroup("1",Order = 1),SerializeField,LabelText("Child List"),ListDrawerSettings(DraggableItems = false)]
	private List<ActiveChild> m_childList = new();

	protected override IEnumerable<ToggleChild> ToggleChildGroup => m_childList;
}