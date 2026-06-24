#if UNITY_EDITOR
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace KZLib.EditorTools
{
	public partial class PathCreator : MonoBehaviour
	{
		[SerializeField,HideInInspector]
		private bool m_reverse = false;
		[SerializeField,HideInInspector]
		private int m_sideCount = 3;
		[SerializeField,HideInInspector]
		private float m_rotation = 0.0f;

		[SerializeField,HideInInspector]
		private Vector3 m_centerPosition = Vector3.zero;
		public Vector3 CenterPosition => m_centerPosition;

		[VerticalGroup("5",Order = 5),SerializeField,PropertyRange(0.0f,1.0f),ListDrawerSettings(DraggableItems = false,HideAddButton = true,HideRemoveButton = true,OnTitleBarGUI = nameof(_OnRefreshDistance)),HideIf(nameof(IsCurveMode))]
		private List<float> m_vertexDistanceList = new();

		private void _GetShapePointArray()
		{
			if(m_vertexDistanceList.Count < m_sideCount)
			{
				for(var i=m_vertexDistanceList.Count;i<m_sideCount;i++)
				{
					m_vertexDistanceList.Add(1.0f);
				}
			}

			m_pointArray = new Vector3[m_sideCount];
			var degrees = (IsReverse ? -1 : +1)*Global.FullAngle/m_sideCount;

			for(var i=0;i<m_sideCount;i++)
			{
				var radian = Mathf.Deg2Rad*(i*degrees+Rotation);
				var distance = Resolution*m_vertexDistanceList[i];
				var x = Mathf.Cos(radian)*distance;
				var y = Mathf.Sin(radian)*distance;

				m_pointArray[i] = m_centerPosition+_GetPlaneOffset(x,y);
			}

			m_handleList.Clear();
			m_handleList.Add(m_centerPosition);
			m_handleList.AddRange(m_pointArray);

			m_closed = true;
		}

		public void SetPolygon(Vector3[] handleArray,float resolution)
		{
			if(handleArray == null || handleArray.Length < 3)
			{
				return;
			}

			m_drawMode = PathDrawMode.Polygon;

			m_closed = true;
			m_resolution = resolution;

			m_centerPosition = handleArray[0];
			m_sideCount = handleArray.Length-1;

			var direction = handleArray[1]-m_centerPosition;

			m_rotation = _GetPolygonRotation(direction);
			m_reverse = _GetPolygonReverse(handleArray[1],handleArray[2]);

			m_vertexDistanceList.Clear();

			for(var i=1;i<handleArray.Length;i++)
			{
				var distance = (Vector3.Distance(handleArray[i],m_centerPosition)/m_resolution).ToLimit(5);

				m_vertexDistanceList.Add(Mathf.Clamp01(distance));
			}

			m_handleList.Clear();

			SetDirty();
		}

		private void _OnRefreshDistance()
		{
			if(SirenixEditorGUI.ToolbarButton(EditorIcons.Refresh))
			{
				SetDirty();
			}
		}

		[VerticalGroup("1",Order = 1),ShowInInspector,PropertyRange(3,360),HideIf(nameof(IsCurveMode))]
		public int SideCount
		{
			get => m_sideCount;
			private set
			{
				if(m_sideCount == value)
				{
					return;
				}

				m_sideCount = value;

				if(m_vertexDistanceList.Count < m_sideCount)
				{
					for(var i=m_vertexDistanceList.Count;i<m_sideCount;i++)
					{
						m_vertexDistanceList.Add(1.0f);
					}
				}
				else
				{
					var difference = m_vertexDistanceList.Count-m_sideCount;
					m_vertexDistanceList.RemoveRange(m_sideCount,difference);
				}

				SetDirty();
			}
		}

		[VerticalGroup("1",Order = 1),ShowInInspector,PropertyRange(-180.0f,+180.0f),HideIf(nameof(IsCurveMode))]
		public float Rotation
		{
			get => m_rotation;
			private set
			{
				if(m_rotation == value)
				{
					return;
				}

				m_rotation = value;

				SetDirty();
			}
		}

		[VerticalGroup("1",Order = 1),ShowInInspector,HideIf(nameof(IsCurveMode))]
		public bool IsReverse
		{
			get => m_reverse;
			private set
			{
				if(m_reverse == value)
				{
					return;
				}

				m_reverse = value;

				SetDirty();
			}
		}

		private void _ResetShape()
		{
			var handle = m_handleList.Count > 0 ? m_handleList[0] : Vector3.zero;
			var position = _ConvertPosition(handle);

			m_handleList.Clear();

			var camera = SceneView.lastActiveSceneView.camera;
			var pivot = (camera == null ? 1.0f : camera.orthographicSize)*0.5f;

			m_sideCount = 3;
			m_resolution = pivot;
			m_rotation = 0.0f;
			m_reverse = false;

			m_vertexDistanceList.Clear();

			m_centerPosition = position;

			for(var i=0;i<m_sideCount;i++)
			{
				m_vertexDistanceList.Add(1.0f);
			}
		}

		public void MovePolygon(int index,Vector3 position)
		{
			var convertPosition = _ConvertPosition(position);

			if(index == 0)
			{
				m_centerPosition = convertPosition;
			}
			else
			{
				var divide = m_vertexDistanceList[index-1];

				if(divide.ApproximatelyZero())
				{
					return;
				}

				Resolution = Vector3.Distance(m_centerPosition,convertPosition)/divide;
			}

			SetDirty();
		}

		private float _GetPolygonRotation(Vector3 direction)
		{
			return PathSpaceType == SpaceType.xz ? Mathf.Atan2(direction.z,direction.x)*Mathf.Rad2Deg : Mathf.Atan2(direction.y,direction.x)*Mathf.Rad2Deg;
		}

		private bool _GetPolygonReverse(Vector3 firstVertex,Vector3 secondVertex)
		{
			var edge0 = firstVertex-m_centerPosition;
			var edge1 = secondVertex-m_centerPosition;
			var cross = Vector3.Cross(edge1,edge0);

			return PathSpaceType == SpaceType.xz ? cross.y > 0.0f : cross.z > 0.0f;
		}

		private void _DrawShapeGizmos()
		{
			_DrawGizmoPathLines(true);
		}
	}
}
#endif
