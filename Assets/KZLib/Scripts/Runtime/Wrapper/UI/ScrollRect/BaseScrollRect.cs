using UnityEngine;
using UnityEngine.UI;

/// <summary>Wrapper base for Unity <see cref="ScrollRect"/>.</summary>
[RequireComponent(typeof(ScrollRect))]
public class BaseScrollRect : BaseComponent
{
	[SerializeField]
	protected ScrollRect m_scrollRect = null;

	protected override void Reset()
	{
		base.Reset();

		if(!m_scrollRect)
		{
			m_scrollRect = GetComponent<ScrollRect>();
		}
	}
}
