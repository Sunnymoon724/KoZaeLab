#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEditor.Localization;
using System.Collections.Generic;
using UnityEngine;

namespace KZLib.EditorInternal.Menus
{
	public static partial class KZMenuItem
	{
		#region String Lingo
		private static StringTableCollection _GetOrCreateStringTableCollection(string sheetName)
		{
			var tableName = $"{sheetName}{c_tableNameSuffix}";
			var collection = LocalizationEditorSettings.GetStringTableCollection(tableName);

			if(collection != null)
			{
				return collection;
			}

			var folderPath = System.IO.Path.Combine("Assets","Localization",c_stringExcelName,sheetName);

			KZFileKit.CreateFolder(KZFileKit.GetAbsolutePath(folderPath,true));

			collection = LocalizationEditorSettings.CreateStringTableCollection(tableName,folderPath);

			KZAssetKit.SaveAsset();

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

		private static bool _ApplyStringToTableCollection(StringTableCollection collection,string sheetName,Dictionary<string,string[]> lingoDict,Dictionary<LocaleIdentifier,SystemLanguage> languageByIdentifier)
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

				if(key.IsEqual(c_key))
				{
					continue;
				}

				key = $"{sheetName.ToLower()}_{key}";

				var stringTableList = collection.StringTables;

				for(var i=0;i<stringTableList.Count;i++)
				{
					var stringTable = stringTableList[i];

					if(!languageByIdentifier.TryGetValue(stringTable.LocaleIdentifier,out var language))
					{
						continue;
					}

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

				keyHashSet.Add(key);
			}

			_CheckUnusedKeys(collection.SharedData,keyHashSet);

			KZAssetKit.SaveAsset();

			return true;
		}
		#endregion String Lingo
	}
}
#endif
