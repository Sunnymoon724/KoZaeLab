using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class BaseScrollRect : MonoBehaviour
{
	[SerializeField]
	protected ScrollRect m_scrollRect = null;

	private void Reset()
	{
		if(!m_scrollRect)
		{
			m_scrollRect = GetComponent<ScrollRect>();
		}
	}
}