#if UNITY_EDITOR
using UnityEditor;

#if KZLIB_LINGO

using System;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using KZLib.KZTool;
using KZLib.KZUtility;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using System.IO;
using UnityEditor.Localization;
using System.Collections.Generic;

#endif

namespace KZLib.KZMenu
{
	public partial class KZMenuItem
	{
		/// <summary>
		/// excel file -> asset file
		/// </summary>
		[MenuItem("KZMenu/Lingo/Generate Lingo",false,MenuOrder.Data.GENERATE)]
		private static void _OnGenerateLingo()
		{
#if KZLIB_LINGO
			var localizationSettings = LocalizationSettings.GetInstanceDontCreateDefault();

			if(localizationSettings == null)
			{
				localizationSettings = ScriptableObject.CreateInstance<LocalizationSettings>();

				CommonUtility.SaveAsset(Path.Combine("Localization","LocalizationSettings.asset"),localizationSettings,true);

				EditorBuildSettings.AddConfigObject("com.unity.localization.settings",localizationSettings,true);
			}

			var lingoRoute = RouteMgr.In.GetOrCreateRoute("defaultRes:lingo");

			foreach(var lingoFilePath in FileUtility.FindAllExcelFileGroupByFolderPath(Global.LINGO_FOLDER_PATH))
			{
				if(!FileUtility.IsExcelFile(lingoFilePath))
				{
					LogTag.System.W($"{lingoFilePath} is not exist. -> generate failed");

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

						CommonUtility.SaveAsset(assetPath,locale,false);
					}

					var tableNameHashSet = new HashSet<string>();
					var fileName = FileUtility.GetOnlyName(lingoFilePath);

					switch(fileName)
					{
						case "String":
							{
								foreach(var pair in lingoDict)
								{
									var collection = _GetOrCreateLocalizationTableCollection(pair.Key,fileName,LocalizationEditorSettings.GetStringTableCollection,LocalizationEditorSettings.CreateStringTableCollection);

									_SyncStringTableCollection(collection,identifierHashSet);

									_ApplyStringToTableCollection(collection,pair.Value);

									tableNameHashSet.Add(collection.name);
								}

								break;
							}
						case "Asset":
							{
								foreach(var pair in lingoDict)
								{
									var collection = _GetOrCreateLocalizationTableCollection(pair.Key,fileName,LocalizationEditorSettings.GetAssetTableCollection,LocalizationEditorSettings.CreateAssetTableCollection);

									_SyncAssetTableCollection(collection,identifierHashSet);

									_ApplyAssetTableCollection(collection,pair.Value);

									tableNameHashSet.Add(collection.name);
								}

								break;
							}
						default:
							{
								LogTag.System.W($"Excel file name must be String or Asset.");

								break;
							}
					}

					var removeList = new List<string>();

					foreach(var stringTable in LocalizationEditorSettings.GetStringTableCollections())
					{
						if(!tableNameHashSet.Contains(stringTable.name))
						{
							removeList.Add(stringTable.name);
						}
					}

					foreach(var assetTable in LocalizationEditorSettings.GetAssetTableCollections())
					{
						if(!tableNameHashSet.Contains(assetTable.name))
						{
							removeList.Add(assetTable.name);
						}
					}

					foreach(var remove in removeList)
					{
						var sheetName = remove.Replace("Table","");
						var folderPath = Path.Combine("Assets","Localization",fileName,sheetName);

						FileUtility.DeleteFolder(FileUtility.GetAbsolutePath(folderPath,true),true);
					}
				}
				catch(Exception exception)
				{
					LogTag.System.E(exception);

					return;
				}
			}

			_DisplayGenerateEnd();
#endif
		}

#if KZLIB_LINGO
		private static void _ApplyStringToTableCollection(StringTableCollection collection,Dictionary<string,string[]> lingoDict)
		{
			var keyHashSet = new HashSet<string>();
			var schemeArray = lingoDict["Key"];

			foreach(var pair in lingoDict)
			{
				var key = pair.Key;

				if(key.IsEqual("Key"))
				{
					continue;
				}

				foreach(var table in collection.StringTables)
				{
					var language = table.LocaleIdentifier.CultureInfo.EnglishName;
					var text = GetValueByLanguage(language,schemeArray,pair.Value);

					var tableEntry = table.GetEntry(key);

					if(tableEntry == null)
					{
						tableEntry = table.AddEntry(key,text);
					}
					else
					{
						tableEntry.Value = text;
					}

					tableEntry.IsSmart = text.Contains("{0}");

					EditorUtility.SetDirty(table);
				}

				keyHashSet.Add(pair.Key);
			}

			CheckUnusedKeys(collection.SharedData,keyHashSet);

			AssetDatabase.SaveAssets();
		}

