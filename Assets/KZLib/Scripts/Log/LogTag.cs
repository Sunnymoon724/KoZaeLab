using UnityEngine;
using KZLib;

#if UNITY_EDITOR

using System;
using UnityEditor.Callbacks;
using UnityEditorInternal;
using UnityEditor;
using System.Reflection;
using System.Text.RegularExpressions;

#endif

public class LogTag : Enumeration
{
	/// <summary>
	/// 시스템 관련
	/// </summary>
	public static readonly LogTag System = new(nameof(System));

	/// <summary>
	/// 씬 관련
	/// </summary>
	public static readonly LogTag Scene = new(nameof(Scene));

	/// <summary>
	/// 빌드 관련
	/// </summary>
	public static readonly LogTag Build = new(nameof(Build));

	/// <summary>
	/// 네트워크 관련 - 서버
	/// </summary>
	public static readonly LogTag Server = new(nameof(Server));
	/// <summary>
	/// 네트워크 관련 - 클라이언트
	/// </summary>
	public static readonly LogTag Client = new(nameof(Client));

	/// <summary>
	/// 데이터 관련
	/// </summary>
	public static readonly LogTag Data = new(nameof(Data));
	/// <summary>
	/// UI 관련
	/// </summary>
	public static readonly LogTag UI = new(nameof(UI));
	/// <summary>
	/// 이펙트 관련
	/// </summary>
	public static readonly LogTag Effect = new(nameof(Effect));
	/// <summary>
	/// 사운드 관련
	/// </summary>
	public static readonly LogTag Sound = new(nameof(Sound));

	/// <summary>
	/// 에디터 관련
	/// </summary>
	public static readonly LogTag Editor = new(nameof(Editor));

	/// <summary>
	/// 파일 관련
	/// </summary>
	public static readonly LogTag File = new(nameof(File));

	/// <summary>
	/// 테스트 관련
	/// </summary>
	public static readonly LogTag Test = new(nameof(Test));

	public LogTag(string _name) : base(_name) { }
}

/// <summary>
/// 실제 로그는 에디터 상태일때만 출력됩니다.
/// </summary>
public static class LogExtension
{
	#region I : Info Log
	public static void I(this LogTag _log,object _message,params object[] _arguments)
	{
		var text = LogMgr.In.ShowLog(_log,_message,_arguments);

#if UNITY_EDITOR
		Debug.Log(text);
#endif
	}
	#endregion I : Info Log

	#region W : Warning Log
	public static void W(this LogTag _log,object _message,params object[] _arguments)
	{
		var text = LogMgr.In.ShowLog(_log,_message,_arguments);

#if UNITY_EDITOR
		Debug.LogWarning(text);
#endif
	}
	#endregion W : Warning Log

	#region E : Error Log
	public static void E(this LogTag _log,object _message,params object[] _arguments)
	{
		var text = LogMgr.In.ShowLog(_log,_message,_arguments);

#if UNITY_EDITOR
		Debug.LogError(text);
#endif
	}
	#endregion E : Error Log

	#region A : Assert Log
	public static void A(this LogTag _log,bool _condition,object _message,params object[] _arguments)
	{
		if(!_condition)
		{
			return;
		}

		var text = LogMgr.In.ShowLog(_log,_message,_arguments);

#if UNITY_EDITOR
		Debug.AssertFormat(_condition,text,_arguments);
#endif
	}
	#endregion A : Assert Log

#if UNITY_EDITOR
	[OnOpenAsset(0)]
	private static bool OnOpenDebugLog(int _instance,int _line)
	{
		var name = EditorUtility.InstanceIDToObject(_instance).name;

		//? LogTag 에서 호출한 Log만 제어하기 위해서
		if(name.IsEmpty() || !name.IsEqual(nameof(LogTag)))
		{
			return false;
		}

		var stackTrace = GetStackTrace();

		if(stackTrace.IsEmpty())
		{
			return false;
		}

		var match = Regex.Match(stackTrace,@"\(at (.+)\)",RegexOptions.IgnoreCase);

		if(match.Success)
		{
			match = match.NextMatch();
		}

		if(match.Success)
		{
			var pathArray = match.Groups[1].Value.Split(':');

			if(pathArray.Length < 2 || !int.TryParse(pathArray[1],out int lineNumber))
			{
				return false;
			}

			InternalEditorUtility.OpenFileAtLineExternal(FileUtility.GetAbsolutePath(pathArray[0],true),lineNumber);

			return true;
		}

		return false;
	}

	private static string GetStackTrace()
	{
		var assembly = Assembly.GetAssembly(typeof(EditorWindow));

		if(assembly == null)
		{
			return null;
		}

		var windowType = assembly.GetType("UnityEditor.ConsoleWindow");

		if(windowType == null)
		{
			return null;
		}

		var windowFieldInfo = windowType.GetField("ms_ConsoleWindow",BindingFlags.Static | BindingFlags.NonPublic);

		if(windowFieldInfo == null)
		{
			return null;
		}

		var window = windowFieldInfo.GetValue(null);

		if(window != (object)EditorWindow.focusedWindow)
		{
			return null;
		}

		var activeFieldInfo = windowType.GetField("m_ActiveText",BindingFlags.Instance | BindingFlags.NonPublic);

		if(activeFieldInfo == null)
		{
			return null;
		}

		return activeFieldInfo.GetValue(window).ToString();
	}
#endif
}