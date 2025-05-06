// using System;
// using System.Collections.Generic;
// using YamlDotNet.Serialization;
// using KZLib.KZUtility;
// using KZLib.KZData;
// using System.IO;
// using UnityEngine;
// using UnityEngine.Events;
// using UnityEngine.Localization.Settings;
// using Cysharp.Threading.Tasks;
// using System.Threading;
// using System.Diagnostics;
// using UnityEngine.ResourceManagement.AsyncOperations;

// namespace KZLib.KZData
// {
// 	public class LingoMgr : Singleton<LingoMgr>
// 	{
// 		private bool m_disposed = false;

// 		private SystemLanguage m_language = SystemLanguage.English;

// 		private readonly Dictionary<SystemLanguage,Dictionary<string,string>> m_languageTextDict = new();

// 		public event Action OnLocalizationChange = null;

// 		private bool m_isLoaded = false;

// 		protected override void Initialize()
// 		{
// 			base.Initialize();

// 			m_languageTextDict.Clear();

// 			var tableList = LocalizationSettings.StringDatabase.GetAllTables();

// 			// foreach(var table in tableList)
// 			// {
				
// 			// }

// 			// localizationSettings.get

// 			// var tables = LocalizationSettings.StringDatabase.GetTableNames();


// 			// var stringTables = LocalizationSettings.StringDatabase.name();

// 			// foreach (var table in stringTables)
// 			// {
// 			// 	var entry = table.GetEntry("TestKey");
// 			// 	if (entry != null)
// 			// 	{
// 			// 		Debug.Log($"Table: {table.TableCollectionName}, Entry: {entry.Key}, Value: {entry.GetLocalizedString()}");
// 			// 	}
// 			// }


// 			// if (table != null)
// 			// {
// 			// 	var entry = table.GetEntry(entryName);
// 			// 	if (entry != null)
// 			// 	{
// 			// 		textComponent.text = entry.GetLocalizedString();
// 			// 	}
// 			// }

// 			// LocalizationSettings.AssetDatabase.GetAllTables();

// 			// // 



// 			// // TODO 유니티 로컬라이즈 찾아보기.... -> 언어는 구글 시트 이용? & 루아 써서 실시간도 생각해보기
// 			// foreach(var textAsset in ResMgr.In.GetTextAssetArray(ConfigMgr.In.Access<ConfigData.GameConfig>().LanguageFolderPath))
// 			// {
// 			// 	var languageDict = JsonConvert.DeserializeObject<Dictionary<string,string>>(textAsset.text);

// 			// 	if(languageDict.IsNullOrEmpty())
// 			// 	{
// 			// 		continue;
// 			// 	}

// 			// 	m_languageTextDict.Add(textAsset.name.ToEnum<SystemLanguage>(),languageDict);
// 			// }

// 			// var optionCfg = ConfigMgr.In.Access<ConfigData.OptionConfig>();

// 			// optionCfg.OnLanguageChange += _OnChangeLanguage;

// 			// _OnChangeLanguage(optionCfg.Language);
// 		}

// 		protected override void Release(bool disposing)
// 		{
// 			if(m_disposed)
// 			{
// 				return;
// 			}

// 			if(disposing)
// 			{
				
// 			}

// 			m_disposed = true;

// 			base.Release(disposing);
// 		}

// 		// public async UniTask<bool> TryLoadAsync(CancellationToken token)
// 		// {
// 		// 	if(m_isLoaded)
// 		// 	{
// 		// 		return true;
// 		// 	}

// 		// 	var start = DateTime.Now;

// 		// 	var handle = LocalizationSettings.StringDatabase.GetAllTables();

// 		// 	await handle.Task;

// 		// 	// 비동기 작업이 완료되면, 테이블에 접근합니다.
// 		// 	if(handle.Status == AsyncOperationStatus.Succeeded)
// 		// 	{
// 		// 		foreach (var table in handle.Result)
// 		// 		{
// 		// 			Debug.Log("Table Name: " + table.TableCollectionName);
// 		// 		}
// 		// 	}
// 		// 	else
// 		// 	{
// 		// 		LogTag.System.E($"Failed to load tables: {handle.OperationException}");
// 		// 	}

// 		// 	if(!_TryGetTextAsset(out var textAssetArray))
// 		// 	{
// 		// 		return false;
// 		// 	}

// 		// 	var accumulatedTime = 0.0d;
// 		// 	var stopwatch = new Stopwatch();

// 		// 	LogTag.System.I("Proto Load Start");

// 		// 	for(var i=0;i<textAssetArray.Length;i++)
// 		// 	{
// 		// 		var textAsset = textAssetArray[i];

// 		// 		if(textAsset == null)
// 		// 		{
// 		// 			LogTag.System.W($"TextAsset is null in {i}");

// 		// 			continue;
// 		// 		}

// 		// 		stopwatch.Restart();

// 		// 		if(!_TryLoadProto(textAsset))
// 		// 		{
// 		// 			return false;
// 		// 		}

// 		// 		accumulatedTime += stopwatch.Elapsed.TotalSeconds;

// 		// 		if(accumulatedTime >= c_frameTime)
// 		// 		{
// 		// 			accumulatedTime = 0.0d;

// 		// 			await UniTask.Delay(c_delayTime,cancellationToken : token);
// 		// 		}
// 		// 	}

// 		// 	stopwatch.Stop();

// 		// 	LogTag.System.I($"Proto Load Complete [Count : {textAssetArray.Length} / Duration : {(DateTime.Now-start).TotalSeconds}]");

// 		// 	m_isLoaded = true;

// 		// 	return true;
// 		// }
// 	}
// }