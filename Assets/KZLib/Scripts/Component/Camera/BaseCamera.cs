using UnityEngine;
using KZLib;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Camera))]
public abstract class BaseCamera : BaseComponent
{
	[SerializeField,LabelText("메인 카메라")]
	protected Camera m_Camera = null;

	protected override void Reset()
	{
		base.Reset();

		if(!m_Camera)
		{
			m_Camera = GetComponent<Camera>();
		}
	}
}