#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace KZLib.UI
{
	public partial class ShapeDrawingEditor : GraphicDrawingEditor
	{
		private bool _CanShowKnot_Ellipse()
		{
			return true;
		}

		private void _Draw_Ellipse()
		{
			_DrawEllipseAngle();
		}

		private void _UpdateKnotList_Ellipse(Vector3 position,Quaternion rotation)
		{
			var fixedEdgePos = _GetEdgePosition(0.0f);
			var fixedPos = position+rotation*fixedEdgePos;

			_AddOrUpdateKnotInfo(1,fixedPos,KnotType.Fixed);

			var majorEdgePos = _GetEdgePosition(m_shapeDrawing.EllipseAngle*Mathf.Deg2Rad);
			var majorPos = position+rotation*majorEdgePos;

			_AddOrUpdateKnotInfo(2,majorPos,KnotType.Major);
		}

		private void _ChangeKnotPosition_Ellipse(int index,Vector3 localPosition)
		{
			var angle = Mathf.Atan2(localPosition.y,localPosition.x)*Mathf.Rad2Deg;

			if(angle < 0.0f)
			{
				angle += 360f;
			}

			m_shapeDrawing.EllipseAngle = angle;
		}

		private void _DrawEllipseAngle()
		{
			EditorGUI.BeginChangeCheck();

			var newAngle = EditorGUILayout.Slider("Angle",m_shapeDrawing.EllipseAngle,Global.ZERO_ANGLE,Global.FULL_ANGLE);

			if(EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(m_shapeDrawing,"Change Angle");

				m_shapeDrawing.EllipseAngle = newAngle;

				m_serializedObject.Update();
			}
		}
	}
}
#endif