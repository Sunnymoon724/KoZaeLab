#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace KZLib.KZDevelop
{
	public partial class PathCreator : BaseComponent
	{
		private void ResetBezier(bool _is2D,bool _isClosed)
		{
			var position = _is2D ? transform.position.MaskZ() : transform.position;

			if(_isClosed)
			{
				m_HandleList.AddRange(new List<Vector3>()
				{
					position+new Vector3(-1.0f,+0.0f,0.0f),
					position+new Vector3(-1.0f,-1.0f,0.0f),
					position+new Vector3(+1.0f,+1.0f,0.0f),
					position+new Vector3(+1.0f,+0.0f,0.0f),
					position+new Vector3(+1.0f,-1.0f,0.0f),
					position+new Vector3(-1.0f,+1.0f,0.0f),
				});
			}
			else
			{
				m_HandleList.AddRange(new List<Vector3>()
				{
					position+new Vector3(-1.0f,+0.0f,0.0f),
					position+new Vector3(-1.0f,-1.0f,0.0f),
					position+new Vector3(+1.0f,+1.0f,0.0f),
					position+new Vector3(+1.0f,+0.0f,0.0f),
				});
			}
		}

		private void AddBezier(Vector3 _position)
		{
			if(IsClosed)
			{
				var control0 = m_HandleList[^1];

				m_HandleList[^1] = (_position+m_HandleList[^2])*0.5f;

				m_HandleList.Add(_position);
				m_HandleList.Add(m_HandleList[^1]*2-m_HandleList[^2]);
				m_HandleList.Add(control0);
			}
			else
			{
				var control1 = 2*m_HandleList[^1]-m_HandleList[^2];
				var control2 = (_position+control1)*0.5f;

				m_HandleList.Add(control1);
				m_HandleList.Add(control2);
				m_HandleList.Add(_position);
			}
		}

		private void InsertBezier(int _index,Vector3 _position)
		{
			var control1 = (_position+m_HandleList[_index+1])*0.5f;

			m_HandleList.Insert(_index+2,(_position+m_HandleList[_index+1])*0.5f);
			m_HandleList.Insert(_index+3,_position);
			m_HandleList.Insert(_index+4,2*_position-control1);
		}

		private bool RemoveBezier(int _index)
		{
			if(_index%3 != 0)
			{
				return false;
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

			return true;
		}

		private void MoveBezier(int _index,Vector3 _position,bool _isFree)
		{
			var length = m_HandleList.Count;
			var deltaMove = _position-m_HandleList[_index];

			m_HandleList[_index] = _position;

			if(_index%3 == 0)
			{
				if(_index+1 < length || IsClosed)
				{
					m_HandleList[Tools.LoopClamp(_index+1,length)] += deltaMove;
				}

				if(_index-1 >= 0 || IsClosed)
				{
					m_HandleList[Tools.LoopClamp(_index-1,length)] += deltaMove;
				}
			}
			else if(!_isFree)
			{
				var nextPointIsAnchor = (_index+1)%3 == 0;
				var controlIndex = nextPointIsAnchor ? _index+2 : _index-2;
				var anchorIndex = nextPointIsAnchor ? _index+1 : _index-1;

				if(controlIndex >= 0 && controlIndex < length || IsClosed)
				{
					var anchor = m_HandleList[Tools.LoopClamp(anchorIndex,length)];
					var index = Tools.LoopClamp(controlIndex,length);

					m_HandleList[index] = anchor+(anchor-_position).normalized*(anchor-m_HandleList[index]).magnitude;
				}
			}
		}
	}
}
#endif