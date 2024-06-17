using UnityEngine.Networking;

namespace KZLib.KZNetwork
{
	public abstract class GoogleSheetGetWebRequest : GoogleSheetWebRequest
	{
		protected GoogleSheetGetWebRequest(string _query) : base(string.Format("{0}{1}",URL,_query),UnityWebRequest.kHttpVerbGET) { }
	}

	public class GoogleSheetGetSheetWebRequest : GoogleSheetGetWebRequest
	{
		public static GoogleSheetGetSheetWebRequest Create(string _fileId,int _sheetIndex)
		{
			return new GoogleSheetGetSheetWebRequest(string.Format("?fileId={0}&sheetIndex={1}",_fileId,_sheetIndex));
		}

		private GoogleSheetGetSheetWebRequest(string _query) : base(_query) { }
	}
}