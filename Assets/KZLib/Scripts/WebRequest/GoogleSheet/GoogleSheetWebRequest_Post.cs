using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace KZLib.KZNetwork
{
	public abstract class GoogleSheetPostWebRequest : GoogleSheetWebRequest
	{
		protected GoogleSheetPostWebRequest(byte[] _data) : base(URL,UnityWebRequest.kHttpVerbPOST)
		{
			m_WebRequest.uploadHandler = new UploadHandlerRaw(_data);
			m_WebRequest.SetRequestHeader("Content-Type","application/x-www-form-urlencoded");
		}
	}

	public class GoogleSheetPostAppendRowWebRequest : GoogleSheetPostWebRequest
	{
		public static GoogleSheetPostAppendRowWebRequest Create(string _fileId,int _sheetIndex,string _data)
		{
			var form = new WWWForm();
			form.AddField("method","AppendRow");
			form.AddField("fileId",_fileId);
			form.AddField("sheetIndex",_sheetIndex);
			form.AddField("data",_data);

			return new GoogleSheetPostAppendRowWebRequest(form.data);
		}

		private GoogleSheetPostAppendRowWebRequest(byte[] _data) : base(_data) { }
	}
}