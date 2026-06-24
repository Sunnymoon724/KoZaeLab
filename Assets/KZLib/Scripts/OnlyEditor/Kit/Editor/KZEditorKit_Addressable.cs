#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

using Object = UnityEngine.Object;

/// <summary>
/// Editor-only utility methods for Addressable asset lookup, registration, and group management.
/// </summary>
public static partial class KZEditorKit
{
	/// <summary>
	/// Tries to find an Addressable entry for the given asset object.
	/// </summary>
	public static bool TryGetAddressableAsset(Object asset,out AddressableAssetEntry assetEntry)
	{
		var assetPath = AssetDatabase.GetAssetPath(asset);

		return TryGetAddressableAsset(assetPath,out assetEntry);
	}

	/// <summary>
	/// Tries to find an Addressable entry for the given asset path.
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
	/// Registers the given asset as Addressable, or updates its group and address when already registered.
	/// </summary>
	public static bool TryRegisterAddressable(Object asset,string addressName,AddressableAssetGroup group,bool readOnly,out AddressableAssetEntry assetEntry)
	{
		var assetPath = AssetDatabase.GetAssetPath(asset);

		return TryRegisterAddressable(assetPath,addressName,group,readOnly,out assetEntry);
	}

	/// <summary>
	/// Creates or moves an Addressable entry into the given group and optionally sets its address.
	/// </summary>
	public static bool TryRegisterAddressable(string assetPath,string addressName,AddressableAssetGroup group,bool readOnly,out AddressableAssetEntry assetEntry)
	{
		if(assetPath.IsEmpty())
		{
			LogChannel.Kit.E($"{assetPath} is null or empty.");

			assetEntry = null;

			return false;
		}

		if(group == null)
		{
			LogChannel.Kit.E("Addressable group is null.");

			assetEntry = null;

			return false;
		}

		if(!_TryGetAddressableSettings(out var settings))
		{
			assetEntry = null;

			return false;
		}

		var guid = AssetDatabase.AssetPathToGUID(assetPath);

		if(guid.IsEmpty())
		{
			LogChannel.Kit.E($"{assetPath} does not exist.");

			assetEntry = null;

			return false;
		}

		var isDirty = false;

		if(TryGetAddressableAsset(assetPath,out assetEntry))
		{
			if(assetEntry.parentGroup != group)
			{
				assetEntry = settings.CreateOrMoveEntry(guid,group,readOnly);

				isDirty = true;
			}

			if(!addressName.IsEmpty() && assetEntry.address != addressName)
			{
				assetEntry.address = addressName;

				isDirty = true;
			}
		}
		else
		{
			assetEntry = settings.CreateOrMoveEntry(guid,group,readOnly);

			if(!addressName.IsEmpty())
			{
				assetEntry.address = addressName;
			}

			isDirty = true;
		}

		if(isDirty)
		{
			KZAssetKit.SaveAsset();
		}

		return true;
	}

	/// <summary>
	/// Moves an already-registered Addressable asset to another group.
	/// </summary>
	public static bool TryChangeAddressableGroup(Object asset,string newGroupName)
	{
		var assetPath = AssetDatabase.GetAssetPath(asset);

		return TryChangeAddressableGroup(assetPath,newGroupName);
	}

	/// <summary>
	/// Moves an already-registered Addressable asset to another group by asset path.
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
			LogChannel.Kit.E($"{assetPath} does not exist.");

			return false;
		}

		var assetEntry = settings.FindAssetEntry(guid);

		if(assetEntry == null)
		{
			LogChannel.Kit.E($"Asset at {assetPath} is not addressable.");

			return false;
		}

		if(assetEntry.parentGroup.Name == newGroupName)
		{
			return true;
		}

		var newGroup = settings.FindGroup(newGroupName);

		if(newGroup == null)
		{
			LogChannel.Kit.E($"{newGroupName} does not exist.");

			return false;
		}

		assetEntry = settings.CreateOrMoveEntry(guid,newGroup,assetEntry.ReadOnly);

		KZAssetKit.SaveAsset();

		settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved,assetEntry,true);

		return true;
	}

	/// <summary>
	/// Returns an Addressable group by name, or the default group when the name is empty or not found.
	/// </summary>
	public static AddressableAssetGroup GetAddressableGroupOrDefault(string groupName)
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
	/// Creates a new Addressable group with the supplied schema list.
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
	/// Returns an Addressable group by exact name, or null when it does not exist.
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
	/// Duplicates group settings under a new name. Optionally moves entries from the source group into the new group.
	/// </summary>
	public static AddressableAssetGroup CopyAddressableGroup(string sourceName,string newGroupName,bool copyEntries = false)
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

		if(newGroup == null)
		{
			return null;
		}

		if(copyEntries)
		{
			var entryArray = new List<AddressableAssetEntry>(sourceGroup.entries);

			for(var i=0;i<entryArray.Count;i++)
			{
				var sourceEntry = entryArray[i];
				var movedEntry = settings.CreateOrMoveEntry(sourceEntry.guid,newGroup,sourceEntry.ReadOnly);

				if(!sourceEntry.address.IsEmpty())
				{
					movedEntry.address = sourceEntry.address;
				}
			}

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

		return settings != null;
	}
}
#endif