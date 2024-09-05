using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas),typeof(GraphicRaycaster))]
public abstract class SortingLayerCanvas : SortingLayerBase
{
	[InfoBox("캔버스가 없습니다.",InfoMessageType.Error,"@this.m_Canvas == null")]
	[VerticalGroup("캔버스",Order = -25),SerializeField,LabelText("캔버스")]
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