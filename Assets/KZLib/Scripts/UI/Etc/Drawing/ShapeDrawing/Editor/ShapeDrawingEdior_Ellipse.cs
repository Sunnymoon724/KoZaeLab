#if UNITY_EDITOR
using System.Reflection;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace KZLib
{
	public partial class ShapeDrawingEditor : OdinEditor
	{
		private PropertyInfo m_ellipseAngleInfo = null;
		private SerializedProperty m_ellipseAngleProperty = null;

		private void _SetEllipse()
		{
			m_ellipseAngleProperty = m_serializedObject.FindProperty("m_ellipseAngle");
			m_ellipseAngleInfo = target.GetType().GetProperty("EllipseAngle",BindingFlags.NonPublic | BindingFlags.Instance);
		}

		private void _DrawEllipse()
		{
			_DrawEllipseAngle();
		}

		private void _DrawEllipseAngle()
		{
			EditorGUI.BeginChangeCheck();

			var newAngle = EditorGUILayout.Slider("Angle",m_ellipseAngleProperty.floatValue,Global.ZERO_ANGLE,Global.FULL_ANGLE);

			if(EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(m_shapeDrawing,"Change Angle");

				m_ellipseAngleInfo.SetValue(target,newAngle);
			}
		}
	}
}
#endif