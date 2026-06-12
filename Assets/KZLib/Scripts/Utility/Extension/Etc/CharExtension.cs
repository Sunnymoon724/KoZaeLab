using System.Globalization;

/// <summary>
/// Extension methods for char.
/// Provides hex conversion and character comparison helpers.
/// </summary>
public static class CharExtension
{
	public static int HexToDecimal(this char character)
	{
		if(!int.TryParse(character.ToString(),NumberStyles.HexNumber,CultureInfo.CurrentCulture,out var number))
		{
			LogChannel.Kit.E($"Failed to convert '{character}' into decimal.");
		}

		return number;
	}

	public static int HexToDecimal(this char character,int defaultNumber)
	{
		return int.TryParse(character.ToString(),NumberStyles.HexNumber,CultureInfo.CurrentCulture,out var number) ? number : defaultNumber;
	}

	public static bool IsEqualLetter(this char character,params char[] characterArray)
	{
		for(var i=0;i<characterArray.Length;i++)
		{
			if(characterArray[i] == character)
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Converts a single hex digit to its numeric value without parsing overhead.
	/// </summary>
	/// <returns>0-15 for valid digits, or -1 when the character is not hex.</returns>
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
			return Global.HexLetterOffset+(character-'A');
		}

		return -1;
	}
}
