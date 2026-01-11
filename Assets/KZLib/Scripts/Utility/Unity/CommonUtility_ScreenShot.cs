using UnityEngine;

public static partial class CommonUtility
{
	public static Texture2D GetScreenShot(TextureFormat textureFormat = TextureFormat.ARGB32)
	{
		var width = Screen.width;
		var height = Screen.height;
		var texture = new Texture2D(width,height,textureFormat,false);

		texture.ReadPixels(new Rect(0,0,width,height),0,0);
		texture.Apply();

		return texture;
	}

	public static Texture2D GetCameraScreenShot(Camera camera,TextureFormat textureFormat = TextureFormat.RGB24)
	{
		if(!camera)
		{
			LogChannel.System.E("Camera is null");

			return null;
		}

		var width = Screen.width;
		var height = Screen.height;
		var renderTexture = new RenderTexture(width,height,24);

		camera.targetTexture = renderTexture;
		camera.Render();

		RenderTexture.active = renderTexture;

		var texture = _CreateTexture2D(width,height,textureFormat);

		camera.targetTexture = null;
		RenderTexture.active = null;

		renderTexture.DestroyObject();

		return texture;
	}

	private static Texture2D _CreateTexture2D(int width,int height,TextureFormat textureFormat)
	{
		if(width == 0 || height == 0)
		{
			LogChannel.System.E($"Size is below zero {width} or {height}");

			return null;
		}

		var texture = new Texture2D(width,height,textureFormat,false);

		texture.ReadPixels(new Rect(0.0f,0.0f,width,height),0,0);
		texture.Apply();

		return texture;
	}
}