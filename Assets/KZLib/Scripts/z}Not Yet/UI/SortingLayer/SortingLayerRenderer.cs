using Sirenix.OdinInspector;
using UnityEngine;

public class SortingLayerRenderer : SortingLayerBase
{
	[VerticalGroup("Renderer",Order = -10),SerializeField,LabelText("Renderer")]
	private Renderer m_Renderer = null;

	protected override void Reset()
	{
		base.Reset();

		if(!m_Renderer)
		{
			m_Renderer = GetComponent<Renderer>();
		}
	}

	public override void SetSortingLayerOrder(int _order)
	{
		base.SetSortingLayerOrder(_order);

		m_Renderer.sortingOrder = SortingLayerOrder;
	}
}