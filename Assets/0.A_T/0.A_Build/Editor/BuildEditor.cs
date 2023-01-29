// using UnityEngine;
// using UnityEditor;
// using UnityEditor.Build.Reporting;
// using System;
// using System.IO;
// using System.Text;
// using System.Collections.Generic;
// using System.Linq;
// using System.Reflection;
// using ABFactory;
// using System.Threading.Tasks;

// class BuildEditor
// {
//     #region Constant Variables
//     private const string ROOT_DIR = "meta_data";
//     private const string TEXT_FILE_EXTENSION = ".epic";
//     private const string META_DATA_ROOT_PATH = "{0}/../../epic_meta/{1}";
//     #endregion

//     public static string APP_NAME = "WOD";
    
//     static string[] SCENES = FindEnabledEditorScenes();

//     //public static string APP_APK_BUNDLE_NAME = "com.actoz.wod";
//     //public static string APP_IPA_BUNDLE_NAME = "com.actoz.wod";
//     //static string KEY_PASSWORD = "dkdlepsxlxl!@#";
//     //static string KEYSTORE_NAME = "epic.keystore";
//     //static string ALIAS_NAME = "epic";
//     //// NTC : Nexon Thai CBT
//     //public static string APP_APK_BUNDLE_NAME_NTC = "com.nxth.wod";
//     //public static string APP_IPA_BUNDLE_NAME_NTC = "com.nxth.wod";
//     //static string KEY_PASSWORD_NTC = "WODN!@321";
//     //static string KEYSTORE_NAME_NTC = "wod.keystore";
//     //static string ALIAS_NAME_NTC = "wod";

//     public static void PerformBuild()
//     {
//         BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
//         switch (buildTarget)
//         {
//             case BuildTarget.Android:
//                 PerformAndroidBuild();
//                 break;

//             case BuildTarget.iOS:
//                 PerformiOSBuild();
//                 break;

//             case BuildTarget.StandaloneWindows:
//             case BuildTarget.StandaloneWindows64:
//                 PerformPCBuild();
//                 break;

//             default:
//                 Debug.Log("PerformBuild Error Target UnKnown : ", buildTarget);
//                 break;
//         }
//     }


//     #region Android

//     public static void PerformAndroidBuild()
//     {
//         var buildOption = PerformBuildParam();

//         AndroidBuild(".apk", buildOption);
//     }

//     public static void PerformAndroidPlatformBuild()
//     {
//         var buildOption = PerformBuildParam();

//         AndroidBuild(".apk", buildOption);
//     }


//     public static async void PerformVersionData()
//     {
//         var buildOption = PerformBuildParam();
//         await GenericBuildInfo(buildOption);
//     }

//     public static BuildParam PerformBuildParam()
//     {
//         var buildParam = new BuildParam();
//         Type buildParamType = buildParam.GetType();
//         var buildParamProperties = buildParamType.GetProperties();
//         for (int i = 0, len = buildParamProperties.Length; i < len; i++)
//         {
//             var buildParamProperty = buildParamProperties[i];
//             var optionName = string.Format("-{0}", buildParamProperty.Name);

//             Debug.Log("[test] buildParamProperty.Name:" + buildParamProperty.Name + " || " + buildParamProperty.PropertyType + " || " + optionName);

//             if (buildParamProperty.PropertyType.Equals(typeof(bool)))
//             {
//                 buildParamProperty.SetValue(buildParam, BuildUtil.GetCmdParamBool(optionName));
//             }
//             else if (buildParamProperty.PropertyType.Equals(typeof(int)))
//             {
//                 buildParamProperty.SetValue(buildParam, BuildUtil.GetCmdParamInt(optionName));
//             }
//             else
//             {
//                 buildParamProperty.SetValue(buildParam, BuildUtil.GetCmdParamString(optionName));
//             }
//         }

//         // Debug.Log("[test] buildParam : " + BuildUtil.ToString(MemberTypes.Property, buildParam));
//         return buildParam;
//     }

//     public static BuildParam PerformCurruntBuildOption()
//     {
//         var buildParam = PerformBuildParam();
//         // buildParam.GameVersion = CurrentBundleVersion.GameVersion;
//         // buildParam.VersionCode = CurrentBundleVersion.VersionCode;
//         // buildParam.AssetBundleVersion = CurrentBundleVersion.BundleVersion;

//         return buildParam;
//     }


//     public static void StreamAssetChange(BuildTargetGroup targetGroup)
//     {
//         ChangeStreamAssets(targetGroup);
//     }

