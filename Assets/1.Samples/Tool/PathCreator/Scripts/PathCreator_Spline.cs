#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace KZLib.KZDevelop
{
	public partial class PathCreator : BaseComponent
	{
		private void ResetSpline(bool _is2D)
		{
			var position = _is2D ? transform.position.MaskZ() : transform.position;

			m_HandleList.AddRange(new List<Vector3>()
			{
				position+new Vector3(-1.0f,-1.0f,0.0f),
				position+new Vector3(-1.0f,+0.0f,0.0f),
				position+new Vector3(+1.0f,+0.0f,0.0f),
				position+new Vector3(+1.0f,+1.0f,0.0f),
			});
		}

		private void AddSpline(Vector3 _position)
		{
			m_HandleList.Add(_position);
		}

		private void InsertSpline(int _index,Vector3 _position)
		{
			m_HandleList.Insert(_index+1,_position);
		}

		private bool RemoveSpline(int _index)
		{
			m_HandleList.RemoveAt(_index);

			return true;
		}

		private void MoveSpline(int _index,Vector3 _position)
		{
			m_HandleList[_index] = _position;
		}
	}
}
#endif