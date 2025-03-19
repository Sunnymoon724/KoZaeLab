using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using KZLib.KZUtility;
using Newtonsoft.Json.Linq;

namespace KZLib
{
	public partial class NetworkMgr : Singleton<NetworkMgr>
	{
		public void PostGoogleSheetAddRow(string sheetName,int sheetOrder,string content)
		{
			PostGoogleSheetAddRowAsync(sheetName,sheetOrder,content).Forget();
		}

		public async UniTask PostGoogleSheetAddRowAsync(string sheetName,int sheetOrder,string content)
		{
			if(!_TryGetSheetId(sheetName,out var sheetId))
			{
				return;
			}

			if(!_TryGetGoogleSheetURL(out var url))
			{
				return;
			}

			await PostWebRequest(PostGoogleSheetAddRowWebRequest.Create(url,sheetId,sheetOrder,content));
		}

		public void GetGoogleSheet(string sheetName,int sheetOrder,Action<string> onAction)
		{
			GetGoogleSheetAsync(sheetName,sheetOrder).ContinueWith(onAction);
		}

		public async UniTask<string> GetGoogleSheetAsync(string sheetName,int sheetOrder)
		{
			if(!_TryGetSheetId(sheetName,out var sheetId))
			{
				return null;
			}

			if(!_TryGetGoogleSheetURL(out var url))
			{
				return null;
			}

			var info = await GetWebRequest(GetGoogleSheetWebRequest.Create(url,sheetId,sheetOrder));

			return info.Result ? info.Content : string.Empty;
		}

		

		public void PostGoogleDriveFile(string folderName,string fileName,byte[] file,string mimeType)
		{
			PostGoogleDriveFileAsync(folderName,fileName,file,mimeType).Forget();
		}

		public async UniTask PostGoogleDriveFileAsync(string folderName,string fileName,byte[] file,string mimeType)
		{
			if(!_TryGetFolderId(folderName,out var folderId))
			{
				return;
			}

			if(!_TryGetGoogleDriveURL(out var url))
			{
				return;
			}

			await PostWebRequest(PostGoogleDriveFileWebRequest.Create(url,folderId,fileName,file,mimeType));
		}

		public void GetGoogleDriveEntry(string folderName,Action<List<string>> onAction)
		{
			GetGoogleDriveEntryAsync(folderName).ContinueWith(onAction);
		}

		public async UniTask<List<string>> GetGoogleDriveEntryAsync(string folderName)
		{
			if(!_TryGetFolderId(folderName,out var folderId))
			{
				return null;
			}

			if(!_TryGetGoogleDriveURL(out var url))
			{
				return null;
			}

			var info = await GetWebRequest(GetGoogleDriveEntryWebRequest.Create(url,folderId));


			if(info.Result)
			{
				try
				{
					var array = JArray.Parse(info.Content);
					var dataList = new List<string>();

					foreach(var token in array)
					{
						dataList.Add(token.ToString());
					}

					return dataList;
				}
				catch(Exception exception)
				{
					LogTag.System.E($"Convert is failed - {exception}");
				}
			}
			else
			{
				LogTag.System.E("Result is failed");
			}

			return null;
		}

		private bool _TryGetSheetId(string sheetName,out string sheetId)
		{
			var serviceCfg = ConfigMgr.In.Access<ConfigData.ServiceConfig>();
			sheetId = serviceCfg.GetGoogleSheetFileId(sheetName);

			if(sheetId.IsEmpty())
			{
				LogTag.System.E("Sheet id is empty");

				return false;
			}

			return true;
		}

		private bool _TryGetGoogleSheetURL(out string url)
		{
			var serviceCfg = ConfigMgr.In.Access<ConfigData.ServiceConfig>();
			url = serviceCfg.GoogleSheetURL;

			if(url.IsEmpty())
			{
				LogTag.System.E("Google Sheet URL is empty");

				return false;
			}

			return true;
		}

		private bool _TryGetFolderId(string folderName,out string folderId)
		{
			var serviceCfg = ConfigMgr.In.Access<ConfigData.ServiceConfig>();
			folderId = serviceCfg.GetGoogleDriveFolderId(folderName);

			if(folderId.IsEmpty())
			{
				LogTag.System.E("Folder id is empty");

				return false;
			}

			return true;
		}

		private bool _TryGetGoogleDriveURL(out string url)
		{
			var serviceCfg = ConfigMgr.In.Access<ConfigData.ServiceConfig>();
			url = serviceCfg.GoogleDriveURL;

			if(url.IsEmpty())
			{
				LogTag.System.E("Google Drive URL is empty");

				return false;
			}

			return true;
		}
	}
}