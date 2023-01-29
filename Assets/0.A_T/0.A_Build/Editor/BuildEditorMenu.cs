// using System.IO;
// using UnityEditor;

// using UnityEngine;

// public class BuildEditorMenu
// {
//     [MenuItem("Epic/Build/Open Build Window", false, 201)]
//     public static void BuildEditorWindowOpen()
//     {
//         RDBuildEditor.BuildEditorWindow.Open();
//     }

//     #region Android
//     [MenuItem("Epic/Build/GenerateVersionFile GAMANIA", priority = 11)]
//     static void GenerateVersionDevFileGAMANIA()
//     {
//         var buildParam = BuildEditor.PerformCurruntBuildOption();
//         buildParam.UseServerType = "GAMANIA";
//         buildParam.GameMode = "NORMAL";
//         buildParam.PatchGroup = "NEXON";
//         buildParam.ServerURL = "https://wod-server-qa-kor.eyedentitygames.com/";
//         buildParam.LoginType = 1;
//         buildParam.PlatformType = PlatformManager.PlatformType.gamania;
//         BuildEditor.GenericBuildInfo(buildParam);
//     }

//     [MenuItem("Epic/Build/GenerateVersionFile QA1", priority = 11)]
//     static void GenerateVersionDevFileQA1()
//     {
//         var buildParam = BuildEditor.PerformCurruntBuildOption();
//         buildParam.UseServerType = "QA1";
//         buildParam.GameMode = "NORMAL";
//         buildParam.PatchGroup = "NEXON";
//         buildParam.ServerURL = "https://wod-server-qa-kor.eyedentitygames.com/";
//         buildParam.LoginType = 2;
//         buildParam.PlatformType = PlatformManager.PlatformType.gamania;
//         BuildEditor.GenericBuildInfo(buildParam);
//     }


//     [MenuItem("Epic/Build/GenerateVersionFile QA2", priority = 11)]
//     static void GenerateVersionDevFileQA2()
//     {
//         var buildParam = BuildEditor.PerformCurruntBuildOption();
//         buildParam.UseServerType = "QA2";
//         buildParam.GameMode = "NORMAL";
//         buildParam.PatchGroup = "NEXON";
//         buildParam.ServerURL = "https://wod-server-qa-kor.eyedentitygames.com/";
//         buildParam.LoginType = 1;
//         buildParam.PlatformType = PlatformManager.PlatformType.wod;
//         BuildEditor.GenericBuildInfo(buildParam);
//     }

//     [MenuItem("Epic/Build/GenerateVersionFile NEXON", priority = 11)]
//     static void GenerateVersionDevFileNEXON()
//     {
//         var buildParam = BuildEditor.PerformCurruntBuildOption();
//         buildParam.UseServerType = "NEXON";
//         buildParam.GameMode = "DEV";
//         buildParam.PatchGroup = "NEXON";
//         buildParam.ServerURL = "https://wodn.dn.nexoncdn.co.kr/";//"https://wod-server-qa-kor.eyedentitygames.com/";
//         buildParam.LoginType = 2;
//         buildParam.PlatformType = PlatformManager.PlatformType.nexon;
//         BuildEditor.GenericBuildInfo(buildParam);
//     }


//     [MenuItem("Epic/Build/Android/QA1", priority = 102)]
//     static void PerformAndroidBuild_QA1()
//     {
//         var buildParam = BuildEditor.PerformCurruntBuildOption();
//         buildParam.UseServerType = "QA1";
//         buildParam.ServerURL = "https://wod-server-qa-kor.eyedentitygames.com/";
//         buildParam.GameMode = "DEV";
//         buildParam.LoginType = 1;
//         buildParam.PlatformType = PlatformManager.PlatformType.wod;
        
//         BuildEditor.GenericBuildInfo(buildParam);
//         BuildEditor.AndroidBuild(".apk", buildParam);
//     }

//     [MenuItem("Epic/Build/Android/QA1 Profiler", priority = 102)]
//     static void PerformAndroidBuild_QA1_Profiler()
//     {
//         var buildParam = BuildEditor.PerformCurruntBuildOption();
//         buildParam.UseServerType = "QA1";
//         buildParam.ServerURL = "https://wod-server-qa-kor.eyedentitygames.com/";
//         buildParam.GameMode = "DEV";
//         buildParam.UseDebug = true;
//         buildParam.UseProfiler = true;
//         buildParam.LoginType = 1;
//         buildParam.PlatformType = PlatformManager.PlatformType.wod;
//         BuildEditor.GenericBuildInfo(buildParam);
//         BuildEditor.AndroidBuild(".apk", buildParam);
//     }

