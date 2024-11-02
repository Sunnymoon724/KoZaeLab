#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;

public static partial class CommonUtility
{
	#region Tag & Layer
	public static void AddTag(string _tag)
	{
		foreach(var tag in InternalEditorUtility.tags)
		{
			if(tag.IsEqual(_tag))
			{
				return;
			}
		}

		InternalEditorUtility.AddTag(_tag);
	}

	private static SerializedObject GetTagManagerObject()
	{
		var assetArray = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");

		if(assetArray.IsNullOrEmpty())
		{
			return null;
		}

		return new SerializedObject(assetArray[0]);
	}

	public static void AddLayer(string _layerName)
	{
		var serialized = GetTagManagerObject();

		if(serialized == null)
		{
			return;
		}

		var layerProperty = serialized.FindProperty("layers");
		int layerCount = layerProperty.arraySize;
		int index = -1;
		
		for(var i=8;i<layerCount;i++)
		{
			var value = layerProperty.GetArrayElementAtIndex(i).stringValue;

			if(index == -1 && value.IsEmpty())
			{
				index = i;
			}			
			else if(value.IsEqual(_layerName))
			{
				index = -1;

				break;
			}
		}
		
		if(index != -1)
		{
			var property = layerProperty.GetArrayElementAtIndex(index);
			property.stringValue = _layerName;
			serialized.ApplyModifiedProperties();
		}
	}
	#endregion Tag & Layer

	#region Player Settings
	public static void AddDefineSymbol(string _symbol,BuildTargetGroup _target)
	{
		var symbolGroup = PlayerSettings.GetScriptingDefineSymbolsForGroup(_target);

		if(symbolGroup.Contains(_symbol))
		{
			return;
		}

		PlayerSettings.SetScriptingDefineSymbolsForGroup(_target,$"{symbolGroup};{_symbol}");
	}

	public static void RemoveDefineSymbol(string _symbol,BuildTargetGroup _target)
	{
		var symbolGroup = PlayerSettings.GetScriptingDefineSymbolsForGroup(_target);

		if(!symbolGroup.Contains(_symbol))
		{
			return;
		}

		PlayerSettings.SetScriptingDefineSymbolsForGroup(_target,symbolGroup.Replace(_symbol,"").Replace(";;",";"));
	}

	public static void ChangeDefineSymbol(string[] _oldSymbolArray,string[] _newSymbolArray)
	{
		foreach(var target in GetBuildTargetGroup())
		{
			var symbolGroup = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);

			if(symbolGroup.IsEmpty())
			{
				continue;
			}

			var symbolList = symbolGroup.Split(';').ToList();

			symbolList.RemoveRange(_oldSymbolArray);
			symbolList.AddRange(_newSymbolArray);

			PlayerSettings.SetScriptingDefineSymbolsForGroup(target,string.Join(";",symbolList));
		}
	}

	public static void ChangePackageName(string _packageName)
	{
		foreach(var target in GetBuildTargetGroup())
		{
			PlayerSettings.SetApplicationIdentifier(target,_packageName);
		}
	}

	private static IEnumerable<BuildTargetGroup> GetBuildTargetGroup()
	{
		return new BuildTargetGroup[] { BuildTargetGroup.Standalone, BuildTargetGroup.Android, BuildTargetGroup.iOS };
	}
	#endregion Player Settings

	#region DisplayDialog
	public static void DisplayError(Exception _exception)
	{
		DisplayDialogWindow("Error",_exception.Message,"Ok");

		throw _exception;
	}

	public static void DisplayInfo(string _message)
	{
		DisplayDialogWindow("Info",_message,"Ok");
	}

	public static bool DisplayCheckBeforeExecute(string _name)
	{
		return DisplayCheck($"Execute {_name}",$"Execute {_name}?");
	}

	public static bool DisplayCheck(string _title,string _message)
	{
		return DisplayDialogWindow(_title,_message,"Yes","No");
	}

	private static bool DisplayDialogWindow(string _title,string _message,string _ok,string _cancel = "")
	{
		return EditorUtility.DisplayDialog(_title,_message,_ok,_cancel);
	}

	public static bool DisplayCancelableProgressBar(string _title,string _message,int _current,int _total)
	{
		return DisplayCancelableProgressBar(_title,_message,_current/(float)_total);
	}

	public static bool DisplayCancelableProgressBar(string _title,string _message,float _progress)
	{
		return EditorUtility.DisplayCancelableProgressBar(_title,_message,_progress);
	}

	public static void ClearProgressBar()
	{
		EditorUtility.ClearProgressBar();
	}
	#endregion DisplayDialog
}
#endif