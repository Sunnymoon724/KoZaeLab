using System;
using System.Collections.Generic;
using System.Text;

namespace KZLib.KZNetwork
{
	// 현재는 Blob Storage만
	public partial class AzureWebRequest : BaseWebRequest
	{
		// https://docs.microsoft.com/en-us/rest/api/storageservices/fileservices/versioning-for-the-azure-storage-services
		protected const string AZURE_STORAGE_VERSION = "2023-01-03";

		private readonly Dictionary<string,string> m_AuthHeaderDict = new()
		{
			{ "Content-Encoding", "" },
			{ "Content-Language", "" },
			{ "Content-Length", "" },
			{ "Content-MD5", "" },
			{ "Content-Type", "" },
			{ "Date", "" },
			{ "If-Modified-Since", "" },
			{ "If-Match", "" },
			{ "If-None-Match", "" },
			{ "If-Unmodified-Since", "" },
			{ "Range", "" }
		};

		protected AzureWebRequest(string _uri,string _method) : base(_uri, _method) { }

		protected string CreateAuthorization(string _method,string _storageName,string _accountKey,int _length,string _type,string _path,string _option)
		{
			var key = Convert.FromBase64String(_accountKey);

			if(_length > 0)
			{
				m_AuthHeaderDict["Content-Length"] = string.Format("{0}",_length);
			}

			if(!_type.IsEmpty())
			{
				m_AuthHeaderDict["Content-Type"] = _type;
			}

			var builder = new StringBuilder();
			builder.AppendFormat("{0}\n",_method);

			foreach(var header in m_AuthHeaderDict.Values)
			{
				builder.AppendFormat("{0}\n",header);
			}

			if(!_option.IsEmpty())
			{
				builder.AppendFormat(_option);
			}

			// date & version
			builder.AppendFormat("x-ms-date:{0}\n",DateTime.UtcNow.ToString("R"));
			builder.AppendFormat("x-ms-version:{0}\n",AZURE_STORAGE_VERSION);

			// container
			builder.AppendFormat("/{0}/{1}",_storageName,_path);
			
			var signature = SecurityUtility.HMACSignature(key,builder.ToString());

			return string.Format("SharedKey {0}:{1}",_storageName,signature);
		}
	}
}