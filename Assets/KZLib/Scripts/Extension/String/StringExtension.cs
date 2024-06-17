
public static partial class StringExtension
{
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
	
	/// <summary>
	/// 기본(영어) 키보드에서 입출력 가능한 모든키
	/// </summary>
	public static bool IsKeyboardKey(this string _text)
	{
		foreach(var character in _text)
		{
			if(character < 0x0020 || character > 0x007E)
			{
				return false;
			}
		}

		return true;
	}
}