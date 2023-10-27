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
	}
}
#endif