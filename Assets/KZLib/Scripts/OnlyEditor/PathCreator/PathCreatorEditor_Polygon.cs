#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector.Editor;

namespace KZLib.EditorTools
{
	public partial class PathCreatorEditor : OdinEditor
	{
		private const int c_shapeCount = 3;

		private void _SetShapePathInput(Event currentEvent)
		{
			var handles = m_pathCreator.Handles;

			if(handles.Count < c_shapeCount)
			{
				return;
			}

			m_mouseOverHandleIndex = Global.InvalidIndex;

			for(var i=0;i<c_shapeCount;i++)
			{
				var radius = _GetHandleDiameter(m_anchorSize,handles[i])/2.0f;
				var position = handles[i].TransformPoint(m_pathCreator.transform,m_pathCreator.PathSpaceType);
	
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

						if(m_mouseOverHandleIndex != Global.InvalidIndex)
						{
							m_dragHandleIndex = m_selectedHandleIndex;
						}
					}
					break;

					case EventType.MouseDrag when m_dragHandleIndex != Global.InvalidIndex:
					{
						if(m_dragHandleIndex < 0 || m_dragHandleIndex >= handles.Count)
						{
							break;
						}

						var currentPosition = handles[m_dragHandleIndex];
						var newPosition = _GetMousePosition();

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
						m_dragHandleIndex = Global.InvalidIndex;
					}
					break;
				}
			}
		}

		private void _DrawLineInShape()
		{
			var cachedColor = Handles.color;

			Handles.color = m_normalLineColor;
			var pointArray = m_pathCreator.PointArray;

			for(var i=0;i<pointArray.Length-1;i++)
			{
				Handles.DrawLine(_ToWorld(pointArray[i+0]),_ToWorld(pointArray[i+1]));
			}

			if(pointArray.Length >= 2 && !pointArray[0].AreEqual(pointArray[^1]))
			{
				Handles.DrawLine(_ToWorld(pointArray[^1]),_ToWorld(pointArray[0]));
			}

			Handles.color = cachedColor;
		}

		private bool _IsShapeAnchor(int index)
		{
			return  index == 0;
		}
	}
}
#endif