//     [MenuItem("Epic/Build/Android/QA2 Profiler", priority = 102)]
//     static void PerformAndroidBuild_QA2_Profiler()
//     {
//         var buildParam = BuildEditor.PerformCurruntBuildOption();
//         buildParam.UseServerType = "QA2";
//         buildParam.ServerURL = "https://wod-server-qa-kor.eyedentitygames.com/";
//         buildParam.GameMode = "DEV";
//         buildParam.UseDebug = true;
//         buildParam.UseProfiler = true;
//         buildParam.LoginType = 1;
//         buildParam.PlatformType = PlatformManager.PlatformType.wod;
//         BuildEditor.GenericBuildInfo(buildParam);
//         BuildEditor.AndroidBuild(".apk", buildParam);
//     }


//     [MenuItem("Epic/Build/Android/DEV", priority = 102)]
//     static void PerformAndroidBuild_DEV()
//     {
//         var buildParam = BuildEditor.PerformCurruntBuildOption();
//         buildParam.UseServerType = "DEV";
//         buildParam.ServerURL = "https://wod-server-dev-kor.eyedentitygames.com/";
//         BuildEditor.GenericBuildInfo(buildParam);
//         BuildEditor.AndroidBuild(".apk", buildParam);
//     }

//     [MenuItem("Epic/Build/Android/DEV Profiler", priority = 102)]
//     static void PerformAndroidBuild_DEV_Profiler()
//     {
//         var buildParam = BuildEditor.PerformCurruntBuildOption();
//         buildParam.UseServerType = "DEV";
//         buildParam.ServerURL = "https://wod-server-qa-kor.eyedentitygames.com/";
//         buildParam.UseDebug = true;
//         buildParam.UseProfiler = true;
//         buildParam.LoginType = 1;
//         buildParam.PlatformType = PlatformManager.PlatformType.wod;
//         BuildEditor.GenericBuildInfo(buildParam);
//         BuildEditor.AndroidBuild(".apk", buildParam);
//     }

//     [MenuItem("Epic/Build/Android/NEXON", priority = 102)]
//     static void PerformAndroidBuild_NEXON()
//     {
//         var buildParam = BuildEditor.PerformCurruntBuildOption();
//         buildParam.VersionCode = 1000;
//         buildParam.UseServerType = "KOREA";
//         buildParam.PatchGroup = "NEXON";
//         buildParam.GameMode = "DEV";
//         buildParam.ServerURL = "https://wod-server-qa-kor.eyedentitygames.com/";
//         buildParam.LoginType = 1;
//         buildParam.PlatformType = PlatformManager.PlatformType.korea;
//         //buildParam.UseMono = true;

//         BuildEditor.GenericBuildInfo(buildParam);
//         BuildEditor.AndroidBuild(".apk", buildParam);
//     }

//     [MenuItem("Epic/Build/Android/NEXON2", priority = 102)]
//     static void PerformAndroidBuild_NEXON2()
//     {
//         var buildParam = BuildEditor.PerformCurruntBuildOption();
//         buildParam.VersionCode = 1000;
//         buildParam.UseServerType = "NEXON2";
//         buildParam.GameVersion = "1.5.3";
//         buildParam.PatchGroup = "NEXON";
//         buildParam.GameMode = "NORMAL";
//         buildParam.ServerURL = "https://wod-server-qa-kor.eyedentitygames.com/";
//         buildParam.LoginType = 2;
//         buildParam.PlatformType = PlatformManager.PlatformType.nexon;
//         //buildParam.UseMono = true;

//         BuildEditor.GenericBuildInfo(buildParam);
//         BuildEditor.AndroidBuild(".apk", buildParam);
//     }


//     [MenuItem("Epic/Build/Android/NEXON LIVE", priority = 102)]
//     static void PerformAndroidBuild_NEXON_LIVE()
//     {
//         var buildParam = BuildEditor.PerformCurruntBuildOption();
//         buildParam.UseServerType = "NEXON_LIVE";
//         buildParam.PatchGroup = "NEXON";
//         buildParam.GameMode = "LIVE";
//         buildParam.ServerURL = "https://wodn.dn.nexoncdn.co.kr/";
//         buildParam.LoginType = 2;
//         BuildEditor.GenericBuildInfo(buildParam);
//         BuildEditor.AndroidBuild(".apk", buildParam);
//     }