//     public static string GetSceneName(PlatformManager.PlatformType platformType)
//     {
//         string defaultScenePath = @"Assets/Scenes/DNOrigin.unity";
//         string str = $"Assets/Scenes/DNOrigin_{platformType.ToString()}.unity";
//         switch (platformType)
//         {
//             case PlatformManager.PlatformType.korea:
//             case PlatformManager.PlatformType.korea_onestore:
//                 str = $"Assets/Scenes/DNOrigin_korea.unity";
//                 break;
//             case PlatformManager.PlatformType.gamania_shengqu:
//                 str = $"Assets/Scenes/DNOrigin_gamania.unity";
//                 break;
//             case PlatformManager.PlatformType.wod:
//             case PlatformManager.PlatformType.SqIntDevEnv:
//             case PlatformManager.PlatformType.SqIntDevEnvIl2Cpp:
//             case PlatformManager.PlatformType.SqExtDevEnv:
//             case PlatformManager.PlatformType.SqQaEnv:
//             case PlatformManager.PlatformType.SqFormalEnv:
//             case PlatformManager.PlatformType.SqIosArraignmentEnv:
//                 str = defaultScenePath;
//                 break;
//             default:
//                 UnityEngine.Debug.LogError("GetSceneName, Unprocessed platformType: " + platformType);
//                 str = defaultScenePath;
//                 break;
//         }

//         if (!File.Exists(str))
//         {
//             UnityEngine.Debug.LogError("scene not exist. path is " + str);
//         }

//         Debug.Log( $"Launching Scene path is {str}" );
//         return str;
//     }

//     public static BuildReport AndroidBuild(string strName, BuildParam buildParam, bool bNeedPopupSavePanel = true, bool bSkipSetDefine = false)
//     {
//         Debug.Log("[test] AndroidBuild : ", strName);
//         //PlatformManager.PlatformChange(buildParam.PlatformType);
//         //PlatformManager.ServerTypeChangeFile(buildParam.UseServerType);

//         string extentionName = buildParam.UseAAB ? ".aab" : strName;

//         string strTargetFileName = string.Format("{0}/{1}{2}", BuildUtil.GetBuildDirectory(), APP_NAME, extentionName);

//         if (bNeedPopupSavePanel && Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
//         {
//             // if (ResourceEditor.CheckBatchMode() == false)
//             // {
//             //     string strPath = EditorUtility.SaveFilePanel("Save", "/Build/", "DNOrigin", buildParam.UseAAB ? "aab" : "apk");

//             //     if (string.IsNullOrEmpty(strPath) == false)
//             //     {
//             //         strTargetFileName = strPath;
//             //     }
//             //     else
//             //     {
//             //         return null;
//             //     }
//             // }
//         }

//         //PrePlatfromSetting();

//         Debug.LogFormat("Build Android Version at {0} -> {1}", strTargetFileName, GetVersion());

//         if(SetAndroidDefaultSetting(buildParam) == false)
//         {
//             return null;
//         }

//         if (!bSkipSetDefine)
//         {
//             SetDefines(BuildTargetGroup.Android, buildParam);
//         }

//         BuildOptions buildOption = BuildOptions.None;
//         if (buildParam.UseProfiler)
//         {
//             buildOption = (BuildOptions.AllowDebugging | BuildOptions.Development | BuildOptions.ConnectWithProfiler);
//         }

//         // 플러그인으로 인해 무조건 gradle 로 빌드해야함 - mutidex 오류 때문에..
//         EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
//         // 앱번들 제작 용
//         Debug.Log("[test] build EditorUserBuildSettings.buildAppBundle :", EditorUserBuildSettings.buildAppBundle);

//         string[] scenes = new string[] { GetSceneName( buildParam.PlatformType ) };
//         return GenericBuild(buildParam, scenes, strTargetFileName, BuildTargetGroup.Android, BuildTarget.Android, buildOption);
//     }
//     #endregion

//     #region iOS
//     public static void PerformiOSPlatformBuild()
//     {
//         var buildOption = PerformBuildParam();

//         iOSBuild(Path.GetFullPath("Build"), buildOption);
//     }

//     public static void PerformiOSBuild()
//     {
//         var buildOption = PerformBuildParam();

//         iOSBuild(Path.GetFullPath("Build"), buildOption);
//     }


//     public static void PerformiOSBuild_Dev()
//     {
//         var buildOption = PerformBuildParam();
//         buildOption.UseDebug = true;
//         iOSBuild(Path.GetFullPath("Build"), buildOption);
//     }


//     public static BuildReport iOSBuild(string strJenkinsPath, BuildParam buildParam, bool bSkipSetDefine = false)
//     {
//         //PlatformManager.PlatformChange(buildParam.PlatformType);

//         if (SetiOSDefaultSetting(buildParam) == false)
//             return null;

//         //PlatformManager.ServerTypeChangeFile(buildParam.UseServerType);

//         if (!bSkipSetDefine)
//         {
//             SetDefines(BuildTargetGroup.iOS, buildParam);
//         }

//         try
//         {
//             BuildOptions buildOption = BuildOptions.AcceptExternalModificationsToPlayer;
            
//             if (Application.platform == RuntimePlatform.OSXEditor)
//             {
//                 buildOption |= BuildOptions.SymlinkLibraries;
//             }

