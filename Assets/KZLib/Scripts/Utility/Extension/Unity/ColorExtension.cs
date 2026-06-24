using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Extension methods for <see cref="Color"/> conversion, masking, and brightness adjustments.
/// </summary>
public static class ColorExtension
{
	private static readonly Dictionary<Color,string> s_colorDict = new();

	private const float c_defaultBrightness = 0.0625f;

	/// <summary>
	/// Converts the color to an HTML-style RGBA hex string.
	/// </summary>
	public static string ToHexCode(this Color color)
	{
		if(!s_colorDict.ContainsKey(color))
		{
			s_colorDict.Add(color,$"{Global.HexColorPrefix}{ColorUtility.ToHtmlStringRGBA(color)}");
		}

		return s_colorDict[color];
	}

	/// <summary>
	/// Converts the color to an HSV vector (hue, saturation, value).
	/// </summary>
	public static Vector3 ToHSV(this Color color)
	{
		Color.RGBToHSV(color,out var hue,out var saturation,out var value);

		return new Vector3(hue,saturation,value);
	}

	#region Mask
	/// <summary>
	/// Replaces the red channel with the given value.
	/// </summary>
	public static Color MaskRed(this Color color,float red = 0.0f) => color.MaskColor(red,null,null,null);

	/// <summary>
	/// Replaces the green channel with the given value.
	/// </summary>
	public static Color MaskGreen(this Color color,float green = 0.0f) => color.MaskColor(null,green,null,null);

	/// <summary>
	/// Replaces the blue channel with the given value.
	/// </summary>
	public static Color MaskBlue(this Color color,float blue = 0.0f) => color.MaskColor(null,null,blue,null);

	/// <summary>
	/// Replaces the alpha channel with the given value.
	/// </summary>
	public static Color MaskAlpha(this Color color,float alpha = 0.0f) => color.MaskColor(null,null,null,alpha);

	/// <summary>
	/// Replaces selected RGBA channels with optional mask values.
	/// </summary>
	public static Color MaskColor(this Color color,float? red = null,float? green = null,float? blue = null,float? alpha = null)
	{
		return new Color(red ?? color.r,green ?? color.g,blue ?? color.b,alpha ?? color.a);
	}
	#endregion Mask

	/// <summary>
	/// Inverts RGB channels while preserving alpha.
	/// </summary>
	public static Color InvertColor(this Color color) => new(1.0f-color.r,1.0f-color.g,1.0f-color.b);

	/// <summary>
	/// Packs RGBA channel values into a single 32-bit integer (ARGB byte order).
	/// </summary>
	public static int ToInt(this Color color)
	{
		var result = 0;

		result |= Mathf.RoundToInt(color.r*Global.ColorMaxValue) << 24;
		result |= Mathf.RoundToInt(color.g*Global.ColorMaxValue) << 16;
		result |= Mathf.RoundToInt(color.b*Global.ColorMaxValue) << 8;
		result |= Mathf.RoundToInt(color.a*Global.ColorMaxValue);

		return result;
	}

	/// <summary>
	/// Lightens RGB channels by adding the given brightness offset.
	/// </summary>
	public static Color Lighter(this Color color,float value = c_defaultBrightness) => color.Brightness(+value);

	/// <summary>
	/// Darkens RGB channels by subtracting the given brightness offset.
	/// </summary>
	public static Color Darker(this Color color,float value = c_defaultBrightness) => color.Brightness(-value);

	/// <summary>
	/// Adjusts RGB channels by the given brightness offset while preserving alpha.
	/// </summary>
	public static Color Brightness(this Color color,float value) => new(color.r+value,color.g+value,color.b+value,color.a);

	/// <summary>
	/// Returns white or black for readable text contrast against this color.
	/// </summary>
	public static Color AutoTextColor(this Color color)
	{
		var max = Mathf.Max(color.r,color.g,color.b);

		return max > 0.5f ? Color.white : Color.black;
	}

	/// <summary>
	/// Creates a gradient between two colors
	/// </summary>
	public static Gradient Gradient(Color color1,Color color2)
	{
		var gradient = new Gradient();

		gradient.SetKeys(new[] { new GradientColorKey(color1,0.0f), new GradientColorKey(color2,1.0f) },new[] { new GradientAlphaKey(color1.a,0.0f), new GradientAlphaKey(color2.a,1.0f) });

		return gradient;
	}
}
