// using Cysharp.Threading.Tasks;
// using KZLib.KZNetwork;
// using Sirenix.OdinInspector;
// using UnityEngine;

// //! 에저는 애셋번들 업로드 용도로만 사용하고 있으므로 현재는 Editor 전용이다.

// public partial class NetworkSettings : InSideSingletonSO<NetworkSettings>
// {
// #if UNITY_EDITOR
// 	[TabGroup("통신 설정","애저")]
// 	[TitleGroup("통신 설정/애저/사용 설정",BoldTitle = false,Order = 0),SerializeField,LabelText("애져 사용"),ToggleLeft]
// 	private bool m_UseAzure = false;

// 	[TitleGroup("통신 설정/애저/기본 설정",BoldTitle = false,Order = 1)]
// 	[BoxGroup("통신 설정/애저/기본 설정/0",ShowLabel = false),SerializeField,LabelText("스토리지 이름"),EnableIf("m_UseAzure")]
// 	private string m_AzureStorageName = null;
// #endif

// 	public string AzureStorageName
// 	{
// 		get
// 		{
// #if UNITY_EDITOR
// 			return m_UseAzure ? m_AzureStorageName : string.Empty;
// #else
// 			return string.Empty;
// #endif
// 		}
// 	}

// #if UNITY_EDITOR
// 	[BoxGroup("통신 설정/애저/기본 설정/0",ShowLabel = false),SerializeField,LabelText("계정 키"),EnableIf("m_UseAzure")]
// 	private string m_AzureAccountKey = null;
// #endif

// 	public string AzureAccountKey
// 	{
// 		get
// 		{
// #if UNITY_EDITOR
// 			return m_UseAzure ? m_AzureAccountKey : string.Empty;
// #else
// 			return string.Empty;
// #endif
// 		}
// 	}

// #if UNITY_EDITOR
// 	[TitleGroup("통신 설정/애저/테스트",BoldTitle = false,Order = 2)]
// 	[BoxGroup("통신 설정/애저/테스트/버튼",ShowLabel = false),Button("이미지 업로드 테스트",ButtonSizes.Large),EnableIf("IsExistAzure")]
// 	private void OnSendUploadTest()
// 	{
// 		SendTestAsync().Forget();
// 	}

// 	private async UniTaskVoid SendTestAsync()
// 	{
// 		var containerName = "test";
// 		var listRequest = AzurePutContainerWebRequest.Create(m_AzureStorageName,m_AzureAccountKey,containerName);

// 		await listRequest.SendAsync();

// 		var uploadRequest = AzurePutImageWebRequest.Create(m_AzureStorageName,m_AzureAccountKey,CommonUtility.GetTestImageData(),containerName,"Ostrich.png");

// 		uploadRequest.Send();
// 	}

// 	private bool IsExistAzure => m_UseAzure && !string.Concat(m_AzureStorageName,m_AzureAccountKey).IsEmpty();
// #endif
// }