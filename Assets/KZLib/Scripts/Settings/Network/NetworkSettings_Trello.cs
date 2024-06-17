using System;
using System.Collections.Generic;
using KZLib.KZNetwork;
using Sirenix.OdinInspector;
using UnityEngine;

public partial class NetworkSettings : InSideSingletonSO<NetworkSettings>
{
	[TabGroup("통신 설정","트렐로")]
	[TitleGroup("통신 설정/트렐로/사용 설정",BoldTitle = false,Order = 0),SerializeField,LabelText("트렐로 사용"),ToggleLeft]
	private bool m_UseTrello = false;

	[TitleGroup("통신 설정/트렐로/기본 설정",BoldTitle = false,Order = 1)]
	[BoxGroup("통신 설정/트렐로/기본 설정/0",ShowLabel = false),SerializeField,LabelText("키"),EnableIf(nameof(m_UseTrello))]
	private string m_TrelloApiKey = null;
	[BoxGroup("통신 설정/트렐로/기본 설정/0",ShowLabel = false),SerializeField,LabelText("토큰"),EnableIf(nameof(m_UseTrello))]
	private string m_TrelloToken = null;

	[BoxGroup("통신 설정/트렐로/기본 설정/0",ShowLabel = false),SerializeField,LabelText("보드 아이디들"),ListDrawerSettings(ShowFoldout = false),EnableIf(nameof(m_UseTrello))]
	private Dictionary<string,string> m_TrelloBoardIdDict = new();

	private bool IsExistTrello => m_UseTrello && !string.Concat(m_TrelloApiKey,m_TrelloToken).IsEmpty();

	public string GetTrelloCoreKey()
	{
		return IsExistTrello ? string.Format("key={0}&token={1}",m_TrelloApiKey,m_TrelloToken) : string.Empty;
	}

	public string GetTrelloBoardId(string _key)
	{
		return IsExistTrello && m_TrelloBoardIdDict.Count > 0 ? m_DiscordLinkDict.FindOrFirst(x=>x.Key.Contains(_key)).Value : string.Empty;
	}

#if UNITY_EDITOR
	[TitleGroup("통신 설정/트렐로/테스트",BoldTitle = false,Order = 2)]
	[BoxGroup("통신 설정/트렐로/테스트/0",ShowLabel = false)]
	[VerticalGroup("통신 설정/트렐로/테스트/0/아이디",Order = 0),ShowInInspector,LabelText("테스트 아이디"),EnableIf("IsExistTrello")]
	private string m_TrelloTestId = null;

	[HorizontalGroup("통신 설정/트렐로/테스트/0/생성버튼",Order = 1),Button("리스트 생성하기",ButtonSizes.Large),EnableIf("HasTestId")]
	private void OnPostList_Trello()
	{
		var request = TrelloPostListWebRequest.Create(GetTrelloCoreKey(),m_TrelloTestId,"Test");

		request.Send();
	}

	[HorizontalGroup("통신 설정/트렐로/테스트/0/생성버튼",Order = 1),Button("카드 생성하기",ButtonSizes.Large),EnableIf("HasTestId")]
	private void OnPostCard_Trello()
	{
		var request = TrelloPostCardWebRequest.Create(GetTrelloCoreKey(),m_TrelloTestId,"Test","Test");

		request.Send();
	}

	[HorizontalGroup("통신 설정/트렐로/테스트/0/확인버튼",Order = 2),Button("보드 확인하기",ButtonSizes.Large),EnableIf("IsExistTrello")]
	private void OnGetBoard_Trello()
	{
		var request = TrelloGetBoardsWebRequest.Create(GetTrelloCoreKey(),OnShowData_Trello);

		request.Send();
	}

	private bool HasTestId => IsExistTrello && !m_TrelloTestId.IsEmpty();

	[HorizontalGroup("통신 설정/트렐로/테스트/0/확인버튼",Order = 2),Button("리스트 확인하기",ButtonSizes.Large),EnableIf("HasTestId")]
	private void OnGetList_Trello()
	{
		var request = TrelloGetListsWebRequest.Create(GetTrelloCoreKey(),m_TrelloTestId,OnShowData_Trello);

		request.Send();
	}

	[HorizontalGroup("통신 설정/트렐로/테스트/0/확인버튼",Order = 2),Button("카드 확인하기",ButtonSizes.Large),EnableIf("HasTestId")]
	private void OnGetCard_Trello()
	{
		var request = TrelloGetCardsWebRequest.Create(GetTrelloCoreKey(),m_TrelloTestId,OnShowData_Trello);

		request.Send();
	}

	[HorizontalGroup("통신 설정/트렐로/테스트/0/결과",Order = 3),ShowInInspector,LabelText("결과 리스트"),EnableIf("IsExistTrello"),TableList(HideToolbar = true,AlwaysExpanded = true)]
	private readonly List<NameIdResultData> m_TrelloResultDataList = new();

	private void OnShowData_Trello(List<string> _dataList)
	{
		m_TrelloResultDataList.Clear();

		if(_dataList.IsNullOrEmpty())
		{
			return;
		}

		for(var i=0;i<_dataList.Count;i++)
		{
			// var data = JObject.Parse(_dataList[i]);

			// m_TrelloResultDataList.Add(new NameIdResultData(data["name"].ToString(),data["id"].ToString()));
		}
	}
#endif
}