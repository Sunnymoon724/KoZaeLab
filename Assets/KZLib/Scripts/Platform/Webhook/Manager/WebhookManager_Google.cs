using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using KZLib.Utilities;
using Newtonsoft.Json.Linq;

namespace KZLib.Webs
{
	public partial class WebhookManager : Singleton<WebhookManager>
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
			GetGoogleSheetAsync(sheetName,sheetOrder).ContinueWith(onAction).Forget();
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
			GetGoogleDriveEntryAsync(folderName).ContinueWith(onAction).Forget();
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
					LogChannel.Web.E($"Convert is failed - {exception}");
				}
			}
			else
			{
				LogChannel.Web.E("Result is failed");
			}

			return null;
		}

		private bool _TryGetSheetId(string sheetName,out string sheetId)
		{
			sheetId = WebhookCfg.GetGoogleSheetFileId(sheetName);

			if(sheetId.IsEmpty())
			{
				LogChannel.Web.E("Sheet id is empty");

				return false;
			}

			return true;
		}

		private bool _TryGetGoogleSheetURL(out string url)
		{
			url = WebhookCfg.GoogleSheetURL;

			if(url.IsEmpty())
			{
				LogChannel.Web.E("Google Sheet URL is empty");

				return false;
			}

			return true;
		}

		private bool _TryGetFolderId(string folderName,out string folderId)
		{
			folderId = WebhookCfg.GetGoogleDriveFolderId(folderName);

			if(folderId.IsEmpty())
			{
				LogChannel.Web.E("Folder id is empty");

				return false;
			}

			return true;
		}

		private bool _TryGetGoogleDriveURL(out string url)
		{
			url = WebhookCfg.GoogleDriveURL;

			if(url.IsEmpty())
			{
				LogChannel.Web.E("Google Drive URL is empty");

				return false;
			}

			return true;
		}
	}
}