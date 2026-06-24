using UnityEngine;
using KZLib;

/// <summary>
/// Wrapper base for URP <b>overlay sub-cameras</b> (UI, post-stack layers, etc.).
/// Registers <see cref="m_camera"/> with <see cref="CameraManager"/> on init and unregisters on destroy.
/// </summary>
/// <remarks>
/// <b>Do not use for the scene main / world camera.</b>
/// Main cameras are bound via <see cref="CameraManager.AttachCamera"/>, not <see cref="CameraManager.AddSubCamera"/>.
/// Putting this on the same <see cref="Camera"/> that later becomes main can corrupt URP stack / render type.
/// For follow or orbit behaviour use <see cref="TrackingCamera"/> / <see cref="ObserveCamera"/> (<see cref="BaseComponent"/> only).
/// </remarks>
[RequireComponent(typeof(Camera))]
public abstract class BaseCamera : BaseComponent
{
	[SerializeField]
	protected Camera m_camera = null;

	/// <summary>
	/// Registers as a sub-camera. Safe when main is not attached yet:
	/// <see cref="CameraManager.AddSubCamera"/> stores the entry and applies it on the next <see cref="CameraManager.AttachCamera"/>.
	/// </summary>
	protected override void _Initialize()
	{
		base._Initialize();

		if(!m_camera)
		{
			m_camera = GetComponent<Camera>();
		}

		// Reset() fills m_camera in the editor; runtime prefabs should assign it in the inspector.
		if(!m_camera)
		{
			return;
		}

		CameraManager.In.AddSubCamera(m_camera);
	}

	/// <summary>
	/// Unregisters and restores depth / clear flags / URP render type via <see cref="CameraManager"/>.
	/// <see cref="CameraManager.HasInstance"/> avoids touching the singleton during domain reload / teardown.
	/// </summary>
	protected override void _Release()
	{
		if(m_camera && CameraManager.HasInstance)
		{
			CameraManager.In.RemoveSubCamera(m_camera);
		}

		base._Release();
	}

	/// <summary>Auto-assigns <see cref="m_camera"/> when the component is added or Reset is invoked in the editor.</summary>
	protected override void Reset()
	{
		base.Reset();

		if(!m_camera)
		{
			m_camera = GetComponent<Camera>();
		}
	}
}
