using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Newtonsoft.Json;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public static class StringExtension
{
	private static readonly Dictionary<string,Color> s_hexColorDict = new();

	#region Normalize
	public static string NormalizeNewLines(this string text)
	{
		return text.Replace("\\n",Environment.NewLine);
	}

	/// <summary>
	/// Change path slash
	/// </summary>
	public static string PathConvertSlash(this string path)
	{
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
		return path.Replace('/',Path.DirectorySeparatorChar);
#elif UNITY_EDITOR_OSX || UNITY_Standalone_OSX || UNITY_IOS || UNITY_ANDROID
		return path.Replace('\\',Path.DirectorySeparatorChar);
#else
		return path.Replace('\\','/');
#endif
	}
	#endregion Normalize

	#region Encoding
	public static string CP949ToUTF8(this string text)
	{
		var cp949 = Encoding.GetEncoding("ks_c_5601-1987");
		var utf8 = Encoding.UTF8;

		return utf8.GetString(Encoding.Convert(cp949,utf8,cp949.GetBytes(text)));
	}
	#endregion Encoding

	#region Character
	/// <summary>
	/// Check character in string
	/// </summary>
	public static bool IsContainsCharacterArray(this string text,params char[] characterArray)
	{
		if(text.IsEmpty() || characterArray.IsNullOrEmpty())
		{
			return false;
		}

		var characterSet = new HashSet<char>(characterArray);

		return text.Any(x => characterSet.Contains(x));
	}
	/// <summary>
	/// Convert first character to uppercase
	/// </summary>
	public static string ToFirstCharacterToUpper(this string text)
	{
		if(text.IsEmpty())
		{
			return text;
		}

		var upperText = char.ToUpperInvariant(text[0]);

		return text.Length > 1 ? $"{upperText}{text[1..]}" : upperText.ToString();
	}

	/// <summary>
	/// Convert first character to lowercase
	/// </summary>
	public static string ToFirstCharacterToLower(this string text)
	{
		if(text.IsEmpty())
		{
			return text;
		}

		var lowerText = char.ToLowerInvariant(text[0]);

		return text.Length > 1 ? $"{lowerText}{text[1..]}" : lowerText.ToString();
	}

	/// <summary>
	/// Get character 0 to count from first
	/// </summary>
	public static string GetStartCharacter(this string text,int count)
	{
		return text.IsEmpty() || count <= 0 ? text : text[..Mathf.Min(count, text.Length)];
	}

	/// <summary>
	/// Get character 0 to count from last
	/// </summary>
	public static string GetEndCharacter(this string text,int count)
	{
		return text.IsEmpty() || count <= 0 ? text : text[^(Mathf.Min(count,text.Length))..];
	}

	/// <summary>
	/// Count character in string
	/// </summary>
	public static int CountOf(this string text,char character)
	{
		return text.Count(x => x == character);
	}

	/// <summary>
	/// Get index of character in string
	/// </summary>
	public static int IndexOfOrder(this string text,char character,int order)
	{
		if(text.IsEmpty())
		{
			return -1;
		}

		var index = -1;

		for(var i=0;i<order;i++)
		{
			index = text.IndexOf(character,index+1);

			if(index == -1)
			{
				return -1;
			}
		}
		return index;
	}
	#endregion Character

	#region Compare
	/// <summary>
	/// Check empty text
	/// </summary>
	/// <param name="_text"></param>
	public static bool IsEmpty(this string text,bool includeSpace = false)
	{
		return includeSpace ? string.IsNullOrWhiteSpace(text) : string.IsNullOrEmpty(text);
	}

	public static bool IsEqual(this string text1,string text2)
	{
		return string.Equals(text1,text2);
	}

	/// <summary>
	/// Check match at index
	/// </summary>
	public static bool IsMatchAt(this string text,int index,string matchText,bool ignoreCase = false)
	{
		if(text.IsEmpty() || matchText.IsEmpty() || index < 0 || index+matchText.Length > text.Length)
		{
			return false;
		}

		return ignoreCase ? string.Equals(text.Substring(index,matchText.Length),matchText,StringComparison.OrdinalIgnoreCase) : text.Substring(index,matchText.Length) == matchText;
	}
	#endregion Compare

	#region Convert Enum
	public static bool IsEnumDefined<TEnum>(this string text)
	{
		return Enum.IsDefined(typeof(TEnum),text);
	}

	public static TEnum ToEnum<TEnum>(this string text) where TEnum : struct
	{
		if(!text.IsEmpty() && Enum.TryParse(text,true,out TEnum value))
		{
			return value;
		}

		LogTag.System.W($"Failed to convert {text} into Enum");

		return default;
	}
	#endregion Convert Enum

	#region Convert Color
	/// <summary>
	/// HexCode to Color
	/// </summary>
	public static Color ToColor(this string hexCode)
	{
		if(hexCode.Length == 7)
		{
			hexCode = $"{hexCode}FF";
		}

		if(s_hexColorDict.TryGetValue(hexCode,out var color))
		{
			return color;
		}

		if(ColorUtility.TryParseHtmlString(hexCode,out color))
		{
			s_hexColorDict.Add(hexCode,color);

			return color;
		}

		LogTag.System.W($"Failed to convert {hexCode} into color");

		return Color.clear;
	}

	/// <summary>
	/// Add richText
	/// </summary>
	public static string ToColorText(this string text,string color)
	{
		return $"<color=#{color}>{text}</color>";
	}

	/// <summary>
	/// Add richText
	/// </summary>
	public static string ToColorText(this string text,Color color)
	{
		return ToColorText(ColorUtility.ToHtmlStringRGBA(color),text);
	}
	#endregion Convert Color

	#region Convert Bool
	public static bool ToBool(this string text)
	{
		if(!text.IsEmpty() && bool.TryParse(text,out var value))
		{
			return value;
		}

		LogTag.System.W($"Failed to convert {text} into bool");

		return false;
	}
	#endregion Convert Bool

	#region Convert Number
	public static BigInteger ToBigInteger(this string text)
	{
		if(!text.IsEmpty() && BigInteger.TryParse(text,out var value))
		{
			return value;
		}

		LogTag.System.W($"Failed to convert {text} into BigInteger");

		return default;
	}

	public static int ToInt(this string text)
	{
		if(!text.IsEmpty())
		{
			if(text.StartsWith("0x",StringComparison.OrdinalIgnoreCase))
			{
				return text.ToHexInt();
			}

			if(int.TryParse(text,out var value))
			{
				return value;
			}
		}

		LogTag.System.W($"Failed to convert {text} into int");

		return default;
	}

	public static float ToFloat(this string text)
	{
		if(!text.IsEmpty() && float.TryParse(text,out var value))
		{
			return value;
		}

		LogTag.System.W($"Failed to convert {text} into float");

		return default;
	}

	public static double ToDouble(this string text)
	{
		if(!text.IsEmpty() && double.TryParse(text,out var value))
		{
			return value;
		}

		LogTag.System.W($"Failed to convert {text} into double");

		return default;
	}

	public static byte ToByte(this string text)
	{
		if(!text.IsEmpty())
		{
			if(text.StartsWith("0x",StringComparison.OrdinalIgnoreCase))
			{
				return Convert.ToByte(text,16);
			}

			if(byte.TryParse(text,out var value))
			{
				return value;
			}
		}

		LogTag.System.W($"Failed to convert {text} into byte");

		return default;
	}

	public static int ToHexInt(this string hexText)
	{
		if(!hexText.IsEmpty() && int.TryParse(hexText,NumberStyles.HexNumber,CultureInfo.CurrentCulture,out var value))
		{
			return value;
		}

		LogTag.System.W($"Failed to convert {hexText} into int");

		return default;
	}

	public static float ToHexFloat(string hexText)
	{
		if(!hexText.IsEmpty() && uint.TryParse(hexText,NumberStyles.AllowHexSpecifier,CultureInfo.CurrentCulture,out var value))
		{
			return BitConverter.ToSingle(BitConverter.GetBytes(value),0);
		}

		LogTag.System.W($"Failed to convert {hexText} into float");

		return default;
	}

	private static float _GetNumberInArray(string[] textArray,int index)
	{
		return textArray.ContainsIndex(index) ? textArray[index].ToFloat() : 0.0f;
	}
	#endregion Convert Number

	#region Convert DateTime
	public static DateTime ToDateTime(this string text,CultureInfo cultureInfo = null, DateTimeStyles dateTimeStyles = DateTimeStyles.AdjustToUniversal)
	{
		if(!text.IsEmpty() && DateTime.TryParseExact(text,"yyyy-MM-dd HH:mm",cultureInfo ?? CultureInfo.CreateSpecificCulture("ko-KR"),dateTimeStyles,out var value))
		{
			return value;
		}

		LogTag.System.W($"Failed to convert {text} into DateTime");

		return default;
	}
	#endregion Convert DateTime

	#region Convert Vector
	public static Vector2 ToVector2(this string text)
	{
		if(!text.IsEmpty() && text.TryToVector2(out var value))
		{
			return value;
		}

		LogTag.System.W($"Failed to convert {text} into Vector2");

		return default;
	}

	public static bool TryToVector2(this string text,out Vector2 vector)
	{
		vector = default;

		var vectorArray = _ConvertVectorArray(text);		

		if(vectorArray.IsNullOrEmpty())
		{
			return false;
		}

		vector = new Vector2(_GetNumberInArray(vectorArray,0),_GetNumberInArray(vectorArray,1));

		return true;
	}

	public static Vector3 ToVector3(this string text)
	{
		if(!text.IsEmpty() && text.TryToVector3(out var value))
		{
			return value;
		}

		LogTag.System.W($"Failed to convert {text} into Vector3");

		return default;
	}

	public static bool TryToVector3(this string text,out Vector3 vector)
	{
		vector = default;

		var vectorArray = _ConvertVectorArray(text);

		if(vectorArray.IsNullOrEmpty())
		{
			return false;
		}

		vector = new Vector3(_GetNumberInArray(vectorArray,0),_GetNumberInArray(vectorArray,1),_GetNumberInArray(vectorArray,2));

		return true;
	}

	private static string[] _ConvertVectorArray(string text)
	{
		return text.IsEmpty() ? null : text.TrimParentheses().Split(',');
	}
	#endregion Convert Vector

	#region Remove
	/// <summary>
	/// Remove text from first
	/// </summary>
	public static string RemoveStart(this string text,string removeText)
	{
		return (text.IsEmpty() || removeText.IsEmpty()) ? text : text.StartsWith(removeText) ? text[removeText.Length..] : text;
	}

	/// <summary>
	/// Remove text from last
	/// </summary>
	public static string RemoveEnd(this string text,string removeText)
	{
		return (text.IsEmpty() || removeText.IsEmpty()) ? text : text.EndsWith(removeText) ? text[..^removeText.Length] : text;
	}

	/// <summary>
	/// Remove text by count from first
	/// </summary>
	public static string RemoveStartCharacter(this string text,int count)
	{
		return (text.IsEmpty() || count <= 0) ? text : count >= text.Length ? string.Empty : text[count..];
	}

	/// <summary>
	/// Remove text by count from last
	/// </summary>
	public static string RemoveEndCharacter(this string text,int count)
	{
		return (text.IsEmpty() || count <= 0) ? text : count >= text.Length ? string.Empty : text[..^count];
	}

	/// <summary>
	/// Remove richText
	/// </summary>
	public static string RemoveRichText(this string text)
	{
		return Regex.Replace(text,"<.*?>",string.Empty);
	}
	#endregion Remove

	#region Trim
	/// <summary>
	/// Trim text from first
	/// </summary>
	public static string TrimTextStart(this string text,string trimText)
	{
		if(text.IsEmpty() || trimText.IsEmpty())
		{
			return text;
		}

		while(text.StartsWith(trimText))
		{
			text = text[trimText.Length..];
		}

		return text;
	}

	/// <summary>
	/// Trim text from last
	/// </summary>
	public static string TrimTextEnd(this string text,string trimText)
	{
		if(text.IsEmpty() || trimText.IsEmpty())
		{
			return text;
		}

		while(text.EndsWith(trimText))
		{
			text = text[..^trimText.Length];
		}

		return text;
	}

	/// <summary>
	/// Trim all space
	/// </summary>
	public static string TrimAllSpace(this string text)
	{
        return text.IsEmpty() ? text : text.Replace(" ",string.Empty);
	}

	/// <summary>
	/// Trim all brackets
	/// </summary>
	public static string TrimBrackets(this string text)
	{
		return text.Trim('(',')','[',']','{','}','<','>');
	}
	public static string TrimParentheses(this string text)
	{
		return text.Trim('(',')');
	}
	public static string TrimBraces(this string text)
	{
		return text.Trim('{','}');
	}
	public static string TrimSquareBrackets(this string text)
	{
		return text.Trim('[',']');
	}
	public static string TrimAngleBrackets(this string text)
	{
		return text.Trim('<','>');
	}
	#endregion Trim

	#region Wrap
	public static string WrapParentheses(this string text)
	{
		return $"({text})";
	}
	public static string WrapBraces(this string text)
	{
		return $"{{{text}}}";
	}
	public static string WrapSquareBrackets(this string text)
	{
		return $"[{text}]";
	}
	public static string WrapAngleBrackets(this string text)
	{
		return $"<{text}>";
	}
	#endregion Wrap

	#region Split
	public static string[] SplitTextByBrackets(this string text,StringSplitOptions stringSplitOption = StringSplitOptions.None)
	{
		return text.Split(new char[] { '(', ')', '[', ']', '{', '}', '<', '>' }, stringSplitOption);
	}

	public static string[] SplitTextByParentheses(this string text,StringSplitOptions stringSplitOption = StringSplitOptions.None)
	{
		return text.Split(new char[] { '(', ')' }, stringSplitOption);
	}

	public static string[] SplitTextByBraces(this string text,StringSplitOptions stringSplitOption = StringSplitOptions.None)
	{
		return text.Split(new char[] { '{', '}' }, stringSplitOption);
	}

	public static string[] SplitTextBySquareBrackets(this string text,StringSplitOptions stringSplitOption = StringSplitOptions.None)
	{
		return text.Split(new char[] { '[', ']' }, stringSplitOption);
	}

	public static string[] SplitTextByAngleBrackets(this string text,StringSplitOptions stringSplitOption = StringSplitOptions.None)
	{
		return text.Split(new char[] { '<', '>' }, stringSplitOption);
	}
	#endregion Split

	#region Extract
	/// <summary>
	/// Extract start to end
	/// </summary>
	public static string ExtractText(this string text,string startText,string endText)
	{
		var start = text.RemoveStart(startText);

		if(text.IsEqual(start))
		{
			return text;
		}

		var end = start.RemoveEnd(endText);

		return start.IsEqual(end) ? text : end;
	}

	/// <summary>
	/// Extract start to end
	/// </summary>
	public static string ExtractText(this string text,char startMark,char endMark)
	{
		return text.ExtractText($"{startMark}",$"{endMark}");
	}

	/// <summary>
	/// Extract start to end
	/// </summary>
	public static string ExtractText(this string text,char startMark,string endText)
	{
		return text.ExtractText($"{startMark}",endText);
	}

	/// <summary>
	/// Extract start to end
	/// </summary>
	public static string ExtractText(this string text,string startText,char endMark)
	{
		return text.ExtractText(startText,$"{endMark}");
	}

	/// <summary>
	/// Extract Letters
	/// </summary>
	public static string ExtractOnlyLetter(this string text)
	{
		return text.IsEmpty() ? null : Regex.Replace(text,@"[^a-zA-Z]",string.Empty);
	}

	/// <summary>
	/// Extract Digits
	/// </summary>
	public static string ExtractOnlyDigit(this string text)
	{
		return text.IsEmpty() ? null : Regex.Replace(text,@"\D",string.Empty);
	}

	/// <summary>
	/// Extract Alphanumeric
	/// </summary>
	public static string ExtractAlphanumeric(this string text)
	{
		return text.IsEmpty() ? null : Regex.Replace(text,@"[^0-9a-zA-Z_]+",string.Empty);
	}

	public static int ExtractOnlyDigitToInt(this string text)
	{
		return ExtractOnlyDigit(text).ToInt();
	}
	#endregion Extract

	#region Layer
	public static int FindLayerByName(this string layerName,bool isAutoCreate = false)
	{
		var layer = LayerMask.NameToLayer(layerName);

#if UNITY_EDITOR
		if(layer == -1 && isAutoCreate)
		{
			CommonUtility.AddLayer(layerName);

			layer = LayerMask.NameToLayer(layerName);
		}
#endif

		return layer;
	}
	#endregion Layer

	public static void ClearCacheData()
	{
		s_hexColorDict.Clear();
	}

	public static string XmlToJson(this string xmlText)
	{
		using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xmlText));
		var xml = new XmlDocument();
		xml.Load(stream);

		return JsonConvert.SerializeXmlNode(xml);
	}
}