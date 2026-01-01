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
		private enum GraphicType { None = 0, Color = 1<<0, Material = 1<<1, All = -1 }

		[HorizontalGroup("0",Order = 0),SerializeField]
		private Graphic m_graphic = null;

		[HorizontalGroup("5",Order = 5),SerializeField]
		private GraphicType m_graphicType = GraphicType.None;

		[HorizontalGroup("6",Order = 6),SerializeField,ShowIf(nameof(IsColor))]
		private Color m_enableColor = Color.white;
		[HorizontalGroup("7",Order = 7),SerializeField,ShowIf(nameof(IsColor))]
		private Color m_disableColor = Color.gray;

		[PropertySpace(5)]
		[HorizontalGroup("8",Order = 8),SerializeField,ShowIf(nameof(IsMaterial))]
		private Material m_enableMaterial = null;
		[HorizontalGroup("9",Order = 9),SerializeField,ShowIf(nameof(IsMaterial))]
		private Material m_disableMaterial = null;

		private bool IsColor => m_graphicType.HasFlag(GraphicType.Color);
		private bool IsMaterial => m_graphicType.HasFlag(GraphicType.Material);

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

	[VerticalGroup("1",Order = 1),SerializeField,ListDrawerSettings(DraggableItems = false)]
	private List<GraphicChild> m_childList = new();

	protected override IEnumerable<ToggleChild> ToggleChildGroup => m_childList;
}