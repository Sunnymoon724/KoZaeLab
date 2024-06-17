#if UNITY_EDITOR
using System;
using UnityEditor;

public static partial class CommonUtility
{
	/// <summary>
	/// 오류를 text로 설명하는 팝업창
	/// </summary>
	public static void DisplayError(string _message)
	{
		DisplayDialog("오류",_message,"확인");

		throw new Exception(_message);
	}

	/// <summary>
	/// path가 잘못되었다고 알려주는 팝업창
	/// </summary>
	public static void DisplayErrorPathLink(string _path)
	{
		DisplayError(string.Format("{0}의 경로가 잘못 되었습니다. 다시 확인해 주세요.",_path));
	}

	/// <summary>
	/// 정보를 text로 설명하는 팝업창
	/// </summary>
	public static void DisplayInfo(string _message)
	{
		DisplayDialog("정보",_message,"확인");
	}

	/// <summary>
	/// 실행 전 실행 여부 파악을 위한 팝업창
	/// </summary>
	public static bool DisplayCheck(string _title,string _message)
	{
		return DisplayDialog(_title,_message,"네","아니요");
	}

	public static bool DisplayCheckBeforeExecute(string _name)
	{
		return DisplayCheck(string.Format("{0} 실행을 진행합니다.",_name),string.Format("{0} 실행을 진행하시겠습니까?",_name));
	}
	
	private static bool DisplayDialog(string _title,string _message,string _ok,string _cancel = "")
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
}
#endif