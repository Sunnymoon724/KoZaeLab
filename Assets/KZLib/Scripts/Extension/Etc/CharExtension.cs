using System.Globalization;

public static class CharExtension
{
	public static int HexToDecimal(this char character)
	{
		return int.Parse(character.ToString(),NumberStyles.HexNumber);
	}

	public static int HexToDecimal(this char character,int defaultNumber)
	{
		return int.TryParse(character.ToString(),NumberStyles.HexNumber,CultureInfo.CurrentCulture,out var number) ? number : defaultNumber;
	}

	public static bool IsEqualLetter(this char character,params char[] characterArray)
	{
		foreach(var letter in characterArray)
		{
			if(letter == character)
			{
				return true;
			}
		}

		return false;
	}

	public static int FromHex(this char character)
	{
		character = char.ToUpperInvariant(character);

		// check number
		if(character >= '0' && character <= '9')
		{
			return character-'0';
		}

		// check hex number
		if (character >= 'A' && character <= 'F')
		{
			return 10+(character-'A');
		}

		return -1;
	}
}