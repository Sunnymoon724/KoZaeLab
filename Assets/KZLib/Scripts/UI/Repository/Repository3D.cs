using System;
using UnityEngine;
using KZLib.UI;

/// <summary>
/// World-space UI repository bound to a scene camera via <see cref="Canvas.worldCamera"/>.
/// </summary>
public class Repository3D : Repository
{
	public Camera CanvasCamera => m_canvas.worldCamera;

	/// <summary>
	/// Assigns the camera used to render this world-space canvas.
	/// </summary>
	public void SetCamera(Camera camera)
	{
		if(!camera)
		{
			throw new InvalidOperationException($"{nameof(camera)} is null.");
		}

		m_canvas.worldCamera = camera;
	}

	protected override bool IsValid(Window window)
	{
		return window != null && window.Is3D && window is Window3D;
	}

	/// <summary>
	/// Parents the window under this repository and registers it as open.
	/// </summary>
	public override void Add(Window window)
	{
		if(!IsValid(window))
		{
			throw new InvalidOperationException($"{window?.NameTag} is not a valid 3D window for {nameof(Repository3D)}.");
		}

		transform.SetChild(window.transform,false);

		_Add(window);
	}
}
