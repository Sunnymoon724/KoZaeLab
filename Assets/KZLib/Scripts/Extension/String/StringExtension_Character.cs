using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

public static partial class StringExtension
{
	public static string ExtractOnlyChar(this string _text)
	{
		return Regex.Replace(_text,@"\d",string.Empty);
	}

	public static string ExtractOnlyDigit(this string _text)
	{
		var match = Regex.Match(_text,@"\d+");

		return match.Success ? match.Value : string.Empty;
	}

	public static int ExtractOnlyDigitToInt(this string _text,int _default)
	{
		return ExtractOnlyDigit(_text).ToInt(_default);
	}

	/// <summary>
	/// 해당 문자가 포함되어 있는지 파악한다.
	/// </summary>
	public static bool IsContainsCharArray(this string _text,params char[] _charArray)
	{
		return _charArray.Any(_text.Contains);
	}

	public static int CountOf(this string _text,char _char)
	{
		return _text.Count(x=> x == _char);
	}

	public static int IndexOfOrder(this string _text,char _char,int _order)
	{
		var index = -1;
		for(var i=0;i<_order;i++)
		{
			index = _text.IndexOf(_char,index+1);

			if(index == -1)
			{
				break;
			}
		}
		return index;
	}

	public static string NormalizeNewLines(this string _text)
	{
		return _text.Replace("\\n",Environment.NewLine);
	}
}