//             if (buildParam.UseProfiler)
//             {
//                 buildOption |= (BuildOptions.AllowDebugging | BuildOptions.Development | BuildOptions.ConnectWithProfiler);
//             }

//             string[] scenes = new string[] { GetSceneName( buildParam.PlatformType ) };

//             //5.6에서 사용안함
//             return GenericBuild(buildParam, scenes, strJenkinsPath, BuildTargetGroup.iOS, BuildTarget.iOS, buildOption);
//         }
//         catch (Exception ex)
//         {
//             UnityEngine.Debug.LogError(ex);
//             return null;
//         }
//     }
//     #endregion

//     #region PC
//     public static void PerformPCBuild()
//     {
//         var buildOption = PerformBuildParam();

//         string path = BuildUtil.GetBuildDirectory();
//         string strTargetFileName = path + "/" + "WOD_PC" + "/" + APP_NAME + ".exe";

//         PCBuild(strTargetFileName, buildOption);
//     }


//     public static BuildReport PCBuild(string strJenkinsPath, BuildParam buildParam, bool bSkipSetDefine = false)
//     {
//         if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
//         {
//             string strPath = EditorUtility.SaveFilePanel("Save", "/Build/", "DNOrigin", "exe");

//             if (!string.IsNullOrEmpty(strPath))
//             {
//                 strJenkinsPath = strPath;
//             }
//         }

//         //PlatformManager.PlatformChange(buildParam.PlatformType);

//         if (buildParam.useV2Settings)
//         {
//             PlayerSettings.fullScreenMode = buildParam.fullScreenMode;
//             PlayerSettings.resizableWindow = buildParam.resizableWindow;
//             PlayerSettings.defaultScreenHeight = buildParam.defaultScreenHeight;
//             PlayerSettings.defaultScreenWidth = buildParam.defaultScreenWidth;
//             PlayerSettings.displayResolutionDialog = buildParam.displayResolutionDialog ? ResolutionDialogSetting.Enabled : ResolutionDialogSetting.Disabled;

//             QualitySettings.masterTextureLimit = buildParam.masterTextureLimit;
//             QualitySettings.SetQualityLevel(buildParam.qualityLevelIndex);
//         }
//         else
//         {
//             PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
//             PlayerSettings.resizableWindow = true;
//             PlayerSettings.defaultScreenHeight = 720;
//             PlayerSettings.defaultScreenWidth = 480;

//             PlayerSettings.displayResolutionDialog = buildParam.UseNoRender ? ResolutionDialogSetting.Disabled : ResolutionDialogSetting.Enabled;
//             if (buildParam.UseNoRender)
//             {
//                 PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
//                 PlayerSettings.defaultScreenHeight = 640;
//                 PlayerSettings.defaultScreenWidth = 480;
//                 QualitySettings.masterTextureLimit = 2;

//                 QualitySettings.SetQualityLevel(0);
//             }
//         }

//         if (!bSkipSetDefine)
//         {
//             SetDefines(BuildTargetGroup.Standalone, buildParam);// new BuildParam() { UseDebug = bShowDebug });
//         }

//         Debug.LogFormat("Build PC Version at {0}", strJenkinsPath);

//         try
//         {
//             string[] launcher_scene = new string[] { GetSceneName( buildParam.PlatformType ) };
//             BuildOptions buildOption = BuildOptions.None;
//             if (buildParam.UseProfiler)
//             {
//                 buildOption |= (BuildOptions.AllowDebugging | BuildOptions.Development | BuildOptions.ConnectWithProfiler);
//             }
//             return GenericBuild(buildParam, launcher_scene, strJenkinsPath, BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64, buildOption);
//         }
//         catch (Exception ex)
//         {
//             UnityEngine.Debug.LogError(ex);
//             // if (ResourceEditor.CheckBatchMode())
//             // {
//             //     EditorApplication.Exit(1);
//             // }
//             return null;
//         }
//     }
//     #endregion


//     static void UpdateDefine(ref List<string> defines, string strDefine, bool on)
//     {
//         int index = defines.FindIndex(define => define.Equals(strDefine));

//         if (on && index.Equals(-1)) //없을 경우 추가 - cmlee.
//         {
//             defines.Add(strDefine);
//         }
//         else if (on == false && index.Equals(-1) == false) // 아닐 때 있으면 삭제 - cmlee.
//         {
//             defines.RemoveAt(index);
//         }
//     }

//     static void SetDefines(BuildTargetGroup targetGroup, BuildParam buildParam)
//     {
//         string strDefines = "";// PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
//         // BuildBundleByConfig.BuildLog("[AB] SetDefines buildParam : " + BuildUtil.ToString(MemberTypes.Property, buildParam));

//         PlatformManager.PlatformType platformType = buildParam.PlatformType;
//         // BuildBundleByConfig.BuildLog($"###### SetDefine {platformType.ToString()}");

//         switch (platformType)
//         {
//             case PlatformManager.PlatformType.wod:
//                 strDefines += "USE_FIREBASE;";
//                 break;

