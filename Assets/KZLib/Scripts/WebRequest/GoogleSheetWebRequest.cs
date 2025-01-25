using UnityEngine;
using UnityEngine.Networking;

namespace KZLib.KZNetwork
{
	public abstract class GoogleSheetWebRequest : BaseWebRequest
	{
		protected const string c_sheet_url = @"https://script.google.com/macros/s/AKfycbyF-dAUHHLuFf6HBU8k0FTvCX2X6ZIhVIK4KcyGm7jkU8TVyyAuC4q_aIyBbs-za2d5/exec";

		protected GoogleSheetWebRequest(string uri,string method) : base(uri,method) { }
	}

	#region Get
	public abstract class GoogleSheetGetWebRequest : GoogleSheetWebRequest
	{
		protected GoogleSheetGetWebRequest(string query) : base($"{c_sheet_url}{query}",UnityWebRequest.kHttpVerbGET) { }
	}

	public class GoogleSheetGetSheetWebRequest : GoogleSheetGetWebRequest
	{
		public static GoogleSheetGetSheetWebRequest Create(string fileId,int sheetIndex)
		{
			return new GoogleSheetGetSheetWebRequest($"?fileId={fileId}&sheetIndex={sheetIndex}");
		}

		private GoogleSheetGetSheetWebRequest(string query) : base(query) { }
	}
	#endregion Get

	#region Post
	public abstract class GoogleSheetPostWebRequest : GoogleSheetWebRequest
	{
		protected GoogleSheetPostWebRequest(byte[] data) : base(c_sheet_url,UnityWebRequest.kHttpVerbPOST)
		{
			m_webRequest.uploadHandler = new UploadHandlerRaw(data);
			m_webRequest.SetRequestHeader("Content-Type","application/x-www-form-urlencoded");
		}
	}

	public class GoogleSheetPostAddRowWebRequest : GoogleSheetPostWebRequest
	{
		public static GoogleSheetPostAddRowWebRequest Create(string sheetId,int sheetIndex,string content)
		{
			var form = new WWWForm();
			form.AddField("method","addRow");
			form.AddField("sheetId",sheetId);
			form.AddField("sheetIndex",sheetIndex);
			form.AddField("content",content);

			return new GoogleSheetPostAddRowWebRequest(form.data);
		}

		private GoogleSheetPostAddRowWebRequest(byte[] data) : base(data) { }
	}
	#endregion Post
}