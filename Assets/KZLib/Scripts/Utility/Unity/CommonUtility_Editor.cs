﻿#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;

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

	private static SerializedObject FindTagManagerObject()
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
		var serialized = FindTagManagerObject();

		if(serialized == null)
		{
			return;
		}

		var layerProperty = serialized.FindProperty("layers");
		var layerCount = layerProperty.arraySize;
		var index = -1;
		
		for(var i=8;i<layerCount;i++)
		{
			var value = layerProperty.GetArrayElementAtIndex(i).stringValue;

			if(index == -1 && value.IsEmpty())
			{
				index = i;
			}			
			else if(value.IsEqual(layerName))
			{
				index = -1;

				break;
			}
		}
		
		if(index != -1)
		{
			var property = layerProperty.GetArrayElementAtIndex(index);
			property.stringValue = layerName;
			serialized.ApplyModifiedProperties();
		}
	}
	#endregion Tag & Layer

	#region Player Settings
	public static void AddDefineSymbol(string defineSymbol,BuildTargetGroup buildTargetGroup)
	{
		var symbolText = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

		if(symbolText.Contains(defineSymbol))
		{
			return;
		}

		PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup,$"{symbolText};{defineSymbol}");
	}

	public static void RemoveDefineSymbol(string defineSymbol,BuildTargetGroup buildTargetGroup)
	{
		var symbolText = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

		if(!symbolText.Contains(defineSymbol))
		{
			return;
		}

		PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup,symbolText.Replace(defineSymbol,"").Replace(";;",";"));
	}

	public static void ChangeDefineSymbol(string[] oldDefineSymbolArray,string[] newDefineSymbolArray)
	{
		foreach(var target in GetBuildTargetGroup())
		{
			var defineSymbolText = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);

			if(defineSymbolText.IsEmpty())
			{
				continue;
			}

			var defineSymbolHashSet = new HashSet<string>(defineSymbolText.Split(';'));

			foreach(var oldDefineSymbol in oldDefineSymbolArray)
			{
				defineSymbolHashSet.Remove(oldDefineSymbol);
			}

			foreach(var newDefineSymbol in newDefineSymbolArray)
			{
				defineSymbolHashSet.Add(newDefineSymbol);
			}

			PlayerSettings.SetScriptingDefineSymbolsForGroup(target,string.Join(";",defineSymbolHashSet));
		}
	}

	public static void ChangePackageName(string packageName)
	{
		foreach(var target in GetBuildTargetGroup())
		{
			PlayerSettings.SetApplicationIdentifier(target,packageName);
		}
	}

	private static IEnumerable<BuildTargetGroup> GetBuildTargetGroup()
	{
		return new BuildTargetGroup[] { BuildTargetGroup.Standalone, BuildTargetGroup.Android, BuildTargetGroup.iOS };
	}
	#endregion Player Settings

	#region DisplayDialog
	public static void DisplayError(Exception exception)
	{
		EditorUtility.DisplayDialog("Error",exception.Message,"Ok","");

		throw exception;
	}

	public static void DisplayInfo(string message)
	{
		EditorUtility.DisplayDialog("Info",message,"Ok","");
	}

	public static bool DisplayCheckBeforeExecute(string name)
	{
		return DisplayCheck($"Execute {name}",$"Execute {name}?");
	}

	public static bool DisplayCheck(string title,string message)
	{
		return EditorUtility.DisplayDialog(title,message,"Yes","No");
	}

	public static bool DisplayCancelableProgressBar(string title,string info,int current,int total)
	{
		return DisplayCancelableProgressBar(title,info,current/(float)total);
	}

	public static bool DisplayCancelableProgressBar(string title,string info,float progress)
	{
		return EditorUtility.DisplayCancelableProgressBar(title,info,progress);
	}

	public static void ClearProgressBar()
	{
		EditorUtility.ClearProgressBar();
	}
	#endregion DisplayDialog
}
#endif