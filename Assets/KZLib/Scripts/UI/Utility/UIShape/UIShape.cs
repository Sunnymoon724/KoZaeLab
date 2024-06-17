using System.Collections.Generic;

namespace UnityEngine.UI
{
	[RequireComponent(typeof(CanvasRenderer))]
	public class UIShape : MaskableGraphic
	{
		private enum ShapeType { Ellipse, Polygon, Triangle }
		private enum FillType { None, Matched, Solid, }

		[SerializeField] private ShapeType m_ShapeType = ShapeType.Ellipse;

		[SerializeField] private float m_OutlineSize = 1.0f;

		[SerializeField] private FillType m_FillType = FillType.Solid;

		#region Ellipse
		[SerializeField] private bool m_EllipseReverse = false;
		[SerializeField] private float m_EllipseAngle = 360.0f;
		[SerializeField] private bool m_EllipseUseCap = false;
		#endregion Ellipse

		#region Polygon
		[SerializeField] private int m_PolygonSideCount = 3;

		[SerializeField] private float[] m_PolygonVertexDistanceArray = new float[] { 1,1,1, };
		#endregion Polygon

		#region Rectangle
		// [SerializeField] private float m_RectangleRound = 0.0f;
		// [SerializeField] private bool m_RectangleEachCorner = false;
		// [SerializeField] private List<float> m_RectangleEachRound = new();
		#endregion Rectangle

		#region Triangle
		[SerializeField] private float m_TriangleOffset = 0.0f;
		#endregion Triangle

		[SerializeField] private Color m_FillColor = Color.white;

