#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor-only utility methods for drawing spline/path knots, borders, and labels in the scene view.
/// </summary>
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

	private static GUIStyle s_labelStyle = null;
	private static GUIStyle s_shadowStyle = null;

	/// <summary>
	/// Draws a non-interactive knot at the given world position.
	/// </summary>
	public static void DrawFixedKnot(int index,Vector3 position)
	{
		_DrawKnot(index,false,position,s_fixedColor);
	}

	/// <summary>
	/// Draws an interactive control knot with hover, selection, and major/minor styling.
	/// </summary>
	public static void DrawControlKnot(int index,Vector3 position,bool isMouseOver,bool isSelected,bool isMajor)
	{
		var highlightColor = isMajor ? s_highlightMajorColor : s_highlightMinorColor;
		var normalColor = isMajor ? s_normalMajorColor : s_normalMinorColor;
		var controlColor = isSelected ? s_selectedColor : isMouseOver ? highlightColor : normalColor;

		_DrawKnot(index,isMajor,position,controlColor);
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

	/// <summary>
	/// Returns whether the mouse is within the knot's circular pick radius.
	/// </summary>
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

	/// <summary>
	/// Draws the closed border polyline around a shape.
	/// </summary>
	public static void DrawBorderLine(Vector3[] positionArray,float width,bool isEditing)
	{
		if(positionArray == null)
		{
			return;
		}

		_DrawLine(positionArray,width,isEditing ? s_editingBorderLineColor : s_normalBorderLineColor);
	}

	private static void _DrawLine(Vector3[] positionArray,float width,Color color)
	{
		var cachedColor = Handles.color;

		Handles.color = color;

		Handles.DrawAAPolyLine(width,positionArray);

		Handles.color = cachedColor;
	}

	/// <summary>
	/// Draws world-space text with an optional offset shadow.
	/// </summary>
	public static void DrawText(string labelText,Vector3 position,Color textColor,bool hasShadow)
	{
		var cachedColor = Handles.color;

		s_labelStyle ??= new GUIStyle(GUI.skin.label);
		s_labelStyle.normal.textColor = textColor;

		Handles.Label(position,labelText,s_labelStyle);

		if(hasShadow)
		{
			s_shadowStyle ??= new GUIStyle(GUI.skin.label);
			s_shadowStyle.normal.textColor = Color.black;

			Handles.Label(position+new Vector3(0.02f,-0.02f,0),labelText,s_shadowStyle);
		}

		Handles.color = cachedColor;
	}
}
#endif
