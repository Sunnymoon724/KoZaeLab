// using Sirenix.OdinInspector;
// using UnityEngine;

// public class SortingLayerRendererGroup : SortingLayerBase
// {
// 	[VerticalGroup("Renderer Group",Order = -10),SerializeField]
// 	private Renderer[] m_rendererArray = null;

// 	protected override void Reset()
// 	{
// 		base.Reset();

// 		m_rendererArray ??= GetComponentsInChildren<Renderer>(true);
// 	}

// 	public override void SetSortingLayerOrder(int sortingLayerOrder)
// 	{
// 		base.SetSortingLayerOrder(sortingLayerOrder);
		
// 		for(var i=0;i<m_rendererArray.Length;i++)
// 		{
// 			m_rendererArray[i].sortingOrder = SortingLayerOrder+i;
// 		}
// 	}
// }