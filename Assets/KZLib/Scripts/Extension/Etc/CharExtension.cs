using System.Globalization;
using System.Linq;

public static class CharExtension
{
	public static int HexToDecimal(this char _char)
	{
		return int.Parse(string.Format("{0}",_char),NumberStyles.HexNumber);
	}

	public static int HexToDecimal(this char _char,int _default)
	{
		return int.TryParse(string.Format("{0}",_char),NumberStyles.HexNumber,CultureInfo.CurrentCulture,out var num) ? num : _default;
	}

	/// <summary>
	/// 해당 문자가 포함되어 있는지 파악한다.
	/// </summary>
	public static bool EqualsLetters(this char _char,params char[] _charArray)
	{
		return _charArray.Any(x=>x == _char);
	}

	public static int FromHex(this char _char)
	{
		_char = char.ToUpperInvariant(_char);

		return _char.IsDigit() ? _char-'0' : _char.IsHexAlphabet() ? 10+_char-'A' : -1;
	}

	public static bool IsDigit(this char _char)
	{
		return _char >= '0' && _char <= '9';
	}

	public static bool IsHexAlphabet(this char _char)
	{
		return _char >= 'A' && _char <= 'F';
	}
}