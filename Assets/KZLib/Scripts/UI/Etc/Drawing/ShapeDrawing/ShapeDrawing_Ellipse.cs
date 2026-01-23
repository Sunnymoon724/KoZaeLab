
namespace UnityEngine.UI
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

				var vertexCount = _GetExpectedVerticesInEllipse(value,FillType,FillColor,OutlineThickness,OutlineColor);

				if(!_IsValidVertex(vertexCount))
				{
					return;
				}

				m_ellipseAngle = value;

				SetVerticesDirty();
			}
		}

		private void _DrawEllipse(VertexHelper vertexHelper,Vector2 centerPoint,Vector2 currentRadius,Vector2 innerRadius)
		{
			var segmentCount = _GetSegmentCountInEllipse(EllipseAngle);
			var segmentAngle = 2.0f*Mathf.PI/c_pivotResolution;

			_DrawCommonShape(vertexHelper,segmentCount,segmentAngle,centerPoint,currentRadius,innerRadius,false);
		}

		private int _GetExpectedVerticesInEllipse(float angle,ShapeFillType fillType,Color fillColor,float outlineThickness,Color outlineColor)
		{
			var segmentCount = _GetSegmentCountInEllipse(angle);

			return _GetExpectedVerticesInCommonShape(segmentCount,fillType,fillColor,outlineThickness,outlineColor);
		}

		private int _GetSegmentCountInEllipse(float angle)
		{
			angle = Mathf.Clamp(angle,Global.ZERO_ANGLE,Global.FULL_ANGLE);

			return Mathf.CeilToInt(c_pivotResolution*angle/Global.FULL_ANGLE);
		}
	}
}