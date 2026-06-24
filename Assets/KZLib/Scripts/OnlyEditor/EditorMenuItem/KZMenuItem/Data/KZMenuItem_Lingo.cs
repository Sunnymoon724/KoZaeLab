#if UNITY_EDITOR
using UnityEditor;
using System;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using KZLib.ToolKits;
using UnityEditor.AddressableAssets;
using UnityEngine;
using System.IO;
using UnityEditor.Localization;
using System.Collections.Generic;
using KZLib.Utilities;

namespace KZLib.EditorInternal.Menus
{
	public static partial class KZMenuItem
	{
		private const string c_key = "Key";
		private const string c_stringExcelName = "String";
		private const string c_assetExcelName = "Asset";
		private const string c_tableNameSuffix = "Table";

		#region Lingo Menu
		/// <summary>
		/// excel file -> asset file
		/// </summary>
		[MenuItem("KZMenu/Lingo/Generate Lingo",false,MenuOrder.Data.GENERATE_LINGO)]
		private static void _OnGenerateLingo()
		{
			if(!KZEditorKit.DisplayCheck(
				"Generate Lingo",
				"Excel sync may remove lingo tables and keys that are not present in the Excel files.\nContinue?"))
			{
				return;
			}

			_EnsureLocalizationSettings();

			var lingoRoute = RouteManager.In.Fetch("defaultRes:lingo");
			var errorCount = 0;

			foreach(var lingoFilePath in KZFileKit.FindExcelFilesInFolder(Global.LingoFolderPath))
			{
				try
				{
					if(!_ProcessLingoExcelFile(lingoFilePath,lingoRoute.AssetPath))
					{
						errorCount++;
					}
				}
				catch(Exception exception)
				{
					LogChannel.Editor.E(exception);

					errorCount++;
				}
			}

			if(errorCount > 0)
			{
				_DisplayInfo($"Generate finished with {errorCount} error(s).\nCheck the log.",true);

				return;
			}

			_DisplayGenerateEnd();
		}

		[MenuItem("KZMenu/Lingo/Open Lingo Folder",false,MenuOrder.Data.OPEN_LINGO)]
		private static void _OnOpenLingoFolder()
		{
			_OpenFolder("Lingo",Global.LingoFolderPath);
		}
		#endregion Lingo Menu

		#region Lingo Generate Pipeline
		private static void _EnsureLocalizationSettings()
		{
			AddressableAssetSettingsDefaultObject.GetSettings(true);

			var localizationSettings = LocalizationSettings.GetInstanceDontCreateDefault();

			if(localizationSettings != null)
			{
				return;
			}

			localizationSettings = ScriptableObject.CreateInstance<LocalizationSettings>();

			KZAssetKit.CreateAsset(Path.Combine("Localization","LocalizationSettings.asset"),localizationSettings,true);

			EditorBuildSettings.AddConfigObject("com.unity.localization.settings",localizationSettings,true);
		}

		private static bool _ProcessLingoExcelFile(string lingoFilePath,string lingoLocaleAssetPath)
		{
			if(!KZFileKit.IsExcelFile(lingoFilePath))
			{
				LogChannel.Editor.W($"{lingoFilePath} is not excel file. -> generate skipped");

				return false;
			}

			if(!LingoGenerator.TryConvertToDictionary<SystemLanguage>(lingoFilePath,out var languageHashSet,out var lingoDict))
			{
				LogChannel.Editor.W($"{lingoFilePath} convert failed. -> generate skipped");

				return false;
			}

			var localeContext = _SyncLocalesFromExcel(languageHashSet,lingoLocaleAssetPath);
			var excelName = KZFileKit.GetOnlyFileName(lingoFilePath);

			if(excelName.IsEqual(c_stringExcelName))
			{
				return _GenerateStringLingo(lingoDict,localeContext);
			}

			if(excelName.IsEqual(c_assetExcelName))
			{
				return _GenerateAssetLingo(lingoDict,localeContext);
			}

			LogChannel.Editor.W($"Excel file name must be {c_stringExcelName} or {c_assetExcelName}. [{excelName}]");

			return false;
		}

		private readonly struct LingoLocaleContext
		{
			public HashSet<LocaleIdentifier> IdentifierHashSet { get; init; }
			public Dictionary<LocaleIdentifier,SystemLanguage> LanguageByIdentifier { get; init; }
		}

		private static LingoLocaleContext _SyncLocalesFromExcel(HashSet<SystemLanguage> languageHashSet,string lingoLocaleAssetPath)
		{
			var identifierHashSet = new HashSet<LocaleIdentifier>();
			var languageByIdentifier = new Dictionary<LocaleIdentifier,SystemLanguage>();

			foreach(var language in languageHashSet)
			{
				var assetPath = Path.Combine(lingoLocaleAssetPath,$"{language}.asset");
				var locale = Locale.CreateLocale(language);

				identifierHashSet.Add(locale.Identifier);
				languageByIdentifier[locale.Identifier] = language;

				KZAssetKit.CreateAsset(assetPath,locale,false);
			}

			return new LingoLocaleContext
			{
				IdentifierHashSet = identifierHashSet,
				LanguageByIdentifier = languageByIdentifier,
			};
		}

