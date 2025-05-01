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
		[TabGroup("Network","Trello")]
		[HorizontalGroup("Network/Trello/0",Order = 0),SerializeField]
		private string m_trelloApiKey = null;
		[HorizontalGroup("Network/Trello/1",Order = 1),SerializeField]
		private string m_trelloToken = null;

		private bool IsExistTrello => !m_trelloApiKey.IsEmpty() && !m_trelloToken.IsEmpty();

		public string TrelloCoreKey => IsExistTrello ? $"key={m_trelloApiKey}&token={m_trelloToken}" : string.Empty;

		[HorizontalGroup("Network/Trello/2",Order = 2),Button("Get Board",ButtonSizes.Large),EnableIf(nameof(IsExistTrello))]
		protected void OnFindBoard_Trello()
		{
			NetworkMgr.In.GetTrelloBoard(TrelloCoreKey,(resultList) =>
			{
				_ShowResult_Trello(resultList);
			});
		}

		[HorizontalGroup("Network/Trello/3",Order = 3),SerializeField,EnableIf(nameof(IsExistTrello))]
		private string m_trelloTestId = null;

		private bool HasTestId => IsExistTrello && !m_trelloTestId.IsEmpty();

		[HorizontalGroup("Network/Trello/4",Order = 4),Button("Get List",ButtonSizes.Large),EnableIf(nameof(HasTestId))]
		protected void OnFindList_Trello()
		{
			NetworkMgr.In.GetTrelloList(TrelloCoreKey,m_trelloTestId,(resultList) =>
			{
				_ShowResult_Trello(resultList);
			});
		}

		[HorizontalGroup("Network/Trello/4",Order = 4),Button("Get Card",ButtonSizes.Large),EnableIf(nameof(HasTestId))]
		protected void OnFindCard_Trello()
		{
			NetworkMgr.In.GetTrelloCard(TrelloCoreKey,m_trelloTestId,(resultList) =>
			{
				_ShowResult_Trello(resultList);
			});
		}

		[HorizontalGroup("Network/Trello/5",Order = 5),Button("Post List",ButtonSizes.Large),EnableIf(nameof(HasTestId))]
		protected void OnPostList_Trello()
		{
			NetworkMgr.In.PostTrelloList(m_trelloTestId,"Test");
		}

		[HorizontalGroup("Network/Trello/5",Order = 5),Button("Post Card",ButtonSizes.Large),EnableIf(nameof(HasTestId))]
		protected void OnPostCard_Trello()
		{
			NetworkMgr.In.PostTrelloCard(m_trelloTestId,"Test","Description");
		}

		[HorizontalGroup("Network/Trello/6",Order = 6),SerializeField,TableList(HideToolbar = true,AlwaysExpanded = true),ShowIf(nameof(IsShowTrelloResultList)),EnableIf(nameof(IsExistTrello))]
		private List<ResultData> m_trelloDataList = new();

		private bool IsShowTrelloResultList => m_trelloDataList.Count > 0;

		private void _ShowResult_Trello(List<string> resultList)
		{
			m_trelloDataList.Clear();

			if(resultList.IsNullOrEmpty())
			{
				return;
			}

			for(var i=0;i<resultList.Count;i++)
			{
				var json = JObject.Parse(resultList[i]);

				m_trelloDataList.Add(new ResultData(json["name"].ToString(),json["id"].ToString()));
			}
		}
	}
}
#endif