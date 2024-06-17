#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;

public static partial class CommonUtility
{
	/// <summary>
	/// 에디터에 태그를 추가한다.
	/// </summary>
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

	private static void AddLayer(string _layerName)
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

	private static SerializedObject GetTagManagerObject()
	{
		var assetArray = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");

		if(assetArray.IsNullOrEmpty())
		{
			return null;
		}

		return new SerializedObject(assetArray[0]);
	}

	/// <summary>
	///  Define Symbols을 추가한다.
	/// </summary>
	public static void AddDefineSymbol(string _symbol,BuildTargetGroup _target)
	{
		var symbolGroup = PlayerSettings.GetScriptingDefineSymbolsForGroup(_target);

		if(symbolGroup.Contains(_symbol))
		{
			return;
		}

		PlayerSettings.SetScriptingDefineSymbolsForGroup(_target,string.Format("{0};{1}",symbolGroup,_symbol));
	}

	/// <summary>
	///  Define Symbols을 제거한다.
	/// </summary>
	public static void RemoveDefineSymbol(string _symbol,BuildTargetGroup _target)
	{
		var symbolGroup = PlayerSettings.GetScriptingDefineSymbolsForGroup(_target);

		if(!symbolGroup.Contains(_symbol))
		{
			return;
		}

		PlayerSettings.SetScriptingDefineSymbolsForGroup(_target,symbolGroup.Replace(_symbol,"").Replace(";;",";"));
	}

	/// <summary>
	///  Define Symbols을 바꾼다.
	/// </summary>
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
}
#endif