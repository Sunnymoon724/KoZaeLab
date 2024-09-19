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

		private int m_SelectedHandleIndex = -1;
		private int m_DragHandleIndex = -1;
		private int m_MouseOverHandleIndex = -1;

		private Color m_HandleSelectColor = "#FFFF00FF".ToColor();

		private readonly float m_AnchorSize = 10.0f;
		private Color m_AnchorNormalColor = "#FF5F5FFF".ToColor();
		private Color m_AnchorHighlightColor = "#BC0A0AFF".ToColor();

		private readonly float m_ControlSize = 7.0f;
		private Color m_ControlNormalColor = "#5999FFFF".ToColor();
		private Color m_ControlHighlightColor = "#3232C0FF".ToColor();

		private Color m_NormalLineColor = "#00FF00FF".ToColor();
		private Color m_GuideLineColor = "#FFFFFFFF".ToColor();

		protected override void OnEnable()
		{
			base.OnEnable();

			m_Creator = target as PathCreator;

			m_Creator.OnChangedPath += OnResetState;

			Undo.undoRedoPerformed -= OnUndoRedo;
			Undo.undoRedoPerformed += OnUndoRedo;

			OnResetState();
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			Tools.hidden = false;
		}

		private void OnUndoRedo()
		{
			m_Creator.SetDirty();

			Repaint();
		}

		private void OnResetState()
		{
			m_MouseOverHandleIndex = -1;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			Tools.hidden = true;
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

		private void SetPathInput(Event _event)
		{
			if(m_Creator.IsCurveMode)
			{
				SetCurvePathInput(_event);
			}
			else
			{
				SetShapePathInput(_event);
			}
		}

		private void DrawPath()
		{
			if(Event.current.type != EventType.Repaint)
			{
				return;
			}

			var handleArray = m_Creator.HandleArray;

			if(m_Creator.IsCurveMode)
			{
				for(var i=0;i<handleArray.Length;i++)
				{
					DrawHandle(i,handleArray[i]);
				}
			}
			else
			{
				if(handleArray.Length >= 3)
				{
					//? 도형은 중심, 시작점, 방향점 3개만 있으면 됨
					for(var i=0;i<3;i++)
					{
						DrawHandle(i,handleArray[i]);
					}
				}
			}

			DrawLine(handleArray);
		}

		private void DrawLine(Vector3[] _handleArray)
		{
			if(m_Creator.IsCurveMode)
			{
				DrawLineInCurve(_handleArray);
			}
			else
			{
				DrawLineInShape(_handleArray);
			}
		}

		private void DrawHandle(int _index,Vector3 _position)
		{
			var position = _position.TransformPoint(m_Creator.transform,m_Creator.PathSpaceType);

			var isSelected = _index == m_SelectedHandleIndex;
			var isMouseOver = _index == m_MouseOverHandleIndex;
			var isAnchor = m_Creator.IsCurveMode ? IsCurveAnchor(_index) : IsShapeAnchor(_index);
			var diameter = GetHandleDiameter(isAnchor ? m_AnchorSize : m_ControlSize,_position);

			var handleId = GUIUtility.GetControlID(_index.GetHashCode(),FocusType.Passive);

			var cachedColor = Handles.color;
			var highlight = isAnchor ? m_AnchorHighlightColor : m_ControlHighlightColor;
			var normal = isAnchor ? m_AnchorNormalColor : m_ControlNormalColor;

			Handles.color = isSelected ? m_HandleSelectColor : isMouseOver ? highlight : normal;

			Handles.SphereHandleCap(handleId,position,Quaternion.LookRotation(Vector3.up),diameter,EventType.Repaint);

			var style = new GUIStyle();
			style.normal.textColor = Color.white;

			Handles.Label(position,string.Format("{0}",_index),style);

			Handles.color = cachedColor;
		}

		private float GetHandleDiameter(float _diameter,Vector3 _position)
		{
			return _diameter*0.01f*HandleUtility.GetHandleSize(_position)*2.5f;
		}

		private Vector3 GetMousePosition(PathCreator _creator,float _depth = 10.0f)
		{
			var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
			var position = ray.GetPoint(_depth);

			if(_creator.PathSpaceType == SpaceType.xy && ray.direction.z != 0.0f)
			{
				position = ray.GetPoint(Mathf.Abs(ray.origin.z/ray.direction.z));
			}
			else if(_creator.PathSpaceType == SpaceType.xz && ray.direction.y != 0)
			{
				position = ray.GetPoint(Mathf.Abs(ray.origin.y/ray.direction.y));
			}

			return position.InverseTransformPoint(_creator.transform,_creator.PathSpaceType);
		}
	}
}
#endif