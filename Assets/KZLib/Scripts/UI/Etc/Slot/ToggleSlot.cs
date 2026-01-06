using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace UnityEngine.UI
{
	public class ToggleSlot : Slot
	{
		[FoldoutGroup("Toggle",Order = 3),SerializeField,ListDrawerSettings(DraggableItems = false)]
		private List<BaseToggleUI> m_childList = new();

		public IEnumerable<BaseToggleUI> ChildGroup => m_childList;

		public virtual void SetToggle(bool isOn)
		{
			foreach(var child in ChildGroup)
			{
				child.Set(isOn,true);
			}
		}

		protected override void _SetButton(Action<IEntryInfo> onClicked)
		{
			void _Clicked(IEntryInfo info)
			{
				foreach(var child in ChildGroup)
				{
					child.Toggle();
				}
			}

			onClicked += _Clicked;

			base._SetButton(onClicked);
		}
	}
}