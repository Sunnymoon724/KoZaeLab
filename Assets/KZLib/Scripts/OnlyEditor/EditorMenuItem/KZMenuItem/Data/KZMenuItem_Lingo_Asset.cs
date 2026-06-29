#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.Localization;
using UnityEditor.Localization;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using System.Collections.Generic;
using KZLib.Utilities;
using UnityEngine;
using UnityEngine.Localization.Tables;

namespace KZLib.EditorInternal.Menus
{
	public static partial class KZMenuItem
	{
		#region Asset Lingo
		private static AssetTableCollection _GetOrCreateAssetTableCollection(string sheetName)
		{
			var tableName = $"{sheetName}{c_tableNameSuffix}";
			var collection = LocalizationEditorSettings.GetAssetTableCollection(tableName);

			if(collection != null)
			{
				return collection;
			}

			var folderPath = System.IO.Path.Combine("Assets","Localization",c_assetExcelName,sheetName);

			KZFileKit.CreateFolder(KZFileKit.GetAbsolutePath(folderPath,true));

			collection = LocalizationEditorSettings.CreateAssetTableCollection(tableName,folderPath);

			KZAssetKit.SaveAsset();

			return collection;
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

		private static bool _ApplyAssetTableCollection(AssetTableCollection collection,string sheetName,Dictionary<string,string[]> lingoDict,Dictionary<LocaleIdentifier,SystemLanguage> languageByIdentifier)
		{
			var keyHashSet = new HashSet<string>();
			var schemeArray = lingoDict.TryGetValue(c_key,out var textArray) ? textArray : null;

			if(schemeArray == null)
			{
				LogChannel.Editor.E($"{c_key} is not found in {sheetName}");

				return false;
			}

			foreach(var pair in lingoDict)
			{
				var key = pair.Key;

				if(string.Equals(key,c_key))
				{
					continue;
				}

				var assetTableList = collection.AssetTables;

				for(var i=0;i<assetTableList.Count;i++)
				{
					var assetTable = assetTableList[i];

					if(!languageByIdentifier.TryGetValue(assetTable.LocaleIdentifier,out var language))
					{
						continue;
					}

					var path = _GetValueByLanguage(language,schemeArray,pair.Value);

					var assetPath = RouteManager.In.Fetch(path).AssetPath;
					var guid = _CreateAddressableGuid(assetPath,language.ToString(),assetTable.LocaleIdentifier.Code);

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

			return true;
		}

		private static AddressableAssetGroup _GetOrCreateAddressableGroup(string language)
		{
			var tableName = $"Localization-Asset-Tables-{language}";
			var assetName = $"Localization-Assets-{language}";

			var assetGroup = KZEditorKit.GetAddressableGroup(assetName);

			if(assetGroup == null)
			{
				assetGroup = KZEditorKit.CopyAddressableGroup(tableName,assetName);

				var namingSchema = assetGroup.GetSchema<BundledAssetGroupSchema>();

				if(namingSchema != null)
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

			if(!KZEditorKit.TryRegisterAddressable(assetPath,KZFileKit.GetOnlyFileName(assetPath),assetGroup,true,out var addressableAsset))
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
		#endregion Asset Lingo
	}
}
#endif