//             case PlatformManager.PlatformType.gamania:
//                 //USE_TERM 전처리부분 주석처리되어있으므로 지움.
//                 strDefines += "SERVICE_GAMANIA;USE_FIREBASE;USE_UNIWEBVIEW;";//USE_TERM;";
//                 break;

//             case PlatformManager.PlatformType.gamania_shengqu:
//                 //USE_TERM 전처리부분 주석처리되어있으므로 지움.
//                 strDefines += "SERVICE_GAMANIA;USE_FIREBASE;USE_UNIWEBVIEW;SERVICE_GAMANIA_SHENGQU;";//USE_TERM;";
//                 break;

//             case PlatformManager.PlatformType.korea:
//                 strDefines += "SERVICE_KOREA;USE_FIREBASE;USE_UNIWEBVIEW;UNITY_POST_PROCESSING_STACK_V2";//USE_TERM;";
//                 break;
//             case PlatformManager.PlatformType.korea_onestore:
//                 strDefines += "SERVICE_KOREA;USE_FIREBASE;USE_UNIWEBVIEW;USE_ONESTORE;UNITY_POST_PROCESSING_STACK_V2";//USE_TERM;";
//                 break;
//             case PlatformManager.PlatformType.SqExtDevEnv:
//                 strDefines += "SERVICE_GHOME;LOG_ALL;LOG_AUTO_QUEST;LOG_NET;USE_INJECTFIX_INJECT;CN_CBT";
//                 break;
//             case PlatformManager.PlatformType.SqQaEnv:
//             case PlatformManager.PlatformType.SqFormalEnv:
//             case PlatformManager.PlatformType.SqIosArraignmentEnv:
//                 strDefines += "SERVICE_GHOME;LOG_ALL;LOG_AUTO_QUEST;LOG_NET;USE_INJECTFIX_INJECT;USE_INJECTFIX_FIX;CN_CBT";
//                 break;
//             case PlatformManager.PlatformType.SqIntDevEnv:
//                 strDefines += "LOG_ALL;LOG_AUTO_QUEST;LOG_NET;CN_CBT";
//                 break;
//             case PlatformManager.PlatformType.SqIntDevEnvIl2Cpp:
//                 strDefines += "LOG_ALL;LOG_CONTENTS_OPEN;LOG_AUTO_QUEST;LOG_NET;USE_UNITY_DEBUG;USE_INJECTFIX_INJECT;CN_CBT;GAME_MODE_DEV;USE_SQ_PATCH;LOG_TIME;ALLOW_DEBUG_BUILD;LOG_TRACE;UNITY_POST_PROCESSING_STACK_V2";
//                 break;
//             default:
//                 UnityEngine.Debug.LogError("BuildEditor.SetDefines, Unprocessed platformType: " + platformType);
//                 break;
//         }

//         List<string> defines = strDefines.Split(';').ToList();
//         if (defines == null)
//         {
//             defines = new List<string>();
//         }

//         if(!string.IsNullOrEmpty(buildParam.GameMode))
//         {
//             bool isDebug = buildParam.GameMode.Equals("DEV") || buildParam.UseDebug;
//             UpdateDefine(ref defines, "USE_UNITY_DEBUG", isDebug);//buildParam.UseDebug);
//         }

//         // 서버 선택 // OpenServer or QAServer
//         if (buildParam.UseOpenServer)
//         {
//             UpdateDefine(ref defines, "OPEN_SERVER", buildParam.UseOpenServer);
//         }
//         else if (buildParam.UseFGTServer)
//         {
//             UpdateDefine(ref defines, "FGT_SERVER", buildParam.UseFGTServer);
//         }
//         else if (buildParam.UseQAServer)
//         {
//             UpdateDefine(ref defines, "QA_SERVER", buildParam.UseQAServer);
//         }

//         //if (!string.IsNullOrEmpty(buildParam.UseServerType))
//         //{
//         //var type = (ABFactory.ServerType)Enum.Parse(typeof(ABFactory.ServerType), buildParam.UseServerType);

//         //switch (type)
//         //{
//         //    case ABFactory.ServerType.QA:
//         //    case ABFactory.ServerType.QA1:
//         //    case ABFactory.ServerType.QA2:
//         //        UpdateDefine(ref defines, "QA_SERVER", true);
//         //        UpdateDefine(ref defines, string.Format("SERVER_TYPE_{0}", buildParam.UseServerType), true);
//         //        break;
//         //}
//         //}

//         if (!string.IsNullOrEmpty(buildParam.GameMode))
//         {
//             UpdateDefine(ref defines, string.Format("GAME_MODE_{0}", buildParam.GameMode.ToUpperInvariant()), true);
//         }

//         UpdateDefine(ref defines, "USE_NORENDER", buildParam.UseNoRender);
//         UpdateDefine(ref defines, "JMETER", buildParam.UseNoRender); // NoRender 일때 JMeter 활성화 

//         UpdateDefine(ref defines, "ONLY_PATCH", buildParam.OnlyPatchMode);

