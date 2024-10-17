using System;
using UnityEngine;
using UnityEngine.Networking;

namespace KZLib.KZNetwork
{
	public abstract class GoogleDriveWebRequest : BaseWebRequest
	{
		protected const string DRIVE_URL = @"https://script.google.com/macros/s/AKfycbxJeWQxhZmFYR1FtoVL8rB1wNgvvvch9mkn356aIt1HWTBnJDuv2jWYkXdF0o2bBaftpw/exec";

		protected GoogleDriveWebRequest(string _uri,string _method) : base(_uri,_method) { }
	}

	#region Get
	public abstract class GoogleDriveGetWebRequest : GoogleDriveWebRequest
	{
		protected GoogleDriveGetWebRequest(string _query) : base($"{DRIVE_URL}{_query}",UnityWebRequest.kHttpVerbGET) { }
	}

	public class GoogleDriveGetEntryWebRequest : GoogleDriveGetWebRequest
	{
		public static GoogleDriveGetEntryWebRequest Create(string _folderId)
		{
			return new GoogleDriveGetEntryWebRequest($"?folderId={_folderId}");
		}

		private GoogleDriveGetEntryWebRequest(string _query) : base(_query) { }
	}
	#endregion Get

	#region Post
	public abstract class GoogleDrivePostWebRequest : GoogleDriveWebRequest
	{
		protected GoogleDrivePostWebRequest(byte[] _data) : base(DRIVE_URL,UnityWebRequest.kHttpVerbPOST)
		{
			m_WebRequest.uploadHandler = new UploadHandlerRaw(_data);
			m_WebRequest.SetRequestHeader("Content-Type","application/x-www-form-urlencoded");
		}
	}

	public class GoogleDrivePostFileWebRequest : GoogleDrivePostWebRequest
	{
		public static GoogleDrivePostFileWebRequest Create(string _folderId,string _fileName,byte[] _content,string _mimeType)
		{
			var form = new WWWForm();
			form.AddField("method","uploadFile");
			form.AddField("folderId",_folderId);
			form.AddField("filename",_fileName);
			form.AddField("content",Convert.ToBase64String(_content));
			form.AddField("mimeType",_mimeType);

			return new GoogleDrivePostFileWebRequest(form.data);
		}

		private GoogleDrivePostFileWebRequest(byte[] _data) : base(_data) { }
	}
	#endregion Post
}