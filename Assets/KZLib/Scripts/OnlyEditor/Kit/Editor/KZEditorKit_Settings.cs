#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditorInternal;

/// <summary>
/// Editor-only utility methods for project tags, layers, define symbols, and package identifiers.
/// </summary>
public static partial class KZEditorKit
{
	#region Tag & Layer
	/// <summary>
	/// Adds a tag when it is not already registered in the project.
	/// </summary>
	public static void AddTag(string newTag)
	{
		foreach(var tag in InternalEditorUtility.tags)
		{
			if(string.Equals(tag,newTag))
			{
				return;
			}
		}

		InternalEditorUtility.AddTag(newTag);
	}

	private static SerializedObject _FindTagManagerObject()
	{
		var assetArray = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");

		if(assetArray.IsNullOrEmpty())
		{
			return null;
		}

		return new SerializedObject(assetArray[0]);
	}

	/// <summary>
	/// Assigns a user layer name to the first empty slot. Skips when the name already exists.
	/// </summary>
	public static void AddLayer(string layerName)
	{
		var serialized = _FindTagManagerObject();

		if(serialized == null)
		{
			return;
		}

		var layerProperty = serialized.FindProperty("layers");
		var layerCount = layerProperty.arraySize;
		var index = Global.InvalidIndex;
		
		for(var i=8;i<layerCount;i++)
		{
			var value = layerProperty.GetArrayElementAtIndex(i).stringValue;

			if(index == Global.InvalidIndex && value.IsEmpty())
			{
				index = i;
			}			
			else if(string.Equals(value,layerName))
			{
				index = Global.InvalidIndex;

				break;
			}
		}
		
		if(index != Global.InvalidIndex)
		{
			var property = layerProperty.GetArrayElementAtIndex(index);
			property.stringValue = layerName;
			serialized.ApplyModifiedProperties();
		}
	}
	#endregion Tag & Layer

	#region Player Settings
	/// <summary>
	/// Appends a scripting define symbol to the given build target when it is not already present.
	/// </summary>
	public static void AddDefineSymbol(string defineSymbol,NamedBuildTarget buildTargetGroup)
	{
		var symbolText = PlayerSettings.GetScriptingDefineSymbols(buildTargetGroup);
		var symbolHashSet = _SplitSymbols(symbolText);

		if(symbolHashSet.Contains(defineSymbol))
		{
			return;
		}

		symbolHashSet.Add(defineSymbol);

		PlayerSettings.SetScriptingDefineSymbols(buildTargetGroup,string.Join(";",symbolHashSet));

		LogChannel.Kit.I($"Add Define Symbol : {defineSymbol}");
	}

	/// <summary>
	/// Appends a scripting define symbol to Standalone, Android, and iOS when it is not already present.
	/// </summary>
	public static void AddDefineSymbolForAllBuildTargets(string defineSymbol)
	{
		foreach(var target in BuildTargetGroup)
		{
			AddDefineSymbol(defineSymbol,target);
		}
	}

	/// <summary>
	/// Removes a scripting define symbol from the given build target when it exists.
	/// </summary>
	public static void RemoveDefineSymbol(string defineSymbol,NamedBuildTarget buildTargetGroup)
	{
		var symbolText = PlayerSettings.GetScriptingDefineSymbols(buildTargetGroup);
		var symbolHashSet = _SplitSymbols(symbolText);

		if(!symbolHashSet.Contains(defineSymbol))
		{
			return;
		}

		symbolHashSet.Remove(defineSymbol);

		PlayerSettings.SetScriptingDefineSymbols(buildTargetGroup,string.Join(";",symbolHashSet));
	}

	private static HashSet<string> _SplitSymbols(string symbolText)
	{
		var result = new HashSet<string>();

		if(symbolText.IsEmpty())
		{
			return result;
		}

		var splitArray = symbolText.Split(';');

		for(var i=0;i<splitArray.Length;i++)
		{
			if(!splitArray[i].IsEmpty())
			{
				result.Add(splitArray[i]);
			}
		}

		return result;
	}

	/// <summary>
	/// Replaces old define symbols with new ones across Standalone, Android, and iOS targets.
	/// </summary>
	public static void ChangeDefineSymbol(string[] oldDefineSymbolArray,string[] newDefineSymbolArray)
	{
		foreach(var target in BuildTargetGroup)
		{
			var defineSymbolText = PlayerSettings.GetScriptingDefineSymbols(target);

			if(defineSymbolText.IsEmpty())
			{
				continue;
			}

			var defineSymbolHashSet = _SplitSymbols(defineSymbolText);

			for(var i=0;i<oldDefineSymbolArray.Length;i++)
			{
				defineSymbolHashSet.Remove(oldDefineSymbolArray[i]);
			}

			for(var i=0;i<newDefineSymbolArray.Length;i++)
			{
				defineSymbolHashSet.Add(newDefineSymbolArray[i]);
			}

			PlayerSettings.SetScriptingDefineSymbols(target,string.Join(";",defineSymbolHashSet));
		}
	}

	/// <summary>
	/// Sets the application identifier for all supported build targets.
	/// </summary>
	public static void ChangePackageName(string packageName)
	{
		foreach(var target in BuildTargetGroup)
		{
			PlayerSettings.SetApplicationIdentifier(target,packageName);
		}
	}
	
	private static IEnumerable<NamedBuildTarget> BuildTargetGroup
	{
		get
		{
			return new NamedBuildTarget[] { NamedBuildTarget.Standalone, NamedBuildTarget.Android, NamedBuildTarget.iOS };
		}
	}
	#endregion Player Settings
}
#endif
