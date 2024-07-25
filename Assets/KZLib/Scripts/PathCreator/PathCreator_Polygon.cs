using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR

using UnityEditor;
using Sirenix.Utilities.Editor;

#endif

namespace KZLib.KZDevelop
{
	public partial class PathCreator : BaseComponent
	{
		[SerializeField,HideInInspector]
		private bool m_Reverse = false;
		[SerializeField,HideInInspector]
		private int m_SideCount = 3;
		[SerializeField,HideInInspector]
		private float m_Rotation = 0.0f;

		[SerializeField,HideInInspector]
		private Vector3 m_CenterPosition = Vector3.zero;
		public Vector3 CenterPosition => m_CenterPosition;

		[VerticalGroup("5",Order = 5),SerializeField,PropertyRange(0.0f,1.0f),ListDrawerSettings(DraggableItems = false,HideAddButton = true,HideRemoveButton = true,OnTitleBarGUI = nameof(OnRefreshDistance)),LabelText("정점 간 거리"),HideIf(nameof(IsCurveMode))]
		private List<float> m_VertexDistanceList = new();

		private void GetShapePointArray()
		{
			m_PointArray = new Vector3[m_SideCount];
			var degrees = (IsReverse ? -1 : +1)*Global.FULL_ANGLE/m_SideCount;

			for(var i=0;i<m_SideCount;i++)
			{
				var radian = Mathf.Deg2Rad*(i*degrees+Rotation);
				var distance = Resolution*m_VertexDistanceList[i];
				var x = Mathf.Cos(radian)*distance;
				var y = Mathf.Sin(radian)*distance;

				m_PointArray[i] = m_CenterPosition+new Vector3(x,y,0.0f);
			}

			m_HandleList.Clear();
			m_HandleList.Add(m_CenterPosition);
			m_HandleList.AddRange(m_PointArray);

			m_Closed = true;
		}

		public void SetPolygon(Vector3[] _handleArray,float _resolution)
		{
			m_DrawMode = PathDrawMode.Polygon;

			m_Closed = true;
			m_Resolution = _resolution;

			m_CenterPosition = _handleArray[0];
			m_SideCount = _handleArray.Length-1;

			var direction = _handleArray[1]-m_CenterPosition;

			m_Rotation = Mathf.Atan2(direction.y,direction.x)*Mathf.Rad2Deg;
			m_Reverse = Vector3.Cross(_handleArray[2],_handleArray[1]).z > 0.0f;

			m_VertexDistanceList.Clear();

			for(var i=1;i<_handleArray.Length;i++)
			{
				var distance = (Vector3.Distance(_handleArray[i],m_CenterPosition)/m_Resolution).ToLimit(5);

				m_VertexDistanceList.Add(Mathf.Clamp01(distance));
			}

			m_HandleList.Clear();

			SetDirty();
		}

		private void OnRefreshDistance()
		{
#if UNITY_EDITOR
			if(SirenixEditorGUI.ToolbarButton(EditorIcons.Refresh))
			{
				SetDirty();
			}
#endif
		}

		[VerticalGroup("1",Order = 1),ShowInInspector,PropertyRange(3,360),LabelText("사이드 갯수"),HideIf(nameof(IsCurveMode))]
		public int SideCount
		{
			get => m_SideCount;
			private set
			{
				if(m_SideCount == value)
				{
					return;
				}

				m_SideCount = value;

				if(m_VertexDistanceList.Count < m_SideCount)
				{
					for(var i=m_VertexDistanceList.Count;i<m_SideCount;i++)
					{
						m_VertexDistanceList.Add(1.0f);
					}
				}
				else
				{
					var difference = m_VertexDistanceList.Count-m_SideCount;
					m_VertexDistanceList.RemoveRange(m_SideCount,difference);
				}

				SetDirty();
			}
		}

		[VerticalGroup("1",Order = 1),ShowInInspector,PropertyRange(-180.0f,+180.0f),LabelText("각도"),HideIf(nameof(IsCurveMode))]
		public float Rotation
		{
			get => m_Rotation;
			private set
			{
				if(m_Rotation == value)
				{
					return;
				}

				m_Rotation = value;

				SetDirty();
			}
		}

		[VerticalGroup("1",Order = 1),ShowInInspector,LabelText("반전 여부"),HideIf(nameof(IsCurveMode))]
		public bool IsReverse
		{
			get => m_Reverse;
			private set
			{
				if(m_Reverse == value)
				{
					return;
				}

				m_Reverse = value;

				SetDirty();
			}
		}

#if UNITY_EDITOR
		private void ResetShape()
		{
			var handle = m_HandleList.Count > 0 ? m_HandleList[0] : Vector3.zero;
			var position = ConvertPosition(handle);

			m_HandleList.Clear();

			var camera = SceneView.lastActiveSceneView.camera;
			var pivot = (camera == null ? 1.0f : camera.orthographicSize)*0.5f;

			m_SideCount = 3;
			m_Resolution = pivot;
			m_Rotation = 0.0f;
			m_Reverse = false;

			m_VertexDistanceList.Clear();

			m_CenterPosition = position;

			for(var i=0;i<m_SideCount;i++)
			{
				m_VertexDistanceList.Add(1.0f);
			}
		}

		public void MovePolygon(int _index,Vector3 _position)
		{
			var position = ConvertPosition(_position);

			if(_index == 0)
			{
				m_CenterPosition = position;
			}
			else
			{
				var divide = m_VertexDistanceList[_index-1];

				if(divide.ApproximatelyZero())
				{
					return;
				}

				Resolution = Vector3.Distance(m_CenterPosition,position)/m_VertexDistanceList[_index-1];
			}

			SetDirty();
		}

		private void DrawShapeGizmos()
		{
			Gizmos.color = Color.green;

			for(var i=0;i<PointArray.Length-1;i++)
			{
				Gizmos.DrawLine(PointArray[i+0],PointArray[i+1]);
			}

			Gizmos.DrawLine(PointArray[^1],PointArray[0]);
		}
#endif
	}
}