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
		private GameObject m_GameObject = null;

		protected override void Set()
		{
			m_GameObject.SetActiveSelf(IsOnNow);
		}
	}

	[VerticalGroup("1",Order = 1),SerializeField,LabelText("Child List"),ListDrawerSettings(DraggableItems = false)]
	private List<ActiveChild> m_ChildList = new();

	protected override IEnumerable<ToggleChild> ChildGroup => m_ChildList;
}