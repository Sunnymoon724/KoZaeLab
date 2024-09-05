using Cysharp.Threading.Tasks;
using KZLib.KZNetwork;
using Sirenix.OdinInspector;
using UnityEngine;

public partial class NetworkSettings : InnerBaseSettings<NetworkSettings>
{
	[TabGroup("통신 설정","구글시트")]
	[TitleGroup("통신 설정/구글시트/사용 설정",BoldTitle = false,Order = 0),SerializeField,LabelText("구글시트 사용"),ToggleLeft]
	private bool m_UseGoogleSheet = false;

	[TitleGroup("통신 설정/구글시트/기본 설정",BoldTitle = false,Order = 1)]
	[BoxGroup("통신 설정/구글시트/기본 설정/0",ShowLabel = false),SerializeField,LabelText("파일 아이디"),EnableIf(nameof(m_UseGoogleSheet))]
	private string m_GoogleSheetFileId = null;

    public string GoogleSheetFileId => m_UseGoogleSheet ? m_GoogleSheetFileId : string.Empty;

#if UNITY_EDITOR
#pragma warning disable IDE0051
    [TitleGroup("통신 설정/구글시트/테스트",BoldTitle = false,Order = 2)]
	[BoxGroup("통신 설정/구글시트/테스트/0",ShowLabel = false)]
	[VerticalGroup("통신 설정/구글시트/테스트/0/인덱스",Order = 0),ShowInInspector,LabelText("시트 인덱스"),EnableIf(nameof(IsExistGoogleSheet))]
	private int m_GoogleSheetIndex = 0;
	[HorizontalGroup("통신 설정/구글시트/테스트/0/버튼",Order = 1),Button("시트 내용 확인하기",ButtonSizes.Large),EnableIf(nameof(IsExistGoogleSheet))]
	private void OnGetText_GoogleSheet()
	{
		GetText_GoogleSheetAsync().Forget();
	}

	private async UniTask GetText_GoogleSheetAsync()
	{
		m_GoogleSheetResultArray = new string[1,1] { { "Loading..." } };

		var result = await WebRequestUtility.GetSheet_GoogleSheetAsync(GoogleSheetFileId,m_GoogleSheetIndex);

		if(result.IsEmpty())
		{
			m_GoogleSheetResultArray = new string[1,1] { { "Empty" } };
		}
		else
		{
			var rowArray = result.Split('\n');

			m_GoogleSheetResultArray = new string[rowArray[0].Split('\t').Length,rowArray.Length];

			for(var i=0;i<rowArray.Length;i++)
			{
				var cellArray = rowArray[i].Split('\t');

				for(var j=0;j<cellArray.Length;j++)
				{
					m_GoogleSheetResultArray[j,i] = cellArray[j];
				}
			}
		}
	}

	[HorizontalGroup("통신 설정/구글시트/테스트/0/버튼",Order = 1),Button("시트 내용 작성하기",ButtonSizes.Large),EnableIf(nameof(IsExistGoogleSheet))]
	private void OnPostText_GoogleSheet()
	{
		PostText_GoogleSheet().Forget();
	}

	private async UniTask PostText_GoogleSheet()
	{
		await WebRequestUtility.PostAppendRow_GoogleSheetAsync(GoogleSheetFileId,m_GoogleSheetIndex,"Test\tAAA\tBBB\tCCC");

		await GetText_GoogleSheetAsync();
	}

	private bool IsExistGoogleSheet => m_UseGoogleSheet && !GoogleSheetFileId.IsEmpty();

	[HorizontalGroup("통신 설정/구글시트/테스트/0/결과",Order = 3),ShowInInspector,LabelText("결과 텍스트"),EnableIf(nameof(IsExistGoogleSheet)),TableMatrix(IsReadOnly = true)]
	private string[,] m_GoogleSheetResultArray = new string[0,0];
#pragma warning restore IDE0051
#endif
}