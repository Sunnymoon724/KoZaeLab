#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using KZLib.KZEditor;
using UnityEditor;
using System;

namespace KZLib.KZDevelop
{
	public partial class PathCreator : BaseComponent
	{
		private enum CurveType { Spline, Bezier,  };

		[SerializeField,HideInInspector]
		private bool m_IsClosed = false;

		[VerticalGroup("0",Order = 0),SerializeField,LabelText("공간 타입")]
		private SpaceType m_PathSpaceType = SpaceType.xyz;
		public SpaceType PathSpaceType => m_PathSpaceType;

		[VerticalGroup("0",Order = 0),SerializeField,LabelText("곡선 타입")]
		private CurveType m_PathCurveType = CurveType.Spline;
		public bool IsSplineCurve => m_PathCurveType == CurveType.Spline;

		[VerticalGroup("0",Order = 0),ShowInInspector,LabelText("핸들 갯수")]
		public int HandleCount => HandleArray.Length;

		[VerticalGroup("1",Order = 1),Button("경로 초기화")]
		private void OnResetPath()
		{
			Undo.RecordObject(this,"Reset Path");

			Reset();
		}

		private Action m_OnChangedPath = null;

		public event Action OnChangedPath
		{
			add { m_OnChangedPath -= value; m_OnChangedPath += value; }
			remove { m_OnChangedPath -= value; }
		}

		protected override void Reset()
		{
			base.Reset();

			m_HandleList.Clear();

			var is2D = EditorSettings.defaultBehaviorMode == EditorBehaviorMode.Mode2D;

			m_PathSpaceType = is2D ? SpaceType.xy : SpaceType.xyz;

			if(IsSplineCurve)
			{
				ResetSpline(is2D);
			}
			else
			{
				ResetBezier(is2D,IsClosed);
			}

			m_PathArray = null;
		}

		[VerticalGroup("0",Order = 0),ShowInInspector,LabelText("폐쇄 여부")]
		public bool IsClosed
		{
			get => m_IsClosed;
			private set
			{
				if(m_IsClosed == value)
				{
					return;
				}

				m_IsClosed = value;
				m_PathArray = null;
			}
		}

		[VerticalGroup("2",Order = 2),SerializeField,LabelText("핸들 리스트")]
		private readonly List<Vector3> m_HandleList = new();
		public Vector3[] HandleArray => m_HandleList.ToArray();
		private Vector3[] m_PathArray = null;

		public Vector3[] PathArray
		{
			get
			{
				if(m_PathArray == null)
				{
					var pathArray = IsSplineCurve ? Tools.GetCatmullRomSplineCurve(HandleArray,IsClosed) : Tools.GetCubicBezierCurve(HandleArray,IsClosed);

					if(pathArray == null)
					{
						m_PathArray = new Vector3[0];
					}

					m_PathArray = pathArray ?? (new Vector3[0]);

					m_OnChangedPath?.Invoke();
				}

				return m_PathArray;
			}
		}

		public void InsertHandle(int _index,Vector3 _position)
		{
			m_HandleList.Insert(_index,_position);

			m_PathArray = null;
		}

		public void AddHandle(Vector3 _position)
		{
			m_HandleList.Add(_position);

			m_PathArray = null;
		}

		public void RemoveHandle(int _index)
		{
			if(m_HandleList.Count <= (IsSplineCurve ? 4 : IsClosed ? 6 : 4))
			{
				return;
			}

			if(IsSplineCurve)
			{
				m_HandleList.RemoveAt(_index);
			}
			else
			{
				if(_index == 0)
				{
					if(IsClosed)
					{
						m_HandleList[^1] = m_HandleList[2];
					}

					m_HandleList.RemoveRange(0,3);
				}
				else if(_index == m_HandleList.Count-1 && !IsClosed)
				{
					m_HandleList.RemoveRange(_index-2,3);
				}
				else
				{
					m_HandleList.RemoveRange(_index-1,3);
				}
			}

			m_HandleList.RemoveAt(_index);

			m_PathArray = null;
		}

		public void MoveHandle(int _index,Vector3 _position)
		{
			var deltaMove = _position-m_HandleList[_index];

			if(m_HandleList.ContainsIndex(_index))
			{
				m_HandleList[_index] = _position;
			}

			if(!IsSplineCurve)
			{
				var isAnchor = _index%3 == 0;
				var length = m_HandleList.Count;

				if(isAnchor)
				{
					if(_index+1 < length || IsClosed)
					{
						m_HandleList[Tools.LoopClamp(_index+1,length)] += deltaMove;
					}

					if(_index-1 < length || IsClosed)
					{
						m_HandleList[Tools.LoopClamp(_index-1,length)] += deltaMove;
					}
				}
				else
				{
					var nextAnchor = (_index+1)%3 == 0;
					var controlIndex = nextAnchor ? _index+2 : _index-2;
					var anchorIndex = nextAnchor ? _index+1 : _index-1;

					if(controlIndex >= 0 && controlIndex < length || IsClosed)
					{
						var anchor = m_HandleList[Tools.LoopClamp(anchorIndex,length)];

						m_HandleList[Tools.LoopClamp(controlIndex,length)] = anchor+(anchor-_position).normalized*(anchor-m_HandleList[Tools.LoopClamp(controlIndex,length)]).magnitude;

						// float distanceFromAnchor = 0;
                        //     // If in aligned mode, then attached control's current distance from anchor point should be maintained
                        //     if (controlMode == ControlMode.Aligned) {
                        //         distanceFromAnchor = (points[LoopIndex (anchorIndex)] - points[LoopIndex (attachedControlIndex)]).magnitude;
                        //     }
                        //     // If in mirrored mode, then both control points should have the same distance from the anchor point
                        //     else if (controlMode == ControlMode.Mirrored) {
                        //         distanceFromAnchor = (points[LoopIndex (anchorIndex)] - points[i]).magnitude;

                        //     }
					}
				}
			}

			m_PathArray = null;
		}

		private void OnDrawGizmos()
		{
			var selected = Selection.activeGameObject;

			if(selected == gameObject || m_HandleList.Count <= 0 || PathArray.Length < 1)
			{
				return;
			}

			Gizmos.color = EditorCustom.EditorPath.NormalLineColor;

			for(var i=0;i<PathArray.Length-1;i++)
			{
				Gizmos.DrawLine(PathArray[i+0],PathArray[i+1]);
			}
		}
	}
}
#endif