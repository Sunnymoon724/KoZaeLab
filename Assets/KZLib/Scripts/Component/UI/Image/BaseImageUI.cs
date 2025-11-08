using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public abstract class BaseImageUI : BaseComponentUI
{
	[SerializeField]
	protected Image m_image = null;

	protected override void Reset()
	{
		base.Reset();

		if(!m_image)
		{
			m_image = GetComponent<Image>();
		}
	}
}