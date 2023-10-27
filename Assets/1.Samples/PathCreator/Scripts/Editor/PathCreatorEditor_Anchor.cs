#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector.Editor;

namespace KZLib.KZEditor
{
	public partial class PathCreatorEditor : OdinEditor
	{
		private const float extraInputRadius = 0.005f;

		private int m_AnchorIndexToDisplayAsTransform = -1;

		private int m_SelectedAnchorId = -1;
		private bool m_MouseIsInAnchor = false;

		private float m_DragStartPoint = float.MaxValue;

		private Vector2 m_AnchorDragStart = Vector2.zero;
		private Vector2 m_AnchorDragEnd = Vector2.zero;
		private Vector3 m_AnchorWorldPoint = Vector2.zero;

		private void DrawAnchor(int _index,Vector3 _position)
		{
			var changed = false;
			var isSelectedAnchor =_index == m_AnchorIndexToDisplayAsTransform;
			var anchorPosition = Tools.TransformPoint(_position,m_Creator.transform,m_Creator.PathSpaceType);
			var anchorDiameter = GetHandleDiameter(m_AnchorSize,_position);
			var anchorId = GetAnchorId(_index);
			var screenPosition = Handles.matrix.MultiplyPoint(_position);
			var cachedMatrix = Handles.matrix;
			var eventType = Event.current.GetTypeForControl(anchorId);
			var anchorRadius = anchorDiameter/2.0f;
			var nearByAnchor = HandleUtility.DistanceToCircle(anchorPosition,anchorRadius+extraInputRadius) == 0.0f;
			var distanceToMouse = HandleUtility.DistanceToCircle(anchorPosition,0.0f);

			if(nearByAnchor)
			{
				if(!m_MouseIsInAnchor)
				{
					HandleUtility.Repaint();
					m_MouseIsInAnchor = true;
				}
			}
			else if(m_MouseIsInAnchor)
			{
				HandleUtility.Repaint();
				m_MouseIsInAnchor = false;
			}

			switch(eventType)
			{
				case EventType.MouseDown when Event.current.button == 0 && nearByAnchor && distanceToMouse < m_DragStartPoint:
				{
					m_DragStartPoint = distanceToMouse;
					GUIUtility.hotControl = anchorId;
					m_AnchorDragEnd = m_AnchorDragStart = Event.current.mousePosition;
					m_AnchorWorldPoint = anchorPosition;
					m_SelectedAnchorId = anchorId;

					if(m_AnchorIndexToDisplayAsTransform != _index)
					{
						m_AnchorIndexToDisplayAsTransform = -1;
						changed = true;
					}

					break;
				}

				case EventType.MouseUp:
				{
					m_DragStartPoint = float.MaxValue;

					if(GUIUtility.hotControl == anchorId && Event.current.button == 0)
					{
						GUIUtility.hotControl = 0;
						m_SelectedAnchorId = -1;
						Event.current.Use();

						m_DraggingHandleIndex = -1;

						m_AnchorIndexToDisplayAsTransform = (Event.current.mousePosition == m_AnchorDragStart) ? (Event.current.shift ? -1 : (m_AnchorIndexToDisplayAsTransform == _index ? -1 : _index)) : -1;

						changed = true;
					}
					break;
				}
				case EventType.MouseDrag when GUIUtility.hotControl == anchorId && Event.current.button == 0:
				{
					m_AnchorDragEnd += new Vector2(Event.current.delta.x, -Event.current.delta.y);

					m_DraggingHandleIndex = _index;
					m_AnchorIndexToDisplayAsTransform = -1;
					changed = true;

					anchorPosition = m_Creator.PathSpaceType == SpaceType.xyz ? Handles.matrix.inverse.MultiplyPoint(Camera.current.ScreenToWorldPoint(Camera.current.WorldToScreenPoint(Handles.matrix.MultiplyPoint(m_AnchorWorldPoint)) + (Vector3)(m_AnchorDragEnd - m_AnchorDragStart))) : GetMouseWorldPosition(m_Creator.PathSpaceType);

					GUI.changed = true;
					Event.current.Use();
					break;
				}
			}

			if(eventType == EventType.Repaint)
			{
				var cachedColor = Handles.color;

				Handles.matrix = Matrix4x4.identity;
				Handles.color = (anchorId == GUIUtility.hotControl || isSelectedAnchor) ? m_AnchorSelectColor : (nearByAnchor && m_SelectedAnchorId == -1) ? m_AnchorHighlightColor : m_AnchorNormalColor;

				Handles.SphereHandleCap(anchorId,anchorPosition,Quaternion.LookRotation(Vector3.up),anchorDiameter,EventType.Repaint);

				Handles.color = Color.white;
				Handles.Label(anchorPosition+Tools.TransformPoint(Vector3.one*0.2f,m_Creator.transform,m_Creator.PathSpaceType),string.Format("{0}",_index));

				Handles.matrix = cachedMatrix;
				Handles.color = cachedColor;
			}
			else if(eventType == EventType.Layout)
			{
				Handles.matrix = Matrix4x4.identity;
				HandleUtility.AddControl(anchorId,HandleUtility.DistanceToCircle(screenPosition, anchorRadius));
				Handles.matrix = cachedMatrix;
			}

			if(isSelectedAnchor)
			{
				anchorPosition = Handles.DoPositionHandle(anchorPosition,Quaternion.identity);
			}

			if(changed)
			{
				Repaint();
			}

			var position = Tools.InverseTransformPoint(anchorPosition,m_Creator.transform,m_Creator.PathSpaceType);

			if(_position != position)
			{
				Undo.RecordObject(m_Creator,"Move Anchor");

				m_Creator.MoveAnchor(_index,position);
			}
		}

		private float GetHandleDiameter(float _diameter,Vector3 _position)
		{
			return _diameter*0.01f*HandleUtility.GetHandleSize(_position)*2.5f;
		}

		private int GetAnchorId(int _index)
		{
			var hash = string.Format("Anchor_{0}",_index);
			return GUIUtility.GetControlID(hash.GetHashCode(),FocusType.Passive);
		}
	}

}
#endif