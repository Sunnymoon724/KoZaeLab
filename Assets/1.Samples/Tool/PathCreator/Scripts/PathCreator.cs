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
		private enum CurveType { Spline, Bezier, };

		[SerializeField,HideInInspector]
		private bool m_IsClosed = false;
		[SerializeField,HideInInspector]
		private CurveType m_PathCurveType = CurveType.Spline;

		[VerticalGroup("0",Order = 0),SerializeField,LabelText("공간 타입")]
		private SpaceType m_PathSpaceType = SpaceType.xyz;
		public SpaceType PathSpaceType => m_PathSpaceType;
		[VerticalGroup("0",Order = 0),ShowInInspector,LabelText("핸들 갯수")]
		public int HandleCount => HandleArray.Length;

		[VerticalGroup("1",Order = 1),Button("경로 초기화",ButtonSizes.Medium)]
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

			ClearPath();
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

				if(!IsSplineCurve)
				{
					if(IsClosed)
					{
						m_HandleList.Add(m_HandleList[^1]*2-m_HandleList[^2]);
						m_HandleList.Add(m_HandleList[0]*2-m_HandleList[1]);
					}else
					{
						m_HandleList.RemoveRange(m_HandleList.Count-2,2);
					}
				}

				ClearPath();
			}
		}

		[VerticalGroup("0",Order = 0),ShowInInspector,LabelText("곡선 타입")]
		private CurveType PathCurveType
		{
			get => m_PathCurveType;
			set
			{
				if(m_PathCurveType == value)
				{
					return;
				}

				m_PathCurveType = value;
				ClearPath();
			}
		}
		public bool IsSplineCurve => PathCurveType == CurveType.Spline;

		[VerticalGroup("2",Order = 2),SerializeField,LabelText("핸들 리스트"),ReadOnly]
		private List<Vector3> m_HandleList = new();
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

		public void AddAnchor(Vector3 _position)
		{
			if(IsSplineCurve)
			{
				AddSpline(_position);
			}
			else
			{
				AddBezier(_position);
			}

			ClearPath();
		}

		public void InsertAnchor(int _index,Vector3 _position)
		{
			if(_index == m_HandleList.Count-1)
			{
				AddAnchor(_position);

				return;
			}

			if(IsSplineCurve)
			{
				InsertSpline(_index,_position);
			}
			else
			{
				InsertBezier(_index,_position);
			}

			ClearPath();
		}

		public void RemoveAnchor(int _index)
		{
			var count = IsClosed ? (IsSplineCurve ? 3 : 6) : 4;

			if(m_HandleList.Count < count)
			{
				return;
			}

			var result = IsSplineCurve ? RemoveSpline(_index) : RemoveBezier(_index);

			if(result)
			{
				ClearPath();
			}
		}

		public void MoveHandle(int _index,Vector3 _position,bool _isFree)
		{
			if(!m_HandleList.ContainsIndex(_index))
			{
				return;
			}

			var position = PathSpaceType == SpaceType.xy ? _position.MaskZ() : PathSpaceType == SpaceType.xz ? _position.MaskY() : _position;

			if(IsSplineCurve)
			{
				MoveSpline(_index,position);
			}
			else
			{
				MoveBezier(_index,position,_isFree);
			}

			ClearPath();
		}

		public void ClearPath()
		{
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