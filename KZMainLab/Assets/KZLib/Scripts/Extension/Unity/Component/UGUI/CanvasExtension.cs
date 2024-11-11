using UnityEngine;

public static class CanvasExtension
{
	public static Camera GetEventCamera(this Canvas _canvas)
	{
		if(!_canvas)
		{
			LogTag.System.E("Canvas is null.");

			return null;
		}

		return !_canvas.worldCamera ? Camera.main : _canvas.worldCamera;
	}
}