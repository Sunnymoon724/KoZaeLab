#if UNITY_EDITOR
using UnityEditor;
using System;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using KZLib.ToolKits;
using KZLib.Utilities;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using System.IO;
using UnityEditor.Localization;
using System.Collections.Generic;

namespace KZLib.EditorInternal.Menus
{
	public static partial class KZMenuItem
	{
		private const string c_key = "Key";

		/// <summary>
		/// excel file -> asset file
		/// </summary>
		[MenuItem("KZMenu/Lingo/Generate Lingo",false,MenuOrder.Data.GENERATE)]
		private static void _OnGenerateLingo()
		{
			AddressableAssetSettingsDefaultObject.GetSettings(true);

			var localizationSettings = LocalizationSettings.GetInstanceDontCreateDefault();

			if(localizationSettings == null)
			{
				localizationSettings = ScriptableObject.CreateInstance<LocalizationSettings>();

				KZAssetKit.CreateAsset(Path.Combine("Localization","LocalizationSettings.asset"),localizationSettings,true);

				EditorBuildSettings.AddConfigObject("com.unity.localization.settings",localizationSettings,true);
			}

			var lingoRoute = RouteManager.In.GetOrCreateRoute("defaultRes:lingo");

			foreach(var lingoFilePath in KZFileKit.FindAllExcelFileGroupByFolderPath(Global.LINGO_FOLDER_PATH))
			{
				if(!KZFileKit.IsExcelFile(lingoFilePath))
				{
					LogChannel.System.W($"{lingoFilePath} is not exist. -> generate failed");

					continue;
				}

				try
				{
					LingoGenerator.TryConvertToDictionary<SystemLanguage>(lingoFilePath,out var languageHashSet,out var lingoDict);

					var identifierHashSet = new HashSet<LocaleIdentifier>();

					foreach(var language in languageHashSet)
					{
						var assetPath = Path.Combine($"{lingoRoute.AssetPath}",$"{language}.asset");
						var locale = Locale.CreateLocale(language);

						identifierHashSet.Add(locale.Identifier);

						KZAssetKit.CreateAsset(assetPath,locale,false);
					}

					var tableNameHashSet = new HashSet<string>();
					var fileName = KZFileKit.GetOnlyName(lingoFilePath);

					switch(fileName)
					{
						case "String":
							{
								foreach(var pair in lingoDict)
								{
									var sheetName = pair.Key;

									var collection = _GetOrCreateLocalizationTableCollection(sheetName,fileName,LocalizationEditorSettings.GetStringTableCollection,LocalizationEditorSettings.CreateStringTableCollection);

									_SyncStringTableCollection(collection,identifierHashSet);

									_ApplyStringToTableCollection(collection,sheetName,pair.Value);

									tableNameHashSet.Add(collection.name);
								}

								break;
							}
						case "Asset":
							{
								foreach(var pair in lingoDict)
								{
									var sheetName = pair.Key;

									var collection = _GetOrCreateLocalizationTableCollection(sheetName,fileName,LocalizationEditorSettings.GetAssetTableCollection,LocalizationEditorSettings.CreateAssetTableCollection);

									_SyncAssetTableCollection(collection,identifierHashSet);

									_ApplyAssetTableCollection(collection,sheetName,pair.Value);

									tableNameHashSet.Add(collection.name);
								}

								break;
							}
						default:
							{
								LogChannel.System.W($"Excel file name must be String or Asset.");

								break;
							}
					}

					var removeList = new List<string>();
					var stringTableList = LocalizationEditorSettings.GetStringTableCollections();

					for(var i=0;i<stringTableList.Count;i++)
					{
						var stringTable = stringTableList[i];

						if(!tableNameHashSet.Contains(stringTable.name))
						{
							removeList.Add(stringTable.name);
						}
					}

					var assetTableList = LocalizationEditorSettings.GetAssetTableCollections();

					for(var i=0;i<assetTableList.Count;i++)
					{
						var assetTable = assetTableList[i];

						if(!tableNameHashSet.Contains(assetTable.name))
						{
							removeList.Add(assetTable.name);
						}
					}

					for(var i=0;i<removeList.Count;i++)
					{
						var sheetName = removeList[i].Replace("Table","");
						var folderPath = Path.Combine("Assets","Localization",fileName,sheetName);

						KZFileKit.DeleteFolder(KZFileKit.GetAbsolutePath(folderPath,true),true);
					}
				}
				catch(Exception exception)
				{
					LogChannel.System.E(exception);

					return;
				}
			}

			_DisplayGenerateEnd();
		}

