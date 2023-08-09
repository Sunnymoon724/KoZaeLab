// #if UNITY_EDITOR
// using UnityEngine;
// using Sirenix.OdinInspector;
// using Newtonsoft.Json.Linq;
// using Newtonsoft.Json;
// using KZLib.Auth;

// public partial class AuthSettings : OutSideSingletonSO<AuthSettings>
// {
// 	public bool IsExistGoogleAuthData => GoogleData.IsExistAuthData;

// 	// private bool IsEnableProject => m_GoogleAuthData.PROJECT_ID.IsEmpty() == false;
// 	// public bool IsEmptyGoogleAuthData => string.Concat(m_GoogleAuthData.CLIENT_ID,m_GoogleAuthData.CLIENT_SECRET).IsEmpty() == true;	
// 	// public bool IsEmptyGoogleDriveAuthData => IsEmptyGoogleAuthData && string.Concat(m_GoogleAuthData.PROJECT_ID,m_GoogleAuthData.FOLDER_ID).IsEmpty() == true;

// 	[FoldoutGroup("인증 설정/구글 설정",Order = 1)]

// 	[HorizontalGroup("인증 설정/구글 설정/1",Order = 1),Button("구글 API 사이트 바로가기",ButtonSizes.Large),PropertyTooltip("구글 API 사이트로 이동합니다.")]
// 	private void OnEnterGoogleAPI_URL()
// 	{
// 		Application.OpenURL(@"http://console.developers.google.com/");
// 	}

// 	[HorizontalGroup("인증 설정/구글 설정/1",Order = 1),Button("인증 파일 가져오기",ButtonSizes.Large),PropertyTooltip("구글 인증 파일을 가져와 바로 파싱 합니다.")]
// 	private void OnGetGoogleAuthFile()
// 	{
// 		var text = Tools.GetJsonFile();

// 		if(text.IsEmpty() == true)
// 		{
// 			return;
// 		}

// 		try
// 		{
// 			GoogleData.SetGoogleData(JObject.Parse(text).First.First);

// 			SaveAuthData();
// 		}
// 		catch(JsonException _ex)
// 		{
// 			Log.Library.E(_ex);
// 		}
// 	}

// 	[HorizontalGroup("인증 설정/구글 설정/1",Order = 1),Button("인증 정보 초기화",ButtonSizes.Large),PropertyTooltip("인증 정보를 초기화 합니다.")]
// 	private void OnResetGoogleAuthData()
// 	{
// 		if(Tools.ShowPopUpBeforeExecute("구글 인증 정보를 초기화") == true)
// 		{
// 			AuthMgr.In.ClearAuthData(Global.GOOGLE_AUTH);
// 		}
// 	}

// 	[BoxGroup("인증 설정/구글 설정/2",Order = 2,ShowLabel = false)]
// 	[HorizontalGroup("인증 설정/구글 설정/2/0",Order = 0),ShowInInspector,LabelWidth(100),LabelText("CLIENT ID")]
// 	public string GoogleClientId { get => GoogleData.Client_Id; private set => SetGoogleClientId(value); }
// 	[HorizontalGroup("인증 설정/구글 설정/2/0",Order = 0),ShowInInspector,LabelWidth(100),LabelText("CLIENT SECRET")]
// 	public string GoogleClientSecret { get => GoogleData.Client_Secret; private set => SetGoogleClientSecret(value); }

// 	[HorizontalGroup("인증 설정/구글 설정/2/0",Order = 0),ShowInInspector,LabelWidth(100),LabelText("PROJECT ID")]
// 	public string GoogleProjectId { get => GoogleData.Project_Id; private set => SetGoogleProjectId(value); }


// 	[HorizontalGroup("인증 설정/구글 설정/2/1",Order = 1),ShowInInspector,LabelWidth(100),LabelText("AUTH URI"),DisplayAsString]
// 	public string GoogleAuthUri { get => GoogleData.Auth_URI; private set {} }

// 	[HorizontalGroup("인증 설정/구글 설정/2/1",Order = 1),ShowInInspector,LabelWidth(100),LabelText("TOKEN URI"),DisplayAsString]
// 	public string GoogleTokenUri { get => GoogleData.Token_URI; private set {} }


// 	[HorizontalGroup("인증 설정/구글 설정/2/2",Order = 2),ShowInInspector,LabelWidth(100),LabelText("ACCESS TOKEN"),DisplayAsString]
// 	public string GoogleAccessToken  { get => GoogleData.Access_Token; private set {} }

// 	[HorizontalGroup("인증 설정/구글 설정/2/2",Order = 2),ShowInInspector,LabelWidth(100),LabelText("REFRESH TOKEN"),DisplayAsString]
// 	public string GoogleRefreshToken  { get => GoogleData.Refresh_Token; private set {} }

// 	public void SetGoogleToken(bool _accessOnly,string _accessToken,string _refreshToken)
// 	{
// 		if(_accessOnly == true)
// 		{
// 			GoogleData.SetToken(_accessToken);
// 		}
// 		else
// 		{
// 			GoogleData.SetToken(_accessToken,_refreshToken);
// 		}

// 		SaveAuthData();
// 	}

// 	private void SetGoogleClientId(string _client_Id)
// 	{
// 		GoogleData.Client_Id = _client_Id;

// 		SaveAuthData();
// 	}
	
// 	private void SetGoogleClientSecret(string _client_Secret)
// 	{
// 		GoogleData.Client_Secret = _client_Secret;

// 		SaveAuthData();
// 	}

// 	private void SetGoogleProjectId(string _project_Id)
// 	{
// 		GoogleData.Project_Id = _project_Id;

// 		SaveAuthData();
// 	}
// }

// #endif