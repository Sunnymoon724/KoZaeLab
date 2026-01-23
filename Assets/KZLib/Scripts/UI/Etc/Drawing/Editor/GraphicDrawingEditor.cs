#if UNITY_EDITOR
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace KZLib
{
	public abstract class GraphicDrawingEditor : OdinEditor
	{
		protected class HandleInfo
		{
			public bool IsAnchor { get; init; }
			public Vector3 Position { get; set; }

			public HandleInfo(Vector3 position,bool isAnchor)
			{
				Position = position;
				IsAnchor = isAnchor;
			}
		}

		private int m_selectedHandleIndex = Global.INVALID_INDEX;
		private int m_dragHandleIndex = Global.INVALID_INDEX;
		private int m_mouseOverHandleIndex = Global.INVALID_INDEX;

		protected bool m_raycastPadding = false;

		protected SerializedObject m_serializedObject = null;

		private readonly Dictionary<int,HandleInfo> m_handleInfoDict = new();
		private readonly HashSet<int> m_handleIndexHashSet = new();
		private readonly List<int> m_handleIndexRemoveList = new();

		protected abstract SpaceType CurrentSpaceType { get; }
		protected abstract Transform CurrentTransform { get; }

		protected override void OnEnable()
		{
			base.OnEnable();

			Undo.undoRedoPerformed -= Repaint;
			Undo.undoRedoPerformed += Repaint;

			m_serializedObject = new SerializedObject(target);

			_DoEnable();
		}

		protected abstract void _DoEnable();
		protected abstract void _DoInspectorGUI();
		protected abstract bool _IsShowHandle();
		protected abstract void _UpdateAnchorPositionList();
		protected abstract void _UpdateControlPositionList();

		public override void OnInspectorGUI()
		{
			m_serializedObject.Update();

			_DoInspectorGUI();

			m_serializedObject.ApplyModifiedProperties();
		}

		protected void _SceneGUI()
		{
			if(!_IsShowHandle())
			{
				return;
			}

			var currentEvent = Event.current;

			using var check = new EditorGUI.ChangeCheckScope();

			if(currentEvent.type != EventType.Repaint && currentEvent.type != EventType.Layout)
			{
				ProcessHandleInput(currentEvent);
			}

			_DrawHandle();

			if(currentEvent.type == EventType.Layout)
			{
				HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
			}

			if(check.changed)
			{
				Undo.RecordObject(target,"Changed Handle");

				EditorUtility.SetDirty(target);
				EditorApplication.QueuePlayerLoopUpdate();
			}
		}

		private void ProcessHandleInput(Event currentEvent)
		{
			_UpdatePositionList();

			m_mouseOverHandleIndex = _GetMouseOverIndex();

			switch(currentEvent.type)
			{
				case EventType.MouseDown:
					{
						_ClickDownMouse();
						break;
					}
				case EventType.MouseDrag when m_dragHandleIndex != Global.INVALID_INDEX:
					{
						if(!m_handleInfoDict.TryGetValue(m_dragHandleIndex,out var handleInfo))
						{
							return;
						}

						var currentPosition = handleInfo.Position;
						var newPosition = _ConvertToMousePosition(currentEvent.mousePosition);

						if(currentPosition != newPosition)
						{
							_DragMouse(m_dragHandleIndex,newPosition);
						}
						break;
					}
				case EventType.MouseUp:
					{
						_ClickUpMouse();
						break;
					}
			}
		}

		protected int _GetMouseOverIndex()
		{
			foreach(var pair in m_handleInfoDict)
			{
				if(KZHandleKit.IsMouseOverHandle(pair.Value.Position))
				{
					return pair.Key;
				}
			}

			return Global.INVALID_INDEX;
		}

		protected virtual void _ClickDownMouse()
		{
			m_selectedHandleIndex = m_mouseOverHandleIndex;

			if(m_mouseOverHandleIndex != Global.INVALID_INDEX)
			{
				m_dragHandleIndex = m_selectedHandleIndex;
			}
		}

		protected virtual void _DragMouse(int index,Vector3 position) { }

		protected virtual void _ClickUpMouse()
		{
			m_dragHandleIndex = -1;
		}

		protected abstract Vector3 _ConvertToMousePosition(Vector3 mousePosition);

		protected virtual void _DrawHandle()
		{
			_UpdatePositionList();

			foreach(var pair in m_handleInfoDict)
			{
				var handleInfo = pair.Value;
				var isMouseOver = m_mouseOverHandleIndex == pair.Key;
				var isSelected = m_selectedHandleIndex == pair.Key;

				KZHandleKit.DrawHandlePosition(pair.Key,handleInfo.Position,isMouseOver,isSelected,handleInfo.IsAnchor);
			}
		}

		private void _UpdatePositionList()
		{
			m_handleIndexHashSet.Clear();
			m_handleIndexRemoveList.Clear();

			_UpdateAnchorPositionList();
			_UpdateControlPositionList();

			foreach(var key in m_handleInfoDict.Keys)
			{
				if(!m_handleIndexHashSet.Contains(key))
				{
					m_handleIndexRemoveList.Add(key);
				}
			}

			foreach(var key in m_handleIndexRemoveList)
			{
				m_handleInfoDict.Remove(key);
			}
		}

		protected void _AddOrUpdateHandlePosition(int index,Vector3 position,bool isAnchor)
		{
			if(m_handleInfoDict.TryGetValue(index,out var handleInfo))
			{
				handleInfo.Position = position;
			}
			else
			{
				m_handleInfoDict.Add(index,new HandleInfo(position,isAnchor));
			}

			m_handleIndexHashSet.Add(index);
		}
	}
}
#endif