#if UNITY_EDITOR
using UnityEditor;

namespace KZLib
{
	public partial class ShapeDrawingEditor : GraphicDrawingEditor
	{
		private void _DrawEllipse()
		{
			_DrawEllipseAngle();
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