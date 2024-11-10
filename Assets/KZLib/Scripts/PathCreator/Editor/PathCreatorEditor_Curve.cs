#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector.Editor;

namespace KZLib.KZEditor
{
	public partial class PathCreatorEditor : OdinEditor
	{
		private void SetCurvePathInput(Event _event)
		{
			var handleArray = m_Creator.HandleArray;
			var handleIndex = (m_MouseOverHandleIndex == Global.INVALID_INDEX) ? 0 : m_MouseOverHandleIndex;

			m_MouseOverHandleIndex = Global.INVALID_INDEX;

			for(var i=0;i<handleArray.Length;i++)
			{
				var index = (handleIndex+i)%handleArray.Length;
				var radius = GetHandleDiameter(m_AnchorSize,handleArray[index])/2.0f;
				var position = handleArray[index].TransformPoint(m_Creator.transform,m_Creator.PathSpaceType);
	
				if(HandleUtility.DistanceToCircle(position,radius) == 0.0f)
				{
					m_MouseOverHandleIndex = index;
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
						//? Add
						if(_event.shift)
						{
							var distance = (Camera.current.transform.position-handleArray[^1]).magnitude;
							var newPosition = GetMousePosition(m_Creator,distance);

							Undo.RecordObject(m_Creator,"Add Anchor");

							if(m_SelectedHandleIndex == -1)
							{
								m_Creator.AddAnchor(newPosition);
							}
							else
							{
								m_Creator.InsertAnchor(m_SelectedHandleIndex,newPosition);
							}
						}
						//? Delete
						else if(m_MouseOverHandleIndex != Global.INVALID_INDEX && (_event.control || _event.command))
						{
							Undo.RecordObject(m_Creator,"Delete Anchor");

							m_Creator.RemoveAnchor(m_MouseOverHandleIndex);

							if(m_MouseOverHandleIndex == m_SelectedHandleIndex)
							{
								m_SelectedHandleIndex = Global.INVALID_INDEX;
							}

							m_MouseOverHandleIndex = Global.INVALID_INDEX;
						}
						//? Select
						else
						{
							m_SelectedHandleIndex = m_MouseOverHandleIndex;

							if(m_MouseOverHandleIndex != Global.INVALID_INDEX)
							{
								m_DragHandleIndex = m_SelectedHandleIndex;
							}
						}
					}
					break;

					case EventType.MouseDrag when m_DragHandleIndex != Global.INVALID_INDEX:
					{
						var currentPosition = handleArray[m_DragHandleIndex];
						var newPosition = GetMousePosition(m_Creator);

						if(currentPosition != newPosition)
						{
							Undo.RecordObject(m_Creator,"Move Handle");

							m_Creator.MoveCurve(m_DragHandleIndex,newPosition,_event.capsLock);

							Repaint();
						}
					}
					break;
					case EventType.MouseUp:
					{
						m_DragHandleIndex = Global.INVALID_INDEX;
					}
					break;
				}
			}
		}

		private void DrawLineInCurve(Vector3[] _handleArray)
		{
			var cachedColor = Handles.color;

			Handles.color = m_GuideLineColor;
			var length = m_Creator.IsClosed ? _handleArray.Length/3 : (_handleArray.Length-1)/3;

			for(var i=0;i<length;i++)
			{
				Handles.DrawDottedLine(_handleArray[i*3+0],_handleArray[i*3+1],5.0f);
				Handles.DrawDottedLine(_handleArray[i*3+2],_handleArray[CommonUtility.LoopClamp(i*3+3,_handleArray.Length)],5.0f);
			}

			Handles.color = m_NormalLineColor;
			var pointArray = m_Creator.PointArray;

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