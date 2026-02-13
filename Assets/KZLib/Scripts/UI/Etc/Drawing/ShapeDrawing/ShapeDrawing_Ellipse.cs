using UnityEngine;
using UnityEngine.UI;

namespace KZLib.UI
{
	public partial class ShapeDrawing : GraphicDrawing
	{
		private const int c_pivotResolution = 54;

		[SerializeField]
		private float m_ellipseAngle = Global.FULL_ANGLE;
		internal float EllipseAngle
		{
			get => m_ellipseAngle;
			set
			{
				if(m_ellipseAngle == value)
				{
					return;
				}

				var vertexCount = _CalculateExpectedVertices_Ellipse(value,FillType,FillColor,OutlineThickness,OutlineColor);

				if(!_IsValidVertex(vertexCount))
				{
					return;
				}

				m_ellipseAngle = value;

				SetVerticesDirty();
			}
		}

		private void _DrawShape_Ellipse(VertexHelper vertexHelper,Vector2 centerPoint,Vector2 currentRadius,Vector2 innerRadius)
		{
			var segmentCount = _GetSegmentCount_Ellipse(EllipseAngle);
			var segmentAngle = EllipseAngle*Mathf.Deg2Rad/segmentCount;

			_DrawCommonShape(vertexHelper,segmentCount,segmentAngle,centerPoint,currentRadius,innerRadius,false);
		}

		private int _CalculateExpectedVertices_Ellipse(float angle,ShapeFillType fillType,Color fillColor,float outlineThickness,Color outlineColor)
		{
			var segmentCount = _GetSegmentCount_Ellipse(angle);

			return _CalculateExpectedVertices_Shape(segmentCount,fillType,fillColor,outlineThickness,outlineColor);
		}

		private int _GetSegmentCount_Ellipse(float angle)
		{
			angle = Mathf.Clamp(angle,Global.ZERO_ANGLE,Global.FULL_ANGLE);

			return Mathf.CeilToInt(c_pivotResolution*angle/Global.FULL_ANGLE);
		}
	}
}