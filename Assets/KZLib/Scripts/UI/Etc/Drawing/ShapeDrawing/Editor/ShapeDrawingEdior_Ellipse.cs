#if UNITY_EDITOR
using UnityEngine;

namespace KZLib.UI
{
	public partial class ShapeDrawingEditor : GraphicDrawingEditor
	{
		private bool _CanShowKnot_Ellipse()
		{
			return true;
		}

		private void _Draw_Ellipse()
		{
			void _SetAngle(float angle)
			{
				m_shapeDrawing.EllipseAngle = angle;
			}

			_DrawSliderInspector("Angle",m_shapeDrawing.EllipseAngle,ShapeDrawing.c_zeroAngle,ShapeDrawing.c_fullAngle,_SetAngle);
		}

		private void _SyncAllKnotInfos_Ellipse()
		{
			var fixedPos = _GetVertexPosition(ShapeDrawing.c_zeroAngle);

			_SyncWorldKnotInfo(1,fixedPos,KnotType.Fixed);

			var majorPos = _GetVertexPosition(m_shapeDrawing.EllipseAngle);

			_SyncWorldKnotInfo(2,majorPos,KnotType.Major);
		}

		private void _RefreshKnotInfo_Ellipse(int _,Vector3 position)
		{
			var angle = Mathf.Atan2(position.y,position.x)*Mathf.Rad2Deg;

			if(angle < ShapeDrawing.c_zeroAngle)
			{
				angle += ShapeDrawing.c_fullAngle;
			}

			m_shapeDrawing.EllipseAngle = angle;
		}
	}
}
#endif