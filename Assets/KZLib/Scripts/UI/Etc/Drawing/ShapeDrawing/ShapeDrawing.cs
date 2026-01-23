
namespace UnityEngine.UI
{
	public partial class ShapeDrawing : GraphicDrawing
	{
		internal enum ShapePrimitiveType { Ellipse, Polygon, } // Rectangle, }
		internal enum ShapeFillType { None, Matched, Solid, }

		[SerializeField]
		private ShapePrimitiveType m_primitiveType = ShapePrimitiveType.Ellipse;
		internal ShapePrimitiveType PrimitiveType
		{
			get => m_primitiveType;
			set
			{
				if(m_primitiveType == value)
				{
					return;
				}

				var vertexCount = _GetTotalExpectedVertices(value,FillType,FillColor,OutlineThickness,OutlineColor);

				if(!_IsValidVertex(vertexCount))
				{
					return;
				}

				m_primitiveType = value;

				SetVerticesDirty();
			}
		}

		[SerializeField]
		private float m_outlineThickness = 1.0f;
		internal float OutlineThickness
		{
			get => m_outlineThickness;
			set
			{
				if(m_outlineThickness == value)
				{
					return;
				}

				var vertexCount = _GetTotalExpectedVertices(PrimitiveType,FillType,FillColor,value,OutlineColor);

				if(!_IsValidVertex(vertexCount))
				{
					return;
				}

				m_outlineThickness = value;

				SetVerticesDirty();
			}
		}

		[SerializeField]
		private Color m_outlineColor = Color.white;
		internal Color OutlineColor
		{
			get => m_outlineColor;
			set
			{
				if(m_outlineColor == value)
				{
					return;
				}

				var vertexCount = _GetTotalExpectedVertices(PrimitiveType,FillType,FillColor,OutlineThickness,value);

				if(!_IsValidVertex(vertexCount))
				{
					return;
				}

				m_outlineColor = value;

				SetVerticesDirty();
			}
		}

		[SerializeField]
		private ShapeFillType m_fillType = ShapeFillType.Solid;
		internal ShapeFillType FillType
		{
			get => m_fillType;
			set
			{
				if(m_fillType == value)
				{
					return;
				}

				var vertexCount = _GetTotalExpectedVertices(PrimitiveType,value,FillColor,OutlineThickness,OutlineColor);

				if(!_IsValidVertex(vertexCount))
				{
					return;
				}

				m_fillType = value;

				SetVerticesDirty();
			}
		}

		[SerializeField]
		private Color m_fillColor = Color.white;
		internal Color FillColor
		{
			get => m_fillColor;
			set
			{
				if(m_fillColor == value)
				{
					return;
				}

				var vertexCount = _GetTotalExpectedVertices(PrimitiveType,FillType,value,OutlineThickness,OutlineColor);

				if(!_IsValidVertex(vertexCount))
				{
					return;
				}

				m_fillColor = value;

				SetVerticesDirty();
			}
		}

		[SerializeField]
		private Vector2 m_radius = Vector2.zero;
		internal Vector2 Radius
		{
			get => m_radius;
			private set => m_radius = value;
		}

		protected override void _PopulateMesh(VertexHelper vertexHelper)
		{
			var rect = GetPixelAdjustedRect();

			Radius = new Vector2(rect.width/2.0f,rect.height/2.0f);
			var centerPoint = rect.center;
			var innerRadius = new Vector2(Radius.x-OutlineThickness,Radius.y-OutlineThickness);

			_DrawShape(vertexHelper,centerPoint,Radius,innerRadius);
		}

		private void _DrawShape(VertexHelper vertexHelper,Vector2 centerPoint,Vector2 currentRadius,Vector2 innerRadius)
		{
			switch(PrimitiveType)
			{
				case ShapePrimitiveType.Ellipse:
					_DrawEllipse(vertexHelper,centerPoint,currentRadius,innerRadius);
					break;
				case ShapePrimitiveType.Polygon:
					_DrawPolygon(vertexHelper,centerPoint,currentRadius,innerRadius);
					break;
				// case ShapeType.Rectangle:
				// 	_DrawRectangle(vertexHelper,centerPoint,currentRadius,innerRadius);
				// 	break;
				default:
					LogChannel.System.E($"{PrimitiveType} is not supported.");
					break;
			}
		}

		private bool _ShouldDrawByFill(ShapeFillType fillType,Color color)
		{
			return fillType != ShapeFillType.None && _ShouldDrawByColor(color);
		}

		private bool _ShouldDrawByOutline(float thickness,Color color)
		{
			return thickness >= 0.0f && _ShouldDrawByColor(color);
		}