//         StringBuilder strb = new StringBuilder();
//         defines.ForEach(define =>
//         {
//             if (string.IsNullOrEmpty(define))
//                 return;

//             strb.Append(define);
//             strb.Append(";");
//         });

//         Debug.Log($"[test] {targetGroup}- macro defines : " + strb.ToString());
//         PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, strb.ToString());
//     }


//     private static string[] FindEnabledEditorScenes()
//     {
//         List<string> EditorScenes = new List<string>();
//         foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
//         {
//             if (scene.enabled == false)
//             {
//                 continue;
//             }

//             EditorScenes.Add(scene.path);
//         }

//         return EditorScenes.ToArray();
//     }

//     public static void SetPlatformChange(PlatformManager.PlatformType platformType, bool bSkipSetDefine = false)
//     {
//         BuildParam buildParam = new BuildParam()
//         {
//             PlatformType = platformType//,
//             //GameMode = "DEV"
//         };
//         SetPlatformChange(buildParam);
//     }

//     public static void SetPlatformChange(BuildParam buildParam, bool bSkipSetDefine = false)
//     {
//         BuildTargetGroup buildTargetGroup = BuildTargetGroup.Android;
// #if UNITY_ANDROID
//         SetAndroidDefaultSetting(buildParam);
//         buildTargetGroup = BuildTargetGroup.Android;
// #elif UNITY_IOS
//         SetiOSDefaultSetting(buildParam);
//         buildTargetGroup = BuildTargetGroup.iOS;
// #elif UNITY_STANDALONE
        
//         buildTargetGroup = BuildTargetGroup.Standalone;
// #else
//         UnityEngine.Debug.LogError("unprocessed BuildTargetGroup!");
//         return;
// #endif

//         if (!bSkipSetDefine)
//         {
//             SetDefines(buildTargetGroup, buildParam);
//         }
//     }

//     public static bool SetAndroidDefaultSetting(BuildParam buildParam)
//     {
//         PlayerSettings.stripEngineCode = false;
//         PlayerSettings.MTRendering = true;

//         BuildEditorKey.KeyInfo keyInfo = BuildEditorKey.GetKeyInfo(buildParam.PlatformType);
//         if(keyInfo == null)
//             return false;

//         PlayerSettings.applicationIdentifier = keyInfo.BundleNameAPK;
//         PlayerSettings.Android.keyaliasName = keyInfo.KeyAliasName;
//         PlayerSettings.keystorePass = keyInfo.KeyStorePass;
//         PlayerSettings.keyaliasPass = keyInfo.KeyAliasPass;
//         PlayerSettings.Android.keystoreName = Application.dataPath + "/../" + keyInfo.KeyStoreName;
//         EditorUserBuildSettings.androidCreateSymbolsZip = true;

//         //Debug.Log("[test] buildParam.UseMono " + buildParam.UseMono);
//         //return false;
//         PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, buildParam.UseMono ? ScriptingImplementation.Mono2x : ScriptingImplementation.IL2CPP); // for arm64
//         PlayerSettings.Android.forceInternetPermission = true;
//         PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.Android, ManagedStrippingLevel.Low);

//         if (buildParam.UseUWA)
//         {
//             PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.Android, ManagedStrippingLevel.Disabled);
//             PlayerSettings.Android.forceInternetPermission = false;
//             PlayerSettings.Android.forceSDCardPermission = true;
//         }

//         // AAB 파일 생성
//         EditorUserBuildSettings.buildAppBundle = buildParam.UseAAB;
//         Debug.Log("[test] EditorUserBuildSettings.buildAppBundle ", EditorUserBuildSettings.buildAppBundle);

//         PlayerSettings.bundleVersion = buildParam.GameVersion;
//         PlayerSettings.productName = GetPlayerTypeName(buildParam.PlatformType);
//         PlayerSettings.Android.bundleVersionCode = buildParam.VersionCode;

//         // BuildBundleByConfig.BuildLog("SetAndroidDefaultSetting，PlayerSettings.bundleVersion=" + PlayerSettings.bundleVersion);

//         AndroidSdkVersions androidSdkVersion = AndroidSdkVersions.AndroidApiLevel19;
//         // UniWebView 가 있을 경우 21 버전이 필요함
//         if (File.Exists($"{Application.dataPath}/Plugins/Android/UniWebView.aar"))
//         {
//             androidSdkVersion = AndroidSdkVersions.AndroidApiLevel21;
//         }
//         PlayerSettings.Android.minSdkVersion = androidSdkVersion;

//         PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
   
//         if (buildParam.UseMono)
//         {
//             PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7;
//         }
//         else
//         {
//             PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;
//         }

//         // DNA.Build.BuildBundleByConfig.BuildLog("[targetArchitectures] SetAndroidDefaultSetting: " + PlayerSettings.Android.targetArchitectures);

//         return true;
//     }

