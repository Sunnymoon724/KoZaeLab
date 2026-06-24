using System;
using UnityEngine;
using UnityEngine.Networking;

namespace KZLib.Webhooks
{
	/// <summary>
	/// Base request type for Google Apps Script endpoints that manage Sheets and Drive.
	/// </summary>
	public abstract class GoogleWebRequest : BaseWebRequest
	{
		// protected const string URL = @"https://script.google.com/macros/s/AKfycbwloic4is9JujQIh041RarXfsTrCyeKFk4P9a-jrBANVgsOEr4BeHmGuZAcP3kKuX2d/exec";

		protected GoogleWebRequest(string name,string uri,string method) : base(name,uri,method) { }
	}

	public abstract class GetGoogleWebRequest : GoogleWebRequest
	{
		protected GetGoogleWebRequest(string name,string url,string query) : base(name,$"{url}{query}",UnityWebRequest.kHttpVerbGET) { }
	}

	/// <summary>
	/// Reads a Google Sheet tab by file id and sheet index.
	/// </summary>
	public class GetGoogleSheetWebRequest : GetGoogleWebRequest
	{
		public static GetGoogleSheetWebRequest Create(string url,string fileId,int sheetIndex)
		{
			return new GetGoogleSheetWebRequest(url,$"?fileId={fileId}&sheetIndex={sheetIndex}");
		}

		private GetGoogleSheetWebRequest(string url,string query) : base("Get GoogleSheet",url,query) { }
	}

	/// <summary>
	/// Lists entries inside a Google Drive folder.
	/// </summary>
	public class GetGoogleDriveEntryWebRequest : GetGoogleWebRequest
	{
		public static GetGoogleDriveEntryWebRequest Create(string url,string folderId)
		{
			return new GetGoogleDriveEntryWebRequest(url,$"?folderId={folderId}");
		}

		private GetGoogleDriveEntryWebRequest(string url,string query) : base("Get GoogleDriveEntry",url,query) { }
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public abstract class PostGoogleWebRequest : GoogleWebRequest
	{
		protected PostGoogleWebRequest(string name,string url,byte[] rawData) : base(name,url,UnityWebRequest.kHttpVerbPOST)
		{
			_CreateUploadHandler(rawData,"application/x-www-form-urlencoded");
		}
	}

	/// <summary>
	/// Appends a row to a Google Sheet through the Apps Script endpoint.
	/// </summary>
	public class PostGoogleSheetAddRowWebRequest : PostGoogleWebRequest
	{
		public static PostGoogleSheetAddRowWebRequest Create(string url,string sheetId,int sheetIndex,string content)
		{
			var form = new WWWForm();
			form.AddField("method","addRow");
			form.AddField("sheetId",sheetId);
			form.AddField("sheetIndex",sheetIndex);
			form.AddField("content",content);

			return new PostGoogleSheetAddRowWebRequest(url,form.data);
		}

		private PostGoogleSheetAddRowWebRequest(string url,byte[] rawData) : base("Post GoogleSheetAddRow",url,rawData) { }
	}

	/// <summary>
	/// Uploads a file into a Google Drive folder through the Apps Script endpoint.
	/// </summary>
	public class PostGoogleDriveFileWebRequest : PostGoogleWebRequest
	{
		public static PostGoogleDriveFileWebRequest Create(string url,string folderId,string fileName,byte[] content,string mimeType)
		{
			var form = new WWWForm();
			form.AddField("method","uploadFile");
			form.AddField("folderId",folderId);
			form.AddField("filename",fileName);
			form.AddField("content",Convert.ToBase64String(content));
			form.AddField("mimeType",mimeType);

			return new PostGoogleDriveFileWebRequest(url,form.data);
		}

		private PostGoogleDriveFileWebRequest(string url,byte[] rawData) : base("Post GoogleDriveFile",url,rawData) { }
	}
}