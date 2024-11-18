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
		private Graphic m_Graphic = null;

		[VerticalGroup("0/2",Order = 10),SerializeField,LabelText("Graphic Mode")]
		private ModeCategory m_Mode = ModeCategory.None;

		[PropertySpace(10)]
		[VerticalGroup("0/2",Order = 10),SerializeField,LabelText("Enable Color"),ShowIf(nameof(IsColor))]
		private Color m_EnableColor = Color.white;
		[VerticalGroup("0/2",Order = 10),SerializeField,LabelText("Disable Color"),ShowIf(nameof(IsColor))]
		private Color m_DisableColor = Color.gray;

		[PropertySpace(10)]
		[VerticalGroup("0/2",Order = 10),SerializeField,LabelText("Enable Material"),ShowIf(nameof(IsMaterial))]
		private Material m_EnableMaterial = null;
		[VerticalGroup("0/2",Order = 10),SerializeField,LabelText("Disable Material"),ShowIf(nameof(IsMaterial))]
		private Material m_DisableMaterial = null;

		private bool IsColor => m_Mode.HasFlag(ModeCategory.Color);
		private bool IsMaterial => m_Mode.HasFlag(ModeCategory.Material);

		protected override void Set()
		{
			if(IsColor)
			{
				m_Graphic.color = IsOnNow ? m_EnableColor : m_DisableColor;
			}

			if(IsMaterial)
			{
				m_Graphic.material = IsOnNow ? m_EnableMaterial : m_DisableMaterial;
			}
		}
		
	}

	[VerticalGroup("1",Order = 1),SerializeField,LabelText("Child List"),ListDrawerSettings(DraggableItems = false)]
	private List<GraphicChild> m_ChildList = new();

	protected override IEnumerable<ToggleChild> ChildGroup => m_ChildList;
}