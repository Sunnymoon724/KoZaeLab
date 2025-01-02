using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas),typeof(GraphicRaycaster))]
public abstract class SortingLayerCanvas : SortingLayerBase
{
	[InfoBox("canvas is null",InfoMessageType.Error,"@this.m_canvas == null")]
	[VerticalGroup("Canvas",Order = -25),SerializeField]
	protected Canvas m_canvas = null;

	protected override void Reset()
	{
		base.Reset();

		if(!m_canvas)
		{
			m_canvas = gameObject.GetComponent<Canvas>();
		}

		m_canvas.overrideSorting = true;
		m_canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1;
		m_canvas.sortingOrder = SortingLayerOrder;

		if(gameObject.TryGetComponent<GraphicRaycaster>(out var raycaster))
		{
			raycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;
		}
	}

	public override void SetSortingLayerOrder(int sortingLayerOrder)
	{
		base.SetSortingLayerOrder(sortingLayerOrder);

		if(m_canvas)
		{
			m_canvas.sortingOrder = SortingLayerOrder;
		}
	}
}