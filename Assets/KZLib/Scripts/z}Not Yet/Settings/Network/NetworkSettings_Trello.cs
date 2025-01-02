using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public partial class NetworkSettings : InnerBaseSettings<NetworkSettings>
{
	[TabGroup("Network","Trello")]
	[TitleGroup("Network/Trello/General",BoldTitle = false,Order = 0)]
	[VerticalGroup("Network/Trello/General/0",Order = 0),SerializeField,LabelText("Use Trello"),ToggleLeft]
	private bool m_UseTrello = false;

	[VerticalGroup("Network/Trello/General/1",Order = 1),SerializeField,LabelText("API Key"),ShowIf(nameof(m_UseTrello))]
	private string m_TrelloApiKey = null;
	[VerticalGroup("Network/Trello/General/2",Order = 2),SerializeField,LabelText("Token"),ShowIf(nameof(m_UseTrello))]
	private string m_TrelloToken = null;

	[VerticalGroup("Network/Trello/General/3",Order = 3),SerializeField,LabelText("Board Id Dict"),DictionaryDrawerSettings(KeyLabel = "Name",ValueLabel = "Id"),ShowIf(nameof(m_UseTrello))]
	private Dictionary<string,string> m_TrelloBoardIdDict = new();

	private bool IsExistTrello => m_UseTrello && !m_TrelloApiKey.IsEmpty() && !m_TrelloToken.IsEmpty();

    public string TrelloCoreKey => IsExistTrello ? $"key={m_TrelloApiKey}&token={m_TrelloToken}" : string.Empty;

    public string GetTrelloBoardId(string _key)
	{
		return IsExistTrello && m_TrelloBoardIdDict.Count > 0 ? m_TrelloBoardIdDict.FindOrFirst(x=>x.Key.Contains(_key)).Value : string.Empty;
	}

#if UNITY_EDITOR
	[TitleGroup("Network/Trello/Test",BoldTitle = false,Order = 2)]
	[HorizontalGroup("Network/Trello/Test/0",Order = 0),Button("Get Board",ButtonSizes.Large),ShowIf(nameof(m_UseTrello)),EnableIf(nameof(IsExistTrello))]
	protected void OnGetBoard_Trello()
	{
		GetBoard_TrelloAsync().Forget();
	}

	private async UniTaskVoid GetBoard_TrelloAsync()
	{
		var data = await CommonUtility.FindBoard_TrelloAsync();

		ShowData_Trello(data);
	}

	[HorizontalGroup("Network/Trello/Test/1",Order = 1),ShowInInspector,LabelText("Test Id"),ShowIf(nameof(m_UseTrello)),EnableIf(nameof(IsExistTrello))]
	private readonly string m_TrelloTestId = null;

	private bool HasTestId => IsExistTrello && !m_TrelloTestId.IsEmpty();

	[HorizontalGroup("Network/Trello/Test/2",Order = 2),Button("Get List",ButtonSizes.Large),ShowIf(nameof(m_UseTrello)),EnableIf(nameof(HasTestId))]
	protected void OnGetList_Trello()
	{
		GetList_TrelloAsync().Forget();
	}

	private async UniTaskVoid GetList_TrelloAsync()
	{
		var data = await CommonUtility.FindList_TrelloAsync(m_TrelloTestId);

		ShowData_Trello(data);
	}

	[HorizontalGroup("Network/Trello/Test/2",Order = 2),Button("Get Card",ButtonSizes.Large),ShowIf(nameof(m_UseTrello)),EnableIf(nameof(HasTestId))]
	protected void OnGetCard_Trello()
	{
		GetCard_TrelloAsync().Forget();
	}

	private async UniTaskVoid GetCard_TrelloAsync()
	{
		var data = await CommonUtility.FindCard_TrelloAsync(m_TrelloTestId);

		ShowData_Trello(data);
	}

	[HorizontalGroup("Network/Trello/Test/3",Order = 3),Button("Post List",ButtonSizes.Large),ShowIf(nameof(m_UseTrello)),EnableIf(nameof(HasTestId))]
	protected void OnPostList_Trello()
	{
		CommonUtility.PostList_Trello(m_TrelloTestId,"Test");
	}

	[HorizontalGroup("Network/Trello/Test/3",Order = 3),Button("Post Card",ButtonSizes.Large),ShowIf(nameof(m_UseTrello)),EnableIf(nameof(HasTestId))]
	protected void OnPostCard_Trello()
	{
		CommonUtility.PostCard_Trello(m_TrelloTestId,"Test","Description");
	}

	[HorizontalGroup("Network/Trello/Test/5",Order = 5),ShowInInspector,TableList(HideToolbar = true,AlwaysExpanded = true),ShowIf(nameof(IsShowTrelloResultList)),EnableIf(nameof(IsExistTrello))]
	private readonly List<ResultData> m_TrelloDataList = new();

	private bool IsShowTrelloResultList => m_UseTrello && m_TrelloDataList.Count > 0;

	private void ShowData_Trello(List<string> _dataList)
	{
		m_TrelloDataList.Clear();

		if(_dataList.IsNullOrEmpty())
		{
			return;
		}

		for(var i=0;i<_dataList.Count;i++)
		{
			var json = JObject.Parse(_dataList[i]);

			m_TrelloDataList.Add(new ResultData(json["name"].ToString(),json["id"].ToString()));
		}
	}
#endif
}