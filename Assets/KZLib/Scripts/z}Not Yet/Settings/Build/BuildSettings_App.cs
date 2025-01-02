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
using System.Collections.Generic;

public partial class BuildSettings : OuterBaseSettings<BuildSettings>
{
	private string AppFullPath => GetFullPath("App");

	[TabGroup("Build","App",Order = 0)]
	[TitleGroup("Build/App/General",BoldTitle = false,Order = 0)]
	[BoxGroup("Build/App/General/Default",Order = 0,ShowLabel = false)]
	[VerticalGroup("Build/App/General/Default/0",Order = 0),ShowInInspector,LabelText("Game Version")]
	public string GameVersion
	{
		get => PlayerSettings.bundleVersion;
		set
		{
			if(PlayerSettings.bundleVersion == value)
			{
				return;
			}

			PlayerSettings.bundleVersion = value;
		}
	}
	[VerticalGroup("Build/App/General/Default/1",Order = 1),ShowInInspector,LabelText("App Path"),KZRichText]
	protected string AppPath => "Builds/App/[BuildTarget]";

	[VerticalGroup("Build/App/General/Default/2",Order = 2),SerializeField,LabelText("Build Platform")]
	private BuildPlatformType m_BuildPlatform = BuildPlatformType.Android;
	private bool IsAndroidBuild => m_BuildPlatform == BuildPlatformType.Android;

	[VerticalGroup("Build/App/General/Default/3",Order = 3),ShowInInspector,LabelText("Package Name")]
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

	[VerticalGroup("Build/App/General/Default/4",Order = 4),ShowInInspector,LabelText("Scene Name"),KZRichText]
	private string SceneName
	{
		get
		{
			var sceneGroup = EditorBuildSettings.scenes.Where(x=>x.enabled).Select(y=>CommonUtility.GetOnlyName(y.path));

			return $"{string.Join("|",sceneGroup)}  [Count : {sceneGroup.Count()}]";
		}
	}

	[VerticalGroup("Build/App/General/Default/5",Order = 5),ShowInInspector,LabelText("Development Build")]
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

	[BoxGroup("Build/App/General/After",Order = 0,ShowLabel = false)]
	[VerticalGroup("Build/App/General/After/0",Order = 0),SerializeField,LabelText("Open Folder After App Build")]
	private bool m_OpenFolderAfterAppBuild = false;

	[VerticalGroup("Build/App/General/After/1",Order = 1),SerializeField,LabelText("Upload After App Build"),DisableIf(nameof(DisableNow))]
	private bool m_UploadAfterAppBuild = false;
	[VerticalGroup("Build/App/General/After/2",Order = 2),SerializeField,LabelText("Upload Type"),ShowIf(nameof(m_UploadAfterAppBuild))]
	private UploadType m_UploadType = UploadType.GoogleDrive;
	private bool UseGoogleDrive => m_UploadAfterAppBuild && m_UploadType.HasFlag(UploadType.GoogleDrive);

	// [VerticalGroup("Build/App/General/After/3",Order = 3),SerializeField,LabelText("Google Drive Folder Id"),ShowIf(nameof(UseGoogleDrive))]
	// private string m_GoogleDriveFolderId = null;

	[TitleGroup("Build/App/AOS",BoldTitle = false,Order = 1)]
	[BoxGroup("Build/App/AOS/0",Order = 0,ShowLabel = false),ShowInInspector,LabelText("Bundle Version Code"),ShowIf(nameof(IsAndroidBuild))]
	protected int BundleVersionCode { get => PlayerSettings.Android.bundleVersionCode; set => PlayerSettings.Android.bundleVersionCode = value; }

	[VerticalGroup("Build/App/Button",Order = 30),Button("Build App",ButtonSizes.Large)]
	public void OnBuildApp()
	{
		if(!CommonUtility.DisplayCheckBeforeExecute("App Build"))
		{
			return;
		}

		BuildAsync(BuildAppAsync).Forget();
	}

	private async UniTask BuildAppAsync()
	{
		LogTag.Build.I("Start App Build");

		var sceneArray = EditorBuildSettings.scenes.Where(x=>x.enabled).Select(y=>y.path).ToArray();
		var appName = $"{Application.productName} [{GameSettings.In.GameMode.First()}_{GameVersion}]";

		foreach(var name in Enum.GetNames(typeof(BuildTarget)))
		{
			var target = name.ToEnum<BuildTarget>();
			var platform = name.ToEnum(BuildPlatformType.None);

			if(platform == BuildPlatformType.None || !m_BuildPlatform.HasFlag(platform))
			{
				continue;
			}

			LogTag.Build.I($"{target} build start.");

			var appPath = GetAppPath(appName);
			var report = BuildPipeline.BuildPlayer(GetBuildPlayerOptions(sceneArray,target,appPath));

			if(report.summary.result == BuildResult.Succeeded)
			{
				LogTag.Build.I($"{target} build success. [duration : {report.summary.totalTime} / path {appPath}]");

				if(m_OpenFolderAfterAppBuild)
				{
					LogTag.Build.I("Open folder.");

					CommonUtility.Open(AppFullPath);
				}

				if(m_UploadAfterAppBuild)
				{
					LogTag.Build.I("Upload app");

					// var (FileName,FileData) = ConvertToFileGroup(appPath);

					// if(UseGoogleDrive && !m_GoogleDriveFolderId.IsEmpty())
					// {
					// 	LogTag.Build.I("Upload google drive.");

					// 	var request = GoogleDrivePostCreateFileWebRequest.Create(m_GoogleDriveFolderId,FileName,FileData);

					// 	var data = await request.SendAsync();

					// 	if(!data.Result)
					// 	{
					// 		LogTag.Build.I("Upload failed.");

					// 		return;
					// 	}

					// 	LogTag.Build.I($"Upload success. {$@"https://drive.google.com/drive/folders/{m_GoogleDriveFolderId}"}");
					// }

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

						textList.Add($"<{message.type}>{message.content}");
					}
				}

				var text = string.Join("\n",textList);

				CommonUtility.DisplayError(new Exception($"{target} build failed. [result : {text}]"));
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
				return CommonUtility.PathCombine(AppFullPath,$"{_appName}.apk");
			}
			case BuildTarget.iOS:
			{
				return CommonUtility.PathCombine(AppFullPath,_appName);
			}
			default:
			{
				return CommonUtility.PathCombine(AppFullPath,_appName);
			}
		}
	}
}
#endif