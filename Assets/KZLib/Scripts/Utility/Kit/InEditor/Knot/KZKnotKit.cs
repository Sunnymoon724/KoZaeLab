#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class KZKnotKit
{
	private const float c_majorSize = 10.0f;
	private const float c_minorSize = 7.0f;

	private readonly static Color s_fixedColor = "#808080".ToColor();
	private readonly static Color s_selectedColor = "#FFFF00FF".ToColor();

	private readonly static Color s_normalMajorColor = "#FF5F5FFF".ToColor();
	private readonly static Color s_highlightMajorColor = "#BC0A0AFF".ToColor();

	private readonly static Color s_normalMinorColor = "#5999FFFF".ToColor();
	private readonly static Color s_highlightMinorColor = "#3232C0FF".ToColor();


	private readonly static Color s_normalBorderLineColor = "#33CC1AFF".ToColor();
	private readonly static Color s_editingBorderLineColor = "#91F48BFF".ToColor();

	private readonly static Color s_normalLineColor = "#00FF00FF".ToColor();
	private readonly static Color s_guideLineColor = "#FFFFFFFF".ToColor();

	public static void DrawFixedKnot(int index,Vector3 position)
	{
		_DrawKnot(index,false,position,s_fixedColor);
	}

	public static void DrawControlKnot(int index,Vector3 position,bool isMouseOver,bool isSelected,bool isMajor)
	{
		var highlightColor = isMajor ? s_highlightMajorColor : s_highlightMinorColor;
		var normalColor = isMajor ? s_normalMajorColor : s_normalMinorColor;
		var controlColor = isSelected ? s_selectedColor : isMouseOver ? highlightColor : normalColor;

		_DrawKnot(index,true,position,controlColor);
	}

	private static void _DrawKnot(int index,bool isMajor,Vector3 position,Color color)
	{
		var knotSize = _GetKnotDiameterSize(isMajor,position);
		var controlID = GUIUtility.GetControlID(index,FocusType.Passive);

		var cachedColor = Handles.color;

		Handles.color = color;

		Handles.SphereHandleCap(controlID,position,Quaternion.LookRotation(Vector3.up),knotSize,EventType.Repaint);

		DrawText($"{index}",position,Color.white,true);

		Handles.color = cachedColor;
	}

	public static bool IsMouseOverHandle(bool isMajor,Vector3 position)
	{
		var diameterSize = _GetKnotDiameterSize(isMajor,position)/2.0f;

		return HandleUtility.DistanceToCircle(position,diameterSize) == 0.0f;
	}

	private static float _GetKnotDiameterSize(bool isMajor,Vector3 position)
	{
		var knotSize = isMajor ? c_majorSize : c_minorSize;

		return knotSize*0.01f*HandleUtility.GetHandleSize(position)*2.5f;
	}

	public static void DrawBorderLine(Vector3[] positionArray,float width,bool isEditing)
	{
		_DrawLine(positionArray,width,isEditing ? s_editingBorderLineColor : s_normalBorderLineColor);
	}

	private static void _DrawLine(Vector3[] positionArray,float width,Color color)
	{
		var cachedColor = Handles.color;

		Handles.color = color;

		Handles.DrawAAPolyLine(width,positionArray);

		Handles.color = cachedColor;
	}

	public static void DrawText(string labelText,Vector3 position,Color textColor,bool hasShadow)
	{
		var cachedColor = Handles.color;

		var labelStyle = new GUIStyle(GUI.skin.label);

		labelStyle.normal.textColor = textColor;

		Handles.Label(position,labelText,labelStyle);

		if(hasShadow)
		{
			var shadowStyle = new GUIStyle(GUI.skin.label);

			shadowStyle.normal.textColor = Color.black;

			Handles.Label(position+new Vector3(0.02f,-0.02f,0),labelText,shadowStyle);
		}

		Handles.color = cachedColor;
	}
}
#endif