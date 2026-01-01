#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using KZLib.KZUtility;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine.Networking;
using YamlDotNet.Serialization;

namespace KZLib.KZMenu
{
	public static partial class KZMenuItem2
	{
		private const string c_venderManifestPath = "defaultRes:Text:Setting:VendorManifest.yaml";
        private const long c_cacheDuration = 6*3600;

		private static readonly Regex s_versionRegex = new(@"^v?(\d+(\.\d+){1,3})");

		private record ResultInfo(bool Result,string Text);

		[Serializable]
		private class VendorManifestInfo
		{
			public long LastCheckTimestamp { get; set; }
			public Dictionary<string,VendorRepositoryInfo> RepositoryInfoDict { get; init; }
		}

		[Serializable]
		private record VendorRepositoryInfo(string GitHubRepositoryName,string LocalPackageJsonPath)
		{
			public VendorRepositoryInfo() : this(string.Empty,string.Empty) { }
		}

		[MenuItem("KZMenu/Explorer/Check Vendor Projects",false,MenuOrder.Explorer.VENDER)]
		private static void _OnCheckVendorProjects()
		{
			if(!CommonUtility.DisplayCheckBeforeExecute("Check vendor projects"))
			{
				return;
			}

			var vendorManifestInfo =_GetOrCreateVendorManifestInfo();

			if(vendorManifestInfo.RepositoryInfoDict.IsNullOrEmpty())
			{
				CommonUtility.DisplayInfo("vendorManifestInfoArray is null or empty.");

				return;
			}

			var timestamp = DateTimeOffset.Now.ToUnixTimeSeconds() - vendorManifestInfo.LastCheckTimestamp;

			if(timestamp < c_cacheDuration)
			{
				if(!CommonUtility.DisplayCheck("Refresh coolTime is left","The cool-down is not over yet. Do you still want to refresh?"))
				{
					return;
				}
			}

			_CheckVendorProjectsAsync(vendorManifestInfo).Forget();
		}

		private static async UniTaskVoid _CheckVendorProjectsAsync(VendorManifestInfo vendorManifestInfo)
		{
			var errorCount = 0;
			var updatedCount = 0;
			var vendorRepositoryInfoDict = vendorManifestInfo.RepositoryInfoDict;
			
			foreach(var pair in vendorRepositoryInfoDict)
			{
				var vendorRepositoryInfo = pair.Value;
				var localPath = vendorRepositoryInfo.LocalPackageJsonPath;
				var displayName = pair.Key;

				var versionResultInfo = _GetLocalVersion(vendorRepositoryInfo.LocalPackageJsonPath);

				if(!versionResultInfo.Result)
				{
					LogSvc.Editor.E($"{displayName} is error - {versionResultInfo.Text}");

					errorCount++;

					continue;
				}
				
				var repositoryName = vendorRepositoryInfo.GitHubRepositoryName;
				var gitHubResultInfo = await _GetLatestGitHubVersionAsync(repositoryName);

				if(!gitHubResultInfo.Result)
				{
					LogSvc.Editor.E($"{displayName} is error - {gitHubResultInfo.Text}");

					errorCount++;

					continue;
				}

				var localVersion = _CleanVersionString(versionResultInfo.Text);
				var latestVersion = _CleanVersionString(gitHubResultInfo.Text);

				try
				{
					if(new Version(latestVersion).CompareTo(new Version(localVersion)) > 0)
					{
						LogSvc.Editor.W($"{displayName} is need to update - [Local:{localVersion} / Latest:{latestVersion}] - <a href=\"https://github.com/{repositoryName}\">{displayName}</a>");

						updatedCount++;
					}
				}
				catch(Exception exception)
				{
					LogSvc.Editor.E($"{displayName} is error - {exception.Message}");
				}
			}
			
			if(errorCount > 0)
			{
				CommonUtility.DisplayInfo("error is exist.\n check the log.");

				return;
			}

			if(updatedCount > 0)
			{
				CommonUtility.DisplayInfo("detected need to update libraries.\n check the log.");
			}
			else
			{
				CommonUtility.DisplayInfo("all vender is latest version.");
			}

			vendorManifestInfo.LastCheckTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds();

			_SaveVendorManifestInfo(vendorManifestInfo);
		}

		private static VendorManifestInfo _GetOrCreateVendorManifestInfo()
		{
			var vendorManifestRoute = RouteManager.In.GetOrCreateRoute(c_venderManifestPath);
			var vendorManifestText = FileUtility.ReadFileToText(vendorManifestRoute.AbsolutePath);

			if(!vendorManifestText.IsEmpty())
			{
				var deserializer = new DeserializerBuilder().Build();

				return deserializer.Deserialize<VendorManifestInfo>(vendorManifestText);
			}
			else
			{
				var newVendorManifestInfo = new VendorManifestInfo()
				{
					LastCheckTimestamp = 0L,
					RepositoryInfoDict = new Dictionary<string,VendorRepositoryInfo>
					{
						{ "UniTask", new VendorRepositoryInfo("Cysharp/UniTask","Assets/KZLib/Plugins/UniTask/package.json") },
					},
				};

				_SaveVendorManifestInfo(newVendorManifestInfo);

				return newVendorManifestInfo;
			}
		}

		private static void _SaveVendorManifestInfo(VendorManifestInfo vendorManifestInfo)
		{
			var vendorRoute = RouteManager.In.GetOrCreateRoute(c_venderManifestPath);

			var serializer = new SerializerBuilder().Build();
			var yamlText = serializer.Serialize(vendorManifestInfo);

			FileUtility.WriteTextToFile(vendorRoute.AbsolutePath,yamlText);

			CommonUtility.SaveAsset();
		}

		private static ResultInfo _GetLocalVersion(string relativePath)
		{
			var absolutePath = FileUtility.GetAbsolutePath(relativePath,true);
			
			if(!FileUtility.IsFileExist(absolutePath))
			{
				return new ResultInfo(false,"File not found");
			}
			
			var json = JObject.Parse(FileUtility.ReadFileToText(absolutePath));

			if(!json.TryGetValue("version",out JToken version))
			{
				return new ResultInfo(false,"Version not found");
			}

			return new ResultInfo(true,version.ToString());
		}

		private static async UniTask<ResultInfo> _GetLatestGitHubVersionAsync(string repositoryName)
		{
			using var webRequest = UnityWebRequest.Get($"https://api.github.com/repos/{repositoryName}/releases/latest");

			webRequest.SetRequestHeader("User-Agent","KZLib-VendorChecker");

			await webRequest.SendWebRequest();

			if(webRequest.result != UnityWebRequest.Result.Success)
			{
				return new ResultInfo(false,$"{repositoryName} is error. [{webRequest.error}]");
			}

			var handlerText = webRequest.downloadHandler.text;
			var json = JObject.Parse(handlerText);

			if(!json.TryGetValue("tag_name",out JToken tagName))
			{
				return new ResultInfo(false,$"{repositoryName} is error. [tag_name not found:{handlerText}]");
			}

			return new ResultInfo(true,tagName.ToString());
		}

		private static string _CleanVersionString(string rawVersion)
		{
			if(!rawVersion.IsEmpty())
			{
				var match = s_versionRegex.Match(rawVersion);

				if(match.Success && match.Groups.Count > 1)
				{
					return match.Groups[1].Value;
				}
			}

			return null; 
		}
	}
}
#endif