		private static void _ApplyAssetTableCollection(AssetTableCollection collection,Dictionary<string,string[]> lingoDict)
		{
			var keyHashSet = new HashSet<string>();
			var schemeArray = lingoDict["Key"];

			AddressableAssetSettingsDefaultObject.GetSettings(true);

			foreach(var pair in lingoDict)
			{
				var key = pair.Key;

				if(key.IsEqual("Key"))
				{
					continue;
				}

				foreach(var table in collection.AssetTables)
				{
					var language = table.LocaleIdentifier.CultureInfo.EnglishName;
					var path = GetValueByLanguage(language,schemeArray,pair.Value);

					var assetPath = RouteMgr.In.GetOrCreateRoute(path).AssetPath;
					var guid = _CreateAddressableGuid(assetPath,language,table.LocaleIdentifier.Code);

					if(guid.IsEmpty())
					{
						continue;
					}

					var tableEntry = table.GetEntry(key);

					if(tableEntry == null)
					{
						table.AddEntry(key,guid);
					}
					else
					{
						tableEntry.Guid = guid;
					}

					EditorUtility.SetDirty(table);
				}

				keyHashSet.Add(pair.Key);
			}

			CheckUnusedKeys(collection.SharedData,keyHashSet);

			AssetDatabase.SaveAssets();
		}

		private static TCollection _GetOrCreateLocalizationTableCollection<TCollection>(string sheetName,string fileName,Func<TableReference,TCollection> onGetCollection,Func<string,string,TCollection> onCreateCollection)
		{
			var tableName = $"{sheetName}Table";
			var collection = onGetCollection(tableName);

			if(collection == null)
			{
				var folderPath = Path.Combine("Assets","Localization",fileName,sheetName);

				FileUtility.CreateFolder(FileUtility.GetAbsolutePath(folderPath,true));

				collection = onCreateCollection(tableName,folderPath);

				AssetDatabase.SaveAssets();
			}

			return collection;
		}

		private static void _SyncStringTableCollection(StringTableCollection collection,HashSet<LocaleIdentifier> identifierHashSet)
		{
			var existingIdentifierHashSet = new HashSet<LocaleIdentifier>();

			foreach(var table in collection.StringTables)
			{
				existingIdentifierHashSet.Add(table.LocaleIdentifier);
			}

			var removeList = new List<StringTable>();

			foreach(var table in collection.StringTables)
			{
				if(!identifierHashSet.Contains(table.LocaleIdentifier))
				{
					removeList.Add(table);
				}
			}

			foreach(var remove in removeList)
			{
				collection.RemoveTable(remove);
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

			foreach(var table in collection.AssetTables)
			{
				existingIdentifierHashSet.Add(table.LocaleIdentifier);
			}

			var removeList = new List<AssetTable>();

			foreach(var table in collection.AssetTables)
			{
				if(!identifierHashSet.Contains(table.LocaleIdentifier))
				{
					removeList.Add(table);
				}
			}

			foreach(var remove in removeList)
			{
				collection.RemoveTable(remove);
			}

			foreach(var identifier in identifierHashSet)
			{
				if(!existingIdentifierHashSet.Contains(identifier))
				{
					collection.AddNewTable(identifier);
				}
			}
		}

		private static void CheckUnusedKeys(SharedTableData tableData,HashSet<string> usedKeyHashSet)
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

		private static string GetValueByLanguage(string language,string[] schemeArray,string[] cellArray)
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

			if(!CommonUtility.TryRegisterAddressable(assetPath,FileUtility.GetOnlyName(assetPath),assetGroup,true,out var addressableAsset))
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
#endif

		[MenuItem("KZMenu/Lingo/Generate Lingo",true,MenuOrder.Data.GENERATE)]
		private static bool _IsValidLingo()
		{
#if KZLIB_LINGO
			return true;
#else
			return false;
#endif
		}

		[MenuItem("KZMenu/Lingo/Open Lingo Folder",false,MenuOrder.Data.OPEN)]
		private static void _OnOpenLingoFolder()
		{
			_OpenFolder("Lingo",Global.LINGO_FOLDER_PATH);
		}
	}
}
#endif