
namespace UnityEngine.UI
{
	[RequireComponent(typeof(CanvasRenderer))]
	public partial class UIShape : MaskableGraphic
	{
		private enum ShapeType { Ellipse, Polygon, } // Rectangle, }
		private enum FillType { None, Matched, Solid, }
		
		[SerializeField]
		private ShapeType m_shapeType = ShapeType.Ellipse;
		[SerializeField]
		private float m_outlineSize = 1.0f;

		[SerializeField]
		private FillType m_fillType = FillType.Solid;

		[SerializeField]
		private Color m_fillColor = Color.white;

		private bool IsEllipse => m_shapeType == ShapeType.Ellipse;
		private bool IsPolygon => m_shapeType == ShapeType.Polygon;
		// private bool IsRectangle => m_shapeType == ShapeType.Rectangle;

		protected override void OnPopulateMesh(VertexHelper vertexHelper)
		{
			vertexHelper.Clear();

			var rect = GetPixelAdjustedRect();

			var currentRadius = new Vector2(rect.width/2.0f,rect.height/2.0f);
			var centerPoint = rect.center;
			var innerRadius = new Vector2(currentRadius.x-m_outlineSize,currentRadius.y-m_outlineSize);

			_DrawShape(vertexHelper,centerPoint,currentRadius,innerRadius);
		}

		private void _DrawShape(VertexHelper vertexHelper,Vector2 centerPoint,Vector2 currentRadius,Vector2 innerRadius)
		{
			if(IsEllipse)
			{
				_DrawEllipse(vertexHelper,centerPoint,currentRadius,innerRadius);
			}
			else if(IsPolygon)
			{
				_DrawPolygon(vertexHelper,centerPoint,currentRadius,innerRadius);
			}
			// else if(IsRectangle)
			// {
			// 	_DrawRectangle(vertexHelper,centerPoint,currentRadius,innerRadius);
			// }
		}

		private void _DrawPolygonWithFill(VertexHelper vertexHelper,int segmentCount,float segmentAngle,Vector2 centerPoint,Vector2 currentRadius,Color drawColor,bool useDistance)
		{
			var count = vertexHelper.currentVertCount;

			vertexHelper.AddVert(centerPoint,drawColor,Vector2.zero);

			for(var i=0;i<=segmentCount;i++)
			{
				var distance = useDistance ? m_polygonVertexDistanceArray[i%segmentCount] : 1.0f;

				var cos = Mathf.Cos(segmentAngle*i)*distance;
				var sin = Mathf.Sin(segmentAngle*i)*distance;

				vertexHelper.AddVert(new (centerPoint.x+cos*currentRadius.x,centerPoint.y+sin*currentRadius.y),drawColor,Vector2.zero);
			}

			for(var i=1;i<=segmentCount;i++)
			{
				vertexHelper.AddTriangle(count,count+i,count+i+1);
			}
		}

		private void _DrawPolygonWithOutline(VertexHelper vertexHelper,int segmentCount,float segmentAngle,Vector2 centerPoint,Vector2 currentRadius,Vector2 innerRadius,bool useDistance)
		{
			// Outline use only base color
			var count = vertexHelper.currentVertCount;

			for(var i=0;i<=segmentCount;i++)
			{
				var distanceRatio = useDistance ? m_polygonVertexDistanceArray[i%segmentCount] : 1.0f;

				var cosAngle = Mathf.Cos(segmentAngle*i)*distanceRatio;
				var sinAngle = Mathf.Sin(segmentAngle*i)*distanceRatio;

				vertexHelper.AddVert(new (centerPoint.x+cosAngle*innerRadius.x,centerPoint.y+sinAngle*innerRadius.y),color,Vector2.zero);
				vertexHelper.AddVert(new (centerPoint.x+cosAngle*currentRadius.x,centerPoint.y+sinAngle*currentRadius.y),color,Vector2.zero);
			}

			for(var i=0;i<segmentCount*2;i+=2)
			{
				vertexHelper.AddTriangle(count+i,count+i+1,count+i+2);
				vertexHelper.AddTriangle(count+i+1,count+i+2,count+i+3);
			}
		}

		private Color _GetColor()
		{
			return m_fillType switch
			{
				FillType.Solid => m_fillColor,
				FillType.Matched => color,
				_ => Color.clear,
			};
		}
	}
}