#if UNITY_EDITOR
using KZLib.Webhooks;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace KZLib.Windows
{
	/// <summary>
	/// Google Sheet read/write tests for <see cref="WebhookTestWindow"/>.
	/// </summary>
	public partial class WebhookTestWindow : OdinEditorWindow
	{
		[TabGroup("Network","GoogleSheet")]
		[HorizontalGroup("Network/GoogleSheet/0",Order = 0),SerializeField]
		private string m_googleSheetFileId = null;
		private bool IsExistGoogleSheet => !m_googleSheetFileId.IsEmpty();

		[HorizontalGroup("Network/GoogleSheet/1",Order = 1),SerializeField,EnableIf(nameof(IsExistGoogleSheet))]
		private int m_googleSheetIndex = 0;

		private bool IsValidGoogleSheetIndex => m_googleSheetIndex >= 0;
		private bool CanRequestGoogleSheet => IsExistGoogleSheet && IsValidGoogleSheetIndex;

		[HorizontalGroup("Network/GoogleSheet/2",Order = 2),Button("Get Sheet",ButtonSizes.Large),EnableIf(nameof(CanRequestGoogleSheet))]
		protected void OnGetSheet_GoogleSheet()
		{
			_SetGoogleSheetMessage("Loading...");

			void _CreateSheet(string result)
			{
				_RunOnEditorMainThread(() => _ApplyGoogleSheetResult(result));
			}

			WebhookManager.In.GetGoogleSheetByFileId(m_googleSheetFileId,m_googleSheetIndex,_CreateSheet);
		}

		[HorizontalGroup("Network/GoogleSheet/2",Order = 2),Button("Post Sheet",ButtonSizes.Large),EnableIf(nameof(CanRequestGoogleSheet))]
		protected async void OnPostText_GoogleSheet()
		{
			_SetGoogleSheetMessage("Posting...");

			await WebhookManager.In.PostGoogleSheetAddRowByFileIdAsync(m_googleSheetFileId,m_googleSheetIndex,"Test\tAAA\tBBB\tCCC");

			OnGetSheet_GoogleSheet();
		}

		[HorizontalGroup("Network/GoogleSheet/5",Order = 5),SerializeField,TableMatrix(IsReadOnly = true),EnableIf(nameof(IsExistGoogleSheet))]
		private string[,] m_googleSheetArray = new string[0,0];

		private void _SetGoogleSheetMessage(string message)
		{
			m_googleSheetArray = new string[1,1] { { message } };
		}

		/// <summary>
		/// Parses tab-separated sheet content into the Odin table matrix.
		/// </summary>
		private void _ApplyGoogleSheetResult(string result)
		{
			if(result.IsEmpty())
			{
				_SetGoogleSheetMessage("Empty");

				return;
			}

			var rowArray = result.Split('\n',System.StringSplitOptions.RemoveEmptyEntries);

			if(rowArray.Length == 0)
			{
				_SetGoogleSheetMessage("Empty");

				return;
			}

			var columnCount = 0;

			for(var i=0;i<rowArray.Length;i++)
			{
				var cellCount = rowArray[i].Split('\t').Length;

				if(cellCount > columnCount)
				{
					columnCount = cellCount;
				}
			}

			if(columnCount == 0)
			{
				_SetGoogleSheetMessage("Empty");

				return;
			}

			m_googleSheetArray = new string[columnCount,rowArray.Length];

			for(var i=0;i<rowArray.Length;i++)
			{
				var cellArray = rowArray[i].Split('\t');

				for(var j=0;j<cellArray.Length;j++)
				{
					m_googleSheetArray[j,i] = cellArray[j];
				}
			}
		}
	}
}
#endif
