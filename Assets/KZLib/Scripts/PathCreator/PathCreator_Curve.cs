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
		[SerializeField,HideInInspector]
		private bool m_Closed = false;

		private void GetCurvePointArray()
		{
			m_PointArray = CommonUtility.GetCubicBezierCurve(HandleArray,IsClosed,m_Resolution) ?? new Vector3[0];
		}

		public void SetCurve(Vector3[] _handleArray,bool _isClosed,float _resolution)
		{
			m_DrawMode = PathDrawMode.Curve;

			m_Closed = _isClosed;
			m_Resolution = _resolution;

			m_HandleList.Clear();
			m_HandleList.AddRange(_handleArray);

			SetDirty();
		}


		[VerticalGroup("1",Order = 1),ShowInInspector,LabelText("폐쇄 여부"),ShowIf(nameof(IsCurveMode))]
		public bool IsClosed
		{
			get => m_Closed;
			private set
			{
				if(m_Closed == value)
				{
					return;
				}

				m_Closed = value;

				if(IsClosed)
				{
					m_HandleList.Add(m_HandleList[^1]*2-m_HandleList[^2]);
					m_HandleList.Add(m_HandleList[0]*2-m_HandleList[1]);
				}
				else
				{
					m_HandleList.RemoveRange(m_HandleList.Count-2,2);
				}

				SetDirty();
			}
		}

#if UNITY_EDITOR
		private void ResetCurve()
		{
			var handle = m_HandleList.Count > 0 ? m_HandleList[0] : Vector3.zero;

			m_Resolution = 50.0f;
			m_HandleList.Clear();

			var position = ConvertPosition(handle);

			var sceneCamera = SceneView.lastActiveSceneView.camera;
			var pivot = (sceneCamera == null ? 1.0f : sceneCamera.orthographicSize)*0.5f;

			m_HandleList.AddRange(new List<Vector3>()
			{
				position+new Vector3(-pivot,+0.0f,0.0f),
				position+new Vector3(-pivot,-pivot,0.0f),
				position+new Vector3(+pivot,+pivot,0.0f),
				position+new Vector3(+pivot,+0.0f,0.0f),
			});

			if(IsClosed)
			{
				m_HandleList.Add(position+new Vector3(+pivot,-pivot,0.0f));
				m_HandleList.Add(position+new Vector3(-pivot,+pivot,0.0f));
			}
		}

		public void AddAnchor(Vector3 _position)
		{
			var control1 = 2*m_HandleList[^1]-m_HandleList[^2];

			if(IsClosed)
			{
				var control0 = m_HandleList[^1];

				m_HandleList[^1] = (_position+m_HandleList[^2])*0.5f;

				m_HandleList.Add(_position);
				m_HandleList.Add(control1);
				m_HandleList.Add(control0);
			}
			else
			{
				var control2 = (_position+control1)*0.5f;

				m_HandleList.Add(control1);
				m_HandleList.Add(control2);
				m_HandleList.Add(_position);
			}

			SetDirty();
		}

		public void InsertAnchor(int _index,Vector3 _position)
		{
			if(_index == m_HandleList.Count-1)
			{
				AddAnchor(_position);

				return;
			}

			var index = _index+1;

			var control = (_position+m_HandleList[index])*0.5f;

			m_HandleList.Insert(index+1,control);
			m_HandleList.Insert(index+2,_position);
			m_HandleList.Insert(index+3,2*_position-control);

			SetDirty();
		}

		public void RemoveAnchor(int _index)
		{
			if(m_HandleList.Count <= (IsClosed ? 6 : 4))
			{
				return;
			}

			if(_index%3 != 0)
			{
				return;
			}

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

			SetDirty();
		}

		public void MoveCurve(int _index,Vector3 _position,bool _isFree)
		{
			if(!m_HandleList.ContainsIndex(_index))
			{
				return;
			}

			var position = ConvertPosition(_position);
			var deltaMove = position-m_HandleList[_index];

			m_HandleList[_index] = position;

			var length = m_HandleList.Count;

			//? 앵커 이후 설정
			if(_index%3 == 0)
			{
				// 이전 컨트롤 점
				if(_index+1 < length || IsClosed)
				{
					m_HandleList[CommonUtility.LoopClamp(_index+1,length)] += deltaMove;
				}

				// 이후 컨트롤 점
				if(_index-1 >= 0 || IsClosed)
				{
					m_HandleList[CommonUtility.LoopClamp(_index-1,length)] += deltaMove;
				}
			}
			//? 반대쪽 컨트롤 설정
			else if(!_isFree)
			{
				var nextPointIsAnchor = (_index+1)%3 == 0;
				var controlIndex = nextPointIsAnchor ? _index+2 : _index-2;
				var anchorIndex = nextPointIsAnchor ? _index+1 : _index-1;

				if(controlIndex >= 0 && controlIndex < length || IsClosed)
				{
					var anchor = m_HandleList[CommonUtility.LoopClamp(anchorIndex,length)];
					var index = CommonUtility.LoopClamp(controlIndex,length);

					m_HandleList[index] = anchor+(anchor-position).normalized*(anchor-m_HandleList[index]).magnitude;
				}
			}

			SetDirty();
		}

		private void DrawCurveGizmos()
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