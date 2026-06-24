using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using KZLib.Utilities;
using Newtonsoft.Json.Linq;

namespace KZLib.Webhooks
{
	public partial class WebhookManager : Singleton<WebhookManager>
	{
		public void PostGoogleSheetAddRow(string sheetName,int sheetOrder,string content)
		{
			PostGoogleSheetAddRowAsync(sheetName,sheetOrder,content).Forget();
		}

		/// <summary>
		/// Appends a row to a configured Google Sheet.
		/// </summary>
		public async UniTask PostGoogleSheetAddRowAsync(string sheetName,int sheetOrder,string content)
		{
			if(!_TryGetSheetId(sheetName,out var sheetId))
			{
				return;
			}

			await PostGoogleSheetAddRowByFileIdAsync(sheetId,sheetOrder,content);
		}

		public void PostGoogleSheetAddRowByFileId(string sheetFileId,int sheetOrder,string content)
		{
			PostGoogleSheetAddRowByFileIdAsync(sheetFileId,sheetOrder,content).Forget();
		}

		/// <summary>
		/// Appends a row to a Google Sheet using the raw spreadsheet file id.
		/// </summary>
		public async UniTask PostGoogleSheetAddRowByFileIdAsync(string sheetFileId,int sheetOrder,string content)
		{
			if(sheetFileId.IsEmpty())
			{
				LogChannel.Webhook.E("Sheet id is empty");

				return;
			}

			if(!_TryGetGoogleSheetURL(out var url))
			{
				return;
			}

			await _SendWebRequest(PostGoogleSheetAddRowWebRequest.Create(url,sheetFileId,sheetOrder,content));
		}

		public void GetGoogleSheet(string sheetName,int sheetOrder,Action<string> onAction)
		{
			GetGoogleSheetAsync(sheetName,sheetOrder).ContinueWith(onAction).Forget();
		}

		/// <summary>
		/// Reads a configured Google Sheet tab and returns the raw response content.
		/// </summary>
		public async UniTask<string> GetGoogleSheetAsync(string sheetName,int sheetOrder)
		{
			if(!_TryGetSheetId(sheetName,out var sheetId))
			{
				return null;
			}

			return await GetGoogleSheetByFileIdAsync(sheetId,sheetOrder);
		}

		public void GetGoogleSheetByFileId(string sheetFileId,int sheetOrder,Action<string> onAction)
		{
			GetGoogleSheetByFileIdAsync(sheetFileId,sheetOrder).ContinueWith(onAction).Forget();
		}

		/// <summary>
		/// Reads a Google Sheet tab using the raw spreadsheet file id.
		/// </summary>
		public async UniTask<string> GetGoogleSheetByFileIdAsync(string sheetFileId,int sheetOrder)
		{
			if(sheetFileId.IsEmpty())
			{
				LogChannel.Webhook.E("Sheet id is empty");

				return null;
			}

			if(!_TryGetGoogleSheetURL(out var url))
			{
				return null;
			}

			var info = await _SendWebRequest(GetGoogleSheetWebRequest.Create(url,sheetFileId,sheetOrder));

			return info.Result ? info.Content : string.Empty;
		}

		public void PostGoogleDriveFile(string folderName,string fileName,byte[] file,string mimeType)
		{
			PostGoogleDriveFileAsync(folderName,fileName,file,mimeType).Forget();
		}

		/// <summary>
		/// Uploads a file to a configured Google Drive folder.
		/// </summary>
		public async UniTask PostGoogleDriveFileAsync(string folderName,string fileName,byte[] file,string mimeType)
		{
			if(!_TryGetFolderId(folderName,out var folderId))
			{
				return;
			}

			await PostGoogleDriveFileByFolderIdAsync(folderId,fileName,file,mimeType);
		}

		public void PostGoogleDriveFileByFolderId(string folderId,string fileName,byte[] file,string mimeType)
		{
			PostGoogleDriveFileByFolderIdAsync(folderId,fileName,file,mimeType).Forget();
		}

		/// <summary>
		/// Uploads a file to a Google Drive folder using the raw folder id.
		/// </summary>
		public async UniTask PostGoogleDriveFileByFolderIdAsync(string folderId,string fileName,byte[] file,string mimeType)
		{
			if(folderId.IsEmpty())
			{
				LogChannel.Webhook.E("Folder id is empty");

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

		/// <summary>
		/// Lists entries in a configured Google Drive folder.
		/// </summary>
		public async UniTask<List<string>> GetGoogleDriveEntryAsync(string folderName)
		{
			if(!_TryGetFolderId(folderName,out var folderId))
			{
				return null;
			}

			return await GetGoogleDriveEntryByFolderIdAsync(folderId);
		}

		public void GetGoogleDriveEntryByFolderId(string folderId,Action<List<string>> onAction)
		{
			GetGoogleDriveEntryByFolderIdAsync(folderId).ContinueWith(onAction).Forget();
		}

		/// <summary>
		/// Lists entries in a Google Drive folder using the raw folder id.
		/// </summary>
		public async UniTask<List<string>> GetGoogleDriveEntryByFolderIdAsync(string folderId)
		{
			if(folderId.IsEmpty())
			{
				LogChannel.Webhook.E("Folder id is empty");

				return null;
			}

			if(!_TryGetGoogleDriveURL(out var url))
			{
				return null;
			}

			var info = await _SendWebRequest(GetGoogleDriveEntryWebRequest.Create(url,folderId));

			return _ParseGoogleDriveEntryList(info);
		}

		private static List<string> _ParseGoogleDriveEntryList(ResponseInfo info)
		{
			if(info == null)
			{
				return null;
			}

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
					LogChannel.Webhook.E($"Failed to convert response: {exception}");
				}
			}
			else
			{
				LogChannel.Webhook.E("Request failed.");
			}

			return null;
		}

		private bool _TryGetSheetId(string sheetName,out string sheetId)
		{
			sheetId = WebhookCfg.GetGoogleSheetFileId(sheetName);

			if(sheetId.IsEmpty())
			{
				LogChannel.Webhook.E("Sheet id is empty");

				return false;
			}

			return true;
		}

		private bool _TryGetGoogleSheetURL(out string url)
		{
			url = WebhookCfg.GoogleSheetURL;

			if(url.IsEmpty())
			{
				LogChannel.Webhook.E("Google Sheet URL is empty");

				return false;
			}

			return true;
		}

		private bool _TryGetFolderId(string folderName,out string folderId)
		{
			folderId = WebhookCfg.GetGoogleDriveFolderId(folderName);

			if(folderId.IsEmpty())
			{
				LogChannel.Webhook.E("Folder id is empty");

				return false;
			}

			return true;
		}

		private bool _TryGetGoogleDriveURL(out string url)
		{
			url = WebhookCfg.GoogleDriveURL;

			if(url.IsEmpty())
			{
				LogChannel.Webhook.E("Google Drive URL is empty");

				return false;
			}

			return true;
		}
	}
}
