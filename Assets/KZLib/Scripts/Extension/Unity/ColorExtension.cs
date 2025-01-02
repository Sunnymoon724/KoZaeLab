using System.Collections.Generic;
using UnityEngine;

public static class ColorExtension
{
	private static readonly Dictionary<Color,string> s_colorDict = new();

	private const float c_brightness_default = 0.0625f;

	/// <summary>
	/// Color to hexCode
	/// </summary>
	public static string ToHexCode(this Color color)
	{
		if(!s_colorDict.ContainsKey(color))
		{
			s_colorDict.Add(color,$"#{ColorUtility.ToHtmlStringRGBA(color)}");
		}

		return s_colorDict[color];
	}

	/// <summary>
	/// Color to HSV
	/// </summary>
	public static Vector3 ToHSV(this Color color)
	{
		Color.RGBToHSV(color,out var hue,out var saturation,out var value);

		return new Vector3(hue,saturation,value);
	}

	#region Mask
	public static Color MaskRed(this Color color,float red = 0.0f) => color.MaskColor(red,null,null,null);
	public static Color MaskGreen(this Color color,float green = 0.0f) => color.MaskColor(null,green,null,null);
	public static Color MaskBlue(this Color color,float blue = 0.0f) => color.MaskColor(null,null,blue,null);
	public static Color MaskAlpha(this Color color,float alpha = 0.0f) => color.MaskColor(null,null,null,alpha);

	public static Color MaskColor(this Color color,float? red = null,float? green = null,float? blue = null,float? alpha = null)
	{
		return new Color(red ?? color.r,green ?? color.g,blue ?? color.b,alpha ?? color.a);
	}
	#endregion Mask

	public static Color InvertColor(this Color color) => new(1.0f-color.r,1.0f-color.g,1.0f-color.b);

	public static int ToInt(this Color color)
	{
		var result = 0;

		result |= Mathf.RoundToInt(color.r*255.0f) << 24;
		result |= Mathf.RoundToInt(color.g*255.0f) << 16;
		result |= Mathf.RoundToInt(color.b*255.0f) << 8;
		result |= Mathf.RoundToInt(color.a*255.0f);

		return result;
	}

	public static Color Lighter(this Color color,float value = c_brightness_default) => color.Brightness(+value);
	public static Color Darker(this Color color,float value = c_brightness_default) => color.Brightness(-value);

	public static Color Brightness(this Color color,float value) => new(color.r+value,color.g+value,color.b+value,color.a);

	/// <summary>
	/// Compare value & 0.5f => white, else => black
	/// </summary>
	public static Color AutoTextColor(this Color color)
	{
		var max = Mathf.Max(color.r,color.g,color.b);

		return max > 0.5f ? Color.white : Color.black;
	}

	public static void ClearCacheData() => s_colorDict.Clear();
}
