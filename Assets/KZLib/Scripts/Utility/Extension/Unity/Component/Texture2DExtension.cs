using UnityEngine;

/// <summary>
/// Extension methods for <see cref="Texture2D"/> manipulation, sprite creation, and splitting.
/// </summary>
public static class Texture2DExtension
{
	/// <summary>
	/// Fills all non-transparent pixels with the given color, leaving fully transparent pixels clear.
	/// </summary>
	public static void ToSolidColor(this Texture2D texture2D,Color color)
	{
		if(!_IsValid(texture2D))
		{
			return;
		}

		texture2D.ToSolidColor((Color32) color);
	}

	public static void ToSolidColor(this Texture2D texture2D,Color32 color32)
	{
		if(!_IsValid(texture2D))
		{
			return;
		}

		var pixelArray = texture2D.GetPixels32();

		for(var i=0;i<pixelArray.Length;i++)
		{
			pixelArray[i] = pixelArray[i].a == 0 ? Color.clear : color32;
		}

		texture2D.SetPixels32(pixelArray);
		texture2D.Apply();
	}

	/// <summary>
	/// Resizes the texture in place using a temporary render texture blit.
	/// </summary>
	public static void ScaleTexture(this Texture2D texture2D,Vector2Int scaleSize)
	{
		if(!_IsValid(texture2D))
		{
			return;
		}

		var renderTexture = RenderTexture.GetTemporary(scaleSize.x,scaleSize.y,0,RenderTextureFormat.ARGB32);

		Graphics.Blit(texture2D,renderTexture);

		var previousRT = RenderTexture.active;

		RenderTexture.active = renderTexture;

		texture2D.Reinitialize(scaleSize.x,scaleSize.y);
		texture2D.ReadPixels(new Rect(0,0,scaleSize.x,scaleSize.y),0,0);
		texture2D.Apply();

		RenderTexture.active = previousRT;
		RenderTexture.ReleaseTemporary(renderTexture);
	}

	public static Texture2D CopyTexture(this Texture2D texture2D)
	{
		if(!_IsValid(texture2D))
		{
			return null;
		}

		var texture = new Texture2D(texture2D.width,texture2D.height,texture2D.format,false);

		texture.SetPixels32(texture2D.GetPixels32());
		texture.Apply();

		return texture;
	}

	public static Sprite CreateSprite(this Texture2D texture2D)
	{
		if(!_IsValid(texture2D))
		{
			return null;
		}

		return Sprite.Create(texture2D,new Rect(0.0f,0.0f,texture2D.width,texture2D.height),new Vector2(0.5f,0.5f));
	}

	/// <summary>
	/// Creates a tiled sprite with repeat wrap mode and optional nine-slice borders.
	/// </summary>
	public static Sprite CreateTiledSprite(this Texture2D texture2D,float border = 0.0f,float pixelsPerUnit = Global.DefaultPixelsPerUnit)
	{
		if(!_IsValid(texture2D))
		{
			return null;
		}

		texture2D.wrapMode = TextureWrapMode.Repeat;

		return Sprite.Create(texture2D,new Rect(0.0f,0.0f,texture2D.width,texture2D.height),Vector2.zero,pixelsPerUnit,0,SpriteMeshType.Tight,new Vector4(border,border,border,border));
	}

	/// <summary>
	/// Divides the texture into a grid of smaller textures of equal size.
	/// </summary>
	public static Texture2D[,] SplitTexture(this Texture2D texture2D,Vector2Int splitCount)
	{
		if(!_IsValid(texture2D))
		{
			return null;
		}

		if(splitCount.x <= 0 || splitCount.y <= 0 )
		{
			LogChannel.Kit.E($"Count is below zero {splitCount.x} or {splitCount.y}");

			return null;
		}

		var width = texture2D.width/splitCount.x;
        var height = texture2D.height/splitCount.y;
		var totalColorArray = texture2D.GetPixels32();

        var resultArray = new Texture2D[splitCount.x,splitCount.y];

        for(var i=0;i<splitCount.x;i++)
        {
            for(var j=0;j<splitCount.y;j++)
            {
				resultArray[i,j] = new Texture2D(width,height);

                var pixelArray = new Color32[width*height];
                var index = 0;

                for(var k=i*width;k<(i+1)*width;k++)
                {
                    for(var l=j*height;l<(j+1)*height;l++)
                    {
                        pixelArray[index] = totalColorArray[l*texture2D.width+k];
                        index++;
                    }
                }

                resultArray[i,j].SetPixels32(pixelArray);
                resultArray[i,j].Apply();
            }
        }

		return resultArray;
	}

