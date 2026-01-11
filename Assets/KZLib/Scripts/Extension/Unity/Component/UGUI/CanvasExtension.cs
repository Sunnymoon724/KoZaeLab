using UnityEngine;

public static class CanvasExtension
{
	public static Camera GetEventCamera(this Canvas canvas)
	{
		if(!_IsValid(canvas))
		{
			return null;
		}

		return !canvas.worldCamera ? Camera.main : canvas.worldCamera;
	}

	private static bool _IsValid(Canvas canvas)
	{
		if(!canvas)
		{
			LogChannel.System.E("Canvas is null");

			return false;
		}

		return true;
	}
}