using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas),typeof(GraphicRaycaster))]
public abstract class SortingLayerCanvas : SortingLayerBase
{
	[InfoBox("Canvas is null",InfoMessageType.Error,"@this.m_Canvas == null")]
	[VerticalGroup("캔버스",Order = -25),SerializeField]
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

		var ray = gameObject.GetComponent<GraphicRaycaster>();

		ray.blockingObjects = GraphicRaycaster.BlockingObjects.None;
	}

	public override void SetSortingLayerOrder(int _sortingLayerOrder)
	{
		base.SetSortingLayerOrder(_sortingLayerOrder);

		m_Canvas.sortingOrder = SortingLayerOrder;
	}
}