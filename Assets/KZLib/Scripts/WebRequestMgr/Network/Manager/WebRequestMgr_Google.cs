using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using KZLib.KZData;
using KZLib.KZUtility;
using Newtonsoft.Json.Linq;

namespace KZLib.KZNetwork
{
	public partial class WebRequestMgr : Singleton<WebRequestMgr>
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

			await _SendWebRequest(PostGoogleSheetAddRowWebRequest.Create(url,sheetId,sheetOrder,content));
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

			var info = await _SendWebRequest(GetGoogleSheetWebRequest.Create(url,sheetId,sheetOrder));

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

			await _SendWebRequest(PostGoogleDriveFileWebRequest.Create(url,folderId,fileName,file,mimeType));
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

			var info = await _SendWebRequest(GetGoogleDriveEntryWebRequest.Create(url,folderId));


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
					LogSvc.Network.E($"Convert is failed - {exception}");
				}
			}
			else
			{
				LogSvc.Network.E("Result is failed");
			}

			return null;
		}

		private bool _TryGetSheetId(string sheetName,out string sheetId)
		{
			var serviceCfg = ConfigMgr.In.Access<ServiceConfig>();
			sheetId = serviceCfg.GetGoogleSheetFileId(sheetName);

			if(sheetId.IsEmpty())
			{
				LogSvc.Network.E("Sheet id is empty");

				return false;
			}

			return true;
		}

		private bool _TryGetGoogleSheetURL(out string url)
		{
			var serviceCfg = ConfigMgr.In.Access<ServiceConfig>();
			url = serviceCfg.GoogleSheetURL;

			if(url.IsEmpty())
			{
				LogSvc.Network.E("Google Sheet URL is empty");

				return false;
			}

			return true;
		}

		private bool _TryGetFolderId(string folderName,out string folderId)
		{
			var serviceCfg = ConfigMgr.In.Access<ServiceConfig>();
			folderId = serviceCfg.GetGoogleDriveFolderId(folderName);

			if(folderId.IsEmpty())
			{
				LogSvc.Network.E("Folder id is empty");

				return false;
			}

			return true;
		}

		private bool _TryGetGoogleDriveURL(out string url)
		{
			var serviceCfg = ConfigMgr.In.Access<ServiceConfig>();
			url = serviceCfg.GoogleDriveURL;

			if(url.IsEmpty())
			{
				LogSvc.Network.E("Google Drive URL is empty");

				return false;
			}

			return true;
		}
	}
}