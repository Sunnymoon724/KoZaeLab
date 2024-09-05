using Sirenix.OdinInspector;
using UnityEngine;

public class SortingLayerRendererGroup : SortingLayerBase
{
	[VerticalGroup("렌더러 그룹",Order = -10),SerializeField,LabelText("렌더러 그룹")]
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