/// <summary>
/// Extension methods for char.
/// Provides hex conversion and character comparison helpers.
/// </summary>
public static class CharExtension
{
	/// <summary>
	/// Converts a single hex digit to 0-15, or -1 when the character is not a hex digit.
	/// </summary>
	public static int FromHexDigit(this char character)
	{
		character = char.ToUpperInvariant(character);

		if(character >= '0' && character <= '9')
		{
			return character-'0';
		}

		if(character >= 'A' && character <= 'F')
		{
			return Global.HexLetterOffset+(character-'A');
		}

		return -1;
	}

	/// <summary>
	/// Converts a single hex digit to 0-15, returning <paramref name="defaultNumber"/> when invalid.
	/// </summary>
	public static int FromHexDigit(this char character,int defaultNumber)
	{
		var value = character.FromHexDigit();

		return value < 0 ? defaultNumber : value;
	}

	/// <summary>
	/// Returns whether the character matches any value in <paramref name="characterArray"/>.
	/// </summary>
	public static bool EqualsAny(this char character,params char[] characterArray)
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
}