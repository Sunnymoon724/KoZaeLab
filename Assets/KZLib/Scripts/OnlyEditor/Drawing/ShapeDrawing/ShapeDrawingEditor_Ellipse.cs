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
			_SyncWorldKnotInfo(1,m_shapeDrawing._GetEllipseOffsetAtAngle(ShapeDrawing.c_zeroAngle),KnotType.Fixed);

			var ellipseAngle = m_shapeDrawing.EllipseAngle;

			_SyncWorldKnotInfo(2,m_shapeDrawing._GetEllipseOffsetAtAngle(ellipseAngle),KnotType.Major);
		}

		private void _RefreshKnotInfo_Ellipse(int _,Vector3 position)
		{
			var radius = m_shapeDrawing.Radius;
			var angle = Mathf.Atan2(position.y/radius.y,position.x/radius.x)*Mathf.Rad2Deg;

			m_shapeDrawing.EllipseAngle = m_shapeDrawing._ClampEllipseEditorAngle(angle);
		}
	}
}
#endif