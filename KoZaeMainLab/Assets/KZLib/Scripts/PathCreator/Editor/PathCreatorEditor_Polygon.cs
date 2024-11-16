#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector.Editor;

namespace KZLib.KZEditor
{
	public partial class PathCreatorEditor : OdinEditor
	{
		private const int SHAPE_COUNT = 3;

		private void SetShapePathInput(Event _event)
		{
			var handleArray = m_Creator.HandleArray;

			m_MouseOverHandleIndex = Global.INVALID_INDEX;

			for(var i=0;i<SHAPE_COUNT;i++)
			{
				var radius = GetHandleDiameter(m_AnchorSize,handleArray[i])/2.0f;
				var position = handleArray[i].TransformPoint(m_Creator.transform,m_Creator.PathSpaceType);
	
				if(HandleUtility.DistanceToCircle(position,radius) == 0.0f)
				{
					m_MouseOverHandleIndex = i;
					break;
				}
			}

			HandleUtility.Repaint();

			if(_event.button == 0)
			{
				switch(_event.type)
				{
					case EventType.MouseDown:
					{
						m_SelectedHandleIndex = m_MouseOverHandleIndex;

						if(m_MouseOverHandleIndex != Global.INVALID_INDEX)
						{
							m_DragHandleIndex = m_SelectedHandleIndex;
						}
					}
					break;

					case EventType.MouseDrag when m_DragHandleIndex != Global.INVALID_INDEX:
					{
						var currentPosition = handleArray[m_DragHandleIndex];
						var newPosition = GetMousePosition(m_Creator);

						if(currentPosition != newPosition)
						{
							Undo.RecordObject(m_Creator,"Move Center");

							m_Creator.MovePolygon(m_DragHandleIndex,newPosition);

							Repaint();
						}
					}
					break;
					case EventType.MouseUp:
					{
						m_DragHandleIndex = -1;
					}
					break;
				}
			}
		}

		private void DrawLineInShape(Vector3[] _)
		{
			var cachedColor = Handles.color;

			Handles.color = m_NormalLineColor;
			var pointArray = m_Creator.PointArray;

			for(var i=0;i<pointArray.Length-1;i++)
			{
				Handles.DrawLine(pointArray[i+0],pointArray[i+1]);
			}

			Handles.DrawLine(pointArray[^1],pointArray[0]);

			Handles.color = cachedColor;
		}

		private bool IsShapeAnchor(int _index)
		{
			return  _index == 0;
		}
	}
}
#endif