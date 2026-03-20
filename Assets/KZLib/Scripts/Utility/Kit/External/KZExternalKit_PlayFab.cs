#if KZLIB_PLAY_FAB
using System.Text;
using System.Text.RegularExpressions;
using PlayFab;
using UnityEngine;

public static partial class KZExternalKit
{
	public static bool IsValidEmailAddress(string emailAddress)
	{
		if(emailAddress.IsEmpty())
		{
			return false;
		}

		var regex = new Regex(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$");

		return regex.IsMatch(emailAddress);
	}

	public static bool IsValidSize(string text,Vector2Int range)
	{
		if(text.IsEmpty())
		{
			return false;
		}

		var size = text.Length;

		return range.x <= size && size <= range.y;
	}

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