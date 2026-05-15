using UnityEngine;
using KZLib;

[RequireComponent(typeof(Camera))]
public abstract class BaseCamera : MonoBehaviour
{
	[SerializeField]
	protected Camera m_camera = null;

	private void Awake()
	{
		if(CameraManager.HasInstance)
		{
			CameraManager.In.AddSubCamera(m_camera);
		}
	}

	private void OnDestroy()
	{
		if(CameraManager.HasInstance)
		{
			CameraManager.In.RemoveSubCamera(m_camera);
		}
	}

	private void Reset()
	{
		if(!m_camera)
		{
			m_camera = GetComponent<Camera>();
		}
	}
}