
namespace UnityEngine.UI
{
	public partial class ShapeDrawing : GraphicDrawing
	{
		[SerializeField]
		private int m_polygonSideCount = Global.MIN_POLYGON_COUNT;
		private int PolygonSideCount
		{
			get => m_polygonSideCount;
			set
			{
				if(m_polygonSideCount == value)
				{
					return;
				}

				var vertexCount = _GetExpectedVerticesInPolygon(value,FillType,FillColor,OutlineThickness,OutlineColor);

				if(!_IsValidVertex(vertexCount))
				{
					return;
				}

				m_polygonSideCount = value;

				SetVerticesDirty();
			}
		}

		[SerializeField]
		private float[] m_polygonVertexDistanceArray = new float[] { 1,1,1, };

		private void _DrawPolygon(VertexHelper vertexHelper,Vector2 centerPoint,Vector2 currentRadius,Vector2 innerRadius)
		{
			var segmentAngle = 2.0f*Mathf.PI/PolygonSideCount;

			_DrawCommonShape(vertexHelper,PolygonSideCount,segmentAngle,centerPoint,currentRadius,innerRadius,true);
		}

		private int _GetExpectedVerticesInPolygon(int sideCount,ShapeFillType fillType,Color fillColor,float outlineThickness,Color outlineColor)
		{
			return _GetExpectedVerticesInCommonShape(sideCount,fillType,fillColor,outlineThickness,outlineColor);
		}
	}
}