using UnityEngine;

public class RepositoryUI3D : RepositoryUI
{
	public Camera CanvasCamera => m_canvas.worldCamera;

	public void SetCamera(Camera camera)
	{
		m_canvas.worldCamera = camera;
	}

	protected override bool IsValid(Window window)
	{
		return window != null && window.Is3D && window is Window3D;
	}

	public override void Add(Window window)
	{
		if(!IsValid(window))
		{
			return;
		}

		transform.SetChild(window.transform,false);

		_Add(window);
	}
}