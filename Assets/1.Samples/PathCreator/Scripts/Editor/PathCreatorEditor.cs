#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using KZLib.KZDevelop;
using Sirenix.OdinInspector.Editor;

namespace KZLib.KZEditor
{
	[CustomEditor(typeof(PathCreator))]
	public partial class PathCreatorEditor : OdinEditor
	{
		private const float SELECT_DISTANCE_THRESHOLD = 10.0f;

		private PathCreator m_Creator = null;

		private int m_SelectedSegmentIndex = -1;
		private int m_DraggingHandleIndex = -1;
		private int m_MouseOverHandleIndex = -1;

		private float m_AnchorSize = 10.0f;
		private Color m_AnchorNormalColor = Color.white;
		private Color m_AnchorHighlightColor = Color.white;
		private Color m_AnchorSelectColor = Color.white;

		protected override void OnEnable()
		{
			base.OnEnable();

			m_Creator = target as PathCreator;

			m_AnchorSize = EditorCustom.EditorPath.AnchorSize;
			m_AnchorNormalColor = EditorCustom.EditorPath.AnchorNormalColor;
			m_AnchorHighlightColor = EditorCustom.EditorPath.AnchorHighlightColor;
			m_AnchorSelectColor = EditorCustom.EditorPath.AnchorSelectColor;

			// data.bezierCreated -= ResetState;
			// data.bezierCreated += ResetState;
			// Undo.undoRedoPerformed -= OnUndoRedo;
			// Undo.undoRedoPerformed += OnUndoRedo;

			// LoadDisplaySettings ();
			// UpdateGlobalDisplaySettings ();
			// ResetState ();
			// SetTransformState (true);
		}

		private void OnSceneGUI()
		{
			var eventType = Event.current.type;

			using var check = new EditorGUI.ChangeCheckScope();

			if(eventType != EventType.Repaint && eventType != EventType.Layout)
			{
				SetPathInput(Event.current);
			}

			DrawPath();

			if(eventType == EventType.Layout)
			{
				HandleUtility.AddDefaultControl(0);
			}

			if (check.changed)
			{
				EditorApplication.QueuePlayerLoopUpdate();
			}
		}

		private void DrawPath()
		{
			var pointArray = m_Creator.AnchorArray;

			for(var i=0;i<pointArray.Length;i++)
			{
				DrawAnchor(i,pointArray[i]);
			}

			if(Event.current.type == EventType.Repaint)
			{
				DrawLine(pointArray);
			}
		}

		private void SetPathInput(Event _event)
		{
			var pointArray = m_Creator.AnchorArray;
			var mouseOverHandleIndex = (m_MouseOverHandleIndex == -1) ? 0 : m_MouseOverHandleIndex;

			m_MouseOverHandleIndex = -1;

			for(var i=0;i<pointArray.Length;i++)
			{
				var index = (mouseOverHandleIndex+i)%pointArray.Length;
				var radius = GetHandleDiameter(m_AnchorSize,pointArray[index])/2.0f;
				var position = Tools.TransformPoint(pointArray[index],m_Creator.transform,m_Creator.PathSpaceType);
	
				if(HandleUtility.DistanceToCircle(position,radius) == 0)
				{
					m_MouseOverHandleIndex = index;
					break;
				}
			}

			if(m_MouseOverHandleIndex == -1)
			{
				if(_event.type == EventType.MouseDown && _event.button == 0 && _event.shift)
				{
					// UpdatePathMouseInfo();

					// if(m_SelectedSegmentIndex != -1 && m_SelectedSegmentIndex < pointArray.Length)
					// {
					// 	var newPosition = Tools.InverseTransformPoint(pathMouseInfo.closestWorldPointToMouse,m_Creator.transform,m_Creator.PathSpaceType);

					// 	Undo.RecordObject(m_Creator,"Split Line");

					// 	m_Creator.SplitLine(newPosition,m_SelectedSegmentIndex);
					// }
					// else if(!m_Creator.IsClosed)
					{
						var distance = (Camera.current.transform.position-pointArray[^1]).magnitude;
						var newPosition = Tools.InverseTransformPoint(GetMouseWorldPosition(m_Creator.PathSpaceType,distance),m_Creator.transform,m_Creator.PathSpaceType);

						Undo.RecordObject(m_Creator,"Add Anchor");

						m_Creator.AddAnchor(newPosition);
					}
				}
			}
			else
			{
				if((_event.control || _event.command) && _event.type == EventType.MouseDown && _event.button == 0)
				{
					Undo.RecordObject(m_Creator,"Delete Anchor");

					m_Creator.RemoveAnchor(m_MouseOverHandleIndex);
					if(m_MouseOverHandleIndex == m_AnchorIndexToDisplayAsTransform)
					{
						m_AnchorIndexToDisplayAsTransform = -1;
					}
					m_MouseOverHandleIndex = -1;
					Repaint ();
				}
			}

			// if(m_DraggingHandleIndex == -1 && m_MouseOverHandleIndex == -1)
			// {
			// 	if(_event.type == EventType.MouseDrag && _event.shift)
			// 	{
			// 		UpdatePathMouseInfo();

			// 		if(pathMouseInfo.mouseDstToLine < SELECT_DISTANCE_THRESHOLD)
			// 		{
			// 			if(pathMouseInfo.closestSegmentIndex != m_SelectedSegmentIndex)
			// 			{
			// 				m_SelectedSegmentIndex = pathMouseInfo.closestSegmentIndex;
			// 				HandleUtility.Repaint();
			// 			}
			// 		}
			// 		else
			// 		{
			// 			m_SelectedSegmentIndex = -1;
			// 			HandleUtility.Repaint();
			// 		}

			// 		// var nearByAnchor = HandleUtility.DistanceToCircle(anchorPosition,anchorRadius+extraInputRadius) == 0.0f;

			// 	}
			// }
		}
	}
}
#endif