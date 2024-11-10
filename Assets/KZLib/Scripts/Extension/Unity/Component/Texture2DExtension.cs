using UnityEngine;

public static class Texture2DExtension
{
	public static void ToSolidColor(this Texture2D _texture,Color _color)
	{
		if(!_texture)
		{
			LogTag.System.E("Texture2D is null");

			return;
		}

		_texture.ToSolidColor((Color32) _color);
	}

	public static void ToSolidColor(this Texture2D _texture,Color32 _color32)
	{
		if(!_texture)
		{
			LogTag.System.E("Texture2D is null");

			return;
		}

		var pixelArray = _texture.GetPixels32();

		for(var i=0;i<pixelArray.Length;i++)
		{
			pixelArray[i] = pixelArray[i].a == 0 ? Color.clear : _color32;
		}

		_texture.SetPixels32(pixelArray);
		_texture.Apply();
	}

	public static void ScaleTexture(this Texture2D _texture,Vector2Int _size)
	{
		if(!_texture)
		{
			LogTag.System.E("Texture2D is null");

			return;
		}

		var scale = new Vector2(1.0f/_size.x,1.0f/_size.y);

		var pixelArray = _texture.GetPixels32();

		for(var i=0;i<pixelArray.Length;i++)
		{
			pixelArray[i] = _texture.GetPixelBilinear(scale.x*(i%_size.x),scale.y*(i/_size.y));
		}

		_texture.SetPixels32(pixelArray);
		_texture.Apply();
	}

	public static Texture2D CopyTexture(this Texture2D _texture)
	{
		if(!_texture)
		{
			LogTag.System.E("Texture2D is null");

			return null;
		}

		var texture = new Texture2D(_texture.width,_texture.height,_texture.format,false);

		texture.SetPixels32(_texture.GetPixels32());
		texture.Apply();

		return texture;
	}

	public static Sprite CreateSprite(this Texture2D _texture)
	{
		if(!_texture)
		{
			LogTag.System.E("Texture2D is null");

			return null;
		}

		return Sprite.Create(_texture,new Rect(0.0f,0.0f,_texture.width,_texture.height),new Vector2(0.5f,0.5f));
	}

	public static Sprite CreateTiledSprite(this Texture2D _texture,float _border = 0.0f,float _pixelsPerUnit = 100.0f)
	{
		if(!_texture)
		{
			LogTag.System.E("Texture2D is null");

			return null;
		}

		_texture.wrapMode = TextureWrapMode.Repeat;

		return Sprite.Create(_texture,new Rect(0.0f,0.0f,_texture.width,_texture.height),Vector2.zero,_pixelsPerUnit,0,SpriteMeshType.Tight,new Vector4(_border,_border,_border,_border));
	}

	public static Texture2D[,] SplitTexture(this Texture2D _texture,Vector2Int _count)
	{
		if(!_texture)
		{
			LogTag.System.E("Texture2D is null");

			return null;
		}

		if(_count.x <= 0 || _count.y <= 0 )
		{
			LogTag.System.E($"Count is below zero {_count.x} or {_count.y}");

			return null;
		}

		var width = _texture.width/_count.x;
        var height = _texture.height/_count.y;
		var totalColorArray = _texture.GetPixels32();

        var resultArray = new Texture2D[_count.x,_count.y];

        for(var i=0;i<_count.x;i++)
        {
            for(var j=0;j<_count.y;j++)
            {
				resultArray[i,j] = new Texture2D(width,height);

                var pixelArray = new Color32[width*height];
                var index = 0;

                for(var k=i*width;k<(i+1)*width;k++)
                {
                    for(var l=j*height;l<(j+1)*height;i++)
                    {
                        pixelArray[index] = totalColorArray[l*_texture.width+k];
                        index++;
                    }
                }

                resultArray[i,j].SetPixels32(pixelArray);
                resultArray[i,j].Apply();
            }
        }

		return resultArray;
	}

	public static Texture2D CropTexture(this Texture2D _texture,RectInt _size)
	{
		if(!_texture)
		{
			LogTag.System.E("Texture2D is null");

			return null;
		}

		if(_size.x <= 0 || _size.y <= 0 )
		{
			LogTag.System.E($"Size is below zero {_size.x} or {_size.y}");

			return null;
		}

		var width = _texture.width-_size.width;
		var height = _texture.height-_size.height;
		var colorArray = _texture.GetPixels(_size.xMin,_size.yMax,width,height);
		var pixelArray = new Color32[width*height];

		for(var i=0;i<pixelArray.Length;i++)
		{
			pixelArray[i] = colorArray[i];
		}

		var result = new Texture2D(width,height,_texture.format,false);

		result.SetPixels32(pixelArray);
		result.Apply();

		return result;
	}

	public static void RotateTexture(this Texture2D _texture,float _angle)
	{
		if(!_texture)
		{
			LogTag.System.E("Texture2D is null");

			return;
		}

		var angle = _angle.ToWrapAngle();

		if(angle.ApproximatelyZero())
		{
			return;
		}

		var width = _texture.width;
		var height = _texture.height;

		var originColorArray = _texture.GetPixels32();
		var rotateColorArray = new Color32[width*height];

		var dxX = RotateX(angle,1.0f,0.0f);
		var dxY = RotateY(angle,1.0f,0.0f);
		var dyX = RotateX(angle,0.0f,1.0f);
		var dyY = RotateY(angle,0.0f,1.0f);

		var rotX = RotateX(angle,-width/2.0f,-height/2.0f)+width/2.0f;
		var rotY = RotateY(angle,-width/2.0f,-height/2.0f)+height/2.0f;

		for(var i=0;i<width;i++)
		{
			var x = rotX;
			var y = rotY;

			for(var j=0;j<height;j++)
			{
				x += dxX;
				y += dxY;

				rotateColorArray[j*width+i] = originColorArray[Mathf.FloorToInt(y*_texture.width)+i];
			}

			rotX += dyX;
			rotY += dyY;
		}

		_texture.SetPixels32(rotateColorArray);
		_texture.Apply();
	}

	private static float RotateX(float _angle,float _x,float _y)
	{
		return _x*Mathf.Cos(_angle/180.0f*Mathf.PI)+_y*-Mathf.Sin(_angle/180.0f*Mathf.PI);
	}

	private static float RotateY(float _angle,float _x,float _y)
	{
		return _x*Mathf.Sin(_angle/180.0f*Mathf.PI)+_y*Mathf.Cos(_angle/180.0f*Mathf.PI);
	}
}