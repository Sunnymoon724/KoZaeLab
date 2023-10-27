#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector.Editor;

namespace KZLib.KZEditor
{
	public partial class PathCreatorEditor : OdinEditor
	{
		private const float extraInputRadius = 0.005f;

		private int m_HandleIndexToDisplayAsTransform = -1;

		private int m_SelectedHandleId = -1;
		private bool m_MouseIsInHandle = false;

		private float m_DragStartPoint = float.MaxValue;

		private Vector2 m_HandleDragStart = Vector2.zero;
		private Vector2 m_HandleDragEnd = Vector2.zero;
		private Vector3 m_HandleWorldPoint = Vector2.zero;

		private void DrawHandle(int _index,Vector3 _position,bool _isAnchor)
		{
			var changed = false;
			var isSelectedHandle =_index == m_HandleIndexToDisplayAsTransform;
			var handlePosition = Tools.TransformPoint(_position,m_Creator.transform,m_Creator.PathSpaceType);
			var diameter = GetHandleDiameter(_isAnchor ? m_AnchorSize : m_ControlSize,_position);
			var handleId = GetHandleId(_index);
			var screenPosition = Handles.matrix.MultiplyPoint(_position);
			var cachedMatrix = Handles.matrix;
			var eventType = Event.current.GetTypeForControl(handleId);
			var radius = diameter/2.0f;
			var nearByHandle = HandleUtility.DistanceToCircle(handlePosition,radius+extraInputRadius) == 0.0f;
			var distanceToMouse = HandleUtility.DistanceToCircle(handlePosition,0.0f);

			if(nearByHandle)
			{
				if(!m_MouseIsInHandle)
				{
					HandleUtility.Repaint();
					m_MouseIsInHandle = true;
				}
			}
			else if(m_MouseIsInHandle)
			{
				HandleUtility.Repaint();
				m_MouseIsInHandle = false;
			}

			switch(eventType)
			{
				case EventType.MouseDown when Event.current.button == 0 && nearByHandle && distanceToMouse < m_DragStartPoint:
				{
					m_DragStartPoint = distanceToMouse;
					GUIUtility.hotControl = handleId;
					m_HandleDragEnd = m_HandleDragStart = Event.current.mousePosition;
					m_HandleWorldPoint = handlePosition;
					m_SelectedHandleId = handleId;

					if(m_HandleIndexToDisplayAsTransform != _index)
					{
						m_HandleIndexToDisplayAsTransform = -1;
						changed = true;
					}

					break;
				}

				case EventType.MouseUp:
				{
					m_DragStartPoint = float.MaxValue;

					if(GUIUtility.hotControl == handleId && Event.current.button == 0)
					{
						GUIUtility.hotControl = 0;
						m_SelectedHandleId = -1;
						Event.current.Use();

						m_DraggingHandleIndex = -1;

						m_HandleIndexToDisplayAsTransform = (Event.current.mousePosition == m_HandleDragStart) ? (Event.current.shift ? -1 : (m_HandleIndexToDisplayAsTransform == _index ? -1 : _index)) : -1;

						changed = true;
					}
					break;
				}
				case EventType.MouseDrag when GUIUtility.hotControl == handleId && Event.current.button == 0:
				{
					m_HandleDragEnd += new Vector2(Event.current.delta.x, -Event.current.delta.y);

					m_DraggingHandleIndex = _index;
					m_HandleIndexToDisplayAsTransform = -1;
					changed = true;

					handlePosition = m_Creator.PathSpaceType == SpaceType.xyz ? Handles.matrix.inverse.MultiplyPoint(Camera.current.ScreenToWorldPoint(Camera.current.WorldToScreenPoint(Handles.matrix.MultiplyPoint(m_HandleWorldPoint)) + (Vector3)(m_HandleDragEnd - m_HandleDragStart))) : GetMouseWorldPosition(m_Creator.PathSpaceType);

					GUI.changed = true;
					Event.current.Use();
					break;
				}
			}

			if(eventType == EventType.Repaint)
			{
				var cachedColor = Handles.color;

				Handles.matrix = Matrix4x4.identity;

				var highlight = _isAnchor ? m_AnchorHighlightColor : m_ControlHighlightColor;
				var normal = _isAnchor ? m_AnchorNormalColor : m_ControlNormalColor;

				Handles.color = (handleId == GUIUtility.hotControl || isSelectedHandle) ? m_HandleSelectColor : (nearByHandle && m_SelectedHandleId == -1) ? highlight : normal;

				Handles.SphereHandleCap(handleId,handlePosition,Quaternion.LookRotation(Vector3.up),diameter,EventType.Repaint);

				Handles.color = Color.white;
				Handles.Label(handlePosition+Tools.TransformPoint(Vector3.one*0.2f,m_Creator.transform,m_Creator.PathSpaceType),string.Format("{0}",_index));

				Handles.matrix = cachedMatrix;
				Handles.color = cachedColor;
			}
			else if(eventType == EventType.Layout)
			{
				Handles.matrix = Matrix4x4.identity;
				HandleUtility.AddControl(handleId,HandleUtility.DistanceToCircle(screenPosition,radius));
				Handles.matrix = cachedMatrix;
			}

			if(changed)
			{
				Repaint();
			}

			var position = Tools.InverseTransformPoint(handlePosition,m_Creator.transform,m_Creator.PathSpaceType);

			if(_position != position)
			{
				Undo.RecordObject(m_Creator,"Move Handle");

				m_Creator.MoveHandle(_index,position);
			}
		}

		private float GetHandleDiameter(float _diameter,Vector3 _position)
		{
			return _diameter*0.01f*HandleUtility.GetHandleSize(_position)*2.5f;
		}

		private int GetHandleId(int _index)
		{
			var hash = string.Format("Handle_{0}",_index);
			return GUIUtility.GetControlID(hash.GetHashCode(),FocusType.Passive);
		}
	}

}
#endif