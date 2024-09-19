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
	private static readonly Dictionary<string,Color> s_HexColorDict = new();

	#region Normalize
	/// <summary>
	/// 슬래시 일반화
	/// </summary>
	public static string NormalizeNewLines(this string _text)
	{
		return _text.Replace("\\n",Environment.NewLine);
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
	#endregion Normalize

	#region Character
	/// <summary>
	/// 해당 문자가 포함되어 있는지 파악한다.
	/// </summary>
	public static bool IsContainsCharacterArray(this string _text,params char[] _characterArray)
	{
		if(_text.IsEmpty() || _characterArray.IsNullOrEmpty())
		{
			return false;
		}

		var characterSet = new HashSet<char>(_characterArray);

		return _text.Any(x => characterSet.Contains(x));
	}
	/// <summary>
	/// 첫 글자를 대문자로 변경
	/// </summary>
	public static string ToFirstCharacterToUpper(this string _text)
	{
		if(_text.IsEmpty())
		{
			return _text;
		}

		var text = char.ToUpperInvariant(_text[0]);

		return _text.Length > 1 ? $"{text}{_text[1..]}" : text.ToString();
	}

	/// <summary>
	/// 첫 글자를 소문자로 변경
	/// </summary>
	public static string ToFirstCharacterToLower(this string _text)
	{
		if(_text.IsEmpty())
		{
			return _text;
		}

		var text = char.ToLowerInvariant(_text[0]);

		return _text.Length > 1 ? $"{text}{_text[1..]}" : text.ToString();
	}

	/// <summary>
	/// 문자열의 첫 번째 문자를 count 수 만큼 가져옴
	/// </summary>
	public static string GetFirstCharacter(this string _text,int _count)
	{
		return _text.IsEmpty() || _count <= 0 ? _text : _text[..Mathf.Min(_count, _text.Length)];
	}

	/// <summary>
	/// 문자열의 끝 부분에서 count 개수 만큼 가져옵니다.
	/// </summary>
	public static string GetLastCharacter(this string _text,int _count)
	{
		return _text.IsEmpty() || _count <= 0 ? _text : _text[^(Mathf.Min(_count,_text.Length))..];
	}

	/// <summary>
	/// 문자열 내부에 character 문자가 몇 개 포함되어 있는지 반환
	/// </summary>
	public static int CountOf(this string _text,char _character)
	{
		return _text.Count(x => x == _character);
	}

	/// <summary>
	/// 문자열 내부에 character 문자가 order번째 포함되어 있는지 반환
	/// </summary>
	public static int IndexOfOrder(this string _text,char _character,int _order)
	{
		if(_text.IsEmpty())
		{
			return -1;
		}

		var index = -1;

		for(var i=0;i<_order;i++)
		{
			index = _text.IndexOf(_character,index+1);

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
	/// includeSpace가 true면 공백도 없는걸로 판단함
	/// </summary>
	/// <param name="_text"></param>
	public static bool IsEmpty(this string _text,bool _includeSpace = false)
	{
		return _includeSpace ? string.IsNullOrWhiteSpace(_text) : string.IsNullOrEmpty(_text);
	}

	/// <summary>
	/// 스트링 전용 비교
	/// </summary>
	public static bool IsEqual(this string _text1,string _text2)
	{
		return string.Equals(_text1,_text2);
	}

	/// <summary>
	/// index에서 match가 일치하는지 확인합니다.
	/// </summary>
	public static bool IsMatchAt(this string _text,int _index,string _match,bool _ignoreCase = false)
	{
		if(_text.IsEmpty() || _match.IsEmpty() || _index < 0 || _index+_match.Length > _text.Length)
		{
			return false;
		}

		return _ignoreCase ? string.Equals(_text.Substring(_index,_match.Length),_match,StringComparison.OrdinalIgnoreCase) : _text.Substring(_index,_match.Length) == _match;
	}
	#endregion Compare

	#region Convert Enum
	public static bool IsEnumDefined<TEnum>(this string _text)
	{
		return Enum.IsDefined(typeof(TEnum),_text);
	}

	public static TEnum ToEnum<TEnum>(this string _text,TEnum _default = default) where TEnum : struct
	{
		return !_text.IsEmpty() && Enum.TryParse(_text,true,out TEnum data) ? data : _default;
	}
	#endregion Convert Enum

	#region Convert Color
	/// <summary>
	/// 16진수 색상 코드를 UnityEngine.Color로 변환합니다.
	/// </summary>
	public static Color ToColor(this string _hexCode)
	{
		if(_hexCode.Length == 7)
		{
			_hexCode = string.Format("{0}FF",_hexCode);
		}

		if(s_HexColorDict.TryGetValue(_hexCode,out var color))
		{
			return color;
		}

		if(ColorUtility.TryParseHtmlString(_hexCode,out color))
		{
			s_HexColorDict.Add(_hexCode,color);

			return color;
		}

		return Color.clear;
	}

	/// <summary>
	/// color으로 문자 색상을 변경합니다.
	/// </summary>
	public static string ToColorText(this string _text,string _color)
	{
		return string.Format("<color=#{0}>{1}</color>",_color,_text);
	}

	/// <summary>
	/// color으로 문자 색상을 변경합니다.
	/// </summary>
	public static string ToColorText(this string _text,Color _color)
	{
		return ToColorText(ColorUtility.ToHtmlStringRGBA(_color),_text);
	}
	#endregion Convert Color

	#region Convert Number
	/// <summary>
	/// 문자열을 BigInteger로 변환합니다.
	/// </summary>
	public static BigInteger ToBigInteger(this string _text,BigInteger _default = default)
	{
		if(_text.IsEmpty())
		{
			return _default;
		}

		return BigInteger.TryParse(_text,out var result) ? result : _default;
	}

	/// <summary>
	/// 문자열을 int로 변환합니다.
	/// </summary>
	public static int ToInt(this string _text,int _default = 0)
	{
		if(_text.IsEmpty())
		{
			return _default;
		}

		if(_text.StartsWith("0x",StringComparison.OrdinalIgnoreCase))
		{
			return _text.ToHexInt(_default);
		}

		return int.TryParse(_text,out var num) ? num : _default;
	}

	/// <summary>
	/// 문자열을 float로 변환합니다.
	/// </summary>
	public static float ToFloat(this string _text,float _default = 0.0f)
	{
		return float.TryParse(_text,out var num) ? num : _default;
	}

	/// <summary>
	/// 문자열을 double로 변환합니다.
	/// </summary>
	public static double ToDouble(this string _text,double _default = 0.0d)
	{
		return double.TryParse(_text,out var num) ? num : _default;
	}

	/// <summary>
	/// 문자열을 byte로 변환합니다.
	/// </summary>
	public static byte ToByte(this string _text,byte _default = 0x00)
	{
		if(_text.StartsWith("0x",StringComparison.OrdinalIgnoreCase))
		{
			return Convert.ToByte(_text,16);
		}

		return byte.TryParse(_text,out var num) ? num : _default;
	}

	/// <summary>
	/// 문자열을 16진수 정수로 변환합니다.
	/// </summary>
	public static int ToHexInt(this string _hexText,int _default = 0)
	{
		return int.TryParse(_hexText,NumberStyles.HexNumber,CultureInfo.CurrentCulture,out var num) ? num : _default;
	}

	/// <summary>
	/// 문자열을 16진수 실수로 변환합니다.
	/// </summary>
	public static float ToHexFloat(string _hexText,float _default = 0.0f)
	{
		return uint.TryParse(_hexText,NumberStyles.AllowHexSpecifier,CultureInfo.CurrentCulture,out var num) ? BitConverter.ToSingle(BitConverter.GetBytes(num),0) : _default;
	}

	/// <summary>
	/// 배열에서 인덱스를 찾아 해당 위치의 문자열을 실수로 변환하여 반환합니다.
	/// </summary>
	private static float GetNumberInArray(string[] _textArray,int _index)
	{
		return _textArray.ContainsIndex(_index) ? _textArray[_index].ToFloat() : 0.0f;
	}
	#endregion Convert Number

	#region Convert DateTime
	/// <summary>
	/// 문자열을 DateTime으로 변환합니다.
	/// </summary>
	public static DateTime ToDateTime(this string _text,CultureInfo _cultureInfo = null, DateTimeStyles _styles = DateTimeStyles.AdjustToUniversal)
	{
		var cultureInfo = _cultureInfo ?? CultureInfo.CreateSpecificCulture("ko-KR");

		return DateTime.ParseExact(_text,"yyyy-MM-dd HH:mm",cultureInfo,_styles);
	}
	#endregion Convert DateTime

	#region Convert Vector
	/// <summary>
	/// 문자열을 Vector2로 변환합니다.
	/// </summary>
	public static Vector2 ToVector2(this string _text)
	{
		return _text.ToVector2(Vector2.zero);
	}

	/// <summary>
	/// 문자열을 Vector2로 변환합니다.
	/// </summary>
	public static Vector2 ToVector2(this string _text,Vector2 _default)
	{
		return _text.TryToVector2(out var result) ? result : _default;
	}

	/// <summary>
	/// 문자열을 Vector2로 변환합니다.
	/// </summary>
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

	/// <summary>
	/// 문자열을 Vector3로 변환합니다.
	/// </summary>
	public static Vector3 ToVector3(this string _text)
	{
		return _text.ToVector3(Vector3.zero);
	}

		/// <summary>
	/// 문자열을 Vector3로 변환합니다.
	/// </summary>
	public static Vector3 ToVector3(this string _text,Vector3 _default)
	{
		return _text.TryToVector3(out var result) ? result : _default;
	}

	/// <summary>
	/// 문자열을 Vector3로 변환합니다.
	/// </summary>
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

	/// <summary>
	/// 문자열을 Vector로 변환하는 메서드에 사용되는 문자열을 변환합니다.
	/// </summary>
	private static string[] ConvertVectorArray(string _text)
	{
		if(_text.IsEmpty())
		{
			return null;
		}

		return _text.TrimParentheses().Split(',');
	}
	#endregion Convert Vector

	#region Remove
	/// <summary>
	/// 문자열의 시작 부분에서 지정된 문자열을 제거합니다.
	/// </summary>
	public static string RemoveStart(this string _text,string _remove)
	{
		if(_text.IsEmpty() || _remove.IsEmpty())
		{
			return _text;
		}

		return _text.StartsWith(_remove) ? _text[_remove.Length..] : _text;
	}

	/// <summary>
	/// 문자열의 끝 부분에서 지정된 문자열을 제거합니다.
	/// </summary>
	public static string RemoveEnd(this string _text,string _remove)
	{
		if(_text.IsEmpty() || _remove.IsEmpty())
		{
			return _text;
		}

		return _text.EndsWith(_remove) ? _text[..^_remove.Length] : _text;
	}

	/// <summary>
	/// 문자열의 시작 부분에서 count 개수 만큼 제거합니다.
	/// </summary>
	public static string RemoveFirstCharacter(this string _text,int _count)
	{
		if(_text.IsEmpty() || _count <= 0)
		{
			return _text;
		}

		return _count >= _text.Length ? string.Empty : _text[_count..];
	}

	/// <summary>
	/// 문자열의 끝 부분에서 count 개수 만큼 제거합니다.
	/// </summary>
	public static string RemoveLastCharacter(this string _text,int _count)
	{
		if(_text.IsEmpty() || _count <= 0)
		{
			return _text;
		}

		return _count >= _text.Length ? string.Empty : _text[..^_count];
	}

	/// <summary>
	/// 문자열에서 리치 텍스트를 제거합니다.
	/// </summary>
	public static string RemoveRichText(this string _text)
	{
		return Regex.Replace(_text,"<.*?>",string.Empty);
	}
	#endregion Remove

	#region Trim
	/// <summary>
	/// trim만큼 앞에서 부터 제거
	/// </summary>
	public static string TrimTextStart(this string _text,string _trim)
	{
		if(_text.IsEmpty() || _trim.IsEmpty())
		{
			return _text;
		}

		while(_text.StartsWith(_trim))
		{
			_text = _text[_trim.Length..];
		}

		return _text;
	}

	/// <summary>
	/// trim만큼 뒤에서 부터 제거
	/// </summary>
	public static string TrimTextEnd(this string _text,string _trim)
	{
		if(_text.IsEmpty() || _trim.IsEmpty())
		{
			return _text;
		}

		while(_text.EndsWith(_trim))
		{
			_text = _text[..^_trim.Length];
		}

		return _text;
	}

	/// <summary>
	/// 빈칸 전부 제거
	/// </summary>
	public static string TrimAllSpace(this string _text)
	{
        return _text.IsEmpty() ? _text : _text.Replace(" ",string.Empty);
	}

	/// <summary>
	/// 모든 괄호 제거
	/// </summary>
	public static string TrimBrackets(this string _text)
	{
		return _text.Trim('(',')','[',']','{','}','<','>');
	}
	/// <summary>
	/// 소괄호 제거
	/// </summary>
	public static string TrimParentheses(this string _text)
	{
		return _text.Trim('(',')');
	}
	/// <summary>
	/// 중괄호 제거
	/// </summary>
	public static string TrimBraces(this string _text)
	{
		return _text.Trim('{','}');
	}
	/// <summary>
	/// 대괄호 제거
	/// </summary>
	public static string TrimSquareBrackets(this string _text)
	{
		return _text.Trim('[',']');
	}
	/// <summary>
	/// 화살괄호 제거
	/// </summary>
	public static string TrimAngleBrackets(this string _text)
	{
		return _text.Trim('<','>');
	}
	#endregion Trim

	#region Wrap
	/// <summary>
	/// 소괄호 래핑
	/// </summary>
	public static string WrapParentheses(this string _text)
	{
		return $"({_text})";
	}
	/// <summary>
	/// 중괄호 래핑
	/// </summary>
	public static string WrapBraces(this string _text)
	{
		return $"{{{_text}}}";
	}
	/// <summary>
	/// 대괄호 래핑
	/// </summary>
	public static string WrapSquareBrackets(this string _text)
	{
		return $"[{_text}]";
	}
	/// <summary>
	/// 화살괄호 래핑
	/// </summary>
	public static string WrapAngleBrackets(this string _text)
	{
		return $"<{_text}>";
	}
	#endregion Wrap

	#region Split
	public static string[] SplitTextByBrackets(this string _text,StringSplitOptions _options = StringSplitOptions.None)
	{
		return _text.Split(new char[] { '(', ')', '[', ']', '{', '}', '<', '>' }, _options);
	}

	public static string[] SplitTextByParentheses(this string _text,StringSplitOptions _options = StringSplitOptions.None)
	{
		return _text.Split(new char[] { '(', ')' }, _options);
	}

	public static string[] SplitTextByBraces(this string _text,StringSplitOptions _options = StringSplitOptions.None)
	{
		return _text.Split(new char[] { '{', '}' }, _options);
	}

	public static string[] SplitTextBySquareBrackets(this string _text,StringSplitOptions _options = StringSplitOptions.None)
	{
		return _text.Split(new char[] { '[', ']' }, _options);
	}

	public static string[] SplitTextByAngleBrackets(this string _text,StringSplitOptions _options = StringSplitOptions.None)
	{
		return _text.Split(new char[] { '<', '>' }, _options);
	}
	#endregion Split

	#region Extract
	/// <summary>
	/// 문자를 앞뒤 문자를 이용하여 자른다.
	/// </summary>
	public static string ExtractData(this string _text,char _startMark,char _endMark)
	{
		var startIdx = _text.IndexOf(_startMark);
		var endIdx = _text.LastIndexOf(_endMark);

		return (startIdx == -1 || endIdx == -1 || startIdx > endIdx) ? _text : _text.Substring(startIdx+1,endIdx-startIdx-1);
	}

	/// <summary>
	/// 문자를 앞뒤 문자를 이용하여 자른다.
	/// </summary>
	public static string ExtractData(this string _text,string _startText,string _endText)
	{
		var start = _text.RemoveStart(_startText);

		if(_text.IsEqual(start))
		{
			return _text;
		}

		var end = start.RemoveEnd(_endText);

		return start.IsEqual(end) ? _text : end;
	}

	/// <summary>
	/// 문자를 앞뒤 문자를 이용하여 자른다.
	/// </summary>
	public static string ExtractData(this string _text,char _startMark,string _endText)
	{
		var index = _text.IndexOf(_startMark);

		if(index == -1)
		{
			return _text;
		}

		var start = _text[..index];
		var end = start.RemoveEnd(_endText);

		return start.IsEqual(end) ? _text : end;
	}

	/// <summary>
	/// 문자를 앞뒤 문자를 이용하여 자른다.
	/// </summary>
	public static string ExtractData(this string _text,string _startText,char _endMark)
	{
		var start = _text.RemoveStart(_startText);

		if(_text.IsEqual(start))
		{
			return _text;
		}

		var index = start.LastIndexOf(_endMark);

		return index == -1 ? _text : start[..index];
	}

	/// <summary>
	/// 문자만 남긴다.
	/// </summary>
	public static string ExtractOnlyCharacter(this string _text)
	{
        return _text.IsEmpty() ? null : Regex.Replace(_text,@"\d",string.Empty);
    }

	/// <summary>
	/// 숫자만 남긴다.
	/// </summary>
	public static string ExtractOnlyNumber(this string _text)
	{
		if(_text.IsEmpty())
		{
			return null;
		}

		var match = Regex.Match(_text,@"\d+");

		return match.Success ? match.Value : string.Empty;
	}

	public static int ExtractOnlyNumberToInt(this string _text,int _default)
	{
		return ExtractOnlyNumber(_text).ToInt(_default);
	}
	#endregion Extract

	#region Layer
	public static int GetLayerByName(this string _layerName,bool _autoCreate = false)
	{
		var layer = LayerMask.NameToLayer(_layerName);

#if UNITY_EDITOR
		if(layer == -1 && _autoCreate)
		{
			UnityUtility.AddLayer(_layerName);

			layer = LayerMask.NameToLayer(_layerName);
		}
#endif

		return layer;
	}
	#endregion Layer

	/// <summary>
	/// 캐시된 색상 데이터를 모두 삭제합니다.
	/// </summary>
	public static void ClearCacheData()
	{
		s_HexColorDict.Clear();
	}

	public static string XmlToJson(this string _xml)
	{
		using var stream = new MemoryStream(Encoding.UTF8.GetBytes(_xml));
		var xml = new XmlDocument();
		xml.Load(stream);

		return JsonConvert.SerializeXmlNode(xml);
	}
}