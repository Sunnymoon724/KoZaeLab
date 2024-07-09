using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public abstract class BaseImageUI : BaseComponentUI
{
	[SerializeField,LabelText("이미지")]
	protected Image m_Image = null;

	protected override void Reset()
	{
		base.Reset();

		if(!m_Image)
		{
			m_Image = GetComponent<Image>();
		}
	}
}