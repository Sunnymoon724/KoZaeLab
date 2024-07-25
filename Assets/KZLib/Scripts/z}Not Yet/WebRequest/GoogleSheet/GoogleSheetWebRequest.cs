namespace KZLib.KZNetwork
{
	public abstract class GoogleSheetWebRequest : BaseWebRequest
	{
		protected const string URL = @"https://script.google.com/macros/s/AKfycbwloic4is9JujQIh041RarXfsTrCyeKFk4P9a-jrBANVgsOEr4BeHmGuZAcP3kKuX2d/exec";

		protected GoogleSheetWebRequest(string _uri,string _method) : base(_uri,_method) { }
	}
}