//     [MenuItem("Epic/Build/Android/NXKR LIVE", priority = 102)]
//     static void PerformAndroidBuild_NXKR_LIVE()
//     {
//         var buildParam = BuildEditor.PerformCurruntBuildOption();
//         buildParam.UseServerType = "NXKR_LIVE";
//         buildParam.PatchGroup = "NEXON";
//         buildParam.GameMode = "LIVE";
//         buildParam.ServerURL = "https://wodn.dn.nexoncdn.co.kr/";
//         buildParam.LoginType = 2;
//         BuildEditor.GenericBuildInfo(buildParam);
//         BuildEditor.AndroidBuild(".apk", buildParam);
//     }

//     [MenuItem("Epic/Build/Android/NEXON STR", priority = 102)]
//     static void PerformAndroidBuild_NEXON_STR()
//     {
//         var buildParam = BuildEditor.PerformCurruntBuildOption();
//         buildParam.UseServerType = "NEXON_STR";
//         buildParam.PatchGroup = "NEXON";
//         buildParam.GameMode = "DEV";
//         buildParam.ServerURL = "https://wodn.dn.nexoncdn.co.kr/";
//         buildParam.LoginType = 2;
//         BuildEditor.GenericBuildInfo(buildParam);
//         BuildEditor.AndroidBuild(".apk", buildParam);
//     }

//     [MenuItem("Epic/Build/Android/NEXON MONO", priority = 102)]
//     static void PerformAndroidBuild_NEXON_MONO()
//     {
//         var buildParam = BuildEditor.PerformCurruntBuildOption();
//         buildParam.UseServerType = "NEXON";
//         buildParam.PatchGroup = "CBT";
//         buildParam.GameMode = "NORMAL";
//         buildParam.ServerURL = "https://wodn.dn.nexoncdn.co.kr/";
//         buildParam.LoginType = 2;
//         buildParam.UseMono = true;
//         BuildEditor.GenericBuildInfo(buildParam);
//         BuildEditor.AndroidBuild(".apk", buildParam);
//     }

//     [MenuItem("Epic/Build/Android/NEXON Profiler", priority = 102)]
//     static void PerformAndroidBuild_NEXON_Profiler()
//     {
//         var buildParam = BuildEditor.PerformCurruntBuildOption();
//         buildParam.UseServerType = "NEXON";
//         buildParam.PatchGroup = "CBT";
//         buildParam.GameMode = "DEV";
//         buildParam.ServerURL = "https://wodn.dn.nexoncdn.co.kr/";
//         buildParam.UseDebug = true;
//         buildParam.UseProfiler = true;
//         buildParam.LoginType = 2;
//         BuildEditor.GenericBuildInfo(buildParam);
//         BuildEditor.AndroidBuild(".apk", buildParam);
//     }

//     [MenuItem("Epic/Build/Android/Gamania", priority = 102)]
//     static void PerformAndroidBuild_Gamania()
//     {
//         var buildParam = BuildEditor.PerformCurruntBuildOption();
//         buildParam.UseServerType = "GAMANIA_QA";
//         buildParam.PatchGroup = "NEXON";
//         buildParam.GameMode = "DEV";
//         buildParam.ServerURL = "https://wod-server-qa-kor.eyedentitygames.com/";
//         buildParam.LoginType = 3;
//         buildParam.PlatformType = PlatformManager.PlatformType.gamania;

//         BuildEditor.GenericBuildInfo(buildParam);
//         BuildEditor.AndroidBuild(".apk", buildParam);
//     }

//     [MenuItem("Epic/Build/Android/UWA", priority = 102)]
//     static void PerformAndroidBuild_UWA()
//     {
//         var buildParam = BuildEditor.PerformCurruntBuildOption();
//         buildParam.UseServerType = "QA1";
//         buildParam.PatchGroup = "NEXON";
//         buildParam.GameMode = "NORMAL";
//         buildParam.ServerURL = "https://wod-server-qa-kor.eyedentitygames.com/";
//         buildParam.LoginType = 1;
//         buildParam.PlatformType = PlatformManager.PlatformType.wod;
//         buildParam.UseDebug = true;
//         buildParam.UseMono = true;
//         buildParam.UseUWA = true;
//         BuildEditor.GenericBuildInfo(buildParam);
//         BuildEditor.AndroidBuild(".apk", buildParam);
//     }

