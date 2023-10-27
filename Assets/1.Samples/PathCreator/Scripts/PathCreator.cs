#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using KZLib.KZCurve;
using KZLib.KZEditor;
using UnityEditor;

namespace KZLib.KZDevelop
{
	public class PathCreator : BaseComponent
	{
		private enum ViewMode { None, OnlyPoint, OnlyLine, Both }

		[SerializeField,HideInInspector]
		private bool m_IsClosed = false;
		[SerializeField,HideInInspector]
		private SplineType m_CurveType = SplineType.CatMullRom;

		[VerticalGroup("0",Order = 0),SerializeField,LabelText("뷰 모드")]
		private ViewMode m_ViewMode = ViewMode.Both;
		public bool IsShowLine => m_ViewMode.HasFlag(ViewMode.OnlyLine);
		public bool IsShowPoint => m_ViewMode.HasFlag(ViewMode.OnlyPoint);

		[VerticalGroup("0",Order = 0),SerializeField,LabelText("공간 타입")]
		private SpaceType m_PathSpaceType = SpaceType.xyz;
		public SpaceType PathSpaceType => m_PathSpaceType;

		[VerticalGroup("0",Order = 0),ShowInInspector]
		public int AnchorCount => m_AnchorList.Count;

		[VerticalGroup("1",Order = 1),Button("경로 초기화")]
		private void OnResetPath()
		{
			Undo.RecordObject(this,"Reset Path");

			Reset();
		}

		public void TogglePath()
		{
			m_ViewMode = (ViewMode)(((int)m_ViewMode+1)%4);
		}

		protected override void Reset()
		{
			base.Reset();

			m_AnchorList.Clear();

			var position = transform.position;

			if(EditorSettings.defaultBehaviorMode == EditorBehaviorMode.Mode2D)
			{
				m_PathSpaceType = SpaceType.xy;

				var point = position.MaskZ();

				m_AnchorList.Add(point);
				m_AnchorList.Add(point+new Vector3(1.0f,1.0f,0.0f));
			}
			else
			{
				m_PathSpaceType = SpaceType.xyz;

				m_AnchorList.Add(position);
				m_AnchorList.Add(position+Vector3.one);
			}
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
				m_CurveArray = null;
			}
		}

		[VerticalGroup("2",Order = 2),SerializeField,LabelText("앵커 리스트")]
		private readonly List<Vector3> m_AnchorList = new();

		private Vector3[] m_CurveArray = null;

		public Vector3[] AnchorArray => m_AnchorList.ToArray();

		public void InsertAnchor(int _index,Vector3 _position)
		{
			m_AnchorList.Insert(_index,_position);

			m_CurveArray = null;
		}

		public void AddAnchor(Vector3 _position)
		{
			m_AnchorList.Add(_position);

			m_CurveArray = null;
		}

		public void RemoveAnchor(int _index)
		{
			m_AnchorList.RemoveAt(_index);

			m_CurveArray = null;
		}

		public void MoveAnchor(int _index,Vector3 _position)
		{
			if(m_AnchorList.ContainsIndex(_index))
			{
				m_AnchorList[_index] = _position;
			}

			m_CurveArray = null;
		}

		public void SplitLine(Vector3 _position,int _index)
		{
			m_AnchorList.Insert(_index,_position);

			m_CurveArray = null;
		}

		private void OnDrawGizmos()
		{
			var selected = Selection.activeGameObject;

			if(selected == gameObject || m_AnchorList.Count <= 0)
			{
				return;
			}

			m_CurveArray ??= Tools.GetCatmullRomSplineCurve(AnchorArray,IsClosed);

			Gizmos.color = EditorCustom.EditorPath.LineNormalColor;

			for(var i=0;i<m_CurveArray.Length-1;i++)
			{
				Gizmos.DrawLine(m_CurveArray[i+0],m_CurveArray[i+1]);
			}
		}
	}
}
#endif