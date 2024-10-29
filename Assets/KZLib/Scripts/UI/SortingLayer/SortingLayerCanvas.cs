using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas),typeof(GraphicRaycaster))]
public abstract class SortingLayerCanvas : SortingLayerBase
{
	[InfoBox("canvas is null",InfoMessageType.Error,"@this.m_Canvas == null")]
	[VerticalGroup("Canvas",Order = -25),SerializeField]
	protected Canvas m_Canvas = null;

	protected override void Reset()
	{
		base.Reset();

		if(!m_Canvas)
		{
			m_Canvas = gameObject.GetComponent<Canvas>();
		}

		m_Canvas.overrideSorting = true;
		m_Canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1;
		m_Canvas.sortingOrder = SortingLayerOrder;

		if(gameObject.TryGetComponent<GraphicRaycaster>(out var raycaster))
		{
			raycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;
		}
	}

	public override void SetSortingLayerOrder(int _order)
	{
		base.SetSortingLayerOrder(_order);

		if(m_Canvas)
		{
			m_Canvas.sortingOrder = SortingLayerOrder;
		}
	}
}