using UnityEngine;

public static class RectExtension
{
	public static Rect ToTexCoords(this Rect _rect,int _width,int _height)
	{
		var result = _rect;

		if(_width != 0.0f && _height != 0.0f)
		{
			result.xMin = _rect.xMin/_width;
			result.xMax = _rect.xMax/_width;
			result.yMin = 1.0f-_rect.yMax/_height;
			result.yMax = 1.0f-_rect.yMin/_height;
		}

		return result;
	}

	public static Rect ToPixels(this Rect _rect,int _width,int _height,bool _round = false)
	{
		var result = _rect;

		if(_round)
		{
			result.xMin = Mathf.RoundToInt(_rect.xMin*_width);
			result.xMax = Mathf.RoundToInt(_rect.xMax*_width);
			result.yMin = Mathf.RoundToInt((1.0f-_rect.yMax)*_height);
			result.yMax = Mathf.RoundToInt((1.0f-_rect.yMin)*_height);
		}
		else
		{
			result.xMin = _rect.xMin*_width;
			result.xMax = _rect.xMax*_width;
			result.yMin = (1.0f-_rect.yMax)*_height;
			result.yMax = (1.0f-_rect.yMin)*_height;
		}
		return result;
	}

	public static Rect MakePixelPerfect(this Rect _rect)
	{
		_rect.xMin = Mathf.RoundToInt(_rect.xMin);
		_rect.yMin = Mathf.RoundToInt(_rect.yMin);
		_rect.xMax = Mathf.RoundToInt(_rect.xMax);
		_rect.yMax = Mathf.RoundToInt(_rect.yMax);

		return _rect;
	}

	public static Rect MakePixelPerfect(this Rect _rect,int _width,int _height)
	{
		_rect = ToPixels(_rect,_width,_height,true);

		_rect.xMin = Mathf.RoundToInt(_rect.xMin);
		_rect.yMin = Mathf.RoundToInt(_rect.yMin);
		_rect.xMax = Mathf.RoundToInt(_rect.xMax);
		_rect.yMax = Mathf.RoundToInt(_rect.yMax);

		return ToTexCoords(_rect,_width,_height);
	}
}