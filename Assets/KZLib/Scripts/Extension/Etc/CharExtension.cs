using System.Globalization;

public static class CharExtension
{
	public static int HexToDecimal(this char _character)
	{
		return int.Parse(_character.ToString(),NumberStyles.HexNumber);
	}

	public static int HexToDecimal(this char _character,int _default)
	{
		return int.TryParse(_character.ToString(),NumberStyles.HexNumber,CultureInfo.CurrentCulture,out var number) ? number : _default;
	}

	public static bool EqualsLetters(this char _character,params char[] _characterArray)
	{
		foreach(var character in _characterArray)
		{
			if(character == _character)
			{
				return true;
			}
		}

		return false;
	}

	public static int FromHex(this char _character)
	{
		_character = char.ToUpperInvariant(_character);

		return _character.IsNumber() ? _character-'0' : _character.IsAlphabet() ? 10+_character-'A' : -1;
	}

	public static bool IsNumber(this char _character)
	{
		return _character >= '0' && _character <= '9';
	}

	public static bool IsAlphabet(this char _character)
	{
		return _character >= 'A' && _character <= 'F';
	}
}