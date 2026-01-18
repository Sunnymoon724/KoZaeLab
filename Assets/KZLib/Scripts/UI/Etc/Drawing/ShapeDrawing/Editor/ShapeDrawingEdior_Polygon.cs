#if UNITY_EDITOR
using System.Reflection;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace KZLib
{
	public partial class ShapeDrawingEditor : OdinEditor
	{
		private SerializedProperty m_polygonSideCountProperty = null;
		private PropertyInfo m_polygonSideCountInfo = null;

		private bool m_polygonArrayFoldout = false;
		private SerializedProperty m_polygonVertexDistanceArrayProperty = null;

		private void _SetPolygon()
		{
			m_polygonSideCountProperty = m_serializedObject.FindProperty("m_polygonSideCount");
			m_polygonSideCountInfo = target.GetType().GetProperty("PolygonSideCount",BindingFlags.NonPublic | BindingFlags.Instance);

			m_polygonVertexDistanceArrayProperty = m_serializedObject.FindProperty("m_polygonVertexDistanceArray");
		}

		private void _DrawPolygon()
		{
			_DrawPolygonSideCount();

			_CheckPolygonVertexArray();

			if(m_polygonVertexDistanceArrayProperty.arraySize > 0)
			{
				m_polygonArrayFoldout = EditorGUILayout.Foldout(m_polygonArrayFoldout,"Vertex Distance Array");

				if(m_polygonArrayFoldout)
				{
					EditorGUILayout.BeginVertical(EditorStyles.helpBox);

					for(var i=0;i<m_polygonVertexDistanceArrayProperty.arraySize;i++)
					{
						_DrawPolygonVertexDistance(i);
					}

					EditorGUILayout.EndVertical();
				}
			}
		}

		private void _DrawPolygonSideCount()
		{
			EditorGUI.BeginChangeCheck();

			var newSideCount = EditorGUILayout.IntSlider("Side Count",m_polygonSideCountProperty.intValue,Global.MIN_POLYGON_COUNT,Global.MAX_POLYGON_COUNT);

			if(EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(m_shapeDrawing,"Change Side Count");

				m_polygonSideCountInfo.SetValue(target,newSideCount);

				m_serializedObject.Update();
			}
		}

		private void _CheckPolygonVertexArray()
		{
			var sideCount = m_polygonSideCountProperty.intValue;
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

		private void _DrawPolygonVertexDistance(int index)
		{
			var distanceProperty = m_polygonVertexDistanceArrayProperty.GetArrayElementAtIndex(index);

			EditorGUI.BeginChangeCheck();

			var vertexName = $"Vertex {index+1}";
			var newDistance = EditorGUILayout.Slider(vertexName,distanceProperty.floatValue,0.0f,1.0f);

			if(EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(m_shapeDrawing,$"Change {vertexName} Distance");

				distanceProperty.floatValue = newDistance;
			}
		}
	}
}
#endif