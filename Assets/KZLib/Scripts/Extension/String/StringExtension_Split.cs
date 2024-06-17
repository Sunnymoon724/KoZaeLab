﻿using System.Collections.Generic;
using System.Text.RegularExpressions;

public static partial class StringExtension
{
	public static string RemoveStart(this string _text,string _remove)
	{
		return _text.StartsWith(_remove) ? _text.Remove(_text.IndexOf(_remove),_remove.Length) : _text;
	}

	public static string RemoveEnd(this string _text,string _remove)
	{
		return _text.EndsWith(_remove) ? _text.Remove(_text.LastIndexOf(_remove)) : _text;
	}

	public static string GetFirstChar(this string _text,int _count)
	{
		return _count >= _text.Length ? _text : _text[.._count];
	}

	public static string RemoveFirstChar(this string _text,int _count)
	{
		return _count >= _text.Length ? string.Empty : _text[_count..];
	}

	public static string GetLastChar(this string _text,int _count)
	{
		return _count >= _text.Length ? _text : _text[^_count..];
	}

	public static string RemoveLastChar(this string _text,int _count)
	{
		return _count >= _text.Length ? string.Empty : _text.Remove(_text.Length-_count);
	}

	public static string TrimTextStart(this string _text,string _trim)
	{
		if(_trim.IsEmpty())
		{
			return _text;
		}

		while(_text.StartsWith(_trim))
		{
			_text = _text[_trim.Length..];
		}

		return _text;
	}

	public static string TrimTextEnd(this string _text,string _trim)
	{
		if(_trim.IsEmpty())
		{
			return _text;
		}
		
		while(_text.EndsWith(_trim))
		{
			_text = _text[..^_trim.Length];
		}

		return _text;
	}

	public static string TrimAllSpace(this string _text)
	{
		return _text.Trim().Replace(" ","");
	}

	public static string TrimParenthesis(this string _text)
	{
		return _text.Trim('(',')');
	}
	public static string TrimBrackets(this string _text)
	{
		return _text.Trim('[',']');
	}
	public static string TrimBraces(this string _text)
	{
		return _text.Trim('{','}');
	}

	public static string TrimChevrons(this string _text)
	{
		return _text.Trim('<','>');
	}

	public static string[] SplitTextParenthesis(this string _text)
	{
		return SplitText(_text,'(',')');
	}

	public static string[] SplitTextBrackets(this string _text)
	{
		return SplitText(_text,'[',']');
	}
	
	public static string[] SplitTextBraces(this string _text)
	{
		return SplitText(_text,'{','}');
	}

	public static string[] SplitTextChevrons(this string _text)
	{
		return SplitText(_text,'<','>');
	}

	private static string[] SplitText(this string _text,char _start,char _close)
	{
		var textList = new List<string>();
		var start = _text.IndexOf(_start);
		var close = _text.IndexOf(_close);

		while(start != -1 || close != -1)
		{
			textList.Add(_text.Substring(start+1,close-1));
			
			start = _text.IndexOf(_start,close+1);
			close = _text.IndexOf(_close,close+1);
		}

		return textList.ToArray();
	}

	public static string RemoveRichText(this string _text)
	{
		return Regex.Replace(_text,"<.*?>",string.Empty);
	}

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
}