	/// <summary>
	/// Returns a new texture containing the region defined by the crop rectangle.
	/// </summary>
	public static Texture2D CropTexture(this Texture2D texture2D,RectInt cropSize)
	{
		if(!_IsValid(texture2D))
		{
			return null;
		}

		var width = texture2D.width-cropSize.width;
		var height = texture2D.height-cropSize.height;

		if(width <= 0 || height <= 0)
		{
			LogChannel.Kit.E($"Crop result size is invalid: width={width}, height={height}");

			return null;
		}

		if(cropSize.xMin < 0 || cropSize.yMin < 0)
		{
			LogChannel.Kit.E($"Crop position is invalid: ({cropSize.xMin}, {cropSize.yMin})");

			return null;
		}
		var colorArray = texture2D.GetPixels(cropSize.xMin,cropSize.yMin,width,height);
		var pixelArray = new Color32[width*height];

		for(var i=0;i<pixelArray.Length;i++)
		{
			pixelArray[i] = colorArray[i];
		}

		var result = new Texture2D(width,height,texture2D.format,false);

		result.SetPixels32(pixelArray);
		result.Apply();

		return result;
	}

	/// <summary>
	/// Rotates pixel data in place by the given angle in degrees.
	/// </summary>
	public static void RotateTexture(this Texture2D texture2D,float rotateAngle)
	{
		if(!_IsValid(texture2D))
		{
			return;
		}

		var angle = rotateAngle.ToWrapAngle();

		if(angle.ApproximatelyZero())
		{
			return;
		}

		var width = texture2D.width;
		var height = texture2D.height;

		var originColorArray = texture2D.GetPixels32();
		var rotateColorArray = new Color32[width*height];

		var dxX = _RotateX(angle,1.0f,0.0f);
		var dxY = _RotateY(angle,1.0f,0.0f);
		var dyX = _RotateX(angle,0.0f,1.0f);
		var dyY = _RotateY(angle,0.0f,1.0f);

		var rotX = _RotateX(angle,-width/2.0f,-height/2.0f)+width/2.0f;
		var rotY = _RotateY(angle,-width/2.0f,-height/2.0f)+height/2.0f;

		for(var i=0;i<width;i++)
		{
			var x = rotX;
			var y = rotY;

			for(var j=0;j<height;j++)
			{
				x += dxX;
				y += dxY;

				var srcX = Mathf.FloorToInt(x);
				var srcY = Mathf.FloorToInt(y);

				rotateColorArray[j*width+i] = (srcX >= 0 && srcX < width && srcY >= 0 && srcY < height)
					? originColorArray[srcY*width+srcX]
					: default;
			}

			rotX += dyX;
			rotY += dyY;
		}

		texture2D.SetPixels32(rotateColorArray);
		texture2D.Apply();
	}

	private static float _RotateX(float _angle,float _x,float _y)
	{
		return _x*Mathf.Cos(_angle/180.0f*Mathf.PI)+_y*-Mathf.Sin(_angle/180.0f*Mathf.PI);
	}

	private static float _RotateY(float _angle,float _x,float _y)
	{
		return _x*Mathf.Sin(_angle/180.0f*Mathf.PI)+_y*Mathf.Cos(_angle/180.0f*Mathf.PI);
	}

	private static bool _IsValid(Texture2D texture2D)
	{
		if(!texture2D)
		{
			LogChannel.Kit.E("Texture2D is null");

			return false;
		}

		return true;
	}
}