using System;
using System.Text;
using UnityEngine;

public class BuildParam
{
    // Jenkins 에서 프로퍼티를 option 으로 사용함
    // int, bool, string 형만 사용

    public bool UseDebug { get; set; }
    public bool UseOpenServer { get; set; }
    public bool UseFGTServer { get; set; }
    public bool UseQAServer { get; set; }
    public bool UseProfiler { get; set; }
    public bool UseNoRender { get; set; }
    public bool OnlyPatchMode { get; set; }
    public string GameMode { get; set; }
    public string GameVersion { get; set; }
    public int VersionCode { get; set; }
    public string AssetBundleVersion { get; set; }
    public string UseServerType { get; set; }
    public string ServerURL { get; set; }
    public string PortarServerUrl { get; set; }
    /// <summary>
    /// CN: 服务器大区ID（可被用于 GHome分大区激活账号，一般 一个大区ID对应2个包 安卓包和iOS包）
    /// </summary>
    public int ServerRegionID { get; set; }

    public string BuildTimeStamp { get; set; }
    // public PlatformManager.PlatformType PlatformType { get; set; }
    public string PatchGroup { get; set; }
    public int LoginType { get; set; }
    public bool UseMono { get; set; }
    public bool UseUWA { get; set; }
    public bool UseAAB { get; set; }

    /// <summary>
    /// CN: 是否 使用盛趣的代码热更sdk
    /// </summary>
    public bool UseSqCodeUpdateSdk { get; set; }

    #region For ios CodeSigning
    public string SIGN_TEAM_ID { get; set; }
    public string PROVISIONING_PROFILE_UUID { get; set; }
    public string SIGN_TEAM_NAME { get; set; }
    #endregion

    #region For PC Settings
    /// <summary>
    /// CN：用于区分韩方已有的设置
    /// </summary>
    public bool useV2Settings = false;
    public int defaultScreenWidth = 1280;
    public int defaultScreenHeight = 800;
    public FullScreenMode fullScreenMode = FullScreenMode.Windowed;
    public bool displayResolutionDialog = false;
    public bool resizableWindow = false;
    public int masterTextureLimit = 2;
    public int qualityLevelIndex = 0;
    #endregion


    public string GetApkExtensionName()
    {
        string extentionName = UseAAB ? ".aab" : ".apk";
        return extentionName;
    }
}
