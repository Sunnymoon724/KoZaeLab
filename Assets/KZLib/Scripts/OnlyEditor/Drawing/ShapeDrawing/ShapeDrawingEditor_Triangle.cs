#if UNITY_EDITOR
using UnityEngine;

namespace KZLib.UI
{
	public partial class ShapeDrawingEditor : GraphicDrawingEditor
	{
		private bool _CanShowKnot_Triangle()
		{
			return true;
		}

		private void _Draw_Triangle()
		{
			void _SetOffset(float offset)
			{
				m_shapeDrawing.TriangleOffset = offset;
			}

			_DrawSliderInspector("Offset",m_shapeDrawing.TriangleOffset,ShapeDrawing.c_minTriangleOffset,ShapeDrawing.c_maxTriangleOffset,_SetOffset);
		}

		private void _SyncAllKnotInfos_Triangle()
		{
			var cornerArray = m_shapeDrawing._GetCornerArray();

			_SyncWorldKnotInfo(1,cornerArray[2],KnotType.Fixed);
			_SyncWorldKnotInfo(2,cornerArray[3],KnotType.Fixed);

			var topPos = Vector3.Lerp(cornerArray[0],cornerArray[1],m_shapeDrawing.TriangleOffset);

			_SyncWorldKnotInfo(3,topPos,KnotType.Major);
		}

		private void _RefreshKnotInfo_Triangle(int _,Vector3 position)
		{
			var radius = m_shapeDrawing.Radius.x;
			var ratio = (position.x+radius)/(radius*2.0f);

			m_shapeDrawing.TriangleOffset = Mathf.Clamp01(ratio);
		}
	}
}
#endif