		protected override void OnPopulateMesh(VertexHelper _vertexHelper)
		{
			_vertexHelper.Clear();

			var rect = GetPixelAdjustedRect();
			var radius = new Vector2(rect.width/2.0f,rect.height/2.0f);
			var center = rect.center;

			if(m_ShapeType == ShapeType.Ellipse)
			{
				if(m_EllipseAngle <= 0.0f)
				{
					return;
				}

				var ellipseResolution = 50;

				var ratio = m_EllipseAngle/Global.FULL_ANGLE;

				var resolution = Mathf.CeilToInt(ellipseResolution*ratio);
				var angleArray = GetAngleArray(ellipseResolution,m_EllipseReverse);

				//? Draw Fill
				if(m_FillType != FillType.None)
				{
					var color = GetColor();

					var forward0 = m_EllipseReverse ? 0 : -1;
					var forward1 = m_EllipseReverse ? -1 : 0;
					var vertex = Vector3.zero;
					var position = new Vector2(0.5f,0.5f);

					_vertexHelper.AddVert(center,color,position);

					vertex.x = center.x+angleArray[0].x*radius.x;
					vertex.y = center.y+angleArray[0].y*radius.y;

					position.x = (angleArray[0].x+1.0f)*0.5f;
					position.y = (angleArray[0].y+1.0f)*0.5f;

					_vertexHelper.AddVert(vertex,color,position);

					for(var i=1;i<resolution;i++)
					{
						vertex.x = center.x+angleArray[i].x*radius.x;
						vertex.y = center.y+angleArray[i].y*radius.y;

						position.x = (angleArray[i].x+1.0f)*0.5f;
						position.y = (angleArray[i].y+1.0f)*0.5f;

						_vertexHelper.AddVert(vertex,color,position);

						_vertexHelper.AddTriangle(0,i+forward0,i+forward1);
					}

					var radian = m_EllipseReverse ? -Mathf.PI*2.0f*ratio : +Mathf.PI*2.0f*ratio;

					var cos = Mathf.Cos(radian);
					var sin = Mathf.Sin(radian);

					vertex.x = center.x+cos*radius.x;
					vertex.y = center.y+sin*radius.y;

					position.x = (cos+1.0f)*0.5f;
					position.y = (sin+1.0f)*0.5f;

					_vertexHelper.AddVert(vertex,color,position);

					if(m_EllipseReverse)
					{
						_vertexHelper.AddTriangle(0,resolution,resolution-1);
						_vertexHelper.AddTriangle(0,resolution+1,resolution);
					}
					else
					{
						_vertexHelper.AddTriangle(0,resolution-1,resolution);
						_vertexHelper.AddTriangle(0,resolution,resolution+1);
					}
				}

				// //? Draw Outline
				if(color != Color.clear)
				{
					if(m_EllipseAngle <= 0.0f)
					{
						return;
					}

					DrawPolygonOutline(_vertexHelper,angleArray,false,center,radius,resolution,ratio,m_EllipseReverse);

					if(m_EllipseUseCap && ratio < 1.0f)
					{
						var capResolution = 50;
						var halfSize = m_OutlineSize/2.0f;
						var capRadius = new Vector2(radius.x-halfSize,radius.y-halfSize);

						var startCenter = new Vector2(center.x+angleArray[0].x*capRadius.x,center.y+angleArray[0].y*capRadius.y);

						DrawPolygon(_vertexHelper,angleArray,false,capResolution,startCenter,new Vector2(halfSize,halfSize),color);

						var radian = m_EllipseReverse ? -Mathf.PI*2.0f*ratio : +Mathf.PI*2.0f*ratio;

						var cos = Mathf.Cos(radian);
						var sin = Mathf.Sin(radian);

						var lastCenter = new Vector2(center.x+cos*capRadius.x,center.y+sin*capRadius.y);

						DrawPolygon(_vertexHelper,angleArray,false,capResolution,lastCenter,new Vector2(halfSize,halfSize),color);
					}
				}
			}
			else if(m_ShapeType == ShapeType.Polygon)
			{
				var angleArray = GetAngleArray(m_PolygonSideCount,false);

				//? Draw Fill
				if(m_FillType != FillType.None)
				{
					var color = GetColor();

					DrawPolygon(_vertexHelper,angleArray,true,m_PolygonSideCount,center,radius,color);
				}

				//? Draw Outline
				if(color != Color.clear)
				{
					DrawPolygonOutline(_vertexHelper,angleArray,true,center,radius,m_PolygonSideCount,1.0f,false);
				}
			}
			// else if(m_ShapeType == ShapeType.Rectangle)
			// {
			// 	// //? Draw Fill
			// 	// if(m_FillType != FillType.None)
			// 	// {
			// 	// 	var color = GetColor();

			// 	// 	var angleArray = new float[] { 0.0f, 90.0f, 180.0f, 270.0f };

			// 	// 	_vertexHelper.AddVert(center,color,new Vector2(0.5f,0.5f));

			// 	// 	for(var i=0;i<1;i++)
			// 	// 	{
			// 	// 		var round = m_RectangleEachCorner ? m_RectangleEachRound[i] : m_RectangleRound;

			// 	// 		DrawCorner(_vertexHelper,angleArray[i],round,center,radius,color);
			// 	// 	}
			// 	// }

			// 	// //? Draw Outline
			// 	// if(color != Color.clear)
			// 	// {
			// 	// 	// DrawPolygonOutline(_vertexHelper,angleArray,true,center,radius,m_PolygonSideCount,1.0f,false);
			// 	// }
			// }
			else if(m_ShapeType == ShapeType.Triangle)
			{
				//? Draw Fill
				if(m_FillType != FillType.None)
				{
					var color = GetColor();

					_vertexHelper.AddVert(new Vector2(-radius.x,-radius.y),color,Vector2.zero);
					_vertexHelper.AddVert(new Vector2(+radius.x,-radius.y),color,Vector2.zero);
					_vertexHelper.AddVert(new Vector2(radius.x*m_TriangleOffset,radius.y),color,Vector2.zero);

					_vertexHelper.AddTriangle(0,1,2);
				}

				//? Draw Outline
				if(color != Color.clear)
				{
					var count = _vertexHelper.currentVertCount;
					var inner = new Vector2(radius.x-m_OutlineSize,radius.y-m_OutlineSize);

					_vertexHelper.AddVert(new Vector2(-radius.x,-radius.y),color,Vector2.zero);
					_vertexHelper.AddVert(new Vector2(-inner.x,-inner.y),color,Vector2.zero);
					_vertexHelper.AddVert(new Vector2(+inner.x,-inner.y),color,Vector2.zero);
					_vertexHelper.AddVert(new Vector2(+radius.x,-radius.y),color,Vector2.zero);
					_vertexHelper.AddVert(new Vector2(inner.x*m_TriangleOffset,inner.y),color,Vector2.zero);
					_vertexHelper.AddVert(new Vector2(radius.x*m_TriangleOffset,radius.y),color,Vector2.zero);

					_vertexHelper.AddTriangle(count,count+1,count+2);
					_vertexHelper.AddTriangle(count,count+2,count+3);
					_vertexHelper.AddTriangle(count,count+4,count+1);
					_vertexHelper.AddTriangle(count,count+5,count+4);
					_vertexHelper.AddTriangle(count+2,count+4,count+5);
					_vertexHelper.AddTriangle(count+2,count+5,count+3);
				}
			}
		}

