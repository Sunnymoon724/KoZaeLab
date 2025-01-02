#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector.Editor;

namespace KZLib.KZDevelop
{
	public partial class PathCreatorEditor : OdinEditor
	{
		private const int c_shape_count = 3;

		private void SetShapePathInput(Event currentEvent)
		{
			var handleArray = m_pathCreator.HandleArray;

			m_mouseOverHandleIndex = Global.INVALID_INDEX;

			for(var i=0;i<c_shape_count;i++)
			{
				var radius = GetHandleDiameter(m_anchorSize,handleArray[i])/2.0f;
				var position = handleArray[i].TransformPoint(m_pathCreator.transform,m_pathCreator.PathSpaceType);
	
				if(HandleUtility.DistanceToCircle(position,radius) == 0.0f)
				{
					m_mouseOverHandleIndex = i;
					break;
				}
			}

			HandleUtility.Repaint();

			if(currentEvent.button == 0)
			{
				switch(currentEvent.type)
				{
					case EventType.MouseDown:
					{
						m_selectedHandleIndex = m_mouseOverHandleIndex;

						if(m_mouseOverHandleIndex != Global.INVALID_INDEX)
						{
							m_dragHandleIndex = m_selectedHandleIndex;
						}
					}
					break;

					case EventType.MouseDrag when m_dragHandleIndex != Global.INVALID_INDEX:
					{
						var currentPosition = handleArray[m_dragHandleIndex];
						var newPosition = GetMousePosition();

						if(currentPosition != newPosition)
						{
							Undo.RecordObject(m_pathCreator,"Move Center");

							m_pathCreator.MovePolygon(m_dragHandleIndex,newPosition);

							Repaint();
						}
					}
					break;
					case EventType.MouseUp:
					{
						m_dragHandleIndex = -1;
					}
					break;
				}
			}
		}

		private void DrawLineInShape()
		{
			var cachedColor = Handles.color;

			Handles.color = m_normalLineColor;
			var pointArray = m_pathCreator.PointArray;

			for(var i=0;i<pointArray.Length-1;i++)
			{
				Handles.DrawLine(pointArray[i+0],pointArray[i+1]);
			}

			Handles.DrawLine(pointArray[^1],pointArray[0]);

			Handles.color = cachedColor;
		}

		private bool IsShapeAnchor(int index)
		{
			return  index == 0;
		}
	}
}
#endif