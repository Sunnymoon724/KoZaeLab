#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using KZLib.KZEditor;
using UnityEditor;
using System;

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
	}
}
#endif