		private void DrawPolygon(VertexHelper _vertexHelper,Vector3[] _angleArray,bool _useDistance,int _resolution,Vector2 _center,Vector2 _radius,Color _color)
		{
			var count = _vertexHelper.currentVertCount;

			var position = new Vector2(0.5f,0.5f);
			var vertex = Vector3.zero;

			_vertexHelper.AddVert(_center,_color,position);

			var distance = _useDistance ? m_PolygonVertexDistanceArray[0] : 1.0f;

			vertex.x = _center.x+_angleArray[0].x*_radius.x*distance;
			vertex.y = _center.y+_angleArray[0].y*_radius.y*distance;

			position.x = (_angleArray[0].x+1.0f)*0.5f;
			position.y = (_angleArray[0].y+1.0f)*0.5f;

			_vertexHelper.AddVert(vertex,_color,position);

			for(var i=1;i<_resolution;i++)
			{
				distance = _useDistance ? m_PolygonVertexDistanceArray[i] : 1.0f;

				vertex.x = _center.x+_angleArray[i].x*_radius.x*distance;
				vertex.y = _center.y+_angleArray[i].y*_radius.y*distance;

				position.x = (_angleArray[i].x+1.0f)*0.5f;
				position.y = (_angleArray[i].y+1.0f)*0.5f;

				_vertexHelper.AddVert(vertex,_color,position);

				_vertexHelper.AddTriangle(count,count+i,count+i+1);
			}

			_vertexHelper.AddTriangle(count,count+_resolution,count+1);
		}

		private void DrawPolygonOutline(VertexHelper _vertexHelper,Vector3[] _angleArray,bool _useDistance,Vector2 _center,Vector2 _radius,int _resolution,float _ratio,bool _reverse)
		{
			var start = _vertexHelper.currentVertCount-1;
			var vertex = Vector3.zero;

			var inner = new Vector2(_radius.x-m_OutlineSize,_radius.y-m_OutlineSize);
			var distance = _useDistance ? m_PolygonVertexDistanceArray[0] : 1.0f;

			vertex.x = _center.x+_angleArray[0].x*inner.x*distance;
			vertex.y = _center.y+_angleArray[0].y*inner.y*distance;

			_vertexHelper.AddVert(vertex,color,Vector2.zero);

			vertex.x = _center.x+_angleArray[0].x*_radius.x*distance;
			vertex.y = _center.y+_angleArray[0].y*_radius.y*distance;

			_vertexHelper.AddVert(vertex,color,Vector2.zero);

			for(var i=1;i<_resolution;i++)
			{
				distance = _useDistance ? m_PolygonVertexDistanceArray[i] : 1.0f;

				vertex.x = _center.x+_angleArray[i].x*inner.x*distance;
				vertex.y = _center.y+_angleArray[i].y*inner.y*distance;

				_vertexHelper.AddVert(vertex,color,Vector2.zero);

				vertex.x = _center.x+_angleArray[i].x*_radius.x*distance;
				vertex.y = _center.y+_angleArray[i].y*_radius.y*distance;

				_vertexHelper.AddVert(vertex,color,Vector2.zero);
			
				var index = start+i*2;

				_vertexHelper.AddTriangle(index-1,index,index+1);
				_vertexHelper.AddTriangle(index,index+2,index+1);
			}

			var radian = _reverse ? -Mathf.PI*2.0f*_ratio : +Mathf.PI*2.0f*_ratio;

			var cos = Mathf.Cos(radian);
			var sin = Mathf.Sin(radian);

			vertex.x = _center.x+cos*inner.x;
			vertex.y = _center.y+sin*inner.y;

			_vertexHelper.AddVert(vertex,color,Vector2.zero);

			vertex.x = _center.x+cos*_radius.x;
			vertex.y = _center.y+sin*_radius.y;

			_vertexHelper.AddVert(vertex,color,Vector2.zero);

			var last = start+_resolution*2;

			_vertexHelper.AddTriangle(last-1,last,last+1);
			_vertexHelper.AddTriangle(last,last+2,last+1);
		}

