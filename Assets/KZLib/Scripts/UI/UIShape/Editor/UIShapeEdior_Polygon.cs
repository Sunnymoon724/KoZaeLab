#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace KZLib
{
	public partial class UIShapeEditor : OdinEditor
	{
		private bool m_polygonArrayFoldout = false;

		private SerializedProperty m_polygonSideCountProperty = null;
		private SerializedProperty m_polygonVertexDistanceArrayProperty = null;

		private void _SetPolygon()
		{
			m_polygonSideCountProperty = m_serializedObject.FindProperty("m_polygonSideCount");
			m_polygonVertexDistanceArrayProperty = m_serializedObject.FindProperty("m_polygonVertexDistanceArray");
		}
		
		private void _DrawPolygon()
		{
			var sideCount = m_polygonSideCountProperty.intValue;

			m_polygonSideCountProperty.intValue = EditorGUILayout.IntSlider("Side Count",m_polygonSideCountProperty.intValue,3,36);

			if(sideCount != m_polygonSideCountProperty.intValue)
			{
				sideCount = m_polygonSideCountProperty.intValue;

				var arrayCount = m_polygonVertexDistanceArrayProperty.arraySize;

				if(arrayCount < sideCount)
				{
					for(var i=arrayCount;i<sideCount;i++)
					{
						m_polygonVertexDistanceArrayProperty.arraySize++;

						var element = m_polygonVertexDistanceArrayProperty.GetArrayElementAtIndex(i);

						element.floatValue = 1;
					}
				}
				else
				{
					var difference = arrayCount-sideCount;

					for(var i=0;i<difference;i++)
					{
						m_polygonVertexDistanceArrayProperty.arraySize--;
					}
				}
			}

			if(m_polygonVertexDistanceArrayProperty.arraySize > 0)
			{
				m_polygonArrayFoldout = EditorGUILayout.Foldout(m_polygonArrayFoldout,"Vertex Distance Array");

				if(m_polygonArrayFoldout)
				{
					EditorGUILayout.BeginVertical(EditorStyles.helpBox);

					for(var i=0;i<m_polygonVertexDistanceArrayProperty.arraySize;i++)
					{
						var distance = m_polygonVertexDistanceArrayProperty.GetArrayElementAtIndex(i);

						distance.floatValue = EditorGUILayout.Slider($"Vertex {i + 1}",distance.floatValue,0.0f,1.0f);
					}

					EditorGUILayout.EndVertical();
				}
			}
		}
	}
}
#endif