//     private static string GetPlayerTypeName(PlatformManager.PlatformType platformType)
//     {
//         string AppName = APP_NAME;
//         switch (platformType)
//         {
//             case PlatformManager.PlatformType.gamania:
//                 AppName = "龍之谷：新世界";
//                 break;
//             case PlatformManager.PlatformType.SqIntDevEnv:
//             case PlatformManager.PlatformType.SqIntDevEnvIl2Cpp:
//             case PlatformManager.PlatformType.SqExtDevEnv:
//             case PlatformManager.PlatformType.SqQaEnv:
//             case PlatformManager.PlatformType.SqFormalEnv:
//             case PlatformManager.PlatformType.SqIosArraignmentEnv:
// #if CN_CBT
//                 AppName = "代号DN";
// #else
//                 AppName = "龙之谷世界";
// #endif

//                 break;
//             default:
//                 UnityEngine.Debug.LogError("GetPlayerTypeName, Unprocessed platformType: " + platformType);
//                 break;
//         }

//         return AppName;
//     }


//     private static bool SetiOSDefaultSetting(BuildParam buildParam)
//     {
//         bool isAutoSigning = false;
//         string developerTeamID = "";
//         string manualProvisioning = "";

//         BuildEditorKey.KeyInfo keyInfo = BuildEditorKey.GetKeyInfo(buildParam.PlatformType);
//         if (keyInfo == null)
//             return false;

//         string appBundleName = keyInfo.BundleNameIPA;

//         if (buildParam.PlatformType.Equals("wod"))
//         {
//             isAutoSigning = true;
//         }

//         PlayerSettings.applicationIdentifier = appBundleName;
//         PlayerSettings.iOS.appleEnableAutomaticSigning = isAutoSigning;
//         PlayerSettings.iOS.appleDeveloperTeamID = developerTeamID;
//         PlayerSettings.iOS.iOSManualProvisioningProfileID = manualProvisioning; // enterprise 계정의 privosioning 이다. 빌드 분기에 따른 처리 필요 by cmlee.
        

//         PlayerSettings.bundleVersion = buildParam.GameVersion;
//         PlayerSettings.iOS.buildNumber = buildParam.VersionCode.ToString();
//         PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;

//         PlayerSettings.statusBarHidden = true;
//         PlayerSettings.stripEngineCode = false;
//         PlayerSettings.scriptingRuntimeVersion = ScriptingRuntimeVersion.Latest;

//         PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.iOS, ApiCompatibilityLevel.NET_4_6);
//         PlayerSettings.SetArchitecture(BuildTargetGroup.iOS, 2); // 0 - None, 1 - ARM64, 2 - Universal.

//         PlayerSettings.iOS.hideHomeButton = true; // iphone X home button 감추기
//         PlayerSettings.iOS.scriptCallOptimization = ScriptCallOptimizationLevel.SlowAndSafe;

//         // Apple은 2018년 7월부터 앱 스토어에 제출되는 모든 iOS 앱은 Xcode 9에 포함된 iOS 11 SDK로 구축되어야 한다고 발표 by cmlee.
//         // iOS 7 지원 삭제: iOS용 Firebase SDK v5.0.0부터 iOS 7 지원이 삭제되었습니다. 
//         // iOS 8 이상을 타겟팅하도록 앱을 업그레이드하세요. iOS 버전의 전 세계 분포를 보려면 Apple 앱 스토어 지원 페이지로 이동하세요.
// #if SERVICE_KOREA
//         PlayerSettings.iOS.targetOSVersionString = "10.0";
// #else
//         PlayerSettings.iOS.targetOSVersionString = "9.0";
// #endif
//         // UnitySaveData.DeleteKey(UnitySaveData.SD_SIGN_TEAM_ID);
//         // UnitySaveData.DeleteKey(UnitySaveData.SD_PROVISIONING_PROFILE_UUID);
//         // UnitySaveData.DeleteKey(UnitySaveData.SD_SIGN_TEAM_NAME);


//         if (string.IsNullOrEmpty(buildParam.SIGN_TEAM_ID) == false)
//         {
//             //For Test!
//             // UnitySaveData.SetString(UnitySaveData.SD_SIGN_TEAM_ID, buildParam.SIGN_TEAM_ID);// "JR647F463T");
//             // UnitySaveData.SetString(UnitySaveData.SD_PROVISIONING_PROFILE_UUID, buildParam.PROVISIONING_PROFILE_UUID);//"337dfde4-4aac-4967-88f7-e1002afabf07");
//             // UnitySaveData.SetString(UnitySaveData.SD_SIGN_TEAM_NAME, buildParam.SIGN_TEAM_NAME);//"iPhone Distribution: Nexon (Thailand) Co., Ltd.");

//             //UnitySaveData.SetInt("TEST_IOS_SIGNING", 1);
//         }

//         //PlayerSettings.iOS.iOSManualProvisioningProfileID = "bd9ec074-0837-48be-b8ae-351a7985a4a6"; // enterprise 계정의 privisioning 이다. 빌드 분기에 따른 처리 필요 by cmlee.
//         return true;
//     }
    