		private void DrawCorner(VertexHelper _vertexHelper,float _angle,float _resolution,Vector2 _center,Vector2 _radius,Color _color)
		{
			var resolution = Mathf.CeilToInt(_resolution);

			resolution = resolution < 1 ? 1 : resolution;

			var vertex = Vector3.zero;
			var radian = _angle*Mathf.Deg2Rad;
			var position = new Vector2(0.5f,0.5f);

			var angleArray = GetCornerAngleArray(radian,resolution);

			vertex.x = _center.x+angleArray[0].x*_radius.x;
			vertex.y = _center.y+angleArray[0].y*_radius.y;

			position.x = (angleArray[0].x+1.0f)*0.5f;
			position.y = (angleArray[0].y+1.0f)*0.5f;

			_vertexHelper.AddVert(vertex,_color,position);

			for(var i=1;i<resolution;i++)
			{
				vertex.x = _center.x+angleArray[i].x*_radius.x;
				vertex.y = _center.y+angleArray[i].y*_radius.y;

				position.x = (angleArray[i].x+1.0f)*0.5f;
				position.y = (angleArray[i].y+1.0f)*0.5f;

				_vertexHelper.AddVert(vertex,_color,position);

				_vertexHelper.AddTriangle(0,i-1,i);
			}

			var angle = Mathf.PI/2.0f+radian;

			var cos = Mathf.Cos(angle);
			var sin = Mathf.Sin(angle);

			vertex.x = _center.x+cos*_radius.x;
			vertex.y = _center.y+sin*_radius.y;

			position.x = (cos+1.0f)*0.5f;
			position.y = (sin+1.0f)*0.5f;

			_vertexHelper.AddVert(vertex,color,position);

			_vertexHelper.AddTriangle(0,resolution,1);
			_vertexHelper.AddTriangle(0,resolution-1,resolution);
			_vertexHelper.AddTriangle(0,resolution,resolution+1);
		}

		private Vector3[] GetCornerAngleArray(float _angle,int _resolution)
		{
			var angleArray = new Vector3[_resolution];
			var pivot = Mathf.PI/2.0f*_resolution;

			for(var i=0;i<_resolution;i++)
			{
				var radian = i*pivot+_angle;

				angleArray[i].x = Mathf.Cos(radian);
				angleArray[i].y = Mathf.Sin(radian);
			}

			return angleArray;
		}
		private Vector3[] GetAngleArray(float _resolution,bool _reverse)
		{
			var resolution = Mathf.CeilToInt(_resolution);
			var angleArray = new Vector3[resolution];
			var pivot = _reverse ? -2.0f*Mathf.PI/_resolution : +2.0f*Mathf.PI/_resolution;

			for(var i=0;i<resolution;i++)
			{
				var angle = i*pivot;

				angleArray[i].x = Mathf.Cos(angle);
				angleArray[i].y = Mathf.Sin(angle);
			}

			return angleArray;
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