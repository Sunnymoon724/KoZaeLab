using UnityEngine;
using UnityEngine.Networking;

namespace KZLib.KZNetwork
{
	public abstract class GoogleSheetWebRequest : BaseWebRequest
	{
		protected const string SHEET_URL = @"https://script.google.com/macros/s/AKfycbzWLZsZ5ai7AiPwAXUua1QigSyQ2w8DLJpH00W9SF8gBx-DU0GfHkUjZQ_4ebtWgfHr/exec";

		protected GoogleSheetWebRequest(string _uri,string _method) : base(_uri,_method) { }
	}

	#region Get
	public abstract class GoogleSheetGetWebRequest : GoogleSheetWebRequest
	{
		protected GoogleSheetGetWebRequest(string _query) : base($"{SHEET_URL}{_query}",UnityWebRequest.kHttpVerbGET) { }
	}

	public class GoogleSheetGetSheetWebRequest : GoogleSheetGetWebRequest
	{
		public static GoogleSheetGetSheetWebRequest Create(string _fileId,int _sheetIndex)
		{
			return new GoogleSheetGetSheetWebRequest($"?fileId={_fileId}&sheetIndex={_sheetIndex}");
		}

		private GoogleSheetGetSheetWebRequest(string _query) : base(_query) { }
	}
	#endregion Get

	#region Post
	public abstract class GoogleSheetPostWebRequest : GoogleSheetWebRequest
	{
		protected GoogleSheetPostWebRequest(byte[] _data) : base(SHEET_URL,UnityWebRequest.kHttpVerbPOST)
		{
			m_WebRequest.uploadHandler = new UploadHandlerRaw(_data);
			m_WebRequest.SetRequestHeader("Content-Type","application/x-www-form-urlencoded");
		}
	}

	public class GoogleSheetPostAddRowWebRequest : GoogleSheetPostWebRequest
	{
		public static GoogleSheetPostAddRowWebRequest Create(string _fileId,int _sheetIndex,string _data)
		{
			var form = new WWWForm();
			form.AddField("method","addRow");
			form.AddField("fileId",_fileId);
			form.AddField("sheetIndex",_sheetIndex);
			form.AddField("data",_data);

			return new GoogleSheetPostAddRowWebRequest(form.data);
		}

		private GoogleSheetPostAddRowWebRequest(byte[] _data) : base(_data) { }
	}
	#endregion Post
}