//     //Jenkins Execute 용도.
//     public static void GenericVersionData(BuildParam buildParam)
//     {
//         var versionData = new ABFactoryVersionData()
//         {
//             GameVersion = buildParam.GameVersion,
//             BundleVersion = buildParam.AssetBundleVersion,
//             VersionCode = buildParam.VersionCode,
//             ServerURL = buildParam.ServerURL,
//             PortarServerUrl = buildParam.PortarServerUrl,
//             ServerRegionID = buildParam.ServerRegionID,
//             ServerType = buildParam.UseServerType,
//             BuildTimeStamp = string.Format("{0}_{1}", DateTime.Now.ToString("MMdd"), DateTime.Now.ToString("HHmm")),//buildParam.BuildTimeStamp,
//             PatchGroup = buildParam.PatchGroup,
//             LoginType = buildParam.LoginType,

//             BuildMono = buildParam.UseMono, //add by hanjun
//             UseSqCodeUpdateSdk = buildParam.UseSqCodeUpdateSdk //add by hanjun
//         };

//         versionData.Revision = BuildUtil.ReadRevisionFile();

//         versionData.SaveVersionJson();

//         //PlatformManager.ServerTypeChangeFile(versionData.ServerType);
//         // Debug.Log("[test] GenericVersionData : " + versionData.GetJsonPath() + " || " + BuildUtil.ToString(MemberTypes.Field, versionData));

// #if UNITY_EDITOR
//         AssetDatabase.Refresh();
// #endif
//     }

//     public static async Task GenericBuildInfo(BuildParam buildParam)
//     {
//         GenericVersionData(buildParam);

//         // if (!ResourceEditor.CheckBatchMode())
//         {
//             await Task.Delay(1);
//         }
//     }

//     public static void DeleteBuildInfo()
//     {
//         string path = string.Format("{0}/Resources/{1}", Application.dataPath, ABFactory.ABFactoryVersionData.JsonFileName + ABFactory.ABFactoryVersionData.JsonFileExtention);
//         //Debug.Log("[test] DeleteBuildInfo ", path);
//         if (File.Exists(path))
//         {
//             File.Delete(path);
//         }
//     }


//     private static void ChangeManifest()
//     {
//         var versionData = ABFactoryVersionData.LoadVersionJson();
//         if (versionData == null)
//             return;

// #if UNITY_STANDALONE
//         return;
// #endif

//         string serverType = versionData.ServerType;

//         if (!versionData.ServerType.Equals("NEXON") &&
//             !versionData.ServerType.Equals("NEXON2") &&
//             !versionData.ServerType.Equals("NXKR_LIVE") &&
//             !versionData.ServerType.Equals("NEXON_LIVE") &&
//             !versionData.ServerType.Equals("NEXON_STR"))
//         {
//             return;
//         }


//         var fromPathRoot = $"{Application.dataPath}/../Platform/Manifest/{versionData.ServerType}";
//         ChangeFile($"{fromPathRoot}/AndroidManifest.xml", $"{Application.dataPath}/Plugins/Android/AndroidManifest.xml");
//         ChangeFile($"{fromPathRoot}/PostProcessBuildPlayer", $"{Application.dataPath}/Editor/PostProcessBuildPlayer");
//         ChangeFile($"{fromPathRoot}/Settings.asset", $"{Application.dataPath}/Tapjoy/Settings.asset");
//     }

//     private static void ChangeStreamAssets(BuildTargetGroup targetGroup)
//     {

//         var fromPathRoot = $"{Application.dataPath}/../Platform/target/ios/Assets/StreamingAssets";
//         var targetPathRoot = $"{Application.streamingAssetsPath}";

//         //List<string> fileList = Directory.GetFiles(fromPathRoot, "*.*").Where(file => file.ToLower().EndsWith(".mp4") || file.ToLower().EndsWith(".mp4.meta")).ToList();
//         List<string> fileList = Directory.GetFiles(fromPathRoot, "*.*").ToList();
//         fileList.ForEach(path =>
//         {
//             string filename = Path.GetFileName(path);
//             string toPath = $"{targetPathRoot}/{filename}";

//             if (targetGroup.Equals(BuildTargetGroup.Android)) // 안드로이드일 경우 파일이 있으면 삭제
//             {
//                 if (File.Exists(toPath))
//                 {
//                     File.Delete(toPath);
//                 }
//                 return;
//             }

//             if (File.Exists(toPath)) // ios 일 경우 파일이 있으면 넘어감
//                 return;

//             string fromPath = $"{fromPathRoot}/{filename}";
//             FileUtil.CopyFileOrDirectory(fromPath, toPath);
//         });
//     }


//     private static void ChangeFile(string fromPath, string toPath)
//     {
//         Debug.Log("[test] ChangeManifest :" + fromPath + " || " + toPath);
//         if (!File.Exists(fromPath))
//         {
//             return;
//         }

