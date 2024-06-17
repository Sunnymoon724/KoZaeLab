
using System;

public static partial class StringExtension
{
	/// <summary>
	/// _includeSpace가 true면 공백도 없는걸로 판단함
	/// </summary>
	/// <param name="_text"></param>
	public static bool IsEmpty(this string _text)
	{
		return string.IsNullOrEmpty(_text);
	}

	public static bool IsEqual(this string _text1,string _text2)
	{
		return string.Equals(_text1,_text2);
	}

	public static bool NotEqual(this string _text1,string _text2)
	{
		return !_text1.IsEqual(_text2);
	}

	public static bool AttemptMatch(this string _text,int _index,string _match,bool _ignoreCase = false)
	{
		if(_text.IsEmpty() || _match.IsEmpty())
		{
			return false;
		}

		if(_index < 0 || _index+_match.Length > _text.Length)
		{
			return false;
		}

		for(var i=0;i<_match.Length;i++)
		{
			if(_ignoreCase)
			{
				if(char.ToUpperInvariant(_text[_index+i]) != char.ToUpperInvariant(_match[i]))
				{
					return false;
				}
			}
			else
			{
				if(_text[_index+i] != _match[i])
				{
					return false;
				}
			}
		}

		return true;
	}
}