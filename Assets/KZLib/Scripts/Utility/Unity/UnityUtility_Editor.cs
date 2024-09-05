#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;

public static partial class UnityUtility
{
	#region Tag & Layer
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
	#endregion Player Settings

	#region DisplayDialog
	/// <summary>
	/// path가 잘못되었다고 알려주는 대화 창
	/// </summary>
	public static void DisplayErrorPathLink(string _path)
	{
		DisplayError(string.Format("{0}의 경로가 잘못 되었습니다. 다시 확인해 주세요.",_path));
	}

	/// <summary>
	/// 오류 message 설명하는 대화 창
	/// </summary>
	public static void DisplayError(string _message)
	{
		DisplayDialogWindow("오류",_message,"확인");

		throw new Exception(_message);
	}

	/// <summary>
	/// 정보 message 설명하는 대화 창
	/// </summary>
	public static void DisplayInfo(string _message)
	{
		DisplayDialogWindow("정보",_message,"확인");
	}

	/// <summary>
	/// 실행 전 실행 여부 파악을 위한 대화 창
	/// </summary>
	/// <param name="_name"></param>
	/// <returns></returns>
	public static bool DisplayCheckBeforeExecute(string _name)
	{
		return DisplayCheck(string.Format("{0} 실행을 진행합니다.",_name),string.Format("{0} 실행을 진행하시겠습니까?",_name));
	}

	/// <summary>
	/// 메세지 알림 창
	/// </summary>
	public static bool DisplayCheck(string _title,string _message)
	{
		return DisplayDialogWindow(_title,_message,"네","아니요");
	}

	/// <summary>
	/// 대화창 열기
	/// </summary>
	private static bool DisplayDialogWindow(string _title,string _message,string _ok,string _cancel = "")
	{
		return EditorUtility.DisplayDialog(_title,_message,_ok,_cancel);
	}

	/// <summary>
	/// 취소 가능한 프로그래스 바 설정
	/// </summary>
	public static bool DisplayCancelableProgressBar(string _title,string _message,int _current,int _total)
	{
		return DisplayCancelableProgressBar(_title,_message,_current/(float)_total);
	}

	/// <summary>
	/// 취소 가능한 프로그래스 바 설정
	/// </summary>
	public static bool DisplayCancelableProgressBar(string _title,string _message,float _progress)
	{
		return EditorUtility.DisplayCancelableProgressBar(_title,_message,_progress);
	}

	/// <summary>
	/// 프로그래스 바 없애기
	/// </summary>
	public static void ClearProgressBar()
	{
		EditorUtility.ClearProgressBar();
	}
	#endregion DisplayDialog
}
#endif