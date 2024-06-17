using System;
using UnityEngine.Networking;

namespace KZLib.KZNetwork
{
	public abstract class AzureDeleteWebRequest : AzureWebRequest
	{
		protected AzureDeleteWebRequest(string _uri,string _storageName,string _accountKey,string _path) : base(_uri,UnityWebRequest.kHttpVerbDELETE)
		{
			m_WebRequest.SetRequestHeader("Authorization",CreateAuthorization(UnityWebRequest.kHttpVerbDELETE,_storageName,_accountKey,0,"",_path,null));
			m_WebRequest.SetRequestHeader("x-ms-date",DateTime.UtcNow.ToString("R"));
			m_WebRequest.SetRequestHeader("x-ms-version",AZURE_STORAGE_VERSION);
		}
	}

	public class AzureDeleteBlobWebRequest : AzureDeleteWebRequest
	{
		public static AzureDeleteBlobWebRequest Create(string _storageName,string _accountKey,string _containerName,string _fileName)
		{
			return new AzureDeleteBlobWebRequest(string.Format("https://{0}.blob.core.windows.net/{1}/{2}",_storageName,_containerName,_fileName),_storageName,_accountKey,string.Format("{0}/{1}",_containerName,_fileName));
		}

		private AzureDeleteBlobWebRequest(string _uri,string _storageName,string _accountKey,string _path) : base(_uri,_storageName,_accountKey,_path) { }
	}

	public class AzureDeleteContainerWebRequest : AzureDeleteWebRequest
	{
		public static AzureDeleteContainerWebRequest Create(string _storageName,string _accountKey,string _containerName)
		{
			return new AzureDeleteContainerWebRequest(string.Format("https://{0}.blob.core.windows.net/{1}?restype=container",_storageName,_containerName),_storageName,_accountKey,string.Format("{0}\n{1}",_containerName,"restype:container"));
		}

		private AzureDeleteContainerWebRequest(string _uri,string _storageName,string _accountKey,string _path) : base(_uri,_storageName,_accountKey,_path) { }
	}
}