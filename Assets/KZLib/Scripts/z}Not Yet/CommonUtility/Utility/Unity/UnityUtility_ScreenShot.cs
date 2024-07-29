using UnityEngine;

public static partial class UnityUtility
{
	public static Texture2D GetScreenShot(TextureFormat _format = TextureFormat.ARGB32)
	{
		var width = Screen.width;
		var height = Screen.height;
		var texture = new Texture2D(width,height,_format,false);

		texture.ReadPixels(new Rect(0,0,width,height),0,0);
		texture.Apply();

		return texture;
	}

	public static Texture2D GetCameraScreenShot(Camera _camera,TextureFormat _format = TextureFormat.RGB24)
	{
		var width = Screen.width;
		var height = Screen.height;
		var renderTexture = new RenderTexture(width,height,24);

		_camera.targetTexture = renderTexture;
		_camera.Render();

		RenderTexture.active = renderTexture;

		var texture = CreateTexture2D(width,height,_format);

		_camera.targetTexture = null;
		RenderTexture.active = null;

		DestroyObject(renderTexture);

		return texture;
	}

	private static Texture2D CreateTexture2D(int _width,int _height,TextureFormat _format)
	{
		var texture = new Texture2D(_width,_height,_format,false);

		texture.ReadPixels(new Rect(0.0f,0.0f,_width,_height),0,0);
		texture.Apply();

		return texture;
	}
}