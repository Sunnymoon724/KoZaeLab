#if UNITY_EDITOR
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace KZLib.UI
{
	public abstract class GraphicDrawingEditor : OdinEditor
	{
		#region KnotInfo
		private class KnotInfo
		{
			public KnotType Type { get; init; }
			public Vector3 Position { get; set; }

			public KnotInfo(Vector3 position,KnotType knotType)
			{
				Position = position;
				Type = knotType;
			}
		}
		#endregion KnotInfo

		protected bool m_isRaycastPaddingExpanded = false;
		protected SerializedObject m_serializedObject = null;

		private bool m_isEditing = false;
		private Tool m_previousTool = Tool.None;

		protected int m_mouseOverKnotIndex = Global.INVALID_INDEX;
		protected int m_selectedKnotIndex = Global.INVALID_INDEX;
		protected int m_dragKnotIndex = Global.INVALID_INDEX;

		private readonly Dictionary<int,KnotInfo> m_knotInfoDict = new();
		private readonly HashSet<int> m_knotIndexHashSet = new();
		private readonly List<int> m_knotIndexRemoveList = new();

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
		protected abstract bool _CanShowKnot();
		protected abstract void _DoUpdateKnotList();
		protected abstract void _ChangeKnotPosition(int index,Vector3 newPosition);
		protected abstract Vector3[] _GetWorldCornerArray();
		protected abstract Vector3 _ConvertToMousePosition(Vector3 mousePosition);

		public override void OnInspectorGUI()
		{
			m_serializedObject.Update();

			GUI.enabled = false;

			EditorGUILayout.ObjectField("Script",MonoScript.FromMonoBehaviour(target as MonoBehaviour),typeof(MonoScript),false);

			GUI.enabled = true;

			_DoInspectorGUI();

			if(GUILayout.Toggle(m_isEditing,"Edit Drawing","Button",GUILayout.Height(40.0f)))
			{
				if(!m_isEditing)
				{
					_StartEditing();
				}

				_SetEditingDescription();
			}
			else
			{
				if(m_isEditing)
				{
					_StopEditing(true);
				}
			}

			m_serializedObject.ApplyModifiedProperties();
		}

		private void _StartEditing()
		{
			m_isEditing = true;

			m_previousTool = Tools.current;
			Tools.current = Tool.None;

			Repaint();
		}

		protected void _StopEditing(bool restoreTool)
		{
			m_isEditing = false;

			if(restoreTool)
			{
				Tools.current = m_previousTool;
			}

			Repaint();
		}

		protected virtual void _SetEditingDescription()
		{
			var labelStyle = new GUIStyle(GUI.skin.label)
			{
				wordWrap = true,
				fontStyle = FontStyle.Bold,
				alignment = TextAnchor.MiddleCenter,
			};

			labelStyle.normal.textColor = Color.green;

			EditorGUILayout.LabelField("Edit Mode Activated",labelStyle);
		}

		protected void _SceneGUI()
		{
			_DrawBorder();

			if(Tools.current != Tool.None)
			{
				_StopEditing(false);

				return;
			}

			if(!m_isEditing || !_CanShowKnot())
			{
				return;
			}

			using var check = new EditorGUI.ChangeCheckScope();

			var curEvt = Event.current;
			var evtType = curEvt.type;

			if(evtType != EventType.Repaint && evtType != EventType.Layout)
			{
				if(curEvt.button == 0)
				{
					_SetInput(curEvt);
				}
			}

			_UpdateKnotInfoList();

			if(evtType == EventType.Repaint)
			{
				_DrawKnot();
			}

			if(evtType == EventType.Layout)
			{
				HandleUtility.AddDefaultControl(0);
			}

			if(check.changed)
			{
				Undo.RecordObject(target,"Changed Knot");

				EditorUtility.SetDirty(target);
				EditorApplication.QueuePlayerLoopUpdate();

				Repaint();
			}
		}

		protected int _FindMouseOverKnotInfoIndex()
		{
			foreach(var pair in m_knotInfoDict)
			{
				var knotInfo = pair.Value;

				if(knotInfo.Type == KnotType.Fixed)
				{
					continue;
				}

				var isMajor = knotInfo.Type == KnotType.Major;

				if(KZKnotKit.IsMouseOverHandle(isMajor,knotInfo.Position))
				{
					return pair.Key;
				}
			}

			return Global.INVALID_INDEX;
		}

		protected virtual void _DrawKnot()
		{
			foreach(var pair in m_knotInfoDict)
			{
				var index = pair.Key;
				var knotInfo = pair.Value;

				switch(knotInfo.Type)
				{
					case KnotType.Fixed:
					{
						KZKnotKit.DrawFixedKnot(index,knotInfo.Position);
						break;
					}
					case KnotType.Major:
					case KnotType.Minor:
					{
						var isMouseOver = index == m_mouseOverKnotIndex;
						var isSelected = index == m_selectedKnotIndex;
						var isMajor = knotInfo.Type == KnotType.Major;
						
						KZKnotKit.DrawControlKnot(index,knotInfo.Position,isMouseOver,isSelected,isMajor);
						break;
					}
				}
			}
		}

		private void _SetInput(Event currentEvent)
		{
			var mousePosition = _ConvertToMousePosition(currentEvent.mousePosition);
			var shouldConsume = false;

			switch(currentEvent.type)
			{
				case EventType.MouseDown:
				{
					if(m_mouseOverKnotIndex == Global.INVALID_INDEX && !_IsInsideRect(mousePosition))
					{
						_StopEditing(true);
					}
					else
					{
						_SetMouseDown(mousePosition);
					}
					shouldConsume = true;
					break;
				}
				case EventType.MouseDrag when m_dragKnotIndex != Global.INVALID_INDEX:
				{
					_SetMouseDrag(mousePosition);
					shouldConsume = true;
					break;
				}
				case EventType.MouseUp:
				{
					_SetMouseUp(mousePosition);
					shouldConsume = true;
					break;
				}
				case EventType.MouseMove:
				{
					var overIndex = _FindMouseOverKnotInfoIndex();

					if(m_mouseOverKnotIndex != overIndex)
					{
						m_mouseOverKnotIndex = overIndex;
						Repaint();
					}
					break;
				}
			}

			if(shouldConsume)
			{
				currentEvent.Use();
				Repaint(); 
			}
		}

		protected virtual void _SetMouseDown(Vector3 _)
		{
			m_selectedKnotIndex = m_mouseOverKnotIndex;

			if(m_mouseOverKnotIndex != Global.INVALID_INDEX)
			{
				m_dragKnotIndex = m_selectedKnotIndex;
			}
		}

		protected virtual void _SetMouseDrag(Vector3 mousePosition)
		{
			var knotInfo = m_knotInfoDict[m_dragKnotIndex];

			var curPosition = knotInfo.Position;
			var newPosition = mousePosition;

			if(curPosition != newPosition)
			{
				_ChangeKnotPosition(m_dragKnotIndex,newPosition);

				Repaint();
			}
		}

		protected virtual void _SetMouseUp(Vector3 _)
		{
			m_dragKnotIndex = Global.INVALID_INDEX;
		}

		protected bool _IsInsideRect(Vector3 mousePosition)
		{
			var rectTrans = CurrentTransform.GetComponent<RectTransform>();
			var localPos = rectTrans.InverseTransformPoint(mousePosition);

			return rectTrans.rect.Contains(localPos);
		}

		private void _UpdateKnotInfoList()
		{
			m_knotIndexHashSet.Clear();
			m_knotIndexRemoveList.Clear();

			_DoUpdateKnotList();

			foreach(var pair in m_knotInfoDict)
			{
				if(m_knotIndexHashSet.Contains(pair.Key))
				{
					continue;
				}

				m_knotIndexRemoveList.Add(pair.Key);
			}

			for(var i=0;i<m_knotIndexRemoveList.Count;i++)
			{
				m_knotInfoDict.Remove(m_knotIndexRemoveList[i]);
			}
		}

		private void _DrawBorder()
		{
			var cornerArray = _GetWorldCornerArray();

			cornerArray[4] = cornerArray[0];

			KZKnotKit.DrawBorderLine(cornerArray,2.0f,m_isEditing);
		}

		protected void _AddOrUpdateKnotInfo(int index,Vector3 position,KnotType knotType)
		{
			if(m_knotInfoDict.TryGetValue(index,out var knotInfo))
			{
				knotInfo.Position = position;
			}
			else
			{
				m_knotInfoDict.Add(index,new KnotInfo(position,knotType));
			}

			m_knotIndexHashSet.Add(index);
		}
	}
}
#endif