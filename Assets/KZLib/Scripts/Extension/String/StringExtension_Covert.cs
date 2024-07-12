using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using UnityEngine;

using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public static partial class StringExtension
{
	private static readonly Dictionary<string,Color> s_HexColorDict = new();

	public static BigInteger ToBigInteger(this string _text,BigInteger _default = default)
	{
		if(_text.IsEmpty())
		{
			return _default;
		}

		if(_text.Contains("e") || _text.Contains("E"))
		{
			var result = double.Parse(_text);

			return new BigInteger(result);
		}
		else
		{
			return BigInteger.Parse(_text);
		}
	}

	public static bool IsEnumDefined<TEnum>(this string _text)
	{
		return Enum.IsDefined(typeof(TEnum),_text);
	}

	public static TEnum ToEnum<TEnum>(this string _text,TEnum _default = default) where TEnum : struct
	{
		return !_text.IsEmpty() && Enum.TryParse(_text,true,out TEnum data) ? data : _default;
	}

	/// <summary>
	/// default는 Color.clear
	/// </summary>
	public static Color ToColor(this string _hexCode)
	{
		_hexCode = _hexCode.Length == 7 ? string.Format("{0}FF",_hexCode) : _hexCode;

		if(!s_HexColorDict.ContainsKey(_hexCode))
		{
			if(ColorUtility.TryParseHtmlString(_hexCode,out var color))
			{
				s_HexColorDict.Add(_hexCode,color);
			}
			else
			{
				return Color.clear;
			}
		}

		return s_HexColorDict[_hexCode];
	}

	public static int ToInt(this string _text,int _default = 0)
	{
		if(_text.IsEmpty())
		{
			return _default;
		}

		if(_text.StartsWith("0x"))
		{
			return _text.ToHexInt(_default);
		}

		return int.TryParse(_text,out var num) ? num : _default;
	}

	public static float ToFloat(this string _text,float _default = 0.0f)
	{
		return float.TryParse(_text,out var num) ? num : _default;
	}

	public static double ToDouble(this string _text,double _default = 0.0d)
	{
		return double.TryParse(_text,out var num) ? num : _default;
	}

	public static byte ToByte(this string _text,byte _default = 0x00)
	{
		if(_text.StartsWith("0x"))
		{
			return Convert.ToByte(_text,16);
		}

		return byte.TryParse(_text,out var num) ? num : _default;
	}
	
	public static int ToHexInt(this string _hexText,int _default = 0)
	{
		return int.TryParse(_hexText,NumberStyles.HexNumber,CultureInfo.CurrentCulture,out var num) ? num : _default;
	}

	public static float ToHexFloat(string _hexText,float _default = 0.0f)
	{
		if(uint.TryParse(_hexText,NumberStyles.AllowHexSpecifier,CultureInfo.CurrentCulture,out var num))
		{
			return BitConverter.ToSingle(BitConverter.GetBytes(num),0);
		}
		else
		{
			return _default;
		}
	}

	public static DateTime ToDateTime(this string _text,CultureInfo _cultureInfo = null, DateTimeStyles _styles = DateTimeStyles.AdjustToUniversal)
	{
		var cultureInfo = _cultureInfo ?? CultureInfo.CreateSpecificCulture("ko-KR");

		return DateTime.ParseExact(_text,"yyyy-MM-dd HH:mm",cultureInfo,_styles);
	}

	public static Vector2 ToVector2(this string _text)
	{
		return _text.ToVector2(Vector2.zero);
	}

	public static Vector2 ToVector2(this string _text,Vector2 _default)
	{
		return _text.TryToVector2(out var result) ? result : _default;
	}

	public static bool TryToVector2(this string _text,out Vector2 _result)
	{
		_result = Vector2.zero;

		var vectorArray = ConvertVectorArray(_text);		

		if(vectorArray.IsNullOrEmpty())
		{
			return false;
		}

		_result = new Vector2(GetNumberInArray(vectorArray,0),GetNumberInArray(vectorArray,1));

		return true;
	}

	public static Vector3 ToVector3(this string _text)
	{
		return _text.ToVector3(Vector3.zero);
	}

	public static Vector3 ToVector3(this string _text,Vector3 _default)
	{
		return _text.TryToVector3(out var result) ? result : _default;
	}

	public static bool TryToVector3(this string _text,out Vector3 _result)
	{
		_result = Vector3.zero;

		var vectorArray = ConvertVectorArray(_text);		

		if(vectorArray.IsNullOrEmpty())
		{
			return false;
		}
		
		_result = new Vector3(GetNumberInArray(vectorArray,0),GetNumberInArray(vectorArray,1),GetNumberInArray(vectorArray,2));
		
		return true;
	}

	private static string[] ConvertVectorArray(string _text)
	{
		if(_text.IsEmpty())
		{
			return null;
		}

		return _text.TrimParenthesis().Split(',');
	}

	private static float GetNumberInArray(string[] _textArray,int _index)
	{
		return _textArray.ContainsIndex(_index) ? _textArray[_index].ToFloat() : 0.0f;
	}

	
	public static string ToColorText(this string _text,string _color)
	{
		return string.Format("<color=#{0}>{1}</color>",_color,_text);
	}
	
	public static string ToColorText(this string _text,Color _color)
	{
		return ToColorText(ColorUtility.ToHtmlStringRGBA(_color),_text);
	}

	public static string ToFirstCharToUpper(this string _text)
	{
		if(_text.IsEmpty())
		{
			return _text;
		}

		var text = char.ToUpperInvariant(_text[0]);

		return _text.Length > 1 ? string.Concat(text,_text[1..]) : text.ToString();
	}

	public static string ToFirstCharToLower(this string _text)
	{
		if(_text.IsEmpty())
		{
			return _text;
		}

		var text = char.ToLowerInvariant(_text[0]);

		return _text.Length > 1 ? string.Concat(text,_text[1..]) : text.ToString();
	}

	public static string XmlToJson(this string _xml)
	{
		using var stream = new MemoryStream(Encoding.UTF8.GetBytes(_xml));
		var xml = new XmlDocument();
		xml.Load(stream);

		return JsonConvert.SerializeXmlNode(xml);
	}

	/// <summary>
	/// 경로 슬래시 변경
	/// </summary>
	public static string PathConvertSlash(this string _path)
	{
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
		return _path.Replace('/',Path.DirectorySeparatorChar);
#elif UNITY_EDITOR_OSX || UNITY_Standalone_OSX || UNITY_IOS || UNITY_ANDROID
		return _path.Replace('\\',Path.DirectorySeparatorChar);
#else
		return _path.Replace('\\','/');
#endif
	}

	/// <summary>
	/// 캐시된 색상 데이터를 모두 삭제합니다.
	/// </summary>
	public static void ClearCacheData()
	{
		s_HexColorDict.Clear();
	}
}