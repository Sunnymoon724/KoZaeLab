using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class GraphicToggleUI : BaseToggleUI
{
	[Serializable]
	protected class GraphicChild : ToggleChild
	{
		[Flags] 
		private enum ModeCategory { None = 0, Color = 1<<0, Material = 1<<1, All = -1 }

		[BoxGroup("0",ShowLabel = false,Order = 0),SerializeField,LabelText("Graphic")]
		private Graphic m_graphic = null;

		[VerticalGroup("0/2",Order = 10),SerializeField,LabelText("Graphic Mode")]
		private ModeCategory m_modeCategory = ModeCategory.None;

		[PropertySpace(10)]
		[VerticalGroup("0/2",Order = 10),SerializeField,LabelText("Enable Color"),ShowIf(nameof(IsColor))]
		private Color m_enableColor = Color.white;
		[VerticalGroup("0/2",Order = 10),SerializeField,LabelText("Disable Color"),ShowIf(nameof(IsColor))]
		private Color m_disableColor = Color.gray;

		[PropertySpace(10)]
		[VerticalGroup("0/2",Order = 10),SerializeField,LabelText("Enable Material"),ShowIf(nameof(IsMaterial))]
		private Material m_enableMaterial = null;
		[VerticalGroup("0/2",Order = 10),SerializeField,LabelText("Disable Material"),ShowIf(nameof(IsMaterial))]
		private Material m_disableMaterial = null;

		private bool IsColor => m_modeCategory.HasFlag(ModeCategory.Color);
		private bool IsMaterial => m_modeCategory.HasFlag(ModeCategory.Material);

		protected override void Set()
		{
			if(IsColor)
			{
				m_graphic.color = IsOnNow ? m_enableColor : m_disableColor;
			}

			if(IsMaterial)
			{
				m_graphic.material = IsOnNow ? m_enableMaterial : m_disableMaterial;
			}
		}
		
	}

	[VerticalGroup("1",Order = 1),SerializeField,LabelText("Child List"),ListDrawerSettings(DraggableItems = false)]
	private List<GraphicChild> m_childList = new();

	protected override IEnumerable<ToggleChild> ToggleChildGroup => m_childList;
}