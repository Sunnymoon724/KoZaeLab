using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(Graphic))]
public class GraphicToggleMount : BaseToggleMount
{
	[Flags]
	private enum GraphicType { None = 0, Color = 1<<0, Material = 1<<1, All = -1 }

	[SerializeField]
	private Graphic m_graphic = null;

	[SerializeField]
	private GraphicType m_graphicType = GraphicType.None;

	[SerializeField,ShowIf(nameof(IsColor))]
	private Color m_enableColor = Color.white;
	[SerializeField,ShowIf(nameof(IsColor))]
	private Color m_disableColor = Color.gray;

	[PropertySpace(5)]

	[SerializeField,ShowIf(nameof(IsMaterial))]
	private Material m_enableMaterial = null;
	[SerializeField,ShowIf(nameof(IsMaterial))]
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