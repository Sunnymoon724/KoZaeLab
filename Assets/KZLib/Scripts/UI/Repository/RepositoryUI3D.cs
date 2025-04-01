using UnityEngine;

public class RepositoryUI3D : RepositoryUI
{
	public Camera CanvasCamera => m_canvas.worldCamera;

	public void SetCamera(Camera camera)
	{
		m_canvas.worldCamera = camera;
	}

	protected override bool IsValid(WindowUI window)
	{
		return window != null && window.Is3D && window is WindowUI3D;
	}

	public override void Add(WindowUI window)
	{
		if(!IsValid(window))
		{
			return;
		}

		transform.SetUIChild(window.transform);

		_Add(window);
	}
}