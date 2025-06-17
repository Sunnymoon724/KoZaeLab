#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

public static partial class CommonUtility
{
	public static bool TryGetAddressableAsset(Object asset,out AddressableAssetEntry entry)
	{
		var assetPath = AssetDatabase.GetAssetPath(asset);

		return TryGetAddressableAsset(assetPath,out entry);
	}

	public static bool TryGetAddressableAsset(string assetPath,out AddressableAssetEntry entry)
	{
		if(assetPath.IsEmpty())
		{
			LogSvc.System.E($"{assetPath} is null or empty.");

			entry = null;

			return false;
		}

		if(!_TryGetAddressableSettings(out var settings))
		{
			entry = null;

			return false;
		}

		entry = settings.FindAssetEntry(AssetDatabase.AssetPathToGUID(assetPath));

		return entry != null;
	}

	public static bool TryRegisterAddressable(Object asset,string addressName,AddressableAssetGroup group,bool readOnly,out AddressableAssetEntry entry)
	{
		var assetPath = AssetDatabase.GetAssetPath(asset);

		return TryRegisterAddressable(assetPath,addressName,group,readOnly,out entry);
	}

	public static bool TryRegisterAddressable(string assetPath,string addressName,AddressableAssetGroup group,bool readOnly,out AddressableAssetEntry entry)
	{
		if(!TryGetAddressableAsset(assetPath,out entry))
		{
			if(assetPath.IsEmpty())
			{
				LogSvc.System.E($"{assetPath} is null or empty.");

				return false;
			}

			if(!_TryGetAddressableSettings(out var settings))
			{
				return false;
			}

			var guid = AssetDatabase.AssetPathToGUID(assetPath);

			if(guid.IsEmpty())
			{
				LogSvc.System.E($"{assetPath} is not exist.");

				return false;
			}

			entry = settings.CreateOrMoveEntry(guid,group,readOnly);

			if(!addressName.IsEmpty())
			{
				entry.address = addressName;
			}

			AssetDatabase.SaveAssets();
		}

		return true;
	}

	public static bool TryChangeAddressableGroup(Object asset,string newGroupName)
	{
		var assetPath = AssetDatabase.GetAssetPath(asset);

		return TryChangeAddressableGroup(assetPath,newGroupName);
	}

	public static bool TryChangeAddressableGroup(string assetPath,string newGroupName)
	{
		if(!_TryGetAddressableSettings(out var settings))
		{
			return false;
		}

		var guid = AssetDatabase.AssetPathToGUID(assetPath);

		if(guid.IsEmpty())
		{
			LogSvc.System.E($"{assetPath} is not exist.");

			return false;
		}

		var assetEntry = settings.FindAssetEntry(guid);

		if(assetEntry == null)
		{
			LogSvc.System.E($"Asset at {assetPath} is not addressable.");

			return false;
		}

		if(assetEntry.parentGroup.Name != newGroupName)
		{
			var newGroup = settings.FindGroup(newGroupName);

			if(newGroup == null)
			{
				LogSvc.System.E($"{newGroupName} is not exist.");

				return false;
			}

			assetEntry.parentGroup = newGroup;

			AssetDatabase.SaveAssets();

			settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved,assetEntry,true);
		}

		return true;
	}

	public static AddressableAssetGroup FindAddressableGroup(string groupName)
	{
		if(!_TryGetAddressableSettings(out var settings))
		{
			return null;
		}

		if(groupName.IsEmpty())
		{
			return settings.DefaultGroup;
		}

		var newGroup = settings.FindGroup(groupName);

		if(newGroup == null)
		{
			return settings.DefaultGroup;
		}

		return newGroup;
	}

	public static AddressableAssetGroup CreateAddressableGroup(string groupName,List<AddressableAssetGroupSchema> schemaList,bool readOnly = false)
	{
		if(!_TryGetAddressableSettings(out var settings))
		{
			return null;
		}

		var newGroup = settings.CreateGroup(groupName,false,readOnly,false,schemaList);

		if(newGroup != null)
		{
			AssetDatabase.SaveAssets();
		}

		return newGroup;
	}

	public static AddressableAssetGroup GetAddressableGroup(string groupName)
	{
		if(!_TryGetAddressableSettings(out var settings))
		{
			return null;
		}

		return settings.FindGroup(groupName);
	}

	public static AddressableAssetGroup CopyAddressableGroup(string sourceName,string newGroupName)
	{
		if(!_TryGetAddressableSettings(out var settings))
		{
			return null;
		}

		var sourceGroup = settings.FindGroup(sourceName);

		if(sourceGroup == null)
		{
			return null;
		}

		var newGroup = CreateAddressableGroup(newGroupName,sourceGroup.Schemas,sourceGroup.ReadOnly);

		if(newGroup != null)
		{
			AssetDatabase.SaveAssets();
		}

		return newGroup;
	}

	private static bool _TryGetAddressableSettings(out AddressableAssetSettings settings)
	{
		settings = AddressableAssetSettingsDefaultObject.Settings;

		if(settings == null)
		{
			LogSvc.System.E("AddressableAssetSettings is null. create first.");
		}

		return settings;
	}
}
#endif