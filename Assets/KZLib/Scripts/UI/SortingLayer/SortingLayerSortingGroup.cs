using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(SortingGroup))]
public class SortingLayerSortingGroup : SortingLayerBase
{
	[VerticalGroup("Sorting Group",Order = -10),SerializeField]
	private SortingGroup m_sortingGroup = null;

	protected override void Reset()
	{
		base.Reset();

		if(!m_sortingGroup)
		{
			m_sortingGroup = GetComponent<SortingGroup>();
		}
	}

	public override void SetSortingLayerOrder(int sortingLayerOrder)
	{
		base.SetSortingLayerOrder(sortingLayerOrder);

		if(m_sortingGroup)
		{
			m_sortingGroup.sortingOrder = SortingLayerOrder;
		}
	}
}