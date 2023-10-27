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
		private PathCreator m_Creator = null;

		private Color m_HandleSelectColor = Color.yellow;

		private float m_AnchorSize = 10.0f;
		private Color m_AnchorNormalColor = Color.red;
		private Color m_AnchorHighlightColor = Color.red;

		private float m_ControlSize = 10.0f;
		private Color m_ControlNormalColor = Color.blue;
		private Color m_ControlHighlightColor = Color.blue;

		private Color m_NormalLineColor = Color.green;
		private Color m_GuideLineColor = Color.white;

		protected override void OnEnable()
		{
			base.OnEnable();

			m_Creator = target as PathCreator;

			m_HandleSelectColor = EditorCustom.EditorPath.HandleSelectColor;

			m_AnchorSize = EditorCustom.EditorPath.AnchorSize;
			m_AnchorNormalColor = EditorCustom.EditorPath.AnchorNormalColor;
			m_AnchorHighlightColor = EditorCustom.EditorPath.AnchorHighlightColor;

			m_ControlSize = EditorCustom.EditorPath.ControlSize;
			m_ControlNormalColor = EditorCustom.EditorPath.ControlNormalColor;
			m_ControlHighlightColor = EditorCustom.EditorPath.ControlHighlightColor;

			m_NormalLineColor = EditorCustom.EditorPath.NormalLineColor;
			m_GuideLineColor = EditorCustom.EditorPath.GuideLineColor;

			m_Creator.OnChangedPath += OnResetState;

			Undo.undoRedoPerformed -= OnUndoRedo;
			Undo.undoRedoPerformed += OnUndoRedo;

			OnResetState();
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			UnityEditor.Tools.hidden = false;
		}

		private void OnUndoRedo()
		{
			m_Creator.ClearPath();

			Repaint();
		}

		private void OnResetState()
		{
			m_MouseOverHandleIndex = -1;
			m_HandleIndexToDisplayAsTransform = -1;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			UnityEditor.Tools.hidden = true;
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

			if(check.changed)
			{
				EditorApplication.QueuePlayerLoopUpdate();
			}
		}

		private void DrawPath()
		{
			var pointArray = m_Creator.HandleArray;

			if(m_Creator.IsSplineCurve)
			{
				for(var i=0;i<pointArray.Length;i++)
				{
					DrawHandle(i,pointArray[i],true);
				}
			}
			else
			{
				for(var i=0;i<pointArray.Length;i+=3)
				{
					DrawHandle(i,pointArray[i],true);
				}

				for(var i=1;i<pointArray.Length-1;i+=3)
				{
					DrawHandle(i,pointArray[i],false);
					DrawHandle(i+1,pointArray[i+1],false);
				}
			}

			if(Event.current.type == EventType.Repaint)
			{
				DrawLine(pointArray);
			}
		}

		private void SetPathInput(Event _event)
		{
			var handleArray = m_Creator.HandleArray;
			var handleIndex = (m_MouseOverHandleIndex == -1) ? 0 : m_MouseOverHandleIndex;

			m_MouseOverHandleIndex = -1;

			for(var i=0;i<handleArray.Length;i++)
			{
				var index = (handleIndex+i)%handleArray.Length;
				var radius = GetHandleDiameter(m_AnchorSize,handleArray[index])/2.0f;
				var position = Tools.TransformPoint(handleArray[index],m_Creator.transform,m_Creator.PathSpaceType);
	
				if(HandleUtility.DistanceToCircle(position,radius) == 0.0f)
				{
					m_MouseOverHandleIndex = index;
					break;
				}
			}

			if(m_MouseOverHandleIndex == -1)
			{
				if(_event.type == EventType.MouseDown && _event.button == 0 && _event.shift)
				{
					var distance = (Camera.current.transform.position-handleArray[^1]).magnitude;
					var newPosition = Tools.InverseTransformPoint(GetMouseWorldPosition(m_Creator.PathSpaceType,distance),m_Creator.transform,m_Creator.PathSpaceType);

					Undo.RecordObject(m_Creator,"Add Anchor");

					if(m_HandleIndexToDisplayAsTransform == -1)
					{
						m_Creator.AddAnchor(newPosition);
					}
					else
					{
						m_Creator.InsertAnchor(m_HandleIndexToDisplayAsTransform,newPosition);
					}
				}
			}
			else
			{
				if((_event.control || _event.command) && _event.type == EventType.MouseDown && _event.button == 0)
				{
					Undo.RecordObject(m_Creator,"Delete Anchor");

					m_Creator.RemoveAnchor(m_MouseOverHandleIndex);

					if(m_MouseOverHandleIndex == m_HandleIndexToDisplayAsTransform)
					{
						m_HandleIndexToDisplayAsTransform = -1;
					}

					m_MouseOverHandleIndex = -1;

					Repaint ();
				}
			}
		}
	}
}
#endif