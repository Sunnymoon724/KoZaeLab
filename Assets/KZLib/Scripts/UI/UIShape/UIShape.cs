
namespace UnityEngine.UI
{
	[RequireComponent(typeof(CanvasRenderer))]
	public class UIShape : MaskableGraphic
	{
		private enum ShapeType { Ellipse, Polygon, }
		private enum FillType { None, Matched, Solid, }

		[SerializeField] private ShapeType m_ShapeType = ShapeType.Ellipse;

		[SerializeField] private float m_OutlineSize = 1.0f;

		[SerializeField] private FillType m_FillType = FillType.Solid;

		#region Ellipse
		[SerializeField] private float m_EllipseAngle = 360.0f;
		#endregion Ellipse

		#region Polygon
		[SerializeField] private int m_PolygonSideCount = 3;

		[SerializeField] private float[] m_PolygonVertexDistanceArray = new float[] { 1,1,1, };
		#endregion Polygon

		[SerializeField] private Color m_FillColor = Color.white;

		protected override void OnPopulateMesh(VertexHelper _vertexHelper)
		{
			_vertexHelper.Clear();

			var rect = GetPixelAdjustedRect();
			var radius = new Vector2(rect.width/2.0f,rect.height/2.0f);
			var center = rect.center;
			var inner = new Vector2(radius.x-m_OutlineSize,radius.y-m_OutlineSize);

			if(m_ShapeType == ShapeType.Ellipse)
			{
				if(m_EllipseAngle <= 0.0f)
				{
					return;
				}

				var resolution = 54;
				var size = Mathf.CeilToInt(resolution*m_EllipseAngle/Global.FULL_ANGLE);

				var angle = 2.0f*Mathf.PI/resolution;

				//? Draw Fill
				if(m_FillType != FillType.None)
				{
					var fillColor = GetColor();

					DrawPolygon(_vertexHelper,size,angle,center,inner,fillColor,false);
				}

				//? Draw Outline
				if(color != Color.clear)
				{
					DrawPolygonOutline(_vertexHelper,size,angle,center,radius,inner,false);
				}
			}
			else if(m_ShapeType == ShapeType.Polygon)
			{
				var angle = 2.0f*Mathf.PI/m_PolygonSideCount;

				//? Draw Fill
				if(m_FillType != FillType.None)
				{
					var fillColor = GetColor();

					DrawPolygon(_vertexHelper,m_PolygonSideCount,angle,center,inner,fillColor,true);
				}

				//? Draw Outline
				if(color != Color.clear)
				{
					DrawPolygonOutline(_vertexHelper,m_PolygonSideCount,angle,center,radius,inner,true);
				}
			}
		}

		private void DrawPolygon(VertexHelper _vertexHelper,int _size,float _angle,Vector2 _center,Vector2 _radius,Color _color,bool _useDistance)
		{
			var count = _vertexHelper.currentVertCount;

			_vertexHelper.AddVert(_center,_color,Vector2.zero);

			for(var i=0;i<=_size;i++)
			{
				var distance = _useDistance ? m_PolygonVertexDistanceArray[i%_size] : 1.0f;

				var cos = Mathf.Cos(_angle*i)*distance;
				var sin = Mathf.Sin(_angle*i)*distance;

				_vertexHelper.AddVert(new (_center.x+cos*_radius.x,_center.y+sin*_radius.y),_color,Vector2.zero);
			}

			for(var i=1;i<=_size;i++)
			{
				_vertexHelper.AddTriangle(count,count+i,count+i+1);
			}
		}

		private void DrawPolygonOutline(VertexHelper _vertexHelper,int _size,float _angle,Vector2 _center,Vector2 _radius,Vector2 _inner,bool _useDistance)
		{
			var count = _vertexHelper.currentVertCount;

			for(var i=0;i<=_size;i++)
			{
				var distance = _useDistance ? m_PolygonVertexDistanceArray[i%_size] : 1.0f;

				var cos = Mathf.Cos(_angle*i)*distance;
				var sin = Mathf.Sin(_angle*i)*distance;

				_vertexHelper.AddVert(new (_center.x+cos*_inner.x,_center.y+sin*_inner.y),color,Vector2.zero);
				_vertexHelper.AddVert(new (_center.x+cos*_radius.x,_center.y+sin*_radius.y),color,Vector2.zero);
			}

			for(var i=0;i<_size*2;i+=2)
			{
				_vertexHelper.AddTriangle(count+i,count+i+1,count+i+2);
				_vertexHelper.AddTriangle(count+i+1,count+i+2,count+i+3);
			}
		}

		private Color GetColor()
		{
			if(m_FillType == FillType.Solid)
			{
				return m_FillColor;
			}
			else if(m_FillType == FillType.Matched)
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