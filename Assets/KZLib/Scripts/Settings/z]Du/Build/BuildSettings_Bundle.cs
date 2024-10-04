#if UNITY_EDITOR
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using Cysharp.Threading.Tasks;
using KZLib.KZAttribute;
using System;

public partial class BuildSettings : OuterBaseSettings<BuildSettings>
{
	private const string PROFILE_NAME = "KZBundle";
	
	private const string DefaultRemoteBundlePath = "http://localhost";

	private string BundleFullPath => GetFullPath("Bundle");

	private bool IsExistAddressable => AddressableAssetSettingsDefaultObject.Settings;

	public void InitializeBundleSetting(AddressableAssetSettings _setting)
	{	
		AddressableAssetSettingsDefaultObject.Settings.activeProfileId = GetProfileId(_setting.profileSettings);
	}

	private string GetProfileId(AddressableAssetProfileSettings _setting)
	{
		var profileId = _setting.GetProfileId(PROFILE_NAME);

		if(profileId.IsEmpty())
		{
			var nameList = _setting.GetAllProfileNames();

			_setting.AddProfile(PROFILE_NAME,_setting.GetProfileId(nameList[0]));

			profileId = _setting.GetProfileId(PROFILE_NAME);

			_setting.SetValue(profileId,"BuildTarget","[UnityEditor.EditorUserBuildSettings.activeBuildTarget]");
			_setting.SetValue(profileId,"Local.BuildPath",LocalBundlePath);
			_setting.SetValue(profileId,"Local.LoadPath",LocalBundlePath);
			_setting.SetValue(profileId,"Remote.BuildPath",LocalBundlePath);
			_setting.SetValue(profileId,"Remote.LoadPath",m_BundleLoadPath);
		}

		return profileId;
	}

	private void SetProfileSetting()
	{

	}

	private void InitializeBundle()
	{
		if(m_BundleLoadPath.IsEmpty())
		{
			m_BundleLoadPath = DefaultRemoteBundlePath;
		}
	}

	[SerializeField,HideInInspector]
	private string m_BundleLoadPath = null;

	[TabGroup("빌드 설정","번들 설정",Order = 1)]
	[TitleGroup("빌드 설정/번들 설정/일반 설정",BoldTitle = false,Order = 0)]
	[BoxGroup("빌드 설정/번들 설정/일반 설정/0",Order = 0,ShowLabel = false),ShowInInspector,LabelText("번들 경로"),KZRichText]
	private string LocalBundlePath => "Builds/Bundle/[BuildTarget]";

	[BoxGroup("빌드 설정/번들 설정/일반 설정/3",Order = 3,ShowLabel = false),SerializeField,LabelText("빌드 후 폴더 열기"),DisableIf(nameof(DisableNow))]
	private bool m_IsOpenFolderAfterBundleBuild = false;

	[BoxGroup("빌드 설정/번들 설정/일반 설정/3",Order = 3,ShowLabel = false),SerializeField,LabelText("빌드 후 업로드 하기"),DisableIf(nameof(DisableNow))]
	private bool m_IsUploadAfterBundleBuild = false;

	[BoxGroup("빌드 설정/번들 설정/일반 설정/3",Order = 3,ShowLabel = false),ShowInInspector,LabelText("리모트 번들 로드 경로"),DisableIf(nameof(DisableNow))]
	protected string BundleLoadPath
	{
		get => m_BundleLoadPath;
		set
		{
			m_BundleLoadPath = value;

			var setting = AddressableAssetSettingsDefaultObject.Settings;

			var profileId = GetProfileId(setting.profileSettings);
			setting.profileSettings.SetValue(profileId,"Remote.LoadPath",m_BundleLoadPath);
		}
	}

	[VerticalGroup("빌드 설정/번들 설정/버튼",Order = 30),Button("번들 빌드",ButtonSizes.Large),PropertyTooltip("번들을 빌드합니다."),DisableIf(nameof(DisableNow))]
	public void OnBuildBundle()
	{
		var setting = AddressableAssetSettingsDefaultObject.Settings;

		if(!UnityUtility.DisplayCheckBeforeExecute("번들 빌드"))
		{
			return;
		}

		BuildAsync(BuildBundAsync).Forget();
	}

