using UnityEngine;

public static class RectExtension
{
	public static Rect ToTexCoords(this Rect rect,int width,int height)
	{
		var result = rect;

		if(width != 0.0f && height != 0.0f)
		{
			result.xMin = rect.xMin/width;
			result.xMax = rect.xMax/width;
			result.yMin = 1.0f-rect.yMax/height;
			result.yMax = 1.0f-rect.yMin/height;
		}

		return result;
	}

	public static Rect ToPixels(this Rect rect,int width,int height,bool round = false)
	{
		var result = rect;

		if(round)
		{
			result.xMin = Mathf.RoundToInt(rect.xMin*width);
			result.xMax = Mathf.RoundToInt(rect.xMax*width);
			result.yMin = Mathf.RoundToInt((1.0f-rect.yMax)*height);
			result.yMax = Mathf.RoundToInt((1.0f-rect.yMin)*height);
		}
		else
		{
			result.xMin = rect.xMin*width;
			result.xMax = rect.xMax*width;
			result.yMin = (1.0f-rect.yMax)*height;
			result.yMax = (1.0f-rect.yMin)*height;
		}
		return result;
	}

	public static Rect MakePixelPerfect(this Rect rect)
	{
		rect.xMin = Mathf.RoundToInt(rect.xMin);
		rect.yMin = Mathf.RoundToInt(rect.yMin);
		rect.xMax = Mathf.RoundToInt(rect.xMax);
		rect.yMax = Mathf.RoundToInt(rect.yMax);

		return rect;
	}

	public static Rect MakePixelPerfect(this Rect rect,int width,int height)
	{
		rect = ToPixels(rect,width,height,true);

		rect.xMin = Mathf.RoundToInt(rect.xMin);
		rect.yMin = Mathf.RoundToInt(rect.yMin);
		rect.xMax = Mathf.RoundToInt(rect.xMax);
		rect.yMax = Mathf.RoundToInt(rect.yMax);

		return ToTexCoords(rect,width,height);
	}
}