		private static void _ApplyStringToTableCollection(StringTableCollection collection,string sheetName,Dictionary<string,string[]> lingoDict)
		{
			var keyHashSet = new HashSet<string>();
			var schemeArray = lingoDict.TryGetValue(c_key,out var textArray) ? textArray : null;

			if(schemeArray == null)
			{
				LogChannel.System.E($"{c_key} is not found in {sheetName}");

				return;
			}

			foreach(var pair in lingoDict)
			{
				var key = pair.Key;

				if(key.IsEqual(c_key))
				{
					continue;
				}

				key = $"{sheetName.ToLower()}_{key}";
				
				var stringTableList = collection.StringTables;

				for(var i=0;i<stringTableList.Count;i++)
				{
					var stringTable = stringTableList[i];
					var language = stringTable.LocaleIdentifier.CultureInfo.EnglishName;
					var text = _GetValueByLanguage(language,schemeArray,pair.Value);

					var tableEntry = stringTable.GetEntry(key);

					if(tableEntry == null)
					{
						tableEntry = stringTable.AddEntry(key,text);
					}
					else
					{
						tableEntry.Value = text;
					}

					tableEntry.IsSmart = text.Contains("{0}");

					EditorUtility.SetDirty(stringTable);
				}

				keyHashSet.Add(pair.Key);
			}

			_CheckUnusedKeys(collection.SharedData,keyHashSet);

			KZAssetKit.SaveAsset();
		}

		private static void _ApplyAssetTableCollection(AssetTableCollection collection,string sheetName,Dictionary<string,string[]> lingoDict)
		{
			var keyHashSet = new HashSet<string>();
			var schemeArray = lingoDict.TryGetValue(c_key,out var textArray) ? textArray : null;

			if(schemeArray == null)
			{
				LogChannel.System.E($"{c_key} is not found in {sheetName}");

				return;
			}

			foreach(var pair in lingoDict)
			{
				var key = pair.Key;

				if(key.IsEqual(c_key))
				{
					continue;
				}

				var assetTableList = collection.AssetTables;

				for(var i=0;i<assetTableList.Count;i++)
				{
					var assetTable = assetTableList[i];
					var language = assetTable.LocaleIdentifier.CultureInfo.EnglishName;
					var path = _GetValueByLanguage(language,schemeArray,pair.Value);

					var assetPath = RouteManager.In.GetOrCreateRoute(path).AssetPath;
					var guid = _CreateAddressableGuid(assetPath,language,assetTable.LocaleIdentifier.Code);

					if(guid.IsEmpty())
					{
						continue;
					}

					var tableEntry = assetTable.GetEntry(key);

					if(tableEntry == null)
					{
						assetTable.AddEntry(key,guid);
					}
					else
					{
						tableEntry.Guid = guid;
					}

					EditorUtility.SetDirty(assetTable);
				}

				keyHashSet.Add(pair.Key);
			}

			_CheckUnusedKeys(collection.SharedData,keyHashSet);

			KZAssetKit.SaveAsset();
		}

		private static TCollection _GetOrCreateLocalizationTableCollection<TCollection>(string sheetName,string fileName,Func<TableReference,TCollection> onGetCollection,Func<string,string,TCollection> onCreateCollection)
		{
			var tableName = $"{sheetName}Table";
			var collection = onGetCollection(tableName);

			if(collection == null)
			{
				var folderPath = Path.Combine("Assets","Localization",fileName,sheetName);

				KZFileKit.CreateFolder(KZFileKit.GetAbsolutePath(folderPath,true));

				collection = onCreateCollection(tableName,folderPath);

				KZAssetKit.SaveAsset();
			}

			return collection;
		}

