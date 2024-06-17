#if UNITY_EDITOR
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using UnityEditor;
using System.Linq;
using UnityEditor.Build.Reporting;
using KZLib.KZAttribute;
using Cysharp.Threading.Tasks;
using System.Text.RegularExpressions;
using KZLib.KZNetwork;
using System.Collections.Generic;

public partial class BuildSettings : OuterBaseSettings<BuildSettings>
{
	private string AppFullPath => GetFullPath("App");

	private void InitializeApp() { }

	[TabGroup("빌드 설정","앱 설정",Order = 0)]
	[TitleGroup("빌드 설정/앱 설정/일반 설정",BoldTitle = false,Order = 0)]
	[BoxGroup("빌드 설정/앱 설정/일반 설정/0",Order = 0,ShowLabel = false)]
	[VerticalGroup("빌드 설정/앱 설정/일반 설정/0/0",Order = 0),ShowInInspector,LabelText("앱 경로"),KZRichText]
	protected string AppPath => "Builds/App/[BuildTarget]";

	[VerticalGroup("빌드 설정/앱 설정/일반 설정/0/1",Order = 1),SerializeField,LabelText("빌드 플랫폼")]
	private BuildPlatformType m_BuildPlatform = BuildPlatformType.Android;
	private bool IsAndroidBuild => m_BuildPlatform == BuildPlatformType.Android;

	[VerticalGroup("빌드 설정/앱 설정/일반 설정/0/2",Order = 2),ShowInInspector,LabelText("패키지 이름")]
	private string PackageName
	{
		get => PlayerSettings.applicationIdentifier;
		set
		{
			if(PlayerSettings.applicationIdentifier == value)
			{
				return;
			}

			var pattern = @"^com\.[a-zA-Z0-9]+\.[a-zA-Z0-9]+$";

			if(!Regex.IsMatch(value,pattern))
			{
				return;
			}

			CommonUtility.ChangePackageName(value);
		}
	}

	[VerticalGroup("빌드 설정/앱 설정/일반 설정/0/3",Order = 3),ShowInInspector,LabelText("씬 리스트"),KZRichText]
	private string SceneName => string.Join("|",EditorBuildSettings.scenes.Where(x=>x.enabled).Select(y=>CommonUtility.GetOnlyName(y.path)));

	[VerticalGroup("빌드 설정/앱 설정/일반 설정/0/4",Order = 4),ShowInInspector,LabelText("개발자 빌드")]
	private bool DevelopmentBuild
	{
		get => EditorUserBuildSettings.development;
		set
		{
			if(EditorUserBuildSettings.development == value)
			{
				return;
			}

			EditorUserBuildSettings.development = value;
		}
	}

	[BoxGroup("빌드 설정/앱 설정/일반 설정/3",Order = 3,ShowLabel = false),SerializeField,LabelText("빌드 후 폴더 열기")]
	private bool m_OpenFolderAfterAppBuild = false;

	[BoxGroup("빌드 설정/앱 설정/일반 설정/3",Order = 3,ShowLabel = false),SerializeField,LabelText("빌드 후 업로드 하기"),DisableIf(nameof(DisableNow))]
	private bool m_UploadAfterAppBuild = false;
	[BoxGroup("빌드 설정/앱 설정/일반 설정/3",Order = 3,ShowLabel = false),SerializeField,LabelText("업로드 방법"),ShowIf(nameof(m_UploadAfterAppBuild))]
	private UploadType m_UploadType = UploadType.GoogleDrive;
	private bool UseGoogleDrive => m_UploadAfterAppBuild && m_UploadType.HasFlag(UploadType.GoogleDrive);

	[BoxGroup("빌드 설정/앱 설정/일반 설정/3",Order = 3,ShowLabel = false),SerializeField,LabelText("구글 드라이브 아이디"),ShowIf(nameof(UseGoogleDrive))]
	private string m_GoogleDriveFolderId = null;

	[TitleGroup("빌드 설정/앱 설정/AOS 설정",BoldTitle = false,Order = 1)]
	[BoxGroup("빌드 설정/앱 설정/AOS 설정/0",Order = 0,ShowLabel = false),ShowInInspector,LabelText("번들 버전 코드"),ShowIf(nameof(IsAndroidBuild))]
	protected int BundleVersionCode { get => PlayerSettings.Android.bundleVersionCode; set => PlayerSettings.Android.bundleVersionCode = value; }