//     [MenuItem("Epic/Build/Android/AAB_TEST", priority = 102)]
//     static void PerformAndroidBuild_AABTest()
//     {
//         var buildParam = BuildEditor.PerformCurruntBuildOption();
//         buildParam.UseServerType = "NEXON";
//         buildParam.GameMode = "DEV";
//         buildParam.PatchGroup = "NEXON";
//         buildParam.ServerURL = "https://wodn.dn.nexoncdn.co.kr/";
//         //buildParam.ServerURL = "https://wod-server-qa-kor.eyedentitygames.com/";
//         BuildEditor.GenericBuildInfo(buildParam);
//         buildParam.LoginType = 2;
//         buildParam.PlatformType = PlatformManager.PlatformType.nexon;
//         buildParam.UseAAB = true;
//         BuildEditor.AndroidBuild(".apk", buildParam);
//     }


//     [MenuItem("Epic/Build/Android/TEST", priority = 102)]
//     static void PerformAndroidBuild_Test()
//     {
//         var buildParam = BuildEditor.PerformCurruntBuildOption();
//         buildParam.UseServerType = "NEXON";
//         buildParam.GameMode = "DEV";
//         buildParam.PatchGroup = "NEXON";
//         buildParam.ServerURL = "https://wodn.dn.nexoncdn.co.kr/";
//         //buildParam.ServerURL = "https://wod-server-qa-kor.eyedentitygames.com/";
//         BuildEditor.GenericBuildInfo(buildParam);
//         buildParam.LoginType = 2;
//         buildParam.PlatformType = PlatformManager.PlatformType.nexon;
//         BuildEditor.AndroidBuild(".apk", buildParam);
//     }

//     [MenuItem("Epic/Build/Android/LCM", priority = 102)]
//     static void PerformAndroidBuild_LCM()
//     {
//         var buildParam = BuildEditor.PerformCurruntBuildOption();
//         buildParam.UseServerType = "LCM";
//         buildParam.GameMode = "DEV";
//         buildParam.PatchGroup = "NEXON";
//         buildParam.ServerURL = "https://wod-server-qa-kor.eyedentitygames.com/";
//         buildParam.LoginType = 1;
//         buildParam.PlatformType = PlatformManager.PlatformType.wod;
//         //buildParam.UseMono = true;

//         //BuildEditor.GenericBuildInfo(buildParam);
//         BuildEditor.AndroidBuild(".apk", buildParam);
//     }

//     [MenuItem("Epic/Build/Android/NexonThai CBT", priority = 102)]
//     static void PerformNTCAndroidBuild()
//     {
//         var buildParam = BuildEditor.PerformBuildParam();
//         buildParam.GameVersion = CurrentBundleVersion.GameVersion;
//         buildParam.AssetBundleVersion = CurrentBundleVersion.BundleVersion;
//         buildParam.VersionCode = CurrentBundleVersion.VersionCode;
//         buildParam.UseServerType = "NEXON";
//         buildParam.PatchGroup = "NEXON";
//         buildParam.ServerURL = "https://wodn.dn.nexoncdn.co.kr/";
//         buildParam.UseMono = false;
//         BuildEditor.GenericBuildInfo( buildParam );
//         BuildEditor.AndroidBuild( ".apk", buildParam );
//     }

//     [MenuItem("Epic/Build/Android/Mono", priority = 102)]
//     static void PerformAndroidBuild_Mono()
//     {
//         var buildParam = BuildEditor.PerformCurruntBuildOption();
//         buildParam.UseServerType = "Mono";
//         buildParam.ServerURL = "https://wod-server-qa-kor.eyedentitygames.com/";
//         buildParam.GameMode = "NORMAL";
//         //buildParam.PlatformType = "wod";
//         BuildEditor.GenericBuildInfo(buildParam);
//         BuildEditor.AndroidBuild(".apk", buildParam);
//     }

//     [MenuItem("Epic/Build/Android/Platform WOD", priority = 113)]
//     static void PerformAndroidBuild_Platform_WOD()
//     {
//         var buildParam = BuildEditor.PerformCurruntBuildOption();
//         buildParam.UseServerType = "LCM";
//         buildParam.GameMode = "NORMAL";
//         buildParam.PatchGroup = "CBT";
//         buildParam.ServerURL = "https://wod-server-qa-kor.eyedentitygames.com/";

//         buildParam.PlatformType = PlatformManager.PlatformType.wod;

