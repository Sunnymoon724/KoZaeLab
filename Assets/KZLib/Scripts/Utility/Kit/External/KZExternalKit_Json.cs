using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static partial class KZExternalKit
{
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