		private static bool _GenerateStringLingo(Dictionary<string,Dictionary<string,string[]>> lingoDict,LingoLocaleContext localeContext)
		{
			var tableNameHashSet = new HashSet<string>();
			var hasError = false;

			foreach(var pair in lingoDict)
			{
				var sheetName = pair.Key;
				var collection = _GetOrCreateStringTableCollection(sheetName);

				_SyncStringTableCollection(collection,localeContext.IdentifierHashSet);

				if(!_ApplyStringToTableCollection(collection,sheetName,pair.Value,localeContext.LanguageByIdentifier))
				{
					hasError = true;
				}

				tableNameHashSet.Add(collection.name);
			}

			_RemoveUnusedStringTableCollections(tableNameHashSet);

			return !hasError;
		}

		private static bool _GenerateAssetLingo(Dictionary<string,Dictionary<string,string[]>> lingoDict,LingoLocaleContext localeContext)
		{
			var tableNameHashSet = new HashSet<string>();
			var hasError = false;

			foreach(var pair in lingoDict)
			{
				var sheetName = pair.Key;
				var collection = _GetOrCreateAssetTableCollection(sheetName);

				_SyncAssetTableCollection(collection,localeContext.IdentifierHashSet);

				if(!_ApplyAssetTableCollection(collection,sheetName,pair.Value,localeContext.LanguageByIdentifier))
				{
					hasError = true;
				}

				tableNameHashSet.Add(collection.name);
			}

			_RemoveUnusedAssetTableCollections(tableNameHashSet);

			return !hasError;
		}

		private static void _RemoveUnusedStringTableCollections(HashSet<string> activeTableNameHashSet)
		{
			_RemoveUnusedTableCollections(activeTableNameHashSet,c_stringExcelName,LocalizationEditorSettings.GetStringTableCollections());
		}

		private static void _RemoveUnusedAssetTableCollections(HashSet<string> activeTableNameHashSet)
		{
			_RemoveUnusedTableCollections(activeTableNameHashSet,c_assetExcelName,LocalizationEditorSettings.GetAssetTableCollections());
		}

		private static void _RemoveUnusedTableCollections<TCollection>(HashSet<string> activeTableNameHashSet,string excelName,IReadOnlyList<TCollection> tableList) where TCollection : UnityEngine.Object
		{
			var removedAny = false;

			for(var i=0;i<tableList.Count;i++)
			{
				var tableName = tableList[i].name;

				if(activeTableNameHashSet.Contains(tableName))
				{
					continue;
				}

				if(_TryRemoveLingoTableCollectionFolder(excelName,tableName))
				{
					removedAny = true;
				}
			}

			if(removedAny)
			{
				AssetDatabase.Refresh();
			}
		}

		private static bool _TryRemoveLingoTableCollectionFolder(string excelName,string tableName)
		{
			var sheetName = _GetSheetNameFromTableName(tableName);
			var folderPath = Path.Combine("Assets","Localization",excelName,sheetName);

			if(AssetDatabase.IsValidFolder(folderPath))
			{
				return AssetDatabase.DeleteAsset(folderPath);
			}

			var absolutePath = KZFileKit.GetAbsolutePath(folderPath,true);

			if(!KZFileKit.IsFolderExist(absolutePath))
			{
				return false;
			}

			KZFileKit.DeleteFolder(absolutePath,true);

			return true;
		}

		private static string _GetSheetNameFromTableName(string tableName)
		{
			if(tableName.EndsWith(c_tableNameSuffix,StringComparison.Ordinal))
			{
				return tableName.Substring(0,tableName.Length - c_tableNameSuffix.Length);
			}

			return tableName;
		}
		#endregion Lingo Generate Pipeline

		#region Lingo Common
		private static void _CheckUnusedKeys(SharedTableData tableData,HashSet<string> usedKeyHashSet)
		{
			var sharedData = tableData;
			var unusedKeyList = new List<string>();

			for(var i=0;i<sharedData.Entries.Count;i++)
			{
				var entryKey = sharedData.Entries[i].Key;

				if(!usedKeyHashSet.Contains(entryKey))
				{
					unusedKeyList.Add(entryKey);
				}
			}

			for(var i=0;i<unusedKeyList.Count;i++)
			{
				sharedData.RemoveKey(unusedKeyList[i]);
			}

			EditorUtility.SetDirty(sharedData);
		}

		private static string _GetValueByLanguage(SystemLanguage language,string[] schemeArray,string[] cellArray)
		{
			for(var i=0;i<schemeArray.Length;i++)
			{
				if(i >= cellArray.Length)
				{
					break;
				}

				var scheme = schemeArray[i];

				if(scheme.IsEqual(language.ToString()))
				{
					return cellArray[i];
				}

				if(Enum.TryParse(scheme,out SystemLanguage schemeLanguage) && schemeLanguage == language)
				{
					return cellArray[i];
				}
			}

			return string.Empty;
		}
		#endregion Lingo Common
	}
}
#endif
