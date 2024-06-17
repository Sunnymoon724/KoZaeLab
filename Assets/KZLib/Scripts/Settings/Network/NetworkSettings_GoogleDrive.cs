using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using KZLib.KZNetwork;
using Sirenix.OdinInspector;
using UnityEngine;

public partial class NetworkSettings : InSideSingletonSO<NetworkSettings>
{
	[TabGroup("통신 설정","구글드라이브")]
	[TitleGroup("통신 설정/구글드라이브/사용 설정",BoldTitle = false,Order = 0),SerializeField,LabelText("구글드라이브 사용"),ToggleLeft]
	private bool m_UseGoogleDrive = false;

	[TitleGroup("통신 설정/구글드라이브/기본 설정",BoldTitle = false,Order = 1)]
	[VerticalGroup("통신 설정/구글드라이브/기본 설정/0"),ShowInInspector,LabelText("폴더 아이디"),EnableIf(nameof(m_UseGoogleDrive))]
	private readonly string m_GoogleDriveFolderId = null;

#if UNITY_EDITOR
	[TitleGroup("통신 설정/구글드라이브/테스트",BoldTitle = false,Order = 2)]
	[BoxGroup("통신 설정/구글드라이브/테스트/0",ShowLabel = false)]
	[HorizontalGroup("통신 설정/구글드라이브/테스트/0/버튼"),Button("폴더 리스트 확인하기",ButtonSizes.Large),EnableIf(nameof(IsExistGoogleDrive))]
	private void OnGetFolderListTest_GoogleDrive()
	{
		GetFolderListTestAsync_GoogleDrive().Forget();
	}

	private async UniTaskVoid GetFolderListTestAsync_GoogleDrive()
	{
		var request = GoogleDriveGetFolderListWebRequest.Create(m_GoogleDriveFolderId);

		Log.Test.I("드라이브 리스트를 얻어옵니다. 아이디 : {0}",m_GoogleDriveFolderId);

		var data = await request.SendAsync();

		if(!data.Result)
		{
			Log.Test.I("리스트 얻어오기를 실패 하였습니다.");

			return;
		}

		Log.Test.I("리스트 얻어오기를 성공 하였습니다.");

		m_GoogleDriveResultList.Clear();

		var rowArray = data.Text.Split('\n');

		for(var i=0;i<rowArray.Length;i++)
		{
			var cellArray = rowArray[i].Split('\t');

			m_GoogleDriveResultList.Add(new NameIdResultData(cellArray[0],cellArray[1]));
		}
	}

	[HorizontalGroup("통신 설정/구글드라이브/테스트/0/버튼"),Button("파일 업로드하기",ButtonSizes.Large),EnableIf(nameof(IsExistGoogleDrive))]
	private void OnPostPhotoTest_GoogleDrive()
	{
		PostPhotoTestAsync_GoogleDrive().Forget();
	}

	private async UniTaskVoid PostPhotoTestAsync_GoogleDrive()
	{
		var request = GoogleDrivePostCreateFileWebRequest.Create(m_GoogleDriveFolderId,"Ostrich.png",CommonUtility.GetTestImageData());

		Log.Test.I("테스트 이미지를 포스트 합니다. 아이디 : {0}",m_GoogleDriveFolderId);

		var data = await request.SendAsync();

		if(!data.Result)
		{
			Log.Test.I("테스트 이미지를 포스트에 실패 하였습니다.");

			return;
		}

		Log.Test.I("테스트 이미지를 포스트에 성공 하였습니다.");
	}

	private bool IsExistGoogleDrive => m_UseGoogleDrive && !m_GoogleDriveFolderId.IsEmpty();

	[HorizontalGroup("통신 설정/구글드라이브/테스트/0/결과",Order = 3),ShowInInspector,LabelText("결과 텍스트"),EnableIf(nameof(IsExistGoogleDrive)),TableList(HideToolbar = true,AlwaysExpanded = true,IsReadOnly = true)]
	private readonly List<NameIdResultData> m_GoogleDriveResultList = new();
#endif
}