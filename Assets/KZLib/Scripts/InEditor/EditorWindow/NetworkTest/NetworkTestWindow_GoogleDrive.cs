#if UNITY_EDITOR
using System.Collections.Generic;
using KZLib.KZNetwork;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace KZLib.KZWindow
{
	public partial class NetworkTestWindow : OdinEditorWindow
	{
		[TabGroup("Network","GoogleDrive")]
		[HorizontalGroup("Network/GoogleDrive/0",Order = 0),SerializeField]
		private string m_googleDriveFolderId = null;
		private bool IsExistGoogleDrive => !m_googleDriveFolderId.IsEmpty();

		[HorizontalGroup("Network/GoogleDrive/1",Order = 1),Button("Get Entry List",ButtonSizes.Large),EnableIf(nameof(IsExistGoogleDrive))]
		protected void OnFindEntry_GoogleDrive()
		{
			NetworkMgr.In.GetGoogleDriveEntry("Test",(dataList) =>
			{
				if(dataList.IsNullOrEmpty())
				{
					return;
				}

				for(var i=0;i<dataList.Count;i++)
				{
					var json = JObject.Parse(dataList[i]);

					m_googleDriveList.Add(new ResultData(json["name"].ToString(),json["id"].ToString()));
				}
			});
		}

		[HorizontalGroup("Network/GoogleDrive/1",Order = 1),Button("Post Image",ButtonSizes.Large),EnableIf(nameof(IsExistGoogleDrive))]
		protected void OnPostImage_GoogleDrive()
		{
			NetworkMgr.In.PostGoogleDriveFile("Test","Ostrich.png",CommonUtility.FindTestImageData(),"image/png");
		}

		[HorizontalGroup("Network/GoogleDrive/3",Order = 3),SerializeField,TableList(HideToolbar = true,AlwaysExpanded = true,IsReadOnly = true),EnableIf(nameof(IsExistGoogleDrive))]
		private List<ResultData> m_googleDriveList = new();
	}
}
#endif