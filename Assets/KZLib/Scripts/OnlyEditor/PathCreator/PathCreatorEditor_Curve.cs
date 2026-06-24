#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector.Editor;

namespace KZLib.EditorTools
{
	public partial class PathCreatorEditor : OdinEditor
	{
		private void _SetCurvePathInput(Event currentEvent)
		{
			var handles = m_pathCreator.Handles;
			var handleIndex = (m_mouseOverHandleIndex == Global.InvalidIndex) ? 0 : m_mouseOverHandleIndex;

			m_mouseOverHandleIndex = Global.InvalidIndex;

			for(var i=0;i<handles.Count;i++)
			{
				var index = (handleIndex+i)%handles.Count;
				var radius = _GetHandleDiameter(m_anchorSize,handles[index])/2.0f;
				var position = handles[index].TransformPoint(m_pathCreator.transform,m_pathCreator.PathSpaceType);
	
				if(HandleUtility.DistanceToCircle(position,radius) == 0.0f)
				{
					m_mouseOverHandleIndex = index;
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
						if(currentEvent.shift)
						{
							if(handles.Count <= 0)
							{
								break;
							}

							var distance = (Camera.current.transform.position-handles[handles.Count-1]).magnitude;
							var newPosition = _GetMousePosition(distance);

							Undo.RecordObject(m_pathCreator,"Add Anchor");

							if(m_selectedHandleIndex == Global.InvalidIndex)
							{
								m_pathCreator.AddAnchor(newPosition);
							}
							else
							{
								m_pathCreator.InsertAnchor(m_selectedHandleIndex,newPosition);
							}
						}
						else if(m_mouseOverHandleIndex != Global.InvalidIndex && (currentEvent.control || currentEvent.command))
						{
							Undo.RecordObject(m_pathCreator,"Delete Anchor");

							m_pathCreator.RemoveAnchor(m_mouseOverHandleIndex);

							if(m_mouseOverHandleIndex == m_selectedHandleIndex)
							{
								m_selectedHandleIndex = Global.InvalidIndex;
							}

							m_mouseOverHandleIndex = Global.InvalidIndex;
						}
						else
						{
							m_selectedHandleIndex = m_mouseOverHandleIndex;

							if(m_mouseOverHandleIndex != Global.InvalidIndex)
							{
								m_dragHandleIndex = m_selectedHandleIndex;
							}
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
							Undo.RecordObject(m_pathCreator,"Move Handle");

							m_pathCreator.MoveCurve(m_dragHandleIndex,newPosition,currentEvent.capsLock);

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

		private void _DrawLineInCurve()
		{
			var handles = m_pathCreator.Handles;
			var cachedColor = Handles.color;

			Handles.color = m_guideLineColor;
			var length = m_pathCreator.IsClosed ? handles.Count/3 : (handles.Count-1)/3;

			for(var i=0;i<length;i++)
			{
				Handles.DrawDottedLine(_ToWorld(handles[i*3+0]),_ToWorld(handles[i*3+1]),5.0f);
				Handles.DrawDottedLine(_ToWorld(handles[i*3+2]),_ToWorld(handles[KZMathKit.LoopClamp(i*3+3,handles.Count)]),5.0f);
			}

			Handles.color = m_normalLineColor;
			var pointArray = m_pathCreator.PointArray;

			for(var i=0;i<pointArray.Length-1;i++)
			{
				Handles.DrawLine(_ToWorld(pointArray[i+0]),_ToWorld(pointArray[i+1]));
			}

			if(m_pathCreator.IsClosed && pointArray.Length >= 2 && !pointArray[0].AreEqual(pointArray[^1]))
			{
				Handles.DrawLine(_ToWorld(pointArray[^1]),_ToWorld(pointArray[0]));
			}

			Handles.color = cachedColor;
		}

		private bool _IsCurveAnchor(int index)
		{
			return  index%3 == 0;
		}
	}
}
#endif