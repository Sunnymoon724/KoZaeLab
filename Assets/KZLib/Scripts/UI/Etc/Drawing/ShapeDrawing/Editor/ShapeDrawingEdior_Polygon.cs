#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace KZLib
{
	public partial class ShapeDrawingEditor : GraphicDrawingEditor
	{
		private bool m_polygonArrayFoldout = false;

		private void _DrawPolygon()
		{
			_DrawPolygonSideCount();

			var vertexDistanceCount = m_shapeDrawing.PolygonVertexDistanceCount;

			if(vertexDistanceCount < 1)
			{
				return;
			}

			m_polygonArrayFoldout = EditorGUILayout.Foldout(m_polygonArrayFoldout,"Vertex Distance Array");

			if(m_polygonArrayFoldout)
			{
				EditorGUILayout.BeginVertical(EditorStyles.helpBox);

				for(var i=0;i<vertexDistanceCount;i++)
				{
					EditorGUI.BeginChangeCheck();

					var vertexName = $"Vertex {i+1}";
					var oldDistance = m_shapeDrawing.GetPolygonVertexDistance(i);

					var newDistance = EditorGUILayout.Slider(vertexName,oldDistance,0.0f,1.0f);

					if(EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(m_shapeDrawing,$"Change {vertexName} Distance");

						m_shapeDrawing.SetPolygonVertexDistance(i,newDistance);
					}
				}

				EditorGUILayout.EndVertical();
			}
		}

		private void _DrawPolygonSideCount()
		{
			EditorGUI.BeginChangeCheck();

			var newSideCount = EditorGUILayout.IntSlider("Side Count",m_shapeDrawing.PolygonSideCount,Global.MIN_POLYGON_COUNT,Global.MAX_POLYGON_COUNT);

			if(EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(m_shapeDrawing,"Change Side Count");

				m_shapeDrawing.PolygonSideCount = newSideCount;

				m_serializedObject.Update();
			}
		}

		protected override void _DragMouse(int index,Vector3 position)
		{
			Undo.RecordObject(m_shapeDrawing,"Move Handle");

			if(index == 0)
			{
				CurrentTransform.position = position;
			}
			else
			{
				var centerPosition = CurrentTransform.position;
				var angle = m_shapeDrawing.PolygonSegmentAngle;
				var controlPosition = _GetPolygonVertexPosition(centerPosition,angle,m_shapeDrawing.Radius,index-1,1.0f);

				var controlDistance = Vector3.Distance(centerPosition,controlPosition);
				var newDistance = Vector3.Distance(centerPosition,position);

				var ratio = controlDistance == 0.0f ? 0.0f : Mathf.Clamp01(newDistance/controlDistance);

				m_shapeDrawing.SetPolygonVertexDistance(index-1,ratio);
			}

			Repaint();
		}

		protected override void _UpdateAnchorPositionList()
		{
			var anchorPosition = CurrentTransform.position;

			_AddOrUpdateHandlePosition(0,anchorPosition,true);
		}

		protected override void _UpdateControlPositionList()
		{
			var vertexDistanceCount = m_shapeDrawing.PolygonVertexDistanceCount;
			var anchorPosition = CurrentTransform.position;
			var angle = m_shapeDrawing.PolygonSegmentAngle;

			for(var i=0;i<vertexDistanceCount;i++)
			{
				var distance = m_shapeDrawing.GetPolygonVertexDistance(i);
				var controlPosition = _GetPolygonVertexPosition(anchorPosition,angle,m_shapeDrawing.Radius,i,distance);

				_AddOrUpdateHandlePosition(i+1,controlPosition,false);
			}
		}

		protected override Vector3 _ConvertToMousePosition(Vector3 mousePosition)
		{
			var ray = HandleUtility.GUIPointToWorldRay(mousePosition);
			var position = ray.GetPoint(10.0f);

			return new Vector3(position.x,position.y);
		}

		private Vector3 _GetPolygonVertexPosition(Vector2 center,float angle,Vector2 radius,int index,float distance)
		{
			var cos = Mathf.Cos(angle*index)*distance;
			var sin = Mathf.Sin(angle*index)*distance;

			return new Vector3(center.x+cos*radius.x,center.y+sin*radius.y);
		}
	}
}
#endif