		private static void _SyncStringTableCollection(StringTableCollection collection,HashSet<LocaleIdentifier> identifierHashSet)
		{
			var existingIdentifierHashSet = new HashSet<LocaleIdentifier>();
			var removeList = new List<StringTable>();

			var stringTableList = collection.StringTables;

			for(var i=0;i<stringTableList.Count;i++)
			{
				var stringTable = stringTableList[i];

				existingIdentifierHashSet.Add(stringTable.LocaleIdentifier);

				if(!identifierHashSet.Contains(stringTable.LocaleIdentifier))
				{
					removeList.Add(stringTable);
				}
			}

			for(var i=0;i<removeList.Count;i++)
			{
				collection.RemoveTable(removeList[i]);
			}

			foreach(var identifier in identifierHashSet)
			{
				if(!existingIdentifierHashSet.Contains(identifier))
				{
					collection.AddNewTable(identifier);
				}
			}
		}

		private static void _SyncAssetTableCollection(AssetTableCollection collection,HashSet<LocaleIdentifier> identifierHashSet)
		{
			var existingIdentifierHashSet = new HashSet<LocaleIdentifier>();
			var removeList = new List<AssetTable>();

			var assetTableList = collection.AssetTables;

			for(var i=0;i<assetTableList.Count;i++)
			{
				var assetTable = assetTableList[i];

				existingIdentifierHashSet.Add(assetTable.LocaleIdentifier);

				if(!identifierHashSet.Contains(assetTable.LocaleIdentifier))
				{
					removeList.Add(assetTable);
				}
			}

			for(var i=0;i<removeList.Count;i++)
			{
				collection.RemoveTable(removeList[i]);
			}

			foreach(var identifier in identifierHashSet)
			{
				if(!existingIdentifierHashSet.Contains(identifier))
				{
					collection.AddNewTable(identifier);
				}
			}
		}

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

		private static string _GetValueByLanguage(string language,string[] schemeArray,string[] cellArray)
		{
			for(var i=0;i<schemeArray.Length;i++)
			{
				var scheme = schemeArray[i];

				if(scheme.Equals(language))
				{
					return cellArray[i];
				}
			}

			return string.Empty;
		}

		private static AddressableAssetGroup _GetOrCreateAddressableGroup(string language)
		{
			var tableName = $"Localization-Asset-Tables-{language}";
			var assetName = $"Localization-Assets-{language}";

			var assetGroup = CommonUtility.GetAddressableGroup(assetName);

			if(assetGroup == null)
			{
				assetGroup = CommonUtility.CopyAddressableGroup(tableName,assetName);

				var namingSchema = assetGroup.GetSchema<BundledAssetGroupSchema>();

				if (namingSchema != null)
				{
					namingSchema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.NoHash;
				}
			}

			return assetGroup;
		}

		private static string _CreateAddressableGuid(string assetPath,string language,string code)
		{
			var assetGroup = _GetOrCreateAddressableGroup(language);
			var guid = AssetDatabase.AssetPathToGUID(assetPath);

			if(!CommonUtility.TryRegisterAddressable(assetPath,KZFileKit.GetOnlyName(assetPath),assetGroup,true,out var addressableAsset))
			{
				return null;
			}

			var label = $"Locale-{code}";

			if(!addressableAsset.labels.Contains(label))
			{
				addressableAsset.SetLabel(label,true);
			}

			return guid;
		}

		[MenuItem("KZMenu/Lingo/Open Lingo Folder",false,MenuOrder.Data.OPEN)]
		private static void _OnOpenLingoFolder()
		{
			_OpenFolder("Lingo",Global.LINGO_FOLDER_PATH);
		}
	}
}
#endif