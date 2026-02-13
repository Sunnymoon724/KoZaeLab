// #if UNITY_EDITOR
// using Sirenix.OdinInspector.Editor;
// using UnityEditor;
// using UnityEngine;
// using UnityEngine.UI;

// namespace KZLib.UI
// {
// 	public partial class UIShapeEditor : OdinEditor
// 	{
// 		private SerializedProperty m_rectangleRadiusProperty = null;

// 		private void _SetRectangle()
// 		{
// 			m_rectangleRadiusProperty = m_serializedObject.FindProperty("m_rectangleRadius");
// 			m_rectangleRadiusProperty = m_serializedObject.FindProperty("m_rectangleRadius");
// 		}
		
// 		private void _DrawRectangle()
// 		{
// 			var size = Mathf.Min(m_rectTransform.rect.size.x,m_rectTransform.rect.size.y)/2.0f;

// 			m_rectangleRadiusProperty.floatValue = EditorGUILayout.Slider("Radius",m_rectangleRadiusProperty.floatValue,0.0f,size);
// 		}
// 	}
// }
// #endif