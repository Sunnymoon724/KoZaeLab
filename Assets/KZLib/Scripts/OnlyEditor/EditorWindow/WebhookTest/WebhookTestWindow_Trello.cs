#if UNITY_EDITOR
using System.Collections.Generic;
using KZLib.Webhooks;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace KZLib.Windows
{
	/// <summary>
	/// Trello board/list/card tests for <see cref="WebhookTestWindow"/>.
	/// Get List expects a board id; Get Card expects a list id.
	/// </summary>
	public partial class WebhookTestWindow : OdinEditorWindow
	{
		[TabGroup("Network","Trello")]
		[HorizontalGroup("Network/Trello/0",Order = 0),SerializeField]
		private string m_trelloApiKey = null;
		[HorizontalGroup("Network/Trello/1",Order = 1),SerializeField]
		private string m_trelloToken = null;

		private bool IsExistTrello => !m_trelloApiKey.IsEmpty() && !m_trelloToken.IsEmpty();

		/// <summary>
		/// Query string fragment passed to Trello REST requests from the test credentials.
		/// </summary>
		public string TrelloCoreKey => IsExistTrello ? $"key={m_trelloApiKey}&token={m_trelloToken}" : string.Empty;

		[HorizontalGroup("Network/Trello/2",Order = 2),Button("Get Board",ButtonSizes.Large),EnableIf(nameof(IsExistTrello))]
		protected void OnFindBoard_Trello()
		{
			_BeginTrelloResultRequest();

			void _ShowResult(List<string> resultList)
			{
				_RunOnEditorMainThread(() => _ShowResult_Trello(resultList));
			}

			WebhookManager.In.GetTrelloBoard(TrelloCoreKey,_ShowResult);
		}

		[HorizontalGroup("Network/Trello/3",Order = 3),LabelText("Board Id"),SerializeField,EnableIf(nameof(IsExistTrello))]
		private string m_trelloBoardId = null;

		private bool HasBoardId => IsExistTrello && !m_trelloBoardId.IsEmpty();

		[HorizontalGroup("Network/Trello/4",Order = 4),Button("Get List",ButtonSizes.Large),EnableIf(nameof(HasBoardId))]
		protected void OnFindList_Trello()
		{
			_BeginTrelloResultRequest();

			void _ShowResult(List<string> resultList)
			{
				_RunOnEditorMainThread(() => _ShowResult_Trello(resultList));
			}

			WebhookManager.In.GetTrelloList(TrelloCoreKey,m_trelloBoardId,_ShowResult);
		}

		[HorizontalGroup("Network/Trello/4",Order = 4),Button("Post List",ButtonSizes.Large),EnableIf(nameof(HasBoardId))]
		protected void OnPostList_Trello()
		{
			WebhookManager.In.PostTrelloList(TrelloCoreKey,m_trelloBoardId,"Test");
		}

		[HorizontalGroup("Network/Trello/5",Order = 5),LabelText("List Id"),SerializeField,EnableIf(nameof(IsExistTrello))]
		private string m_trelloListId = null;

		private bool HasListId => IsExistTrello && !m_trelloListId.IsEmpty();

		[HorizontalGroup("Network/Trello/6",Order = 6),Button("Get Card",ButtonSizes.Large),EnableIf(nameof(HasListId))]
		protected void OnFindCard_Trello()
		{
			_BeginTrelloResultRequest();

			void _ShowResult(List<string> resultList)
			{
				_RunOnEditorMainThread(() => _ShowResult_Trello(resultList));
			}

			WebhookManager.In.GetTrelloCard(TrelloCoreKey,m_trelloListId,_ShowResult);
		}

		[HorizontalGroup("Network/Trello/6",Order = 6),Button("Post Card",ButtonSizes.Large),EnableIf(nameof(HasListId))]
		protected void OnPostCard_Trello()
		{
			WebhookManager.In.PostTrelloCard(TrelloCoreKey,m_trelloListId,"Test","Description");
		}

		[HorizontalGroup("Network/Trello/7",Order = 7),SerializeField,TableList(HideToolbar = true,AlwaysExpanded = true),ShowIf(nameof(IsShowTrelloResultList)),EnableIf(nameof(IsExistTrello))]
		private List<ResultInfo> m_trelloResultInfoList = new();

		private bool IsShowTrelloResultList => m_trelloResultInfoList.Count > 0;

		private void _BeginTrelloResultRequest()
		{
			m_trelloResultInfoList.Clear();
			m_trelloResultInfoList.Add(ResultInfo.CreatePlaceholder("Loading..."));
		}

		/// <summary>
		/// Populates the shared Trello result table from webhook JSON rows.
		/// </summary>
		private void _ShowResult_Trello(List<string> resultList)
		{
			m_trelloResultInfoList.Clear();

			if(resultList.IsNullOrEmpty())
			{
				m_trelloResultInfoList.Add(ResultInfo.CreatePlaceholder(c_emptyResultName));

				return;
			}

			for(var i=0;i<resultList.Count;i++)
			{
				if(!_TryCreateResultInfo(resultList[i],out var resultInfo))
				{
					continue;
				}

				m_trelloResultInfoList.Add(resultInfo);
			}

			if(m_trelloResultInfoList.Count == 0)
			{
				m_trelloResultInfoList.Add(ResultInfo.CreatePlaceholder("(No parseable items)"));
			}
		}
	}
}
#endif
