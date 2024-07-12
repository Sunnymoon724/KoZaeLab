using System;

public static partial class StringExtension
{
	/// <summary>
	/// 해당 문자가 포함되어 있는지 파악한다.
	/// </summary>
	public static bool IsContainsCharArray(this string _text,params char[] _characterArray)
	{
		if(_text.IsEmpty() || _characterArray.IsNullOrEmpty())
		{
			return false;
		}

		foreach(var character in _characterArray)
		{
			if(_text.Contains(character))
			{
				return true;
			}
		}

		return false;
	}

	public static int CountOf(this string _text,char _character)
	{
		if(_text.IsEmpty())
		{
			return -1;
		}

		var count = 0;

		foreach(var character in _text)
		{
			if(character == _character)
			{
				count++;
			}
		}

		return count;
	}

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
				break;
			}
		}
		return index;
	}

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

	public static string NormalizeNewLines(this string _text)
	{
		return _text.Replace("\\n",Environment.NewLine);
	}
}