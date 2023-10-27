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
			var length = _pointArray.Length;
			var start = m_Creator.IsClosed ? 0 : 1;
			var finish = m_Creator.IsClosed ? length : length-2;

			var cachedColor = Handles.color;

			for(var i=start;i<finish;i++)
			{
				var dataArray = Tools.GetCatmullRomSplineCurve(_pointArray[Tools.LoopClamp(i-1,length)],_pointArray[Tools.LoopClamp(i+0,length)],_pointArray[Tools.LoopClamp(i+1,length)],_pointArray[Tools.LoopClamp(i+2,length)]);

				for(var j=0;j<dataArray.Length;j++)
				{
					dataArray[j] = Tools.TransformPoint(dataArray[j],m_Creator.transform,m_Creator.PathSpaceType);
				}

				Handles.color = i == m_SelectedSegmentIndex && Event.current.shift && m_DraggingHandleIndex == -1 && m_MouseOverHandleIndex == -1 ? EditorCustom.EditorPath.LineHighlightColor : EditorCustom.EditorPath.LineNormalColor;

				for(var j=0;j<dataArray.Length-1;j++)
				{
					Handles.DrawLine(dataArray[j+0],dataArray[j+1]);
				}
			}

			Handles.color = cachedColor;
		}
	}
}
#endif