#if UNITY_EDITOR
using KZLib.KZNetwork;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace KZLib.KZWindow
{
	public partial class NetworkTestWindow : OdinEditorWindow
	{
		[TabGroup("Network","GoogleSheet")]
		[HorizontalGroup("Network/GoogleSheet/0",Order = 0),SerializeField]
		private string m_googleSheetFileId = null;
		private bool IsExistGoogleSheet => !m_googleSheetFileId.IsEmpty();

		[HorizontalGroup("Network/GoogleSheet/1",Order = 1),SerializeField,EnableIf(nameof(IsExistGoogleSheet))]
		private int m_googleSheetIndex = 0;
		[HorizontalGroup("Network/GoogleSheet/2",Order = 2),Button("Get Sheet",ButtonSizes.Large),EnableIf(nameof(IsExistGoogleSheet))]
		protected void OnGetSheet_GoogleSheet()
		{
			m_googleSheetArray = new string[1,1] { { "Loading..." } };

			void _CreateSheet(string result)
			{
				if(result.IsEmpty())
				{
					m_googleSheetArray = new string[1,1] { { "Empty" } };
				}
				else
				{
					var rowArray = result.Split('\n');

					m_googleSheetArray = new string[rowArray[0].Split('\t').Length,rowArray.Length];

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

			WebRequestManager.In.GetGoogleSheet("Test",m_googleSheetIndex,_CreateSheet);
		}

		[HorizontalGroup("Network/GoogleSheet/2",Order = 2),Button("Post Sheet",ButtonSizes.Large),EnableIf(nameof(IsExistGoogleSheet))]
		protected void OnPostText_GoogleSheet()
		{
			WebRequestManager.In.PostGoogleSheetAddRow("Test",m_googleSheetIndex,"Test\tAAA\tBBB\tCCC");

			OnGetSheet_GoogleSheet();
		}

		[HorizontalGroup("Network/GoogleSheet/5",Order = 5),SerializeField,TableMatrix(IsReadOnly = true),EnableIf(nameof(IsExistGoogleSheet))]
		private string[,] m_googleSheetArray = new string[0,0];
	}
}
#endif