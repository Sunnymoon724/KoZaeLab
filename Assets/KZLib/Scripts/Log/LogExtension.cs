using UnityEngine;
using KZLib;

public enum Log
{
	// 태그 없는 일반 로그용
	Normal,

	// 시스템 관련
	System,

	// 씬 관련
	Scene,

	// 빌드 관련
	Build,

	// 네트워크 관련
	Network,
	Server,
	Client,

	// 데이터 관련
	Bundle,
	Data,
	UI,
	Effect,

	InGame,

	// 사운드 관련
	Sound,

	// 디벨로프 관련
	Develop,

	// 에디터 관련
	Editor,

	// 파일 관련
	Files,

	// 테스트
	Test,
}

/// <summary>
/// 실제 로그는 에디터 상태와 디벨롭 모드일때만 출력됩니다.
/// </summary>
public static class LogExtension
{
	#region I : Info Log
	public static void I(this Log _log,object _message,params object[] _arguments)
	{
		var text = LogMgr.In.ShowLog(_log,LogType.Log,_message,_arguments);

#if UNITY_EDITOR
		Debug.Log(text);
#endif
	}
	#endregion I : Info Log

	#region W : Warning Log
	public static void W(this Log _log,object _message,params object[] _arguments)
	{
		var text = LogMgr.In.ShowLog(_log,LogType.Warning,_message,_arguments);

#if UNITY_EDITOR
		Debug.LogWarning(text);
#endif
	}
	#endregion W : Warning Log

	#region E : Error Log
	public static void E(this Log _log,object _message,params object[] _arguments)
	{
		var text = LogMgr.In.ShowLog(_log,LogType.Error,_message,_arguments);

#if UNITY_EDITOR
		Debug.LogError(text);
#endif
	}
	#endregion E : Error Log

	#region F : Fatal Log 에러를 서버로 보내고 싶을때 
	public static void F(this Log _log,object _message,params object[] _arguments)
	{
		var text = LogMgr.In.ShowLog(_log,LogType.Exception,_message,_arguments);

#if UNITY_EDITOR
		Debug.LogError(text);
#endif
	}
	#endregion F : Fatal Log

	#region A : Assert Log
	public static void A(this Log _log,bool _condition,object _message,params object[] _arguments)
	{
		if(!_condition)
		{
			return;
		}

		var text = LogMgr.In.ShowLog(_log,LogType.Assert,_message,_arguments);

#if UNITY_EDITOR
		Debug.AssertFormat(_condition,text,_arguments);
#endif
	}
	#endregion A : Assert Log
}