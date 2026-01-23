#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static partial class KZHandleKit
{
	private const float c_anchorSize = 10.0f;
	private const float c_controlSize = 7.0f;
	
	private readonly static Color s_handleSelectColor = "#FFFF00FF".ToColor();

	private readonly static Color s_anchorNormalColor = "#FF5F5FFF".ToColor();
	private readonly static Color s_anchorHighlightColor = "#BC0A0AFF".ToColor();

	private readonly static Color s_controlNormalColor = "#5999FFFF".ToColor();
	private readonly static Color s_controlHighlightColor = "#3232C0FF".ToColor();


	private readonly static Color s_normalLineColor = "#00FF00FF".ToColor();
	private readonly static Color s_guideLineColor = "#FFFFFFFF".ToColor();

	public static void DrawHandlePosition(int index,Vector3 position,bool isMouseOver,bool isSelected,bool isAnchor)
	{
		var diameter = _GetHandleDiameter(isAnchor ? c_anchorSize : c_controlSize,position);
		var handleId = GUIUtility.GetControlID(index.GetHashCode(),FocusType.Passive);

		var cachedColor = Handles.color;

		var highlightColor = isAnchor ? s_anchorHighlightColor : s_controlHighlightColor;
		var normalColor = isAnchor ? s_anchorNormalColor : s_controlNormalColor;

		Handles.color = isSelected ? s_handleSelectColor : isMouseOver ? highlightColor : normalColor;

		Handles.SphereHandleCap(handleId,position,Quaternion.LookRotation(Vector3.up),diameter,EventType.Repaint);

		var style = new GUIStyle();

		style.normal.textColor = Color.white;

		Handles.Label(position,$"{index}",style);

		Handles.color = cachedColor;
	}

	public static bool IsMouseOverHandle(Vector3 location)
	{
		var diameter = _GetHandleDiameter(c_anchorSize,location)/2.0f;

		return HandleUtility.DistanceToCircle(location,diameter) == 0.0f;
	}

	public static Vector3 ConvertToPoint(Vector3 position,Transform transform,SpaceType spaceType)
	{
		return position.TransformPoint(transform.transform,spaceType);
	}

	private static float _GetHandleDiameter(float diameter,Vector3 position)
	{
		return diameter*0.01f*HandleUtility.GetHandleSize(position)*2.5f;
	}
}
#endif