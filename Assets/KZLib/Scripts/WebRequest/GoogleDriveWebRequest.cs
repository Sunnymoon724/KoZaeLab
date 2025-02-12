using System;
using UnityEngine;
using UnityEngine.Networking;

namespace KZLib.KZNetwork
{
	public abstract class GoogleDriveWebRequest : BaseWebRequest
	{
		protected const string c_drive_url = @"https://script.google.com/macros/s/AKfycbxJeWQxhZmFYR1FtoVL8rB1wNgvvvch9mkn356aIt1HWTBnJDuv2jWYkXdF0o2bBaftpw/exec";

		protected GoogleDriveWebRequest(string uri,string method) : base(uri,method) { }
	}

	#region Get
	public abstract class GoogleDriveGetWebRequest : GoogleDriveWebRequest
	{
		protected GoogleDriveGetWebRequest(string query) : base($"{c_drive_url}{query}",UnityWebRequest.kHttpVerbGET) { }
	}

	public class GoogleDriveGetEntryWebRequest : GoogleDriveGetWebRequest
	{
		public static GoogleDriveGetEntryWebRequest Create(string folderId)
		{
			return new GoogleDriveGetEntryWebRequest($"?folderId={folderId}");
		}

		private GoogleDriveGetEntryWebRequest(string query) : base(query) { }
	}
	#endregion Get

	#region Post
	public abstract class GoogleDrivePostWebRequest : GoogleDriveWebRequest
	{
		protected GoogleDrivePostWebRequest(byte[] data) : base(c_drive_url,UnityWebRequest.kHttpVerbPOST)
		{
			m_webRequest.uploadHandler = new UploadHandlerRaw(data);
			m_webRequest.SetRequestHeader("Content-Type","application/x-www-form-urlencoded");
		}
	}

	public class GoogleDrivePostFileWebRequest : GoogleDrivePostWebRequest
	{
		public static GoogleDrivePostFileWebRequest Create(string folderId,string fileName,byte[] content,string mimeType)
		{
			var form = new WWWForm();
			form.AddField("method","uploadFile");
			form.AddField("folderId",folderId);
			form.AddField("filename",fileName);
			form.AddField("content",Convert.ToBase64String(content));
			form.AddField("mimeType",mimeType);

			return new GoogleDrivePostFileWebRequest(form.data);
		}

		private GoogleDrivePostFileWebRequest(byte[] data) : base(data) { }
	}
	#endregion Post
}