using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace KZLib.KZDevelop
{
	public enum PathDrawMode { Curve, Polygon };

	public partial class PathCreator : BaseComponent
	{
		[VerticalGroup("0",Order = 0),SerializeField,LabelText("공간 타입")]
		private SpaceType m_PathSpaceType = SpaceType.xyz;
		public SpaceType PathSpaceType => m_PathSpaceType;

		[SerializeField,HideInInspector]
		private PathDrawMode m_DrawMode = PathDrawMode.Curve;

		public bool IsCurveMode => DrawMode == PathDrawMode.Curve;
		public bool IsPolygonMode => DrawMode == PathDrawMode.Polygon;

		[SerializeField,HideInInspector]
		private float m_Resolution = 50.0f;

		private string ResolutionLabel => IsCurveMode ? "곡선 정밀도" : "정점까지 최대 길이";
		private float MinResolution => IsCurveMode ? 1.0f : 0.0f;

		[VerticalGroup("5",Order = 5),SerializeField,LabelText("핸들 리스트"),DisplayAsString,ListDrawerSettings(DraggableItems = false,HideAddButton = true,HideRemoveButton = true)]
		private List<Vector3> m_HandleList = new();
		public Vector3[] HandleArray => m_HandleList.ToArray();

		[SerializeField,HideInInspector]
		private Vector3[] m_PointArray = null;

		[VerticalGroup("3",Order = 3),SerializeField,LabelText("경로 길이"),DisplayAsString]
		private float m_PathLength = 0.0f;
		public float PathLength => m_PathLength;

		[VerticalGroup("3",Order = 3),SerializeField,LabelText("라인 렌더러")]
		private LineRenderer m_LineRenderer = null;

		[NonSerialized]
		private bool m_Dirty = false;

		public Vector3[] PointArray
		{
			get
			{
				if(m_PointArray == null || m_Dirty)
				{
					if(IsCurveMode)
					{
						GetCurvePointArray();
					}
					else
					{
						GetShapePointArray();
					}

					m_OnChangedPath?.Invoke();

					m_PathLength = MathUtility.GetTotalDistance(m_PointArray);

					if(m_LineRenderer)
					{
						m_LineRenderer.loop = IsClosed;
						m_LineRenderer.positionCount = m_PointArray.Length;
						m_LineRenderer.SetPositions(m_PointArray);
					}

					m_Dirty = false;
				}

				return m_PointArray;
			}
		}

		private Action m_OnChangedPath = null;

		public event Action OnChangedPath
		{
			add { m_OnChangedPath -= value; m_OnChangedPath += value; }
			remove { m_OnChangedPath -= value; }
		}

		public void SetDirty()
		{
			m_Dirty = true;
		}

		public void SetLineRendererData(Vector2 _width)
		{
			m_LineRenderer.startWidth = _width.x;
			m_LineRenderer.endWidth = _width.y;
		}

		[VerticalGroup("0",Order = 0),ShowInInspector,LabelText("그리기 모드")]
		public PathDrawMode DrawMode
		{
			get => m_DrawMode;
			private set
			{
				if(m_DrawMode == value)
				{
					return;
				}

				m_DrawMode = value;

				Reset();
			}
		}

		[VerticalGroup("2",Order = 2),ShowInInspector,LabelText(nameof(ResolutionLabel)),MinValue("$MinResolution")]
		public float Resolution
		{
			get => m_Resolution;
			private set
			{
				if(m_Resolution == value)
				{
					return;
				}

				m_Resolution = value;

				SetDirty();
			}
		}

#if UNITY_EDITOR
		[VerticalGroup("10",Order = 10),Button("경로 초기화",ButtonSizes.Medium)]
		public void OnResetPath()
		{
			Undo.RecordObject(this,"Reset Path");

			Reset();
		}

		protected override void Reset()
		{
			base.Reset();

			var is2D = EditorSettings.defaultBehaviorMode == EditorBehaviorMode.Mode2D;

			m_PathSpaceType = is2D ? SpaceType.xy : SpaceType.xyz;

			if(IsCurveMode)
			{
				ResetCurve();
			}
			else
			{
				ResetShape();
			}

			SetDirty();
		}

		private Vector3 ConvertPosition(Vector3 _position)
		{
			return PathSpaceType == SpaceType.xy ? _position.MaskZ() : PathSpaceType == SpaceType.xz ? _position.MaskY() : _position;
		}

		private void OnDrawGizmos()
		{
			var selected = Selection.activeGameObject;

			if(selected == gameObject || m_HandleList.Count <= 0)
			{
				return;
			}

			if(IsCurveMode)
			{
				DrawCurveGizmos();
			}
			else
			{
				DrawShapeGizmos();
			}
		}
#endif
	}
}