using UnityEngine;

/// <summary>
/// Extension methods for <see cref="Canvas"/> camera resolution.
/// </summary>
public static class CanvasExtension
{
	/// <summary>
	/// Returns <see cref="Canvas.worldCamera"/> when assigned, otherwise falls back to <see cref="Camera.main"/>.
	/// </summary>
	public static Camera GetEventCamera(this Canvas canvas)
	{
		if(!_IsValid(canvas))
		{
			return null;
		}

		if(canvas.worldCamera)
		{
			return canvas.worldCamera;
		}

		if(!Camera.main)
		{
			LogChannel.Kit.W("Camera.main is null. No camera tagged 'MainCamera' found.");

			return null;
		}

		return Camera.main;
	}

	private static bool _IsValid(Canvas canvas)
	{
		if(!canvas)
		{
			LogChannel.Kit.E("Canvas is null");

			return false;
		}

		return true;
	}
}
