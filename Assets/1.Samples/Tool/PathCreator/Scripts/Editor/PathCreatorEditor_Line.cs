#if UNITY_EDITOR
using UnityEngine;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace KZLib.KZEditor
{
	public partial class PathCreatorEditor : OdinEditor
	{
		private void DrawLine(Vector3[] _pointArray)
		{
			var cachedColor = Handles.color;
			var pathArray = m_Creator.PathArray;

			if(m_Creator.IsSplineCurve)
			{
				if(!m_Creator.IsClosed)
				{
					Handles.color = m_GuideLineColor;
					var length = _pointArray.Length;

					Handles.DrawDottedLine(_pointArray[0],_pointArray[1],5.0f);
					Handles.DrawDottedLine(_pointArray[length-2],_pointArray[length-1],5.0f);
				}

				Handles.color = m_NormalLineColor;

				for(var i=0;i<pathArray.Length-1;i++)
				{
					Handles.DrawLine(pathArray[i+0],pathArray[i+1]);
				}
			}
			else
			{
				Handles.color = m_GuideLineColor;
				var length = m_Creator.IsClosed ? _pointArray.Length/3 : (_pointArray.Length-1)/3;

				for(var i=0;i<length;i++)
				{
					Handles.DrawDottedLine(_pointArray[i*3+0],_pointArray[i*3+1],5.0f);
					Handles.DrawDottedLine(_pointArray[i*3+2],_pointArray[Tools.LoopClamp(i*3+3,_pointArray.Length)],5.0f);
				}

				Handles.color = m_NormalLineColor;

				for(var i=0;i<pathArray.Length-1;i++)
				{
					Handles.DrawLine(pathArray[i+0],pathArray[i+1]);
				}
			}

			Handles.color = cachedColor;
		}
	}
}
#endif