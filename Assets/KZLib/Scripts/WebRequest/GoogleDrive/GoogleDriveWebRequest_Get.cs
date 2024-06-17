using System;
using UnityEngine.Networking;

namespace KZLib.KZNetwork
{
	public abstract class GoogleDriveGetWebRequest : GoogleDriveWebRequest
	{
		protected GoogleDriveGetWebRequest(string _query) : base(string.Format("{0}{1}",URL,_query),UnityWebRequest.kHttpVerbGET) { }
	}

	public class GoogleDriveGetFolderListWebRequest : GoogleDriveGetWebRequest
	{
		public static GoogleDriveGetFolderListWebRequest Create(string _folderId)
		{
			return new GoogleDriveGetFolderListWebRequest(string.Format("?folderId={0}",_folderId));
		}

		private GoogleDriveGetFolderListWebRequest(string _query) : base(_query) { }
	}
}