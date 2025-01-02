using Sirenix.OdinInspector;
using UnityEngine;

public class SortingLayerRenderer : SortingLayerBase
{
	[VerticalGroup("Renderer",Order = -10),SerializeField,LabelText("Renderer")]
	private Renderer m_renderer = null;

	protected override void Reset()
	{
		base.Reset();

		if(!m_renderer)
		{
			m_renderer = GetComponent<Renderer>();
		}
	}

	public override void SetSortingLayerOrder(int sortingLayerOrder)
	{
		base.SetSortingLayerOrder(sortingLayerOrder);

		if(m_renderer)
		{
			m_renderer.sortingOrder = SortingLayerOrder;
		}
	}
}