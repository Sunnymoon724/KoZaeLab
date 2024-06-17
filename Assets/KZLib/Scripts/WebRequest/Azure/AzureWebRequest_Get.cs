using UnityEngine.Networking;
using System.Collections.Generic;
using System;
using Newtonsoft.Json.Linq;

namespace KZLib.KZNetwork
{
	public abstract class AzureGetWebRequest : AzureWebRequest
	{
		protected AzureGetWebRequest(string _uri,string _storageName,string _accountKey,string _path) : base(_uri,UnityWebRequest.kHttpVerbGET)
		{
			m_WebRequest.SetRequestHeader("Authorization",CreateAuthorization(UnityWebRequest.kHttpVerbGET,_storageName,_accountKey,0,"",_path,null));
			m_WebRequest.SetRequestHeader("x-ms-date",DateTime.UtcNow.ToString("R"));
			m_WebRequest.SetRequestHeader("x-ms-version",AZURE_STORAGE_VERSION);
		}
	}

	public class AzureGetListContainerWebRequest : AzureGetWebRequest
	{
		protected const string GET_REQUEST_URI = "https://{0}.blob.core.windows.net/?comp=list";

		public static AzureGetListContainerWebRequest Create(string _storageName,string _accountKey)
		{
			return new AzureGetListContainerWebRequest(string.Format("https://{0}.blob.core.windows.net/?comp=list",_storageName),_storageName,_accountKey,"\ncomp:list");
			// ,(result,text)=>
			// {
			// 	var enumeration = JObject.Parse(text.XmlToJson())["EnumerationResults"] as JObject;
			// 	var containers = enumeration["Containers"];

			// 	if(containers.Type == JTokenType.Null)
			// 	{
			// 		_onComplete?.Invoke(null);

			// 		return;
			// 	}

			// 	var dataList = new List<string>();

			// 	if(containers.Type == JTokenType.Object)
			// 	{
			// 		var container = containers["Container"] as JObject;

			// 		dataList.Add(container.Value<string>("Name"));

			// 		_onComplete?.Invoke(dataList);

			// 		return;
			// 	}
			// 	else if(containers.Type == JTokenType.Array)
			// 	{
			// 		foreach(var container in containers["Container"] as JArray)
			// 		{
			// 			dataList.Add(container.Value<string>("Name"));
			// 		}

			// 		_onComplete?.Invoke(dataList.IsNullOrEmpty() ? null : dataList);

			// 		return;
			// 	}

			// 	_onComplete?.Invoke(null);
			// });
		}

		private AzureGetListContainerWebRequest(string _uri,string _storageName,string _accountKey,string _path) : base(_uri,_storageName,_accountKey,_path) { }
	}

	public class AzureGetListBlobWebRequest : AzureGetWebRequest
	{
		public static AzureGetListBlobWebRequest Create(string _storageName,string _accountKey,string _containerName)
		{
			return new AzureGetListBlobWebRequest(string.Format("https://{0}.blob.core.windows.net/{1}?comp=list&restype=container",_storageName,_containerName),_storageName,_accountKey,string.Format("{0}\ncomp:list\nrestype:container",_containerName));//,(result,text)=>
		// 	{
		// 		if(!result)
		// 		{
		// 			_onComplete?.Invoke(new List<string>());

		// 			return;
		// 		}

		// 		var enumeration = JObject.Parse(text.XmlToJson())["EnumerationResults"] as JObject;
		// 		var blobs = enumeration["Blobs"];

		// 		if(blobs.Type == JTokenType.Null)
		// 		{
		// 			_onComplete?.Invoke(new List<string>());

		// 			return;
		// 		}

		// 		var dataList = new List<string>();

		// 		var blob = blobs["Blob"];

		// 		if(blob.Type == JTokenType.Object)
		// 		{
		// 			var data = blob as JObject;

		// 			dataList.Add(data.Value<string>("Name"));

		// 			_onComplete?.Invoke(dataList);

		// 			return;
		// 		}
		// 		else if(blob.Type == JTokenType.Array)
		// 		{
		// 			foreach(var data in blob as JArray)
		// 			{
		// 				dataList.Add(data.Value<string>("Name"));
		// 			}

		// 			_onComplete?.Invoke(dataList);

		// 			return;
		// 		}

		// 		_onComplete?.Invoke(null);
		// 	});
		}

		private AzureGetListBlobWebRequest(string _uri,string _storageName,string _accountKey,string _path) : base(_uri,_storageName,_accountKey,_path) { }
	}
}