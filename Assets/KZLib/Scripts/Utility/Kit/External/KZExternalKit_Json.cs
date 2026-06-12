using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
/// Utility methods for working with third-party libraries such as JSON, UniTask, DOTween, R3, and PlayFab.
/// </summary>
public static partial class KZExternalKit
{
	/// <summary>
	/// Attempts to parse and reformat JSON text with indentation.
	/// Returns the original text when parsing fails.
	/// </summary>
	public static string PrettifyJson(string text)
	{
		try
		{
			var parsedText = JToken.Parse(text);

			return parsedText.ToString(Formatting.Indented);
		}
		catch
		{
			return text;
		}
	}
}
