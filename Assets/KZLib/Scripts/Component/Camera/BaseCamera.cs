using UnityEngine;
using KZLib;

[RequireComponent(typeof(Camera))]
public abstract class BaseCamera : BaseComponent
{
	[SerializeField]
	protected Camera m_camera = null;

	protected override void _Initialize()
	{
		base._Initialize();

		if(CameraManager.HasInstance)
		{
			CameraManager.In.AddSubCamera(m_camera);
		}
	}

	protected override void _Release()
	{
		base._Release();

		if(CameraManager.HasInstance)
		{
			CameraManager.In.RemoveSubCamera(m_camera);
		}
	}

	protected override void Reset()
	{
		base.Reset();

		if(!m_camera)
		{
			m_camera = GetComponent<Camera>();
		}
	}
}