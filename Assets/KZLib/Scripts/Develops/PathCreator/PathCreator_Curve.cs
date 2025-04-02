using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace KZLib.KZDevelop
{
	public partial class PathCreator : BaseComponent
	{
		private void _GetCurvePointArray()
		{
			m_pointArray = CommonUtility.CalculateCubicBezierCurve(HandleArray,IsClosed,m_resolution) ?? new Vector3[0];
		}

		public void SetCurve(Vector3[] handleArray,bool isClosed,float resolution)
		{
			m_drawMode = PathDrawMode.Curve;

			m_closed = isClosed;
			m_resolution = resolution;

			m_handleList.Clear();
			m_handleList.AddRange(handleArray);

			SetDirty();
		}


		[VerticalGroup("1",Order = 1),ShowInInspector,ShowIf(nameof(IsCurveMode))]
		public bool IsClosed
		{
			get => m_closed;
			private set
			{
				if(m_closed == value)
				{
					return;
				}

				m_closed = value;

				if(IsClosed)
				{
					m_handleList.Add(m_handleList[^1]*2-m_handleList[^2]);
					m_handleList.Add(m_handleList[0]*2-m_handleList[1]);
				}
				else
				{
					m_handleList.RemoveRange(m_handleList.Count-2,2);
				}

				SetDirty();
			}
		}

#if UNITY_EDITOR
		private void _ResetCurve()
		{
			var handle = m_handleList.Count > 0 ? m_handleList[0] : Vector3.zero;

			m_resolution = 50.0f;
			m_handleList.Clear();

			var position = _ConvertPosition(handle);

			var sceneCamera = SceneView.lastActiveSceneView.camera;
			var pivot = (sceneCamera == null ? 1.0f : sceneCamera.orthographicSize)*0.5f;

			m_handleList.AddRange(new List<Vector3>()
			{
				position+new Vector3(-pivot,+0.0f,0.0f),
				position+new Vector3(-pivot,-pivot,0.0f),
				position+new Vector3(+pivot,+pivot,0.0f),
				position+new Vector3(+pivot,+0.0f,0.0f),
			});

			if(IsClosed)
			{
				m_handleList.Add(position+new Vector3(+pivot,-pivot,0.0f));
				m_handleList.Add(position+new Vector3(-pivot,+pivot,0.0f));
			}
		}

		public void AddAnchor(Vector3 position)
		{
			var control1 = 2*m_handleList[^1]-m_handleList[^2];

			if(IsClosed)
			{
				var control0 = m_handleList[^1];

				m_handleList[^1] = (position+m_handleList[^2])*0.5f;

				m_handleList.Add(position);
				m_handleList.Add(control1);
				m_handleList.Add(control0);
			}
			else
			{
				var control2 = (position+control1)*0.5f;

				m_handleList.Add(control1);
				m_handleList.Add(control2);
				m_handleList.Add(position);
			}

			SetDirty();
		}

		public void InsertAnchor(int index,Vector3 position)
		{
			if(index == m_handleList.Count-1)
			{
				AddAnchor(position);

				return;
			}

			var newIndex = index+1;

			var control = (position+m_handleList[newIndex])*0.5f;

			m_handleList.Insert(newIndex+1,control);
			m_handleList.Insert(newIndex+2,position);
			m_handleList.Insert(newIndex+3,2*position-control);

			SetDirty();
		}

		public void RemoveAnchor(int index)
		{
			if(m_handleList.Count <= (IsClosed ? 6 : 4))
			{
				return;
			}

			if(index%3 != 0)
			{
				return;
			}

			if(index == 0)
			{
				if(IsClosed)
				{
					m_handleList[^1] = m_handleList[2];
				}

				m_handleList.RemoveRange(0,3);
			}
			else if(index == m_handleList.Count-1 && !IsClosed)
			{
				m_handleList.RemoveRange(index-2,3);
			}
			else
			{
				m_handleList.RemoveRange(index-1,3);
			}

			SetDirty();
		}

		public void MoveCurve(int index,Vector3 position,bool isFree)
		{
			if(!m_handleList.ContainsIndex(index))
			{
				return;
			}

			var convertPosition = _ConvertPosition(position);
			var deltaMove = convertPosition-m_handleList[index];

			m_handleList[index] = convertPosition;

			var length = m_handleList.Count;

			//? 앵커 이후 설정
			if(index%3 == 0)
			{
				// 이전 컨트롤 점
				if(index+1 < length || IsClosed)
				{
					m_handleList[CommonUtility.LoopClamp(index+1,length)] += deltaMove;
				}

				// 이후 컨트롤 점
				if(index-1 >= 0 || IsClosed)
				{
					m_handleList[CommonUtility.LoopClamp(index-1,length)] += deltaMove;
				}
			}
			//? 반대쪽 컨트롤 설정
			else if(!isFree)
			{
				var nextPointIsAnchor = (index+1)%3 == 0;
				var controlIndex = nextPointIsAnchor ? index+2 : index-2;
				var anchorIndex = nextPointIsAnchor ? index+1 : index-1;

				if(controlIndex >= 0 && controlIndex < length || IsClosed)
				{
					var anchor = m_handleList[CommonUtility.LoopClamp(anchorIndex,length)];
					var newIndex = CommonUtility.LoopClamp(controlIndex,length);

					m_handleList[newIndex] = anchor+(anchor-convertPosition).normalized*(anchor-m_handleList[newIndex]).magnitude;
				}
			}

			SetDirty();
		}

		private void _DrawCurveGizmos()
		{
			Gizmos.color = Color.green;

			for(var i=0;i<PointArray.Length-1;i++)
			{
				Gizmos.DrawLine(PointArray[i+0],PointArray[i+1]);
			}
		}
#endif
	}
}