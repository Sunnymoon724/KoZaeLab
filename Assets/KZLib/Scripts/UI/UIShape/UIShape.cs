
namespace UnityEngine.UI
{
	[RequireComponent(typeof(CanvasRenderer))]
	public class UIShape : MaskableGraphic
	{
		private enum ShapeType { Ellipse, Polygon, }
		private enum FillType { None, Matched, Solid, }

		[SerializeField] private ShapeType m_shapeType = ShapeType.Ellipse;

		[SerializeField] private float m_outlineSize = 1.0f;

		[SerializeField] private FillType m_fillType = FillType.Solid;

		#region Ellipse
		[SerializeField] private float m_ellipseAngle = 360.0f;
		#endregion Ellipse

		#region Polygon
		[SerializeField] private int m_polygonSideCount = 3;

		[SerializeField] private float[] m_polygonVertexDistanceArray = new float[] { 1,1,1, };
		#endregion Polygon

		[SerializeField] private Color m_fillColor = Color.white;

		protected override void OnPopulateMesh(VertexHelper vertexHelper)
		{
			vertexHelper.Clear();

			var rect = GetPixelAdjustedRect();
			var radius = new Vector2(rect.width/2.0f,rect.height/2.0f);
			var center = rect.center;
			var inner = new Vector2(radius.x-m_outlineSize,radius.y-m_outlineSize);

			if(m_shapeType == ShapeType.Ellipse)
			{
				if(m_ellipseAngle <= 0.0f)
				{
					return;
				}

				var resolution = 54;
				var size = Mathf.CeilToInt(resolution*m_ellipseAngle/Global.FULL_ANGLE);

				var angle = 2.0f*Mathf.PI/resolution;

				//? Draw Fill
				if(m_fillType != FillType.None)
				{
					var fillColor = _GetColor();

					_DrawPolygon(vertexHelper,size,angle,center,inner,fillColor,false);
				}

				//? Draw Outline
				if(color != Color.clear)
				{
					_DrawPolygonOutline(vertexHelper,size,angle,center,radius,inner,false);
				}
			}
			else if(m_shapeType == ShapeType.Polygon)
			{
				var angle = 2.0f*Mathf.PI/m_polygonSideCount;

				//? Draw Fill
				if(m_fillType != FillType.None)
				{
					var fillColor = _GetColor();

					_DrawPolygon(vertexHelper,m_polygonSideCount,angle,center,inner,fillColor,true);
				}

				//? Draw Outline
				if(color != Color.clear)
				{
					_DrawPolygonOutline(vertexHelper,m_polygonSideCount,angle,center,radius,inner,true);
				}
			}
		}

		private void _DrawPolygon(VertexHelper vertexHelper,int size,float angle,Vector2 center,Vector2 radius,Color color,bool useDistance)
		{
			var count = vertexHelper.currentVertCount;

			vertexHelper.AddVert(center,color,Vector2.zero);

			for(var i=0;i<=size;i++)
			{
				var distance = useDistance ? m_polygonVertexDistanceArray[i%size] : 1.0f;

				var cos = Mathf.Cos(angle*i)*distance;
				var sin = Mathf.Sin(angle*i)*distance;

				vertexHelper.AddVert(new (center.x+cos*radius.x,center.y+sin*radius.y),color,Vector2.zero);
			}

			for(var i=1;i<=size;i++)
			{
				vertexHelper.AddTriangle(count,count+i,count+i+1);
			}
		}

		private void _DrawPolygonOutline(VertexHelper vertexHelper,int size,float angle,Vector2 center,Vector2 radius,Vector2 inner,bool useDistance)
		{
			var count = vertexHelper.currentVertCount;

			for(var i=0;i<=size;i++)
			{
				var distance = useDistance ? m_polygonVertexDistanceArray[i%size] : 1.0f;

				var cos = Mathf.Cos(angle*i)*distance;
				var sin = Mathf.Sin(angle*i)*distance;

				vertexHelper.AddVert(new (center.x+cos*inner.x,center.y+sin*inner.y),color,Vector2.zero);
				vertexHelper.AddVert(new (center.x+cos*radius.x,center.y+sin*radius.y),color,Vector2.zero);
			}

			for(var i=0;i<size*2;i+=2)
			{
				vertexHelper.AddTriangle(count+i,count+i+1,count+i+2);
				vertexHelper.AddTriangle(count+i+1,count+i+2,count+i+3);
			}
		}

		private Color _GetColor()
		{
			if(m_fillType == FillType.Solid)
			{
				return m_fillColor;
			}
			else if(m_fillType == FillType.Matched)
			{
				return color;
			}
			else
			{
				return Color.clear;
			}
		}
	}
}