#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace KZLib.UI
{
	public partial class ShapeDrawingEditor : GraphicDrawingEditor
	{
		private bool m_isPolygonArrayExpanded = false;

		private bool _CanShowKnot_Polygon()
		{
			return m_shapeDrawing.PolygonVertexDistanceCount >= Global.MIN_POLYGON_COUNT;
		}

		private void _Draw_Polygon()
		{
			_DrawPolygonSideCount();

			var vertexDistCnt = m_shapeDrawing.PolygonVertexDistanceCount;

			if(vertexDistCnt < 1)
			{
				return;
			}

			m_isPolygonArrayExpanded = EditorGUILayout.Foldout(m_isPolygonArrayExpanded,"Vertex Distance Array");

			if(m_isPolygonArrayExpanded)
			{
				EditorGUI.indentLevel++;

				for(var i=0;i<vertexDistCnt;i++)
				{
					EditorGUI.BeginChangeCheck();

					var vertexName = $"Vertex {i+1}";

					var oldDist = m_shapeDrawing.GetPolygonVertexDistance(i);
					var newDist = EditorGUILayout.Slider(vertexName,oldDist,0.0f,1.0f);

					if(EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(m_shapeDrawing,$"Change {vertexName} Distance");

						m_shapeDrawing.SetPolygonVertexDistance(i,newDist);

						EditorUtility.SetDirty(m_shapeDrawing);
					}
				}

				EditorGUI.indentLevel--;
			}
		}

		private void _UpdateKnotList_Polygon(Vector3 position,Quaternion rotation)
		{
			for(var i=0;i<m_shapeDrawing.PolygonVertexDistanceCount;i++)
			{
				var edgePos = _GetPolygonEdgePosition(i);
				var localPos = edgePos*m_shapeDrawing.GetPolygonVertexDistance(i);
				var controlPos = position+rotation*localPos;

				_AddOrUpdateKnotInfo(i+1,controlPos,KnotType.Major);
			}
		}

		private void _ChangeKnotPosition_Polygon(int index,Vector3 localPosition)
		{
			var edgePos = _GetPolygonEdgePosition(index-1);
			var projectedDist = Vector3.Dot(localPosition,edgePos.normalized);
			var ratio = Mathf.Clamp01(projectedDist/edgePos.magnitude);
			m_shapeDrawing.SetPolygonVertexDistance(index-1,ratio);
		}

		private void _DrawPolygonSideCount()
		{
			EditorGUI.BeginChangeCheck();

			var newSideCnt = EditorGUILayout.IntSlider("Side Count",m_shapeDrawing.PolygonSideCount,Global.MIN_POLYGON_COUNT,Global.MAX_POLYGON_COUNT);

			if(EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(m_shapeDrawing,"Change Side Count");

				m_shapeDrawing.PolygonSideCount = newSideCnt;

				m_serializedObject.Update();
			}
		}

		private Vector3 _GetPolygonEdgePosition(int index)
		{
			return _GetEdgePosition(m_shapeDrawing.PolygonSegmentAngle*index);
		}
	}
}
#endif