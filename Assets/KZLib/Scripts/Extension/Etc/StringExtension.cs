using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using KZLib.KZData;
using KZLib.KZUtility;
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
	
	#region Localize
	public static string ToLocalize(this string text)
	{
		return LingoManager.In.FindString(text);
	}
	#endregion Localize

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

		for(var i=0;i<text.Length;i++)
		{
			var character = text[i];

			if(characterSet.Contains(character))
			{
				return true;
			}
		}

		return false;
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
		var count = 0;
		var index = text.IndexOf(character);

		while(index != Global.INVALID_INDEX)
		{
			count++;
			index = text.IndexOf(character,index+1);
		}

		return count;
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

		var index = Global.INVALID_INDEX;

		for(var i=0;i<order;i++)
		{
			index = text.IndexOf(character,index+1);

			if(index == Global.INVALID_INDEX)
			{
				return Global.INVALID_INDEX;
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
	public static bool IsEnumDefined<TEnum>(this string text) where TEnum : struct
	{
		return Enum.IsDefined(typeof(TEnum),text);
	}

	public static TEnum ToEnum<TEnum>(this string text) where TEnum : struct
	{
		var result = text.TryToEnum<TEnum>(out var value);

		if(!result)
		{
			LogChannel.System.W($"Failed to convert {text} into Enum");
		}

		return value;
	}

	public static bool TryToEnum<TEnum>(this string text,out TEnum value) where TEnum : struct
	{
		if(!text.IsEmpty() && Enum.TryParse(text,true,out value))
		{
			return true;
		}

		value = default;

		return false;
	}
	#endregion Convert Enum

	#region Convert CustomTag
	public static bool IsCustomTagDefined<TCustomTag>(this string text) where TCustomTag : CustomTag
	{
		return CustomTag.IsDefined<TCustomTag>(text);
	}

	public static TCustomTag ToCustomTag<TCustomTag>(this string text) where TCustomTag : CustomTag
	{
		var result = text.TryToCustomTag<TCustomTag>(out var value);

		if(!result)
		{
			LogChannel.System.W($"Failed to convert {text} into CustomTag");
		}

		return value;
	}

	public static bool TryToCustomTag<TCustomTag>(this string text,out TCustomTag value) where TCustomTag : CustomTag
	{
		if(!text.IsEmpty() && CustomTag.TryParse(text,out value))
		{
			return true;
		}

		value = null;

		return false;
	}
	#endregion Convert CustomTag

	#region Convert Color
	/// <summary>
	/// HexCode to Color
	/// </summary>
	public static Color ToColor(this string hexCode)
	{
		var result = hexCode.TryToColor(out var value);

		if(!result)
		{
			LogChannel.System.W($"Failed to convert {hexCode} into color");
		}

		return value;
	}

	public static bool TryToColor(this string hexCode,out Color value)
	{
		if(!hexCode.IsEmpty())
		{
			if(hexCode.Length == 7)
			{
				hexCode = $"{hexCode}FF";
			}

			if(s_hexColorDict.TryGetValue(hexCode,out value))
			{
				return true;
			}

			if(ColorUtility.TryParseHtmlString(hexCode,out value))
			{
				s_hexColorDict.Add(hexCode,value);

				return true;
			}
		}

		value = Color.clear;

		return false;
	}

	/// <summary>
	/// Add richText
	/// </summary>
	public static string ToColorText(this string text,Color color)
	{
		return ToColorText(color.ToHexCode(),text);
	}

	/// <summary>
	/// Add richText
	/// </summary>
	public static string ToColorText(this string text,string hexCode)
	{
		return $"<color=#{hexCode}>{text}</color>";
	}
	#endregion Convert Color

	#region Convert Bool
	public static bool ToBool(this string text)
	{
		var result = text.TryToBool(out var value);

		if(!result)
		{
			LogChannel.System.W($"Failed to convert {text} into bool");
		}

		return value;
	}

	public static bool TryToBool(this string text,out bool value)
	{
		if(!text.IsEmpty() && bool.TryParse(text,out value))
		{
			return value;
		}

		var lower = text.ToLower();

		if(lower.IsEqual("t") || lower.IsEqual("1") || lower.IsEqual("yes") || lower.IsEqual("y"))
		{
			value = true;

			return true;
		}

		if(lower.IsEqual("f") || lower.IsEqual("0") || lower.IsEqual("no") || lower.IsEqual("n"))
		{
			value = false;

			return true;
		}

		value = false;

		return false;
	}
	#endregion Convert Bool

	#region Convert Number
	public static BigInteger ToBigInteger(this string text)
	{
		var result = TryToBigInteger(text,out var value);

		if(!result)
		{
			LogChannel.System.W($"Failed to convert {text} into BigInteger");
		}

		return value;
	}

	public static bool TryToBigInteger(this string text,out BigInteger bigInteger)
	{
		if(!text.IsEmpty() && BigInteger.TryParse(text,out bigInteger))
		{
			return true;
		}

		bigInteger = BigInteger.Zero;

		return false;
	}

	public static int ToInt(this string text)
	{
		var result = text.TryToInt(out var value);

		if(!result)
		{
			LogChannel.System.W($"Failed to convert {text} into int");
		}

		return value;
	}

	public static bool TryToInt(this string text,out int value)
	{
		if(!text.IsEmpty())
		{
			if(text.StartsWith("0x",StringComparison.OrdinalIgnoreCase))
			{
				value = text.ToHexInt();

				return true;
			}

			if(int.TryParse(text,out value))
			{
				return true;
			}
		}

		value = 0;

		return false;
	}

	public static float ToFloat(this string text)
	{
		var result = text.TryToFloat(out var value);

		if(!result)
		{
			LogChannel.System.W($"Failed to convert {text} into float");
		}

		return value;
	}

	public static bool TryToFloat(this string text,out float value)
	{
		if(!text.IsEmpty() && float.TryParse(text,out value))
		{
			return true;
		}

		value = 0.0f;

		return false;
	}

	public static double ToDouble(this string text)
	{
		var result = text.TryToDouble(out var value);

		if(!result)
		{
			LogChannel.System.W($"Failed to convert {text} into double");
		}

		return value;
	}

	public static bool TryToDouble(this string text,out double value)
	{
		if(!text.IsEmpty() && double.TryParse(text,out value))
		{
			return true;
		}

		value = 0.0d;

		return false;
	}

	public static byte ToByte(this string text)
	{
		var result = text.TryToByte(out var value);

		if(!result)
		{
			LogChannel.System.W($"Failed to convert {text} into byte");
		}

		return value;
	}

	public static bool TryToByte(this string text,out byte value)
	{
		if(!text.IsEmpty())
		{
			if(text.StartsWith("0x",StringComparison.OrdinalIgnoreCase))
			{
				value = Convert.ToByte(text,16);

				return true;
			}

			if(byte.TryParse(text,out value))
			{
				return true;
			}
		}

		value = 0x00;

		return false;
	}

	public static int ToHexInt(this string text)
	{
		var result = text.TryToByte(out var value);

		if(!result)
		{
			LogChannel.System.W($"Failed to convert {text} into int");
		}

		return value;
	}

	public static bool TryToHexInt(this string text,out int value)
	{
		if(!text.IsEmpty() && int.TryParse(text,NumberStyles.HexNumber,CultureInfo.CurrentCulture,out value))
		{
			return true;
		}

		value = 0;

		return false;
	}

	public static float ToHexFloat(string text)
	{
		var result = text.TryToByte(out var value);

		if(!result)
		{
			LogChannel.System.W($"Failed to convert {text} into float");
		}

		return value;
	}

	public static bool TryToHexFloat(this string text,out float value)
	{
		if(!text.IsEmpty() && uint.TryParse(text,NumberStyles.AllowHexSpecifier,CultureInfo.CurrentCulture,out var integer))
		{
			value = BitConverter.ToSingle(BitConverter.GetBytes(integer),0);

			return true;
		}

		value = 0.0f;

		return false;
	}
	#endregion Convert Number

	#region Convert DateTime
	public static DateTime ToDateTime(this string text,CultureInfo cultureInfo = null,DateTimeStyles dateTimeStyles = DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal)
	{
		var result = text.TryToDateTime(out var value,cultureInfo,dateTimeStyles);

		if(!result)
		{
			LogChannel.System.W($"Failed to convert {text} into DateTime");
		}

		return value;
	}

	public static bool TryToDateTime(this string text,out DateTime value,CultureInfo cultureInfo = null,DateTimeStyles dateTimeStyles = DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal)
	{
		var defaultCulture = cultureInfo ?? CultureInfo.InvariantCulture;

		if(!text.IsEmpty() && DateTime.TryParseExact(text,"yyyy-MM-dd HH:mm",defaultCulture,dateTimeStyles,out value))
		{
			return true;
		}

		value = DateTime.MinValue;

		return false;
	}
	#endregion Convert DateTime

	#region Convert Vector
	public static Vector2 ToVector2(this string text)
	{
		var result = text.TryToVector2(out var value);

		if(!result)
		{
			LogChannel.System.W($"Failed to convert {text} into Vector2");
		}

		return value;
	}

	public static bool TryToVector2(this string text,out Vector2 value)
	{
		if(!text.IsEmpty())
		{
			var vectorArray = _ConvertVectorArray(text);

			if(!vectorArray.IsNullOrEmpty())
			{
				value = new Vector2(_GetNumberInArray(vectorArray,0),_GetNumberInArray(vectorArray,1));

				return true;
			}
		}

		value = Vector2.zero;

		return false;
	}

	public static Vector3 ToVector3(this string text)
	{
		var result = text.TryToVector3(out var value);

		if(!result)
		{
			LogChannel.System.W($"Failed to convert {text} into Vector3");
		}

		return value;
	}

	public static bool TryToVector3(this string text,out Vector3 value)
	{
		if(!text.IsEmpty())
		{
			var vectorArray = _ConvertVectorArray(text);

			if(!vectorArray.IsNullOrEmpty())
			{
				value = new Vector3(_GetNumberInArray(vectorArray,0),_GetNumberInArray(vectorArray,1),_GetNumberInArray(vectorArray,2));

				return true;
			}
		}

		value = Vector3.zero;

		return false;
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
		if(layer == Global.INVALID_INDEX && isAutoCreate)
		{
			CommonUtility.AddLayer(layerName);

			layer = LayerMask.NameToLayer(layerName);
		}
#endif

		return layer;
	}
	#endregion Layer

	public static void ClearCache()
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

	private static float _GetNumberInArray(string[] textArray,int index)
	{
		return textArray.ContainsIndex(index) ? textArray[index].ToFloat() : 0.0f;
	}

	private static string[] _ConvertVectorArray(string text)
	{
		return text.IsEmpty() ? null : text.TrimParentheses().Split(',');
	}
}