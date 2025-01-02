#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector.Editor;

namespace KZLib.KZDevelop
{
	public partial class PathCreatorEditor : OdinEditor
	{
		private void SetCurvePathInput(Event currentEvent)
		{
			var handleArray = m_pathCreator.HandleArray;
			var handleIndex = (m_mouseOverHandleIndex == Global.INVALID_INDEX) ? 0 : m_mouseOverHandleIndex;

			m_mouseOverHandleIndex = Global.INVALID_INDEX;

			for(var i=0;i<handleArray.Length;i++)
			{
				var index = (handleIndex+i)%handleArray.Length;
				var radius = GetHandleDiameter(m_anchorSize,handleArray[index])/2.0f;
				var position = handleArray[index].TransformPoint(m_pathCreator.transform,m_pathCreator.PathSpaceType);
	
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
						//? Add
						if(currentEvent.shift)
						{
							var distance = (Camera.current.transform.position-handleArray[^1]).magnitude;
							var newPosition = GetMousePosition(distance);

							Undo.RecordObject(m_pathCreator,"Add Anchor");

							if(m_selectedHandleIndex == -1)
							{
								m_pathCreator.AddAnchor(newPosition);
							}
							else
							{
								m_pathCreator.InsertAnchor(m_selectedHandleIndex,newPosition);
							}
						}
						//? Delete
						else if(m_mouseOverHandleIndex != Global.INVALID_INDEX && (currentEvent.control || currentEvent.command))
						{
							Undo.RecordObject(m_pathCreator,"Delete Anchor");

							m_pathCreator.RemoveAnchor(m_mouseOverHandleIndex);

							if(m_mouseOverHandleIndex == m_selectedHandleIndex)
							{
								m_selectedHandleIndex = Global.INVALID_INDEX;
							}

							m_mouseOverHandleIndex = Global.INVALID_INDEX;
						}
						//? Select
						else
						{
							m_selectedHandleIndex = m_mouseOverHandleIndex;

							if(m_mouseOverHandleIndex != Global.INVALID_INDEX)
							{
								m_dragHandleIndex = m_selectedHandleIndex;
							}
						}
					}
					break;

					case EventType.MouseDrag when m_dragHandleIndex != Global.INVALID_INDEX:
					{
						var currentPosition = handleArray[m_dragHandleIndex];
						var newPosition = GetMousePosition();

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
						m_dragHandleIndex = Global.INVALID_INDEX;
					}
					break;
				}
			}
		}

		private void DrawLineInCurve(Vector3[] handleArray)
		{
			var cachedColor = Handles.color;

			Handles.color = m_guideLineColor;
			var length = m_pathCreator.IsClosed ? handleArray.Length/3 : (handleArray.Length-1)/3;

			for(var i=0;i<length;i++)
			{
				Handles.DrawDottedLine(handleArray[i*3+0],handleArray[i*3+1],5.0f);
				Handles.DrawDottedLine(handleArray[i*3+2],handleArray[CommonUtility.LoopClamp(i*3+3,handleArray.Length)],5.0f);
			}

			Handles.color = m_normalLineColor;
			var pointArray = m_pathCreator.PointArray;

			for(var i=0;i<pointArray.Length-1;i++)
			{
				Handles.DrawLine(pointArray[i+0],pointArray[i+1]);
			}

			Handles.color = cachedColor;
		}

		private bool IsCurveAnchor(int _index)
		{
			return  _index%3 == 0;
		}
	}
}
#endif