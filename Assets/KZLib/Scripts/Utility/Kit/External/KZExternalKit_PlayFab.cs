#if KZLIB_PLAY_FAB
using System.Text;
using System.Text.RegularExpressions;
using PlayFab;
using UnityEngine;

public static partial class KZExternalKit
{
	private static readonly Regex s_emailRegex = new Regex(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$",RegexOptions.Compiled);

	/// <summary>
	/// Returns whether the given string is a valid email address format.
	/// </summary>
	public static bool IsValidEmailAddress(string emailAddress)
	{
		if(emailAddress.IsEmpty())
		{
			return false;
		}

		return s_emailRegex.IsMatch(emailAddress);
	}

	/// <summary>
	/// Returns whether the text length falls within the inclusive range defined by range.x and range.y.
	/// </summary>
	public static bool IsValidSize(string text,Vector2Int range)
	{
		if(text.IsEmpty())
		{
			return false;
		}

		var size = text.Length;

		return range.x <= size && size <= range.y;
	}

	/// <summary>
	/// Formats a PlayFabError into a readable string including error details when available.
	/// </summary>
	public static string GetErrorMessage(PlayFabError playFabError)
	{
		var builder = new StringBuilder();

		if(!playFabError.ErrorDetails.IsNullOrEmpty())
		{
			foreach(var details in playFabError.ErrorDetails)
			{
				builder.AppendFormat("{0} : {1}",details.Key,string.Join("/",details.Value));
			}
		}

		return string.Format("{0}/[{1}]",playFabError.Error,builder.ToString());
	}
}
#endif
