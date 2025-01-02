using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using UnityEngine.Events;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace KZLib.KZDevelop
{
	public enum PathDrawMode { Curve, Polygon };

	public partial class PathCreator : BaseComponent
	{
		[VerticalGroup("0",Order = 0),SerializeField,LabelText("Space Type")]
		private SpaceType m_pathSpaceType = SpaceType.xyz;
		public SpaceType PathSpaceType => m_pathSpaceType;

		[SerializeField,HideInInspector]
		private PathDrawMode m_drawMode = PathDrawMode.Curve;

		public bool IsCurveMode => DrawMode == PathDrawMode.Curve;
		public bool IsPolygonMode => DrawMode == PathDrawMode.Polygon;

		[SerializeField,HideInInspector]
		private float m_resolution = 50.0f;

		[SerializeField,HideInInspector]
		private bool m_closed = false;

		private string ResolutionLabel => IsCurveMode ? "Curve Resolution" : "Vertex Distance";
		private float MinResolution => IsCurveMode ? 1.0f : 0.0f;

		[VerticalGroup("5",Order = 5),SerializeField,LabelText("Handle List"),DisplayAsString,ListDrawerSettings(DraggableItems = false,HideAddButton = true,HideRemoveButton = true)]
		private List<Vector3> m_handleList = new();
		public Vector3[] HandleArray => m_handleList.ToArray();

		[SerializeField,HideInInspector]
		private Vector3[] m_pointArray = null;

		[VerticalGroup("3",Order = 3),SerializeField,LabelText("Path Length"),DisplayAsString]
		private float m_pathLength = 0.0f;
		public float PathLength => m_pathLength;

		[VerticalGroup("3",Order = 3),SerializeField,LabelText("Line Renderer")]
		private LineRenderer m_lineRenderer = null;

		[NonSerialized]
		private bool m_dirty = false;

		public Vector3[] PointArray
		{
			get
			{
				if(m_pointArray == null || m_dirty)
				{
					if(IsCurveMode)
					{
						GetCurvePointArray();
					}
					else
					{
						GetShapePointArray();
					}

					OnPathChanged?.Invoke();

					m_pathLength = CommonUtility.GetTotalDistance(m_pointArray);

					if(m_lineRenderer)
					{
						m_lineRenderer.loop = IsClosed;
						m_lineRenderer.positionCount = m_pointArray.Length;
						m_lineRenderer.SetPositions(m_pointArray);
					}

					m_dirty = false;
				}

				return m_pointArray;
			}
		}

		public event UnityAction OnPathChanged;

		public void SetDirty()
		{
			m_dirty = true;
		}

		public void SetLineRendererData(Vector2 width)
		{
			m_lineRenderer.startWidth = width.x;
			m_lineRenderer.endWidth = width.y;
		}

		[VerticalGroup("0",Order = 0),ShowInInspector,LabelText("Current DrawMode")]
		public PathDrawMode DrawMode
		{
			get => m_drawMode;
			private set
			{
				if(m_drawMode == value)
				{
					return;
				}

				m_drawMode = value;

				Reset();
			}
		}

		[VerticalGroup("2",Order = 2),ShowInInspector,LabelText(nameof(ResolutionLabel)),MinValue("$MinResolution")]
		public float Resolution
		{
			get => m_resolution;
			private set
			{
				if(m_resolution == value)
				{
					return;
				}

				m_resolution = value;

				SetDirty();
			}
		}

#if UNITY_EDITOR
		[VerticalGroup("10",Order = 10),Button("Reset Path",ButtonSizes.Medium)]
		public void OnResetPath()
		{
			Undo.RecordObject(this,"Reset Path");

			Reset();
		}

		protected override void Reset()
		{
			base.Reset();

			var is2D = EditorSettings.defaultBehaviorMode == EditorBehaviorMode.Mode2D;

			m_pathSpaceType = is2D ? SpaceType.xy : SpaceType.xyz;

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

		private Vector3 ConvertPosition(Vector3 position)
		{
			return PathSpaceType == SpaceType.xy ? position.SetZ() : PathSpaceType == SpaceType.xz ? position.SetY() : position;
		}

		private void OnDrawGizmos()
		{
			var selected = Selection.activeGameObject;

			if(selected == gameObject || m_handleList.Count <= 0)
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