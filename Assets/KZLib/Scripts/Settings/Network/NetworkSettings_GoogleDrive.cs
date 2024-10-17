using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public partial class NetworkSettings : InnerBaseSettings<NetworkSettings>
{
	[TabGroup("Network","GoogleDrive")]
	[TitleGroup("Network/GoogleDrive/General",BoldTitle = false,Order = 0)]
	[VerticalGroup("Network/GoogleDrive/General/0",Order = 0),SerializeField,LabelText("Use GoogleDrive"),ToggleLeft]
	private bool m_UseGoogleDrive = false;

	[VerticalGroup("Network/GoogleDrive/General/1",Order = 1),SerializeField,LabelText("Folder Id Dict"),DictionaryDrawerSettings(KeyLabel = "Name",ValueLabel = "Id"),ShowIf(nameof(m_UseGoogleDrive))]
	private Dictionary<string,string> m_GoogleDriveFolderIdDict = new();

	private bool IsExistGoogleDrive => m_UseGoogleDrive && m_GoogleDriveFolderIdDict.Count > 0;

	public string GetGoogleDriveFolderId(string _key)
	{
		return IsExistGoogleDrive ? m_GoogleDriveFolderIdDict.FindOrFirst(x=>x.Key.Contains(_key)).Value : string.Empty;
	}

#if UNITY_EDITOR
	[TitleGroup("Network/GoogleDrive/Test",BoldTitle = false,Order = 2)]
	[HorizontalGroup("Network/GoogleDrive/Test/0",Order = 0),Button("Get Entry List",ButtonSizes.Large),ShowIf(nameof(m_UseGoogleDrive)),EnableIf(nameof(IsExistGoogleDrive))]
	protected void OnGetEntry_GoogleDrive()
	{
		GetEntryAsync_GoogleDrive().Forget();
	}

	private async UniTaskVoid GetEntryAsync_GoogleDrive()
	{
		m_GoogleDriveList.Clear();

		var dataList = await WebRequestUtility.GetEntry_GoogleDriveAsync("Test");

		if(dataList.IsNullOrEmpty())
		{
			return;
		}

		for(var i=0;i<dataList.Count;i++)
		{
			var json = JObject.Parse(dataList[i]);

			m_GoogleDriveList.Add(new ResultData(json["name"].ToString(),json["id"].ToString()));
		}
	}

	[HorizontalGroup("Network/GoogleDrive/Test/0",Order = 0),Button("Post Image",ButtonSizes.Large),ShowIf(nameof(m_UseGoogleDrive)),EnableIf(nameof(IsExistGoogleDrive))]
	protected void OnPostImage_GoogleDrive()
	{
		WebRequestUtility.PostFile_GoogleDrive("Test","Ostrich.png",GameUtility.GetTestImageData(),"image/png");
	}

	[HorizontalGroup("Network/GoogleDrive/Test/5",Order = 5),ShowInInspector,TableList(HideToolbar = true,AlwaysExpanded = true,IsReadOnly = true),ShowIf(nameof(m_UseGoogleDrive)),EnableIf(nameof(IsExistGoogleDrive))]
	private readonly List<ResultData> m_GoogleDriveList = new();
#endif
}