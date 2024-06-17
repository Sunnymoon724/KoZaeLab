using System;

namespace KZLib.KZNetwork
{
	public abstract class GoogleDriveWebRequest : BaseWebRequest
	{
		protected const string URL = @"https://script.google.com/macros/s/AKfycbw_WZNfyVEuerpqEpViunDmxLMWJ22_VI2FomzYlshOBGrI9TW5upler9cKPy2KxApoHg/exec";

		protected GoogleDriveWebRequest(string _uri,string _method) : base(_uri,_method) { }
	}
}