//         BuildEditor.AndroidBuild(".apk", buildParam);
//     }

//     [MenuItem("Epic/Build/Android/STH", priority = 102)]
//     static void PerformAndroidBuild_STH()
//     {
//         var buildParam = BuildEditor.PerformCurruntBuildOption();
//         buildParam.UseServerType = "STH";
//         buildParam.GameMode = "DEV";
//         buildParam.PatchGroup = "NEXON";
//         buildParam.ServerURL = "http://wod-server-qa-kor.eyedentitygames.com/";
//         buildParam.LoginType = 2;
//         BuildEditor.GenericBuildInfo(buildParam);
//         buildParam.PlatformType = PlatformManager.PlatformType.wod;
//         BuildEditor.AndroidBuild(".apk", buildParam);
//     }

//     #endregion

//     #region iOS

//     //[MenuItem("Epic/Build/iOS", priority = 2)]
//     static void PerformiOSBuild()
//     {
//         BuildEditor.PerformiOSBuild();
//     }

//     [MenuItem("Epic/Build/iOS/QA1", priority = 214)]
//     static void PerformiOSBuild_QA1()
//     {
//         var buildParam = BuildEditor.PerformCurruntBuildOption();
//         buildParam.UseServerType = "QA1";
//         buildParam.GameMode = "NORMAL";
//         buildParam.PatchGroup = "NEXON";
//         buildParam.ServerURL = "https://wod-server-qa-kor.eyedentitygames.com/";
//         buildParam.LoginType = 1;
//         buildParam.PlatformType = PlatformManager.PlatformType.wod;
//         //PlatformManager.PlatformChange(buildParam.PlatformType);
//         BuildEditor.GenericBuildInfo(buildParam);
//         BuildEditor.iOSBuild(Path.GetFullPath("Build"), buildParam);
//     }

//     [MenuItem("Epic/Build/iOS/NEXON", priority = 214)]
//     static void PerformiOSBuild_NEXON()
//     {
//         var buildParam = BuildEditor.PerformCurruntBuildOption();
//         buildParam.UseServerType = "KOREA";
//         buildParam.GameMode = "DEV";
//         buildParam.PatchGroup = "NEXON";
//         buildParam.ServerURL = "https://wod-server-qa-kor.eyedentitygames.com/";
//         buildParam.LoginType = 1;
//         buildParam.PlatformType = PlatformManager.PlatformType.korea;
//         BuildEditor.GenericBuildInfo(buildParam);
//         BuildEditor.iOSBuild(Path.GetFullPath("Build"), buildParam);
//     }


//     [MenuItem("Epic/Build/iOS/Platform WOD", priority = 213)]
//     static void PerformiOSBuild_Platform_WOD()
//     {
//         var buildParam = BuildEditor.PerformCurruntBuildOption();
//         buildParam.UseServerType = "NEXON";
//         buildParam.GameMode = "NORMAL";
//         buildParam.PatchGroup = "CBT";
//         buildParam.ServerURL = "https://wod-server-qa-kor.eyedentitygames.com/";
//         buildParam.LoginType = 1;
//         buildParam.PlatformType = PlatformManager.PlatformType.wod;

//         BuildEditor.iOSBuild(Path.GetFullPath("Build"), buildParam);
//         //BuildEditor.AndroidBuild(".apk", buildParam);
//     }


//     [MenuItem("Epic/Build/iOS/Dev", priority = 202)]
//     static void PerformiOSBuild_Dev()
//     {
//         BuildEditor.PerformiOSBuild_Dev();
//     }
//     #endregion

//     #region PC

//     [MenuItem("Epic/Build/PC/Test", priority = 301)]
//     static void PerformPCBuild_Test()
//     {
//         var buildParam = BuildEditor.PerformBuildParam();
//         buildParam.GameVersion = "0.0.3";
//         buildParam.AssetBundleVersion = "0.0.3";
//         buildParam.VersionCode = 3;
//         buildParam.UseServerType = "PC";
//         buildParam.ServerURL = "https://wod-server-dev-kor.eyedentitygames.com/";
//         buildParam.GameMode = "DEV";
//         buildParam.PatchGroup = "CBT";
//         buildParam.LoginType = 1;
//         BuildEditor.GenericBuildInfo(buildParam);

//         string strTargetFileName = BuildUtil.GetBuildDirectory() + "/" + "WOD_PC" + "/" + BuildEditor.APP_NAME + ".exe";
//         BuildEditor.PCBuild(strTargetFileName, buildParam);
//     }

