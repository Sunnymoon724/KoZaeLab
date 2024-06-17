using UnityEngine;

public static partial class CommonUtility
{
	public static void Quit()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}
	
	/// <summary>
	/// 모바일 체크
	/// </summary>
	public static bool IsMobile => Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;
	
	public static int AdjustByDPI(float _height)
	{
		var dpi = Screen.dpi;

		if(dpi == 0.0f)
		{
			dpi = IsMobile ? 160.0f : 96.0f;
		}

		var height = Mathf.RoundToInt(_height*(96.0f/dpi));

		if((height & 1) == 1)
		{
			++height;
		} 
		return height;
	}
	
	public static bool IsEditor()
	{
#if UNITY_EDITOR
		return true;
#else
		return false;
#endif
	}
}