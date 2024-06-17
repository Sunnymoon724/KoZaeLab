using System;
using System.Text;
using UnityEngine.Networking;

namespace KZLib.KZNetwork
{
	public abstract class AzurePutWebRequest : AzureWebRequest
	{
		protected AzurePutWebRequest(string _uri) : base(_uri,UnityWebRequest.kHttpVerbPUT) { }
	}

	public class AzurePutContainerWebRequest : AzurePutWebRequest
	{
		public static AzurePutContainerWebRequest Create(string _storageName,string _accountKey,string _containerName)
		{
			return new AzurePutContainerWebRequest(string.Format("https://{0}.blob.core.windows.net/{1}?restype=container",_storageName,_containerName),_storageName,_accountKey,_containerName);
		}

		protected AzurePutContainerWebRequest(string _uri,string _storageName,string _accountKey,string _containerName) : base(_uri)
		{
			m_WebRequest.SetRequestHeader("Authorization",CreateAuthorization(UnityWebRequest.kHttpVerbPUT,_storageName,_accountKey,0,"",string.Format("{0}\n{1}",_containerName,"restype:container"),"x-ms-blob-public-access:blob\n"));
			m_WebRequest.SetRequestHeader("x-ms-date",DateTime.UtcNow.ToString("R"));
			m_WebRequest.SetRequestHeader("x-ms-version",AZURE_STORAGE_VERSION);

			m_WebRequest.SetRequestHeader("x-ms-blob-public-access","blob");
		}
	}

	public abstract class AzurePutBlobWebRequest : AzurePutWebRequest
	{
		protected AzurePutBlobWebRequest(string _uri,string _storageName,string _accountKey,byte[] _file,string _containerName,string _fileName,string _contentType) : base(_uri)
		{
			m_WebRequest.SetRequestHeader("Authorization",CreateAuthorization(UnityWebRequest.kHttpVerbPUT,_storageName,_accountKey,_file.Length,_contentType,string.Format("{0}/{1}",_containerName,_fileName),string.Format("x-ms-blob-content-disposition:attachment; filename=\"{0}\"\nx-ms-blob-type:BlockBlob\n",_fileName)));
			m_WebRequest.SetRequestHeader("x-ms-date",DateTime.UtcNow.ToString("R"));
			m_WebRequest.SetRequestHeader("x-ms-version",AZURE_STORAGE_VERSION);

			m_WebRequest.SetRequestHeader("Content-Type",_contentType);
			m_WebRequest.SetRequestHeader("x-ms-blob-content-disposition",string.Format("attachment; filename=\"{0}\"",_fileName));
			m_WebRequest.SetRequestHeader("x-ms-blob-type","BlockBlob");

            m_WebRequest.uploadHandler = new UploadHandlerRaw(_file)
            {
                contentType = _contentType
            };
		}
	}

	public class AzurePutTextWebRequest : AzurePutBlobWebRequest
	{
		public static AzurePutTextWebRequest Create(string _storageName,string _accountKey,string _content,string _containerName,string _fileName,string _contentType = "text/plain; charset=UTF-8")
		{
			return new AzurePutTextWebRequest(string.Format("https://{0}.blob.core.windows.net/{1}/{2}",_storageName,_containerName,_fileName),_storageName,_accountKey,Encoding.UTF8.GetBytes(_content),_containerName,_fileName,_contentType);
		}

		private AzurePutTextWebRequest(string _uri,string _storageName,string _accountKey,byte[] _file,string _containerName,string _fileName,string _contentType) : base(_uri,_storageName,_accountKey,_file,_containerName,_fileName,_contentType) { }
	}

	public class AzurePutImageWebRequest : AzurePutBlobWebRequest
	{
		public static AzurePutImageWebRequest Create(string _storageName,string _accountKey,byte[] _file,string _containerName,string _fileName,string _contentType = "image/png")
		{
			return new AzurePutImageWebRequest(string.Format("https://{0}.blob.core.windows.net/{1}/{2}",_storageName,_containerName,_fileName),_storageName,_accountKey,_file,_containerName,_fileName,_contentType);
		}

		private AzurePutImageWebRequest(string _uri,string _storageName,string _accountKey,byte[] _file,string _containerName,string _fileName,string _contentType) : base(_uri,_storageName,_accountKey,_file,_containerName,_fileName,_contentType) { }
	}

	public class AzurePutAudioWebRequest : AzurePutBlobWebRequest
	{
		public static AzurePutAudioWebRequest Create(string _storageName,string _accountKey,byte[] _file,string _containerName,string _fileName,string _contentType = "audio/wav")
		{
			return new AzurePutAudioWebRequest(string.Format("https://{0}.blob.core.windows.net/{1}/{2}",_storageName,_containerName,_fileName),_storageName,_accountKey,_file,_containerName,_fileName,_contentType);
		}

		private AzurePutAudioWebRequest(string _uri,string _storageName,string _accountKey,byte[] _file,string _containerName,string _fileName,string _contentType) : base(_uri,_storageName,_accountKey,_file,_containerName,_fileName,_contentType) { }
	}

	public class AzurePutAssetWebRequest : AzurePutBlobWebRequest
	{
		public static AzurePutAssetWebRequest Create(string _storageName,string _accountKey,byte[] _file,string _containerName,string _fileName,string _contentType = "application/octet-stream")
		{
			return new AzurePutAssetWebRequest(string.Format("https://{0}.blob.core.windows.net/{1}/{2}",_storageName,_containerName,_fileName),_storageName,_accountKey,_file,_containerName,_fileName,_contentType);
		}

		private AzurePutAssetWebRequest(string _uri,string _storageName,string _accountKey,byte[] _file,string _containerName,string _fileName,string _contentType) : base(_uri,_storageName,_accountKey,_file,_containerName,_fileName,_contentType) { }
	}
}