//     [MenuItem("Epic/Build/PC/Nexon", priority = 301)]
//     static void PerformPCBuild_Nexon()
//     {
//         var buildParam = BuildEditor.PerformCurruntBuildOption();
//         buildParam.UseServerType = "NEXON";
//         buildParam.GameMode = "NORMAL";
//         buildParam.PatchGroup = "CBT";
//         buildParam.ServerURL = "https://wodn.dn.nexoncdn.co.kr/";
//         buildParam.LoginType = 2;

//         BuildEditor.GenericBuildInfo(buildParam);

//         string strTargetFileName = BuildUtil.GetBuildDirectory() + "/" + "WOD_PC" + "/" + BuildEditor.APP_NAME + ".exe";
//         BuildEditor.PCBuild(strTargetFileName, buildParam);
//     }

//     [MenuItem("Epic/Build/PC/DEV", priority = 302)]
//     static void PerformPCBuild()
//     {
//         var buildParam = BuildEditor.PerformCurruntBuildOption();
//         buildParam.UseServerType = "DEV";
//         buildParam.GameMode = "DEV";
//         buildParam.PatchGroup = "NEXON";
//         buildParam.ServerURL = "https://wod-server-qa-kor.eyedentitygames.com/";
//         buildParam.LoginType = 1;
//         buildParam.PlatformType = PlatformManager.PlatformType.wod;
//         BuildEditor.GenericBuildInfo(buildParam);

//         string path = BuildUtil.GetBuildDirectory();
//         string strTargetFileName = path + "/" + "WOD_PC" + "/" + BuildEditor.APP_NAME + ".exe";

//         BuildEditor.PCBuild(strTargetFileName, buildParam);
//     }


//     [MenuItem("Epic/Build/PC/Debug", priority = 303)]
//     static void PerformPCBuild_Dev()
//     {
//         var buildOption = BuildEditor.PerformBuildParam();
//         buildOption.UseDebug = true;
//         buildOption.GameMode = "DEV";

//         string path = BuildUtil.GetBuildDirectory();
//         string strTargetFileName = path + "/" + "WOD_PC_DEV" + "/" + BuildEditor.APP_NAME + "_dev.exe";

//         BuildEditor.PCBuild(strTargetFileName, buildOption);
//     }

//     [MenuItem("Epic/Build/PC/NoRender", priority = 304)]
//     static void PerformPCBuild_NoRender()
//     {
//         string path = BuildUtil.GetBuildDirectory();
//         string strTargetFileName = path + "/" + "WOD_PC_NoRender" + "/" + BuildEditor.APP_NAME + ".exe";

//         BuildEditor.PCBuild(strTargetFileName, new BuildParam() { UseDebug = false, UseNoRender = true });
//     }


//     [MenuItem("Epic/Build/PC/NoRender Patch", priority = 305)]
//     static void PerformPCBuild_NoRender_patch()
//     {
//         string path = BuildUtil.GetBuildDirectory();
//         string strTargetFileName = path + "/" + "WOD_PC_NoRender" + "/" + BuildEditor.APP_NAME + "_patch.exe";

//         BuildEditor.PCBuild(strTargetFileName, new BuildParam() { UseDebug = false, UseNoRender = true, OnlyPatchMode = true });
//     }
//     #endregion
//     //[MenuItem("Epic/Encrypt/Android", priority = 1)]
//     //static void PerformAndroidEncrypt()
//     //{
//     //    string appName = BuildUtil.GetCmdParamString("-appname");
//     //    if (string.IsNullOrEmpty(appName))
//     //    {
//     //        appName = APP_NAME;
//     //    }
//     //    string strTargetFileName = string.Format("{0}/{1}{2}", GetBuildDirectory(), appName, ".apk");
//     //    string key = KEY_PASSWORD;
//     //    bool onCommand = true;

//     //    //J3Tech.CryptAndroidAssemblyEditor.EncryptApk(strTargetFileName, key, onCommand);
//     //}

//     // for test
//     [MenuItem("Epic/Build/BuildTest1", priority = 402)]
//     static void BuildTest1()
//     {
//         BuildEditor.StreamAssetChange(BuildTargetGroup.Android);
//     }
//     [MenuItem("Epic/Build/BuildTest2", priority = 402)]
//     static void BuildTest2()
//     {
//         BuildEditor.StreamAssetChange(BuildTargetGroup.iOS);
//     }
// }

