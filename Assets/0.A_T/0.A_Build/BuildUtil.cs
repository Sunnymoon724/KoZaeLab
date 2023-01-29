using System;
using System.IO;
using System.Text;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

public class BuildUtil
{
    public static readonly string ASSETBUNDLES_NAME = "assetbundles";        // 에셋번
    public static readonly string JSON_NAME = "PatchList.txt";    // 에셋번
    public static readonly string PLATFORM_NAME = "platformData";

    public static readonly string BUNDLE_LIST = "ABFactoryFileList.json";
    public static readonly string BUILD_VERSION_NAME = "build_version";
    public static readonly string JSON_EXT = ".json";

    public static readonly string ASSETBUNDLE_EXT = ".epic";               // 어셋번들 확장자
    public static readonly string OTHERFILE_PATH = "OtherFile";

#if USE_NORENDER
    public static readonly string ROOT_PATH = Application.dataPath + "/StreamingAssets";
#elif UNITY_EDITOR && EDIT_USE_ASSETBUNDLE && USE_PATCH && PREVENT_PATCH_AT_USE_PATCH
    public static readonly string ROOT_PATH = Application.dataPath + "/..";
#else
    public static readonly string ROOT_PATH = Application.persistentDataPath;
#endif

    public static string LOCAL_PATH = ROOT_PATH + "/" + ASSETBUNDLES_NAME + "/" + GetPlatform() + "/";


    private static string GetCmdParam(string key)
    {
        string[] args = Environment.GetCommandLineArgs();
        int argIdx = Array.FindIndex(args, a => string.Compare(a, key, StringComparison.OrdinalIgnoreCase) == 0);
        if (argIdx != -1 && args.Length > argIdx + 1)
            return args[argIdx + 1];

        return null;
    }

    public static string GetCmdParamString(string key, string defaultValue = "")
    {
        string param = GetCmdParam(key);
        return param == null ? defaultValue : param;
    }

    public static int GetCmdParamInt(string key, int defaultValue = 0)
    {
        string param = GetCmdParam(key);
        return param == null ? defaultValue : int.Parse(param);
    }

    public static bool GetCmdParamBool(string key, bool defaultValue = false)
    {
        string param = GetCmdParam(key);

        if (param == null)
        {
            return defaultValue;
        }

        return param.ToLower().Equals("true") ? true : false;
    }

    public static bool HasCmdParam(string key)
    {
        return GetCmdParam(key) == null ? false : true;
    }

    /// <summary>
    /// CN：是否时命令行模式
    /// </summary>
    /// <returns></returns>
    public static bool CheckBatchMode()
    {
        return System.Environment.CommandLine.Contains("-batchMode");
    }
    
    public static string GetBuildDirectory()
    {
        string strPath = Application.dataPath.Remove(Application.dataPath.LastIndexOf("/Assets"), "/Assets".Length) + "/Build";
        if (Directory.Exists(strPath) == false)
        {
            Directory.CreateDirectory(strPath);
        }

        return strPath;
    }

    public static T ReadParam<T>() where T : new()
    {
        T buildParam = new T();
        Type buildParamType = buildParam.GetType();
        var buildParamProperties = buildParamType.GetProperties();
        for (int i = 0, len = buildParamProperties.Length; i < len; i++)
        {
            var buildParamProperty = buildParamProperties[i];
            var optionName = string.Format("-{0}", buildParamProperty.Name);

            Debug.Log("[test] buildParamProperty.Name:" + buildParamProperty.Name + " || " + buildParamProperty.PropertyType + " || " + optionName);

            if (buildParamProperty.PropertyType.Equals(typeof(bool)))
            {
                buildParamProperty.SetValue(buildParam, GetCmdParamBool(optionName));
            }
            else if (buildParamProperty.PropertyType.Equals(typeof(int)))
            {
                buildParamProperty.SetValue(buildParam, GetCmdParamInt(optionName));
            }
            else
            {
                buildParamProperty.SetValue(buildParam, GetCmdParamString(optionName));
            }
        }

        return buildParam;
    }


    public static int GetVersionCode(string version)
    {
        if (string.IsNullOrEmpty(version))
            return 0;

        var versionSplit = version.Split('.').Select(s => int.Parse(s)).ToList();
        if (versionSplit.Count < 4)
        {
            versionSplit.Add(0);
        }
        var count = 0;
        var len = versionSplit.Count;
        for (var i = 0; i < len; i++)
        {
            var splitN = versionSplit[((len - 1) - i)];
            var code = splitN << (i * 8);
            count += code;
        }
        return count;
    }

    public static int ReadRevisionFile()
    {
        int revisionSVN = 0;
        string revisionPath = Application.dataPath + "/../" + "svn_revision.txt";
        if (!File.Exists(revisionPath))
            return revisionSVN;

        string revisionFile = File.ReadAllText(revisionPath);
        if (string.IsNullOrEmpty(revisionFile))
            return revisionSVN;

        try
        {
            revisionSVN = int.Parse(revisionFile);
        }
        catch(Exception e)
        {
            // Debug.Log("[AB] ReadRevisionFile " , e , "[", revisionFile , "]");
        }

        return revisionSVN;
    }

    public static void SaveJsonFile(string path, object data)
    {
        Debug.Log("SaveJsonFile: " + path);

        string json = JsonUtility.ToJson(data, true);
        Tools.WriteDataToFile(path, json);
    }

    public static T LoadJsonFile<T>(string path)
    {
        Debug.Log("LoadJsonFile: " + path);
        if (File.Exists(path))
        {
            string jsonFile = File.ReadAllText(path);
            T jsonData = JsonUtility.FromJson<T>(jsonFile);
            return jsonData;
        }

        return default;
    }

    public static async Task<T> LoadJsonFileAsync<T>(string path)
    {
        if (!File.Exists(path))
            return default;
       
        string jsonFile = await Tools.ReadDataFromFileAsync(path);
        T jsonData = JsonUtility.FromJson<T>(jsonFile);
        return jsonData;
    }

    public static string GetPlatform()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsPlayer:
                return "standalonewindows64";
            case RuntimePlatform.IPhonePlayer:
                return "ios";
            case RuntimePlatform.Android:
                return "android";
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.LinuxEditor:
#if UNITY_ANDROID
                return "android";
#elif UNITY_IOS
                return "ios";
#elif UNITY_STANDALONE_WIN
                return "standalonewindows64";
#else
                UnityEngine.Debug.LogError("Unprocessed RuntimePlatform:" + Application.platform);
                return "android";
#endif
            default:
                UnityEngine.Debug.LogError("Unprocessed RuntimePlatform:" + Application.platform);
                return "android";
        }
    }


    public static long MemoryCheck()
    {
        return GC.GetTotalMemory(true);
    }
}