	[VerticalGroup("빌드 설정/앱 설정/버튼",Order = 30),Button("앱 빌드",ButtonSizes.Large),PropertyTooltip("앱을 빌드합니다.")]
	public void OnBuildApp()
	{
		if(!CommonUtility.DisplayCheckBeforeExecute("앱 빌드"))
		{
			return;
		}

		BuildAsync(BuildAppAsync).Forget();
	}

	private async UniTask BuildAppAsync()
	{
		Log.Build.I("앱 빌드 시작");

		var sceneArray = EditorBuildSettings.scenes.Where(x=>x.enabled).Select(y=>y.path).ToArray();
		var appName = string.Format("{0} [{1}_{2}]",Application.productName,GameSettings.In.GameMode.First(),GameSettings.In.GameVersion);

		foreach(var target in CommonUtility.GetAllEnumValues<BuildTarget>())
		{
			if(!Enum.TryParse(target.ToString(),true,out BuildPlatformType platform))
			{
				continue;
			}

			if(!m_BuildPlatform.HasFlag(platform))
			{
				continue;
			}

			Log.Build.I("{0} 빌드 시작",target);

			var appPath = GetAppPath(appName);
			var report = BuildPipeline.BuildPlayer(GetBuildPlayerOptions(sceneArray,target,appPath));

			if(report.summary.result == BuildResult.Succeeded)
			{
				Log.Build.I("{0} 빌드 성공 [걸린 시간 : {1} 경로 : {2}]",target,report.summary.totalTime,appPath);

				if(m_OpenFolderAfterAppBuild)
				{
					Log.Build.I("빌드가 있는 폴더를 오픈 합니다.");

					CommonUtility.OpenFolder(AppFullPath);
				}

				if(m_UploadAfterAppBuild)
				{
					var fileGroup = ConvertToFileGroup(appPath);

					if(UseGoogleDrive && !m_GoogleDriveFolderId.IsEmpty())
					{
						Log.Build.I("앱을 구글 드라이브에 업로드합니다.");

						var request = GoogleDrivePostCreateFileWebRequest.Create(m_GoogleDriveFolderId,fileGroup.Item1,fileGroup.Item2);

						var data = await request.SendAsync();

						if(!data.Result)
						{
							Log.Build.I("업로드에 실패 하였습니다.");

							return;
						}

						Log.Build.I("업로드에 성공 하였습니다. {0}",string.Format(@"https://drive.google.com/drive/folders/{0}",m_GoogleDriveFolderId));
					}

					if(m_UploadType.HasFlag(UploadType.Azure))
					{
						Log.Build.I("앱을 애저에 업로드합니다.");
					}

					await UniTask.Yield();
				}
			}
			else
			{
				var textList = new List<string>();

				foreach(var step in report.steps)
				{
					foreach(var message in step.messages)
					{
						if(message.type == LogType.Log || message.type == LogType.Warning)
						{
							continue;
						}

						textList.Add(string.Format("<{0}>{1}",message.type,message.content));
					}
				}

				var text = string.Join("\n",textList);

				throw new Exception(string.Format("{0} 빌드에 오류가 있습니다.\n{1}",target,text));
			}
		}
	}

	private BuildPlayerOptions GetBuildPlayerOptions(string[] _sceneArray,BuildTarget _target,string _appPath)
	{
		var buildOption = BuildOptions.DetailedBuildReport;

		if(DevelopmentBuild)
		{
			buildOption |= BuildOptions.Development;
		}

		var playerOption = new BuildPlayerOptions
		{
			scenes = _sceneArray,
			target = _target,
			locationPathName = _appPath,
			options = buildOption,
		};

		return playerOption;
	}

	private string GetAppPath(string _appName)
	{
		switch(CurrentBuildTarget)
		{
			case BuildTarget.Android:
			{
				return CommonUtility.PathCombine(AppFullPath,string.Format("{0}.apk",_appName));
			}
			case BuildTarget.iOS:
			{
				return CommonUtility.PathCombine(AppFullPath,string.Format("{0}",_appName));
			}
			default:
			{
				return CommonUtility.PathCombine(AppFullPath,string.Format("{0}",_appName));
			}
		}
	}
}
#endif