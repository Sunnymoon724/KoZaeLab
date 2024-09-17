using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;

public class ActiveToggleUI : BaseToggleUI
{
	[Serializable]
	protected class ActiveChild : ToggleChild
	{
		[BoxGroup("0",ShowLabel = false,Order = 0),SerializeField,LabelText("오브젝트")]
		private GameObject m_Child = null;

		protected override void Set()
		{
			m_Child.SetActiveSelf(IsOnNow);
		}
	}

	[VerticalGroup("1",Order = 1),SerializeField,LabelText("자식 리스트"),ListDrawerSettings(DraggableItems = false)]
	private List<ActiveChild> m_ChildList = new();

	protected override IEnumerable<ToggleChild> ChildGroup => m_ChildList;
}