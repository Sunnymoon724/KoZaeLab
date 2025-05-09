#if KZLIB_PLAY_FAB
using System.Text;
using System.Text.RegularExpressions;
using PlayFab;
using UnityEngine;

public static partial class CommonUtility
{
	public static bool IsValidEmailAddress(string _emailAddress)
	{
		if(_emailAddress.IsEmpty())
		{
			return false;
		}

		var regex = new Regex(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$");

		return regex.IsMatch(_emailAddress);
	}

	public static bool IsValidSize(string _text,Vector2Int _range)
	{
		if(_text.IsEmpty())
		{
			return false;
		}

		var size = _text.Length;

		return _range.x <= size && size <= _range.y;
	}

	public static string GetErrorMessage(PlayFabError _playFabError)
	{
		var builder = new StringBuilder();

		if(!_playFabError.ErrorDetails.IsNullOrEmpty())
		{
			foreach(var details in _playFabError.ErrorDetails)
			{
				builder.AppendFormat("{0} : {1}",details.Key,string.Join("/",details.Value));
			}
		}

		return string.Format("{0}/[{1}]",_playFabError.Error,builder.ToString());
	}
}
#endif