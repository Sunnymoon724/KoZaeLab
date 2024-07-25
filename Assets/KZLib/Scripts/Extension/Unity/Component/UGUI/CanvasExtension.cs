using UnityEngine;

public static class CanvasExtension
{
	public static Camera GetEventCamera(this Canvas _canvas)
	{
		return !_canvas.worldCamera ? Camera.main : _canvas.worldCamera;
	}
}