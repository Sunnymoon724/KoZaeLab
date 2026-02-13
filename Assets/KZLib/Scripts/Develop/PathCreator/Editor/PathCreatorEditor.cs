#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector.Editor;
using R3;

namespace KZLib.Development
{
	[CustomEditor(typeof(PathCreator))]
	public partial class PathCreatorEditor : OdinEditor
	{
		private readonly CompositeDisposable m_disposable = new();

		private PathCreator m_pathCreator = null;

		private int m_selectedHandleIndex = Global.INVALID_INDEX;
		private int m_dragHandleIndex = Global.INVALID_INDEX;
		private int m_mouseOverHandleIndex = Global.INVALID_INDEX;

		private Color m_handleSelectColor = "#FFFF00FF".ToColor();

		private readonly float m_anchorSize = 10.0f;
		private Color m_anchorNormalColor = "#FF5F5FFF".ToColor();
		private Color m_anchorHighlightColor = "#BC0A0AFF".ToColor();

		private readonly float m_controlSize = 7.0f;
		private Color m_controlNormalColor = "#5999FFFF".ToColor();
		private Color m_controlHighlightColor = "#3232C0FF".ToColor();

		private Color m_normalLineColor = "#00FF00FF".ToColor();
		private Color m_guideLineColor = "#FFFFFFFF".ToColor();

		protected override void OnEnable()
		{
			base.OnEnable();

			m_pathCreator = target as PathCreator;

			m_pathCreator.OnChangedPath.Subscribe(_OnResetState).AddTo(m_disposable);

			Undo.undoRedoPerformed -= _OnUndoRedo;
			Undo.undoRedoPerformed += _OnUndoRedo;

			_OnResetState(Unit.Default);
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			m_disposable?.Dispose(); 

			Undo.undoRedoPerformed -= _OnUndoRedo;

			Tools.hidden = false;
		}

		private void _OnUndoRedo()
		{
			m_pathCreator.SetDirty();

			Repaint();
		}

		private void _OnResetState(Unit _)
		{
			m_mouseOverHandleIndex = Global.INVALID_INDEX;
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
				_SetPathInput(Event.current);
			}

			_DrawPath();

			if(eventType == EventType.Layout)
			{
				HandleUtility.AddDefaultControl(0);
			}

			if(check.changed)
			{
				EditorApplication.QueuePlayerLoopUpdate();
			}
		}

		private void _SetPathInput(Event currentEvent)
		{
			if(m_pathCreator.IsCurveMode)
			{
				_SetCurvePathInput(currentEvent);
			}
			else
			{
				_SetShapePathInput(currentEvent);
			}
		}

		private void _DrawPath()
		{
			if(Event.current.type != EventType.Repaint)
			{
				return;
			}

			var handleArray = m_pathCreator.HandleArray;

			if(m_pathCreator.IsCurveMode)
			{
				for(var i=0;i<handleArray.Length;i++)
				{
					_DrawHandle(i,handleArray[i]);
				}
			}
			else
			{
				if(handleArray.Length >= 3)
				{
					//? center,start,direction
					for(var i=0;i<3;i++)
					{
						_DrawHandle(i,handleArray[i]);
					}
				}
			}

			_DrawLine(handleArray);
		}

		private void _DrawLine(Vector3[] handleArray)
		{
			if(m_pathCreator.IsCurveMode)
			{
				_DrawLineInCurve(handleArray);
			}
			else
			{
				_DrawLineInShape();
			}
		}

		private void _DrawHandle(int index,Vector3 position)
		{
			var transformPoint = position.TransformPoint(m_pathCreator.transform,m_pathCreator.PathSpaceType);

			var isSelected = index == m_selectedHandleIndex;
			var isMouseOver = index == m_mouseOverHandleIndex;
			var isAnchor = m_pathCreator.IsCurveMode ? _IsCurveAnchor(index) : _IsShapeAnchor(index);
			var diameter = _GetHandleDiameter(isAnchor ? m_anchorSize : m_controlSize,position);

			var handleId = GUIUtility.GetControlID(index.GetHashCode(),FocusType.Passive);

			var cachedColor = Handles.color;
			var highlight = isAnchor ? m_anchorHighlightColor : m_controlHighlightColor;
			var normal = isAnchor ? m_anchorNormalColor : m_controlNormalColor;

			Handles.color = isSelected ? m_handleSelectColor : isMouseOver ? highlight : normal;

			Handles.SphereHandleCap(handleId,transformPoint,Quaternion.LookRotation(Vector3.up),diameter,EventType.Repaint);

			var style = new GUIStyle();
			style.normal.textColor = Color.white;

			Handles.Label(transformPoint,$"{index}",style);

			Handles.color = cachedColor;
		}

		private float _GetHandleDiameter(float diameter,Vector3 position)
		{
			return diameter*0.01f*HandleUtility.GetHandleSize(position)*2.5f;
		}

		private Vector3 _GetMousePosition(float depth = 10.0f)
		{
			var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
			var position = ray.GetPoint(depth);

			if(m_pathCreator.PathSpaceType == SpaceType.xy && ray.direction.z != 0.0f)
			{
				position = ray.GetPoint(Mathf.Abs(ray.origin.z/ray.direction.z));
			}
			else if(m_pathCreator.PathSpaceType == SpaceType.xz && ray.direction.y != 0)
			{
				position = ray.GetPoint(Mathf.Abs(ray.origin.y/ray.direction.y));
			}

			return position.InverseTransformPoint(m_pathCreator.transform,m_pathCreator.PathSpaceType);
		}
	}
}
#endif