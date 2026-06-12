using UnityEngine;

/// <summary>
/// Utility methods for capturing screen and camera render output as Texture2D.
/// </summary>
public static class KZTextureKit
{
	/// <summary>
	/// Captures the current screen contents into a new Texture2D.
	/// </summary>
	public static Texture2D GetScreenShot(TextureFormat textureFormat = TextureFormat.ARGB32)
	{
		var width = Screen.width;
		var height = Screen.height;
		var texture = new Texture2D(width,height,textureFormat,false);

		texture.ReadPixels(new Rect(0,0,width,height),0,0);
		texture.Apply();

		return texture;
	}

	/// <summary>
	/// Renders the given camera to a temporary RenderTexture and returns the result as a Texture2D.
	/// </summary>
	public static Texture2D GetCameraScreenShot(Camera camera,TextureFormat textureFormat = TextureFormat.RGB24)
	{
		if(!camera)
		{
			LogChannel.Kit.E("Camera is null");

			return null;
		}

		var width = Screen.width;
		var height = Screen.height;
		var renderTexture = new RenderTexture(width,height,24);

		Texture2D texture = null;

		try
		{
			camera.targetTexture = renderTexture;
			camera.Render();

			RenderTexture.active = renderTexture;

			texture = _CreateTexture2D(width,height,textureFormat);
		}
		finally
		{
			camera.targetTexture = null;
			RenderTexture.active = null;

			renderTexture.DestroyObject();
		}

		return texture;
	}

	/// <summary>
	/// Reads pixels from the active RenderTexture into a new Texture2D.
	/// </summary>
	private static Texture2D _CreateTexture2D(int width,int height,TextureFormat textureFormat)
	{
		if(width == 0 || height == 0)
		{
			LogChannel.Kit.E($"Size is zero: width={width}, height={height}");

			return null;
		}

		var texture = new Texture2D(width,height,textureFormat,false);

		texture.ReadPixels(new Rect(0.0f,0.0f,width,height),0,0);
		texture.Apply();

		return texture;
	}
}