		private bool _ShouldDrawByColor(Color color)
		{
			return color.a >= 0.0f;
		}

		private void _DrawCommonShape(VertexHelper vertexHelper,int segmentCount,float segmentAngle,Vector2 centerPoint,Vector2 currentRadius,Vector2 innerRadius,bool useDistance)
		{
			var realFillColor = _GetFillColor(FillType,FillColor,OutlineColor);

			//? Draw Fill
			if(_ShouldDrawByFill(FillType,realFillColor))
			{
				_DrawPolygonWithFill(vertexHelper,segmentCount,segmentAngle,centerPoint,innerRadius,realFillColor,useDistance);
			}

			//? Draw Outline
			if(_ShouldDrawByOutline(OutlineThickness,OutlineColor))
			{
				_DrawPolygonWithOutline(vertexHelper,segmentCount,segmentAngle,centerPoint,currentRadius,innerRadius,useDistance);
			}
		}

		private void _DrawPolygonWithFill(VertexHelper vertexHelper,int segmentCount,float segmentAngle,Vector2 centerPoint,Vector2 currentRadius,Color drawColor,bool useDistance)
		{
			var count = vertexHelper.currentVertCount;

			vertexHelper.AddVert(centerPoint,drawColor,Vector2.zero);

			for(var i=0;i<=segmentCount;i++)
			{
				var distance = useDistance ? GetPolygonVertexDistance(i%segmentCount) : 1.0f;

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
				var distanceRatio = useDistance ? GetPolygonVertexDistance(i%segmentCount) : 1.0f;

				var cosAngle = Mathf.Cos(segmentAngle*i)*distanceRatio;
				var sinAngle = Mathf.Sin(segmentAngle*i)*distanceRatio;

				vertexHelper.AddVert(new (centerPoint.x+cosAngle*innerRadius.x,centerPoint.y+sinAngle*innerRadius.y),OutlineColor,Vector2.zero);
				vertexHelper.AddVert(new (centerPoint.x+cosAngle*currentRadius.x,centerPoint.y+sinAngle*currentRadius.y),OutlineColor,Vector2.zero);
			}

			for(var i=0;i<segmentCount*2;i+=2)
			{
				vertexHelper.AddTriangle(count+i,count+i+1,count+i+2);
				vertexHelper.AddTriangle(count+i+1,count+i+2,count+i+3);
			}
		}

		protected override int _GetTotalExpectedVertices()
		{
			return _GetTotalExpectedVertices(PrimitiveType,FillType,FillColor,OutlineThickness,OutlineColor);
		}

		private int _GetTotalExpectedVertices(ShapePrimitiveType primitiveType,ShapeFillType fillType,Color fillColor,float outlineThickness,Color outlineColor)
		{
			switch(primitiveType)
			{
				case ShapePrimitiveType.Ellipse:
					return _GetExpectedVerticesInEllipse(EllipseAngle,fillType,fillColor,outlineThickness,outlineColor);
				case ShapePrimitiveType.Polygon:
					return _GetExpectedVerticesInPolygon(PolygonSideCount,fillType,fillColor,outlineThickness,outlineColor);
				// case ShapeType.Rectangle:
				// 	return _DrawRectangle(vertexHelper,centerPoint,currentRadius,innerRadius);
				// 	break;
				default:
					LogChannel.System.E($"{PrimitiveType} is not supported.");
					return 0;
			}
		}

		private int _GetExpectedVerticesInCommonShape(int sideCount,ShapeFillType fillType,Color fillColor,float outlineThickness,Color outlineColor)
		{
			var count = 0;
			var realFillColor = _GetFillColor(fillType,fillColor,outlineColor);

			//? Draw Fill
			if(_ShouldDrawByFill(fillType,realFillColor))
			{
				count += _GetExpectedVerticesInPolygonWithFill(sideCount);
			}

			//? Draw Outline
			if(_ShouldDrawByOutline(outlineThickness,outlineColor))
			{
				count += _GetExpectedVerticesInPolygonWithOutline(sideCount);
			}

			return count;
		}

		private int _GetExpectedVerticesInPolygonWithFill(int segmentCount)
		{
			var count = 1;

			count += segmentCount+1;

			return count;
		}

		private int _GetExpectedVerticesInPolygonWithOutline(int segmentCount)
		{
			var count = 0;

			count += (segmentCount+1)*2;

			return count;
		}

		private Color _GetFillColor(ShapeFillType fillType,Color fillColor,Color outlineColor)
		{
			return fillType switch
			{
				ShapeFillType.Solid => fillColor,
				ShapeFillType.Matched => outlineColor,
				_ => Color.clear,
			};
		}
	}
}