#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace KZLib
{
	public partial class UIShapeEditor : OdinEditor
	{
		private SerializedProperty m_ellipseAngleProperty = null;

		private void _SetEllipse()
		{
			m_ellipseAngleProperty = m_serializedObject.FindProperty("m_ellipseAngle");
		}
		
		private void _DrawEllipse()
		{
			m_ellipseAngleProperty.floatValue = EditorGUILayout.Slider("Angle",m_ellipseAngleProperty.floatValue,0.0f,360.0f);
		}
	}
}
#endif