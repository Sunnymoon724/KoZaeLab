using Sirenix.OdinInspector;
using UnityEngine;

public class SortingLayerRendererGroup : SortingLayerBase
{
	[VerticalGroup("RendererArray",Order = -10),SerializeField,LabelText("Renderer Array")]
	private Renderer[] m_RendererArray = null;

	protected override void Reset()
	{
		base.Reset();

		m_RendererArray ??= GetComponentsInChildren<Renderer>(true);
	}

	public override void SetSortingLayerOrder(int _order)
	{
		base.SetSortingLayerOrder(_order);
		
		for(var i=0;i<m_RendererArray.Length;i++)
		{
			m_RendererArray[i].sortingOrder = SortingLayerOrder+i;
		}
	}
}