//         if (File.Exists(toPath))
//         {
//             File.Delete(toPath);
//         }

//         FileUtil.CopyFileOrDirectory(fromPath, toPath);
//     }


//     static BuildReport GenericBuild(BuildParam buildParam, string[] scenes, string target_filename, BuildTargetGroup build_group, BuildTarget build_target, BuildOptions build_options, bool bThrowExption = true)
//     {
//         // 번들 리스트 제작 //
//         //ABFactory.ABFactoryListManager.Instance.CreateAssetList();
//         var versionData = ABFactoryVersionData.LoadVersionJson();

//         //TODO : 미리 생성한 버전데이터 항상 잘 적용되는지 확인 후에 빌드파람 빼도 될듯.
//         if (versionData != null)
//         {
//             Debug.LogFormat("Get versionData! versionCode : {0} // timestamp : {1}", versionData.VersionCode, versionData.BuildTimeStamp);
//             UnityEngine.Debug.LogWarning("GenericBuild::Replace version Data!!!");
//         }

//         GenericVersionData(buildParam);

//         // ChangeManifest(); 플랫폼 체인지 후로 변경
//         //ChangeStreamAssets(build_group);
//         DateTime StartTime = DateTime.Now;
//         Debug.Log("[Build Start]: " + StartTime + ", [FilePath]:" + target_filename + " || " + build_group + " || " + build_target);

//         //다른경우에만 switch 하게 처리.(link :: https://stackoverflow.com/questions/20553398/unity3d-commandline-autobuild-not-running-postbuild 참고).
//         if (EditorUserBuildSettings.activeBuildTarget != build_target)
//         {
//             Debug.Log($"빌드 타겟 바꿈 {EditorUserBuildSettings.activeBuildTarget.ToString()} to {build_target.ToString()}");
//             EditorUserBuildSettings.SwitchActiveBuildTarget(build_group, build_target);
//         }

//         Debug.Log("[test] GenericBuild EditorUserBuildSettings.buildAppBundle :", EditorUserBuildSettings.buildAppBundle);
//         BuildReport buildReport = BuildPipeline.BuildPlayer(scenes, target_filename, build_target, build_options);

//         if (buildReport.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
//         {
//             BuildResult(false);
            
//             // if ( ResourceEditor.CheckBatchMode() )
//             {
//                 EditorApplication.Exit( 1 );
//             }

//             if (bThrowExption)
//             {
//                 throw new Exception("BuildPlayer failure: " + buildReport.summary.result);
//             }
//             else
//             {
//                 return buildReport;
//             }
//         }
        
//         DateTime EndTime = DateTime.Now;
//         DateTime DurationTime = new DateTime(EndTime.Ticks - StartTime.Ticks);
//         string strDurTime = string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D4}", DurationTime.Hour, DurationTime.Minute, DurationTime.Second, DurationTime.Millisecond);

//         Debug.Log("[Build End]: " + EndTime);
//         Debug.Log("Build Duration Time: " + strDurTime);

//         GenericVersionJson(target_filename, build_group);

//         BuildResult(true);

//         return buildReport;
//     }

//     static void GenericVersionJson(string target_filename, BuildTargetGroup build_group)
//     {
//         var filename = target_filename.Split('/').Last();
//         var path = target_filename.Substring(0, target_filename.Length - filename.Length);//  target_filename.Replace(filename, "");
//         Debug.Log("[test] path : " + path);
//         string strBuildVersion = string.Format("{0}version_{1}.json", path, build_group);
//         // string json = JsonUtility.ToJson(new PatchManager.PatchVersion()
//         // {
//         //     MinVersion = CurrentBundleVersion.GameVersion
//         // });

//         //Debug.Log("[test] GenericVersionJson : " + strBuildVersion);
//         // BuildUtil.WriteToFile(strBuildVersion, json);
//     }


//     private static void BuildResult(bool bSuccess)
//     {
//         Debug.Log("[test] BuildResult " + bSuccess);
//         string strTitle = "";
//         string strDesc = "";

//         if (bSuccess == true)
//         {
//             strTitle = "Success";
//             strDesc = "Build Complete !!!!!";
//         }
//         else
//         {
//             strTitle = "Error";
//             strDesc = "Build Failed !!!!!";
//         }

//         // if (ResourceEditor.CheckBatchMode())
//         {
//             Debug.Log(strDesc);
//             return;
//         }

//         if ((Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor))
//         {
//             EditorUtility.DisplayDialog(strTitle, strDesc, "OK");
//         }
//     }

//     private static string GetVersion()
//     {
//         string output = string.Empty;
// #if !UNITY_EDITOR
//         string path = Application.dataPath.Remove(Application.dataPath.LastIndexOf("/Assets"), "/Assets".Length) + "/version.txt";
//         using (StreamReader r = new StreamReader(path))
//         {
//             output = r.ReadToEnd().Trim();
//         }
// #endif
//         return output;
//     }
// }
