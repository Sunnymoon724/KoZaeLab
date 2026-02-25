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
			return m_shapeDrawing.PolygonVertexDistanceCount >= ShapeDrawing.c_minPolygonCount;
		}

		private void _Draw_Polygon()
		{
			void _SetSideCount(int count)
			{
				m_shapeDrawing.PolygonSideCount = count;
			}

			_DrawIntSliderInspector("Side Count",m_shapeDrawing.PolygonSideCount,ShapeDrawing.c_minPolygonCount,ShapeDrawing.c_maxPolygonCount,_SetSideCount);

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
					void _SetVertexDistance(float distance)
					{
						m_shapeDrawing.SetPolygonVertexDistance(i,distance);
					}

					_DrawSliderInspector($"Vertex {i+1}",m_shapeDrawing.GetPolygonVertexDistance(i),0.0f,1.0f,_SetVertexDistance);
				}

				EditorGUI.indentLevel--;
			}
		}

		private void _SyncAllKnotInfos_Polygon()
		{
			for(var i=0;i<m_shapeDrawing.PolygonVertexDistanceCount;i++)
			{
				var vertPos = _GetPolygonVertexPosition(i)*m_shapeDrawing.GetPolygonVertexDistance(i);

				_SyncWorldKnotInfo(i+1,vertPos,KnotType.Major);
			}
		}

		private void _RefreshKnotInfo_Polygon(int index,Vector3 position)
		{
			var vertPos = _GetPolygonVertexPosition(index-1);
			var projected = Vector3.Dot(position,vertPos.normalized);
			var ratio = Mathf.Clamp01(projected/vertPos.magnitude);

			m_shapeDrawing.SetPolygonVertexDistance(index-1,ratio);
		}

		private Vector3 _GetPolygonVertexPosition(int index)
		{
			var angle = ShapeDrawing.c_fullAngle/m_shapeDrawing.PolygonSideCount;

			return _GetVertexPosition(angle*index);
		}
	}
}
#endif