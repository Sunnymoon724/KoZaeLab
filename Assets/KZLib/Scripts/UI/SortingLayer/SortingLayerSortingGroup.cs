using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(SortingGroup))]
public class SortingLayerSortingGroup : SortingLayerBase
{
	[VerticalGroup("소팅 그룹",Order = -10),SerializeField,LabelText("소팅 그룹")]
	private SortingGroup m_SortingGroup = null;

	protected override void Reset()
	{
		base.Reset();

		if(!m_SortingGroup)
		{
			m_SortingGroup = GetComponent<SortingGroup>();
		}
	}

	public override void SetSortingLayerOrder(int _order)
	{
		base.SetSortingLayerOrder(_order);

		if(m_SortingGroup)
		{
			m_SortingGroup.sortingOrder = SortingLayerOrder;
		}
	}
}