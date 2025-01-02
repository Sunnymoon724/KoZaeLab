using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public partial class NetworkSettings : InnerBaseSettings<NetworkSettings>
{
	[TabGroup("Network","GoogleSheet")]
	[TitleGroup("Network/GoogleSheet/General",BoldTitle = false,Order = 0)]
	[VerticalGroup("Network/GoogleSheet/General/0",Order = 0),SerializeField,LabelText("Use GoogleSheet"),ToggleLeft]
	private bool m_UseGoogleSheet = false;

	[VerticalGroup("Network/GoogleSheet/General/1",Order = 1),SerializeField,LabelText("File Id Dict"),DictionaryDrawerSettings(KeyLabel = "Title",ValueLabel = "Id"),ShowIf(nameof(m_UseGoogleSheet))]
	private Dictionary<string,string> m_GoogleSheetFileIdDict = new();

	private bool IsExistGoogleSheet => m_UseGoogleSheet && m_GoogleSheetFileIdDict.Count > 0;

	public string GetGoogleSheetFileId(string _key)
	{
		return IsExistGoogleSheet ? m_GoogleSheetFileIdDict.FindOrFirst(x=>x.Key.Contains(_key)).Value : string.Empty;
	}

#if UNITY_EDITOR
	[TitleGroup("Network/GoogleSheet/Test",BoldTitle = false,Order = 2)]
	[VerticalGroup("Network/GoogleSheet/Test/0",Order = 0),ShowInInspector,LabelText("Sheet Index"),ShowIf(nameof(m_UseGoogleSheet)),EnableIf(nameof(IsExistGoogleSheet))]
	private readonly int m_GoogleSheetIndex = 0;
	[HorizontalGroup("Network/GoogleSheet/Test/1",Order = 1),Button("Get Sheet",ButtonSizes.Large),ShowIf(nameof(m_UseGoogleSheet)),EnableIf(nameof(IsExistGoogleSheet))]
	protected void OnGetSheet_GoogleSheet()
	{
		GetSheet_GoogleSheetAsync().Forget();
	}

	private async UniTask GetSheet_GoogleSheetAsync()
	{
		m_GoogleSheetArray = new string[1,1] { { "Loading..." } };

		var result = await CommonUtility.FindSheet_GoogleSheetAsync("Test",m_GoogleSheetIndex);

		if(result.IsEmpty())
		{
			m_GoogleSheetArray = new string[1,1] { { "Empty" } };
		}
		else
		{
			var rowArray = result.Split('\n');

			m_GoogleSheetArray = new string[rowArray[0].Split('\t').Length,rowArray.Length];

			for(var i=0;i<rowArray.Length;i++)
			{
				var cellArray = rowArray[i].Split('\t');

				for(var j=0;j<cellArray.Length;j++)
				{
					m_GoogleSheetArray[j,i] = cellArray[j];
				}
			}
		}
	}

	[HorizontalGroup("Network/GoogleSheet/Test/1",Order = 1),Button("Post Sheet",ButtonSizes.Large),ShowIf(nameof(m_UseGoogleSheet)),EnableIf(nameof(IsExistGoogleSheet))]
	protected void OnPostText_GoogleSheet()
	{
		PostText_GoogleSheet().Forget();
	}

	private async UniTaskVoid PostText_GoogleSheet()
	{
		await CommonUtility.PostAddRow_GoogleSheetAsync("Test",m_GoogleSheetIndex,"Test\tAAA\tBBB\tCCC");

		await GetSheet_GoogleSheetAsync();
	}

	[HorizontalGroup("Network/GoogleSheet/Test/5",Order = 5),ShowInInspector,TableMatrix(IsReadOnly = true),ShowIf(nameof(m_UseGoogleSheet)),EnableIf(nameof(IsExistGoogleSheet))]
	private string[,] m_GoogleSheetArray = new string[0,0];
#endif
}