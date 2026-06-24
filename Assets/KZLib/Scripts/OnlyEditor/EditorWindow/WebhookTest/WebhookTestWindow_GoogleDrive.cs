#if UNITY_EDITOR
using System.Collections.Generic;
using KZLib.Webhooks;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace KZLib.Windows
{
	/// <summary>
	/// Google Drive folder listing and image upload tests for <see cref="WebhookTestWindow"/>.
	/// </summary>
	public partial class WebhookTestWindow : OdinEditorWindow
	{
		[TabGroup("Network","GoogleDrive")]
		[HorizontalGroup("Network/GoogleDrive/0",Order = 0),SerializeField]
		private string m_googleDriveFolderId = null;
		private bool IsExistGoogleDrive => !m_googleDriveFolderId.IsEmpty();

		[HorizontalGroup("Network/GoogleDrive/1",Order = 1),Button("Get Entry List",ButtonSizes.Large),EnableIf(nameof(IsExistGoogleDrive))]
		protected void OnFindEntry_GoogleDrive()
		{
			m_googleDriveList.Clear();
			m_googleDriveList.Add(ResultInfo.CreatePlaceholder("Loading..."));

			void _FindEntry(List<string> entryList)
			{
				_RunOnEditorMainThread(() => _ApplyGoogleDriveEntryList(entryList));
			}

			WebhookManager.In.GetGoogleDriveEntryByFolderId(m_googleDriveFolderId,_FindEntry);
		}

		[HorizontalGroup("Network/GoogleDrive/1",Order = 1),Button("Post Image",ButtonSizes.Large),EnableIf(nameof(IsExistGoogleDrive))]
		protected void OnPostImage_GoogleDrive()
		{
			if(!_TryReadTemplateTestImage(out var imageBytes))
			{
				return;
			}

			WebhookManager.In.PostGoogleDriveFileByFolderId(m_googleDriveFolderId,"Ostrich.png",imageBytes,"image/png");
		}

		[HorizontalGroup("Network/GoogleDrive/3",Order = 3),SerializeField,TableList(HideToolbar = true,AlwaysExpanded = true,IsReadOnly = true),EnableIf(nameof(IsExistGoogleDrive))]
		private List<ResultInfo> m_googleDriveList = new();

		/// <summary>
		/// Fills the Drive table from webhook JSON entries or shows a placeholder row on failure.
		/// </summary>
		private void _ApplyGoogleDriveEntryList(List<string> entryList)
		{
			m_googleDriveList.Clear();

			if(entryList.IsNullOrEmpty())
			{
				m_googleDriveList.Add(ResultInfo.CreatePlaceholder(c_emptyResultName));

				return;
			}

			for(var i=0;i<entryList.Count;i++)
			{
				if(!_TryCreateResultInfo(entryList[i],out var resultInfo))
				{
					continue;
				}

				m_googleDriveList.Add(resultInfo);
			}

			if(m_googleDriveList.Count == 0)
			{
				m_googleDriveList.Add(ResultInfo.CreatePlaceholder("(No parseable entries)"));
			}
		}
	}
}
#endif
