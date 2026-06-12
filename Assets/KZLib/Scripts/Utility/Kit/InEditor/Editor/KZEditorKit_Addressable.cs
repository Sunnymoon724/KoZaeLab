#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

/// <summary>
/// Partial editor utility for Addressable asset registration, lookup, and group management.
/// </summary>
public static partial class KZEditorKit
{
	/// <summary>
	/// Attempts to find the Addressable entry for the given asset object.
	/// </summary>
	public static bool TryGetAddressableAsset(Object asset,out AddressableAssetEntry assetEntry)
	{
		var assetPath = AssetDatabase.GetAssetPath(asset);

		return TryGetAddressableAsset(assetPath,out assetEntry);
	}

	/// <summary>
	/// Attempts to find the Addressable entry for the asset at the given project path.
	/// </summary>
	public static bool TryGetAddressableAsset(string assetPath,out AddressableAssetEntry assetEntry)
	{
		if(assetPath.IsEmpty())
		{
			LogChannel.Kit.E($"{assetPath} is null or empty.");

			assetEntry = null;

			return false;
		}

		if(!_TryGetAddressableSettings(out var settings))
		{
			assetEntry = null;

			return false;
		}

		assetEntry = settings.FindAssetEntry(AssetDatabase.AssetPathToGUID(assetPath));

		return assetEntry != null;
	}

	/// <summary>
	/// Registers the given asset as Addressable when it is not already registered, optionally setting its address.
	/// </summary>
	public static bool TryRegisterAddressable(Object asset,string addressName,AddressableAssetGroup group,bool readOnly,out AddressableAssetEntry assetEntry)
	{
		var assetPath = AssetDatabase.GetAssetPath(asset);

		return TryRegisterAddressable(assetPath,addressName,group,readOnly,out assetEntry);
	}

	/// <summary>
	/// Registers the asset at the given path as Addressable when it is not already registered, optionally setting its address.
	/// </summary>
	public static bool TryRegisterAddressable(string assetPath,string addressName,AddressableAssetGroup group,bool readOnly,out AddressableAssetEntry assetEntry)
	{
		if(!TryGetAddressableAsset(assetPath,out assetEntry))
		{
			if(assetPath.IsEmpty())
			{
				LogChannel.Kit.E($"{assetPath} is null or empty.");

				return false;
			}

			if(!_TryGetAddressableSettings(out var settings))
			{
				return false;
			}

			var guid = AssetDatabase.AssetPathToGUID(assetPath);

			if(guid.IsEmpty())
			{
				LogChannel.Kit.E($"{assetPath} is not exist.");

				return false;
			}

			assetEntry = settings.CreateOrMoveEntry(guid,group,readOnly);

			if(!addressName.IsEmpty())
			{
				assetEntry.address = addressName;
			}

			KZAssetKit.SaveAsset();
		}

		return true;
	}

	/// <summary>
	/// Moves an Addressable asset to the group with the given name.
	/// </summary>
	public static bool TryChangeAddressableGroup(Object asset,string newGroupName)
	{
		var assetPath = AssetDatabase.GetAssetPath(asset);

		return TryChangeAddressableGroup(assetPath,newGroupName);
	}

	/// <summary>
	/// Moves the Addressable asset at the given path to the group with the given name.
	/// </summary>
	public static bool TryChangeAddressableGroup(string assetPath,string newGroupName)
	{
		if(!_TryGetAddressableSettings(out var settings))
		{
			return false;
		}

		var guid = AssetDatabase.AssetPathToGUID(assetPath);

		if(guid.IsEmpty())
		{
			LogChannel.Kit.E($"{assetPath} is not exist.");

			return false;
		}

		var assetEntry = settings.FindAssetEntry(guid);

		if(assetEntry == null)
		{
			LogChannel.Kit.E($"Asset at {assetPath} is not addressable.");

			return false;
		}

		if(assetEntry.parentGroup.Name != newGroupName)
		{
			var newGroup = settings.FindGroup(newGroupName);

			if(newGroup == null)
			{
				LogChannel.Kit.E($"{newGroupName} is not exist.");

				return false;
			}

			assetEntry.parentGroup = newGroup;

			KZAssetKit.SaveAsset();

			settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved,assetEntry,true);
		}

		return true;
	}

	/// <summary>
	/// Finds an Addressable group by name, falling back to the default group when the name is empty or not found.
	/// </summary>
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

	/// <summary>
	/// Creates a new Addressable group with the given schemas.
	/// </summary>
	public static AddressableAssetGroup CreateAddressableGroup(string groupName,List<AddressableAssetGroupSchema> schemaList,bool readOnly = false)
	{
		if(!_TryGetAddressableSettings(out var settings))
		{
			return null;
		}

		var newGroup = settings.CreateGroup(groupName,false,readOnly,false,schemaList);

		if(newGroup != null)
		{
			KZAssetKit.SaveAsset();
		}

		return newGroup;
	}

	/// <summary>
	/// Returns the Addressable group with the given name, or null when settings or the group are unavailable.
	/// </summary>
	public static AddressableAssetGroup GetAddressableGroup(string groupName)
	{
		if(!_TryGetAddressableSettings(out var settings))
		{
			return null;
		}

		return settings.FindGroup(groupName);
	}

	/// <summary>
	/// Creates a new Addressable group by copying schemas and read-only state from an existing group.
	/// </summary>
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
			KZAssetKit.SaveAsset();
		}

		return newGroup;
	}

	private static bool _TryGetAddressableSettings(out AddressableAssetSettings settings)
	{
		settings = AddressableAssetSettingsDefaultObject.Settings;

		if(settings == null)
		{
			LogChannel.Kit.E("AddressableAssetSettings is null. create first.");
		}

		return settings;
	}
}
#endif
