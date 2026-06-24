#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using R3;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace KZLib.EditorTools
{
	public enum PathDrawMode { Curve, Polygon };

	public partial class PathCreator : MonoBehaviour
	{
		[VerticalGroup("0",Order = 0),SerializeField,HideInInspector]
		private SpaceType m_pathSpaceType = SpaceType.xyz;

		[VerticalGroup("0",Order = 0),ShowInInspector]
		public SpaceType PathSpaceType
		{
			get => m_pathSpaceType;
			private set
			{
				if(m_pathSpaceType == value)
				{
					return;
				}

				m_pathSpaceType = value;

				SetDirty();
			}
		}

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

		[SerializeField,HideInInspector]
		private List<Vector3> m_handleList = new();

		public IReadOnlyList<Vector3> Handles => m_handleList;

		[VerticalGroup("5",Order = 5),ShowInInspector,DisplayAsString]
		private string HandleInfo => $"Handle Count: {m_handleList.Count}";

		[SerializeField,HideInInspector]
		private Vector3[] m_pointArray = null;

		[VerticalGroup("3",Order = 3),SerializeField,DisplayAsString]
		private float m_pathLength = 0.0f;
		public float PathLength => m_pathLength;

		[VerticalGroup("3",Order = 3),SerializeField]
		private LineRenderer m_lineRenderer = null;

		[NonSerialized]
		private bool m_dirty = true;

		public Vector3[] PointArray
		{
			get
			{
				RefreshPath();

				return m_pointArray ?? Array.Empty<Vector3>();
			}
		}

		private readonly Subject<Unit> m_pathSubject = new();
		public Observable<Unit> OnChangedPath => m_pathSubject;

		public void SetDirty()
		{
			m_dirty = true;
		}

		public void RefreshPath()
		{
			if(!m_dirty && m_pointArray != null)
			{
				return;
			}

			if(IsCurveMode)
			{
				_GetCurvePointArray();
			}
			else
			{
				_GetShapePointArray();
			}

			m_pathLength = _CalculatePathLength();
			_ApplyLineRenderer();

			m_pathSubject.OnNext(Unit.Default);

			m_dirty = false;
		}

		public void PrepareForBuild()
		{
			m_dirty = true;

			RefreshPath();
		}

		public void SetLineRenderer(Vector2 width)
		{
			if(!m_lineRenderer)
			{
				return;
			}

			m_lineRenderer.startWidth = width.x;
			m_lineRenderer.endWidth = width.y;
		}

		[VerticalGroup("0",Order = 0),ShowInInspector]
		public PathDrawMode DrawMode
		{
			get => m_drawMode;
			private set
			{
				if(m_drawMode == value)
				{
					return;
				}

				Undo.RecordObject(this,"Change Draw Mode");

				m_drawMode = value;

				_ResetPathData();
			}
		}

		[VerticalGroup("2",Order = 2),ShowInInspector,LabelText(nameof(ResolutionLabel)),MinValue(nameof(MinResolution))]
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

		[VerticalGroup("10",Order = 10),Button("Reset Path",ButtonSizes.Medium)]
		public void OnResetPath()
		{
			Undo.RecordObject(this,"Reset Path");

			Reset();
		}

		private void Reset()
		{
			var is2D = EditorSettings.defaultBehaviorMode == EditorBehaviorMode.Mode2D;

			m_pathSpaceType = is2D ? SpaceType.xy : SpaceType.xyz;

			_ResetPathData();
		}

		private void _ResetPathData()
		{
			if(IsCurveMode)
			{
				_ResetCurve();
			}
			else
			{
				_ResetShape();
			}

			SetDirty();
		}

		private void OnValidate()
		{
			if(!m_lineRenderer)
			{
				m_lineRenderer = GetComponent<LineRenderer>();
			}
		}

		private void _ApplyLineRenderer()
		{
			if(!m_lineRenderer || m_pointArray == null)
			{
				return;
			}

			m_lineRenderer.loop = _UsesLoopForPath();
			m_lineRenderer.positionCount = m_pointArray.Length;
			m_lineRenderer.SetPositions(m_pointArray);
		}

		private Vector3 _ConvertPosition(Vector3 position)
		{
			return PathSpaceType == SpaceType.xy ? position.SetZ() : PathSpaceType == SpaceType.xz ? position.SetY() : position;
		}

		private Vector3 _GetPlaneOffset(float axis0,float axis1)
		{
			return PathSpaceType == SpaceType.xz ? new Vector3(axis0,0.0f,axis1) : new Vector3(axis0,axis1,0.0f);
		}

		private bool _UsesLoopForPath() => IsPolygonMode || (IsCurveMode && IsClosed);

		private bool _ShouldDrawClosingSegment(Vector3[] pointArray)
		{
			return pointArray.Length >= 2 && !pointArray[0].AreEqual(pointArray[^1]);
		}

		private float _CalculatePathLength()
		{
			if(m_pointArray == null || m_pointArray.Length < 2)
			{
				return 0.0f;
			}

			var length = KZMathKit.GetTotalDistance(m_pointArray);

			if(_UsesLoopForPath() && _ShouldDrawClosingSegment(m_pointArray))
			{
				length += Vector3.Distance(m_pointArray[^1],m_pointArray[0]);
			}

			return length;
		}

		private void _DrawGizmoPathLines(bool closeLoop)
		{
			var pointArray = PointArray;

			if(pointArray.Length < 2)
			{
				return;
			}

			Gizmos.color = Color.green;

			for(var i=0;i<pointArray.Length-1;i++)
			{
				var from = pointArray[i+0].TransformPoint(transform,PathSpaceType);
				var to = pointArray[i+1].TransformPoint(transform,PathSpaceType);

				Gizmos.DrawLine(from,to);
			}

			if(closeLoop && _ShouldDrawClosingSegment(pointArray))
			{
				var from = pointArray[^1].TransformPoint(transform,PathSpaceType);
				var to = pointArray[0].TransformPoint(transform,PathSpaceType);

				Gizmos.DrawLine(from,to);
			}
		}

		private void OnDestroy()
		{
			m_pathSubject?.Dispose();
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
				_DrawCurveGizmos();
			}
			else
			{
				_DrawShapeGizmos();
			}
		}
	}
}
#endif
