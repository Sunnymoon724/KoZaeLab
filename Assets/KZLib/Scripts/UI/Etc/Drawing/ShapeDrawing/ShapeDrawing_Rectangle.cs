
// namespace UnityEngine.UI
// {
// 	public partial class UIShape : MaskableGraphic
// 	{
// 		[SerializeField]
//		private float m_rectangleRadius = 0.0f;
//
// 		private void _DrawRectangle(VertexHelper vertexHelper,Vector2 centerPoint,Vector2 currentRadius,Vector2 innerRadius)
// 		{
// 			float maxRadius = Mathf.Min(currentRadius.x,currentRadius.y)/2.0f;
			
// 			//? Draw left-top corner

			
// 			//? Draw top
			
// 			// RoundedProperties.UpdateAdjusted(pixelRect, 0.0f);
// 			// AntiAliasingProperties.UpdateAdjusted(canvas);
// 			// OutlineProperties.UpdateAdjusted();
// 			// ShadowProperties.UpdateAdjusted();
			
// 			//? Draw Fill
// 			// if(m_fillType != FillType.None)
// 			// {
// 			// 	var fillColor = _GetColor();

// 			// 	_DrawPolygonWithFill(vertexHelper,size,angle,center,inner,fillColor,false);
// 			// }

// 			//? Draw Outline
// 			// if(color != Color.clear)
// 			// {
// 			// 	_DrawPolygonWithOutline(vertexHelper,size,angle,center,radius,inner,false);
// 			// }
// 		}

// 		private void _DrawRectangleWithFill(VertexHelper vertexHelper,int size,float angle,Vector2 center,Vector2 radius,Color drawColor)
// 		{
// 			var count = vertexHelper.currentVertCount;

// 			vertexHelper.AddVert(center,drawColor,Vector2.zero);

// 			for(var i=0;i<=size;i++)
// 			{
// 				var distance = useDistance ? m_polygonVertexDistanceArray[i%size] : 1.0f;

// 				var cos = Mathf.Cos(angle*i)*distance;
// 				var sin = Mathf.Sin(angle*i)*distance;

// 				vertexHelper.AddVert(new (center.x+cos*radius.x,center.y+sin*radius.y),color,Vector2.zero);
// 			}

// 			for(var i=1;i<=size;i++)
// 			{
// 				vertexHelper.AddTriangle(count,count+i,count+i+1);
// 			}
			
// 			int cornerSegments = 5;
// 			float angleStart = Mathf.PI;            // 180도, 좌상단 시작
// 			float angleEnd = Mathf.PI * 1.5f;       // 270도, 좌상단 끝

// 			for(int i = 0; i <= cornerSegments; i++)
// 			{
// 				float t = i / (float)cornerSegments;
// 				float angle = Mathf.Lerp(angleStart, angleEnd, t);
// 				float x = cornerCenter.x + Mathf.Cos(angle) * radius;
// 				float y = cornerCenter.y + Mathf.Sin(angle) * radius;
// 				vh.AddVert(new Vector3(x, y), color, uv);
// 			}
			
// 		}
		
// 		private void _DrawRectWithOutline(VertexHelper vertexHelper,int size,float angle,Vector2 center,Vector2 radius,Vector2 inner,bool useDistance)
// 		{
			
// 		}
		
// 		private void _DrawRectangleCorner(VertexHelper vertexHelper,int size,float angle,Vector2 center,Vector2 radius,Color drawColor)
// 		{
// 			// var count = vertexHelper.currentVertCount;

// 			// vertexHelper.AddVert(center,drawColor,Vector2.zero);

// 			// for(var i=0;i<=size;i++)
// 			// {
// 			// 	var distance = useDistance ? m_polygonVertexDistanceArray[i%size] : 1.0f;

// 			// 	var cos = Mathf.Cos(angle*i)*distance;
// 			// 	var sin = Mathf.Sin(angle*i)*distance;

// 			// 	vertexHelper.AddVert(new (center.x+cos*radius.x,center.y+sin*radius.y),color,Vector2.zero);
// 			// }

// 			// for(var i=1;i<=size;i++)
// 			// {
// 			// 	vertexHelper.AddTriangle(count,count+i,count+i+1);
// 			// }
			
// 			// int cornerSegments = 5;
// 			// float angleStart = Mathf.PI;            // 180도, 좌상단 시작
// 			// float angleEnd = Mathf.PI * 1.5f;       // 270도, 좌상단 끝

// 			// for(int i = 0; i <= cornerSegments; i++)
// 			// {
// 			// 	float t = i / (float)cornerSegments;
// 			// 	float angle = Mathf.Lerp(angleStart, angleEnd, t);
// 			// 	float x = cornerCenter.x + Mathf.Cos(angle) * radius;
// 			// 	float y = cornerCenter.y + Mathf.Sin(angle) * radius;
// 			// 	vh.AddVert(new Vector3(x, y), color, uv);
// 			// }
			
// 		}
// 	}
// }