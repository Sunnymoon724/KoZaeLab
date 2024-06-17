using System.Collections.Generic;
using UnityEngine;

public static class ColorExtension
{
	private static readonly Dictionary<Color,string> s_CacheDict = new();

	private const float BRIGHTNESS_DEFAULT = 0.0625f;

	public static string ToHexCode(this Color _color)
	{
		if(!s_CacheDict.ContainsKey(_color))
		{
			s_CacheDict.Add(_color,string.Format("#{0}",ColorUtility.ToHtmlStringRGBA(_color)));
		}

		return s_CacheDict[_color];
	}

	public static Vector3 ToHSV(this Color _color)
	{
		Color.RGBToHSV(_color,out var _hue,out var _saturation,out var _value);

		return new Vector3(_hue,_saturation,_value);
	}

	#region Mask
	public static Color MaskRed(this Color _color,float _red = 0.0f)
	{
		return _color.MaskColor(_red,null,null,null);
	}

	public static Color MaskGreen(this Color _color,float _green = 0.0f)
	{
		return _color.MaskColor(null,_green,null,null);
	}

	public static Color MaskBlue(this Color _color,float _blue = 0.0f)
	{
		return _color.MaskColor(null,null,_blue,null);
	}

	public static Color MaskAlpha(this Color _color,float _alpha = 0.0f)
	{
		return _color.MaskColor(null,null,null,_alpha);
	}

	public static Color MaskColor(this Color _color,float? _red = null,float? _green = null,float? _blue = null,float? _alpha = null)
	{
		return new Color(_red ?? _color.r,_green ?? _color.g,_blue ?? _color.b,_alpha ?? _color.a);
	}
	#endregion Mask

	public static Color InvertColor(this Color _color)
	{
		return new Color(1.0f-_color.r,1.0f-_color.g,1.0f-_color.b);
	}

	public static int ToInt(this Color _color)
	{
		var result = 0;

		result |= Mathf.RoundToInt(_color.r*255.0f) << 24;
		result |= Mathf.RoundToInt(_color.g*255.0f) << 16;
		result |= Mathf.RoundToInt(_color.b*255.0f) << 8;
		result |= Mathf.RoundToInt(_color.a*255.0f);

		return result;
	}

	public static Color Lighter(this Color _color,float _value = BRIGHTNESS_DEFAULT)
	{
		return _color.Brightness(+_value);
	}

	public static Color Darker(this Color _color,float _value = BRIGHTNESS_DEFAULT)
	{
		return _color.Brightness(-_value);
	}

	public static Color Brightness(this Color _color,float _value)
	{
		return new Color(_color.r+_value,_color.g+_value,_color.b+_value,_color.a);
	}

	public static Color AutoTextColor(this Color _color)
	{
		var max = Mathf.Max(_color.r,_color.g,_color.b);

		return max/255.0f > 0.5f ? Color.white : Color.black;
	}
}