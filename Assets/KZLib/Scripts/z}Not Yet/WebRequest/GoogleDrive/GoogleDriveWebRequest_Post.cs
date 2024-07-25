using System;
using UnityEngine;
using UnityEngine.Networking;

namespace KZLib.KZNetwork
{
	public abstract class GoogleDrivePostWebRequest : GoogleDriveWebRequest
	{
		protected GoogleDrivePostWebRequest(byte[] _data) : base(URL,UnityWebRequest.kHttpVerbPOST)
		{
			m_WebRequest.uploadHandler = new UploadHandlerRaw(_data);
			m_WebRequest.SetRequestHeader("Content-Type","application/x-www-form-urlencoded");
		}
	}

	public class GoogleDrivePostCreateFileWebRequest : GoogleDrivePostWebRequest
	{
		public static GoogleDrivePostCreateFileWebRequest Create(string _folderId,string _fileName,byte[] _content)
		{
			var form = new WWWForm();
			form.AddField("method","CreateFile");
			form.AddField("folderId",_folderId);
			form.AddField("filename",_fileName);
			form.AddField("content",Convert.ToBase64String(_content));

			return new GoogleDrivePostCreateFileWebRequest(form.data);
		}

		private GoogleDrivePostCreateFileWebRequest(byte[] _data) : base(_data) { }
	}
}