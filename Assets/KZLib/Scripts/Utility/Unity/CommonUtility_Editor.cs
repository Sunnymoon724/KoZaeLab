#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using KZLib.Utilities;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;

using Object = UnityEngine.Object;

public static partial class CommonUtility
{
	#region Tag & Layer
	public static void AddTag(string newTag)
	{
		foreach(var tag in InternalEditorUtility.tags)
		{
			if(tag.IsEqual(newTag))
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

	public static void AddLayer(string layerName)
	{
		var serialized = _FindTagManagerObject();

		if(serialized == null)
		{
			return;
		}

		var layerProperty = serialized.FindProperty("layers");
		var layerCount = layerProperty.arraySize;
		var index = Global.INVALID_INDEX;
		
		for(var i=8;i<layerCount;i++)
		{
			var value = layerProperty.GetArrayElementAtIndex(i).stringValue;

			if(index == Global.INVALID_INDEX && value.IsEmpty())
			{
				index = i;
			}			
			else if(value.IsEqual(layerName))
			{
				index = Global.INVALID_INDEX;

				break;
			}
		}
		
		if(index != Global.INVALID_INDEX)
		{
			var property = layerProperty.GetArrayElementAtIndex(index);
			property.stringValue = layerName;
			serialized.ApplyModifiedProperties();
		}
	}
	#endregion Tag & Layer

	#region Player Settings
	public static void AddDefineSymbol(string defineSymbol,NamedBuildTarget buildTargetGroup)
	{
		var symbolText = PlayerSettings.GetScriptingDefineSymbols(buildTargetGroup);

		if(symbolText.Contains(defineSymbol))
		{
			return;
		}

		PlayerSettings.SetScriptingDefineSymbols(buildTargetGroup,$"{symbolText};{defineSymbol}");
	}

	public static void RemoveDefineSymbol(string defineSymbol,NamedBuildTarget buildTargetGroup)
	{
		var symbolText = PlayerSettings.GetScriptingDefineSymbols(buildTargetGroup);

		if(!symbolText.Contains(defineSymbol))
		{
			return;
		}

		PlayerSettings.SetScriptingDefineSymbols(buildTargetGroup,symbolText.Replace(defineSymbol,"").Replace(";;",";"));
	}

	public static void ChangeDefineSymbol(string[] oldDefineSymbolArray,string[] newDefineSymbolArray)
	{
		foreach(var target in BuildTargetGroup)
		{
			var defineSymbolText = PlayerSettings.GetScriptingDefineSymbols(target);

			if(defineSymbolText.IsEmpty())
			{
				continue;
			}

			var defineSymbolHashSet = new HashSet<string>(defineSymbolText.Split(';'));

			for(var i=0;i<oldDefineSymbolArray.Length;i++)
			{
				defineSymbolHashSet.Add(oldDefineSymbolArray[i]);
			}

			for(var i=0;i<newDefineSymbolArray.Length;i++)
			{
				defineSymbolHashSet.Add(newDefineSymbolArray[i]);
			}

			PlayerSettings.SetScriptingDefineSymbols(target,string.Join(";",defineSymbolHashSet));
		}
	}

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