	private async UniTask BuildBundAsync()
	{
		LogTag.Build.I("번들 빌드 시작");

		// var bundleName = string.Format("{0} [{1}_{2}]",Application.productName,GameSettings.In.GameVersion,DateTime.Now.ToString("MM-dd HH-mm"));

		// 모두 리모트로 변경
		var setting = AddressableAssetSettingsDefaultObject.Settings;
		var profileId = GetProfileId(setting.profileSettings);

		setting.profileSettings.SetValue(profileId,"Remote.LoadPath",string.Format("{0}/{1}",m_BundleLoadPath,CurrentBuildTargetToLower));
		
		AddressableAssetSettingsDefaultObject.Settings.activeProfileId = profileId;

		setting.RemoteCatalogBuildPath.SetVariableByName(setting,"Remote.BuildPath");
		setting.RemoteCatalogLoadPath.SetVariableByName(setting,"Remote.LoadPath");

		AddressableAssetSettings.CleanPlayerContent(AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilder);
		AddressableAssetSettings.BuildPlayerContent(out var report);

		//? 실패
		if(!report.Error.IsEmpty())
		{
			throw new Exception(string.Format("번들 빌드에 오류가 있습니다. [{0}]", report.Error));
		}

		//? 성공
		LogTag.Build.I("앱 빌드 성공 [걸린 시간 : {0:F3} 경로 : {1}]",report.Duration,BundleFullPath);

		if(m_IsOpenFolderAfterBundleBuild)
		{
			LogTag.Build.I("번들이 있는 폴더를 오픈 합니다.");

			FileUtility.Open(BundleFullPath);
		}

		if(m_IsUploadAfterBundleBuild)
		{
			LogTag.Build.I("번들을 업로드합니다.");

			await UniTask.Yield();

			// TODO 업로드 구현할 것

			// var storageName = NetworkSettings.In.AzureStorageName;
			// var accountKey = NetworkSettings.In.AzureAccountKey;
			// var containerName = CurrntBuildTargetToLower;

			// Log.Build.I("컨테이너 리스트를 확인합니다.");

			// var containerList = new List<string>();
			// var containerRequest = AzureGetListContainerWebRequest.Create(storageName,accountKey);
			// ,(dataList)=>
			// {
			// 	containerList.AddRange(dataList);
			// });
			
			// await containerRequest.SendAsync();

			// if(containerList.Contains(containerName))
			// {
			// 	Log.Build.I("{0}의 리스트를 확인합니다.",containerName);

			// 	var removeList = new List<string>();
			// 	var listRequest = AzureGetListBlobWebRequest.Create(storageName,accountKey,containerName);
			// 	// ,(dataList)=>
			// 	// {
			// 	// 	removeList.AddRange(dataList);
			// 	// });

			// 	await listRequest.SendAsync();

			// 	var fileArray = CommonUtility.GetAllFilePathInFolder(BundleFullPath);

			// 	Log.Build.I("{0}개의 파일을 업로드합니다.",fileArray.Length);

			// 	if(CommonUtility.DisplayCancelableProgressBar("파일을 업로드합니다.","",0.0f))
			// 	{
			// 		return;
			// 	}

			// 	for(var i=0;i<fileArray.Length;i++)
			// 	{
			// 		var fileName = CommonUtility.GetFileName(fileArray[i]);

			// 		if(removeList.Contains(fileName))
			// 		{
			// 			removeList.Remove(fileName);
			// 		}

			// 		var text = string.Format("{0}을 업로드합니다.[{1}/{2}]",fileName,i,fileArray.Length);

			// 		Log.Build.I(text);

			// 		if(CommonUtility.DisplayCancelableProgressBar("파일을 업로드합니다.",text,i,fileArray.Length))
			// 		{
			// 			return;
			// 		}

			// 		var uploadRequest = AzurePutAssetWebRequest.Create(storageName,accountKey,CommonUtility.ReadFile(fileArray[i]),containerName,fileName);

			// 		await uploadRequest.SendAsync();
			// 	}

			// 	Log.Build.I("파일을 업로드가 끝났습니다.");

			// 	if(removeList.Count > 0)
			// 	{
			// 		Log.Build.I("더미 파일을 삭제합니다.");
			// 		Log.Build.I("{0}개의 파일을 삭제합니다.",removeList.Count);

			// 		for(var i=0;i<removeList.Count;i++)
			// 		{
			// 			var text = string.Format("{0}을 삭제합니다.[{1}/{2}]",removeList[i],i,fileArray.Length);

			// 			Log.Build.I(text);

			// 			if(CommonUtility.DisplayCancelableProgressBar("파일을 업로드합니다.",text,i,removeList.Count))
			// 			{
			// 				return;
			// 			}

			// 			var deleteRequest = AzureDeleteBlobWebRequest.Create(storageName,accountKey,containerName,removeList[i]);

			// 			await deleteRequest.SendAsync();
			// 		}
			// 	}
			// }
			// else
			// {
			// 	Log.Build.I("{0}가 없어서 생성합니다.",containerName);

			// 	var createRequest = AzurePutContainerWebRequest.Create(storageName,accountKey,containerName);

			// 	await createRequest.SendAsync();

			// 	var fileArray = CommonUtility.GetAllFilePathInFolder(BundleFullPath);

			// 	Log.Build.I("{0}개의 파일을 업로드 합니다.",fileArray.Length);

			// 	for(var i=0;i<fileArray.Length;i++)
			// 	{
			// 		var fileName = CommonUtility.GetFileName(fileArray[i]);
			// 		var text = string.Format("{0}을 업로드합니다.[{1}/{2}]",fileName,i,fileArray.Length);

			// 		Log.Build.I(text);

			// 		if(CommonUtility.DisplayCancelableProgressBar("파일을 업로드합니다.",text,i,fileArray.Length))
			// 		{
			// 			return;
			// 		}

			// 		var uploadRequest = AzurePutAssetWebRequest.Create(storageName,accountKey,CommonUtility.ReadFile(fileArray[i]),containerName,fileName);
					
			// 		await uploadRequest.SendAsync();
			// 	}
			// }
		}
	}
}
#endif