// #if UNITY_EDITOR
// using System.Collections.Generic;
// using System.Text;
// using UnityEngine.Networking;

// namespace KZLib.Auth
// {
// 	public abstract class GoogleWebRequest<T> : CustomWebRequest<T>
// 	{
// 		protected override void DoSetUnauthorizedResponse()
// 		{
// 			var provider = GetGoogleAccessTokenProvider();

// 			provider.ProvideAccessToken(OnAccessTokenProviderCompleted);
// 		}

// 		public override void DoCancelAuth()
// 		{
// 			OnAccessTokenProviderCompleted(GetGoogleAccessTokenProvider());
// 		}

// 		protected GoogleAccessTokenProvider GetGoogleAccessTokenProvider()
// 		{
// 			return AuthMgr.In.GetAccessTokenProvider<GoogleAccessTokenProvider>(Global.GOOGLE_AUTH);
// 		}
// 	}

// 	public class GoogleDriveUploadWebRequest : GoogleWebRequest<GoogleFile>
// 	{
// 		private const string DEFAULT_MIME_TYPE = "application/octet-stream";

// 		private readonly string m_Name;
// 		private readonly byte[] m_Content;
// 		private readonly List<string> m_ParentList;

// 		private readonly string m_MimeType;

// 		[QueryParameter] public virtual string UploadType => m_Content != null ? "multipart" : null;

// 		public static GoogleDriveUploadWebRequest Create(string _name,byte[] _content,string _mimeType = null,List<string> _parentList = null,List<string> _fieldList = null)
// 		{
// 			var request = new GoogleDriveUploadWebRequest(_name,_content,_mimeType,_parentList);

// 			if(_fieldList != null)
// 			{
// 				request.Fields = _fieldList;
// 			}

// 			return request;
// 		}

// 		public GoogleDriveUploadWebRequest(string _name,byte[] _content,string _mimeType,List<string> _parentList)
// 		{
// 			m_Name = _name;			
// 			m_ParentList = _parentList;

// 			if(_content != null)
// 			{
// 				m_Content = _content;
// 				m_MimeType = _mimeType.IsEmpty() == true ? DEFAULT_MIME_TYPE : _mimeType;
// 			}
// 		}

// 		protected override UnityWebRequest CreateWebRequest()
// 		{
// 			var url = m_Content != null ? @"https://www.googleapis.com/upload/drive/v3/files" : @"https://www.googleapis.com/drive/v3/files";

// 			m_WebRequest = new UnityWebRequest(url,UnityWebRequest.kHttpVerbPOST);
// 			SetAuthorizationHeader(m_WebRequest);			
// 			SetQueryPayload(m_WebRequest);

// 			m_WebRequest.downloadHandler = new DownloadHandlerBuffer();
			
// 			return m_Content != null ? CreateMultipartUpload(m_WebRequest) : CreateSimpleUpload(m_WebRequest);
// 		}

// 		protected UnityWebRequest CreateMultipartUpload(UnityWebRequest _webRequest)
// 		{
// 			// Can't use MultipartFormDataSection utils to build multipart body,
// 			// because Google has added strict requirements for the body format. 
// 			// Issue: https://github.com/Elringus/UnityGoogleDrive/issues/30).

// 			var boundary = Encoding.ASCII.GetString(UnityWebRequest.GenerateBoundary());
			
// 			var dataList = new List<byte>();
// 			dataList.AddRange(Encoding.UTF8.GetBytes(string.Format("\r\n\r\n--{0}\r\nContent-Type: application/json; charset=UTF-8\r\n\r\n{1}\r\n\r\n--{0}\r\nContent-Type: {2}\r\n\r\n",boundary,Tools.ToJsonPrivateCamel(new GoogleFile() { Name = m_Name, Parents = m_ParentList}),DEFAULT_MIME_TYPE)));
// 			dataList.AddRange(m_Content);
// 			dataList.AddRange(Encoding.UTF8.GetBytes(string.Format("\r\n--{0}--",boundary)));

// 			_webRequest.uploadHandler = new UploadHandlerRaw(dataList.ToArray());
// 			_webRequest.SetRequestHeader("Content-Type",string.Format("multipart/related; boundary={0}",boundary));

// 			return _webRequest;
// 		}

// 		protected UnityWebRequest CreateSimpleUpload(UnityWebRequest _webRequest)
// 		{
// 			var data = Encoding.UTF8.GetBytes(Tools.ToJsonPrivateCamel(new GoogleFile() { Name = m_Name, Parents = m_ParentList}));

// 			SetJsonContentHeader(m_WebRequest);
// 			_webRequest.uploadHandler = new UploadHandlerRaw(data);

// 			return _webRequest;
// 		}
// 	}

// 	public class GoogleDriveFileListWebRequest : GoogleWebRequest<GoogleFileList>
// 	{
// 		private string m_FileId;

// 		public static GoogleDriveFileListWebRequest Create(string _fileID)
// 		{
// 			return new GoogleDriveFileListWebRequest(_fileID);
// 		}

// 		public GoogleDriveFileListWebRequest(string _fileID)
// 		{
// 			m_FileId = _fileID;
// 		} 

// 		protected override UnityWebRequest CreateWebRequest()
// 		{
// 			m_WebRequest = new UnityWebRequest(string.Format("https://www.googleapis.com/drive/v3/files/{0}/list",m_FileId),UnityWebRequest.kHttpVerbGET);
// 			SetAuthorizationHeader(m_WebRequest);
//             SetHtmlContentHeader(m_WebRequest);
//             SetQueryPayload(m_WebRequest);
//             m_WebRequest.downloadHandler = new DownloadHandlerBuffer();
			
// 			return m_WebRequest;
// 		}
// 	}
// }
// #endif