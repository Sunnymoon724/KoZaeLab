using System;
using System.Collections.Generic;
using KZLib.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace KZLib.UI
{
	public partial class ShapeDrawing : GraphicDrawing
	{
		internal enum ShapePrimitiveType { Ellipse, Polygon, Triangle, } // Rectangle, Freeform, }
		internal enum ShapeFillType { None, Matched, Solid, }

		private record VertexInfo(Vector3 Outer,Vector3 Inner,Vector3 Anti);

		[SerializeField]
		private ShapePrimitiveType m_primitiveType = ShapePrimitiveType.Ellipse;
		internal ShapePrimitiveType PrimitiveType
		{
			get => m_primitiveType;
			set
			{
				int _CalculateExpectedVertexCount_PrimitiveType(ShapePrimitiveType primitiveType)
				{
					return _CalculateExpectedVertexCount_Inner(primitiveType:primitiveType);
				}

				_SetValueWithVertexCheck(ref m_primitiveType,value,_CalculateExpectedVertexCount_PrimitiveType,null);
			}
		}

		[SerializeField]
		private float m_outlineThickness = 1.0f;
		internal float OutlineThickness
		{
			get => m_outlineThickness;
			set
			{
				int _CalculateExpectedVertexCount_OutlineThickness(float outlineThickness)
				{
					return _CalculateExpectedVertexCount_Inner(outlineThickness:outlineThickness);
				}

				_SetValueWithVertexCheck(ref m_outlineThickness,value,_CalculateExpectedVertexCount_OutlineThickness,null);
			}
		}

		[SerializeField]
		private Color m_outlineColor = Color.white;
		internal Color OutlineColor
		{
			get => m_outlineColor;
			set
			{
				int _CalculateExpectedVertexCount_OutlineColor(Color outlineColor)
				{
					return _CalculateExpectedVertexCount_Inner(outlineColor:outlineColor);
				}

				_SetValueWithVertexCheck(ref m_outlineColor,value,_CalculateExpectedVertexCount_OutlineColor,null);
			}
		}

		[SerializeField]
		private ShapeFillType m_fillType = ShapeFillType.Solid;
		internal ShapeFillType FillType
		{
			get => m_fillType;
			set
			{
				int _CalculateExpectedVertexCount_FillType(ShapeFillType fillType)
				{
					return _CalculateExpectedVertexCount_Inner(fillType:fillType);
				}

				_SetValueWithVertexCheck(ref m_fillType,value,_CalculateExpectedVertexCount_FillType,null);
			}
		}

		[SerializeField]
		private Color m_fillColor = Color.white;
		internal Color FillColor
		{
			get => m_fillColor;
			set
			{
				int _CalculateExpectedVertexCount_FillColor(Color fillColor)
				{
					return _CalculateExpectedVertexCount_Inner(fillColor:fillColor);
				}

				_SetValueWithVertexCheck(ref m_fillColor,value,_CalculateExpectedVertexCount_FillColor,null);
			}
		}

		[SerializeField]
		private float m_antiAliasing = 0.0f;
		internal float AntiAliasing
		{
			get => m_antiAliasing;
			set
			{
				value = Mathf.Max(value,0.0f);

				int _CalculateExpectedVertexCount_AntiAliasing(float antiAliasing)
				{
					return _CalculateExpectedVertexCount_Inner(antiAliasing:antiAliasing);
				}

				_SetValueWithVertexCheck(ref m_antiAliasing,value,_CalculateExpectedVertexCount_AntiAliasing,null);
			}
		}

		private class ShapeCatalog : StrategyCatalog<ShapeDrawing,ShapePrimitiveType,ShapeStrategy>
		{
			public ShapeCatalog(ShapeDrawing owner) : base(owner) { }

			protected override Dictionary<ShapePrimitiveType,ShapeStrategy> _BindStrategy()
			{
				return new()
				{
					[ShapePrimitiveType.Ellipse] = new(m_owner._DrawFill_Ellipse,m_owner._DrawOutline_Ellipse,m_owner._GetSegmentCount_Ellipse,m_owner._CalculateExpectedFillVertexCount_Ellipse,m_owner._CalculateExpectedOutlineVertexCount_Ellipse),
					[ShapePrimitiveType.Polygon] = new(m_owner._DrawFill_Polygon,m_owner._DrawOutline_Polygon,m_owner._GetSegmentCount_Polygon,m_owner._CalculateExpectedFillVertexCount_Polygon,m_owner._CalculateExpectedOutlineVertexCount_Polygon),
					[ShapePrimitiveType.Triangle] = new(m_owner._DrawFill_Triangle,m_owner._DrawOutline_Triangle,m_owner._GetSegmentCount_Triangle,m_owner._CalculateExpectedFillVertexCount_Triangle,m_owner._CalculateExpectedOutlineVertexCount_Triangle),
				};
			}
		}

		private readonly struct ShapeStrategy
		{
			public readonly Action<VertexHelper,Color,bool> DrawFill;
			public readonly Action<VertexHelper,bool> DrawOutline;

			public readonly Func<int> GetSegmentCount;

			public readonly Func<int,bool,int> CalculateExpectedFillVertexCount;
			public readonly Func<int,bool,int> CalculateExpectedOutlineVertexCount;

			public ShapeStrategy(Action<VertexHelper,Color,bool> drawFill,Action<VertexHelper,bool> drawOutline,Func<int> getSegmentCount,Func<int,bool,int> calculateExpectedFillVertexCount,Func<int,bool,int> calculateExpectedOutlineVertexCount)
			{
				DrawFill = drawFill;
				DrawOutline = drawOutline;

				GetSegmentCount = getSegmentCount;

				CalculateExpectedFillVertexCount = calculateExpectedFillVertexCount;
				CalculateExpectedOutlineVertexCount = calculateExpectedOutlineVertexCount;
			}
		}

		private ShapeCatalog m_shapeCatalog = null;

		private bool _TryGetStrategy(ShapePrimitiveType primitiveType,out ShapeStrategy strategy)
		{
			m_shapeCatalog ??= new ShapeCatalog(this);

			return m_shapeCatalog.TryGetStrategy(primitiveType,out strategy);
		}

		protected override void _PopulateMesh(VertexHelper vertexHelper)
		{
			var realFillColor = _GetFillColor(FillType,FillColor,OutlineColor);

			var canDrawFill = _CanDrawFill(FillType,realFillColor);
			var canDrawOutline = _CanDrawOutline(OutlineThickness,OutlineColor);

			var canDrawAntiAliasing = AntiAliasing > 0.0f;

			//? Draw Fill
			if(canDrawFill)
			{
				if(_TryGetStrategy(PrimitiveType,out var strategy))
				{
					strategy.DrawFill(vertexHelper,realFillColor,!canDrawOutline && canDrawAntiAliasing);
				}
			}

			//? Draw Outline
			if(canDrawOutline)
			{
				if(_TryGetStrategy(PrimitiveType,out var strategy))
				{
					strategy.DrawOutline(vertexHelper,canDrawAntiAliasing);
				}
			}
		}

		private bool _CanDrawFill(ShapeFillType fillType,Color color)
		{
			return fillType != ShapeFillType.None && _CanDraw(color);
		}

		private bool _CanDrawOutline(float thickness,Color color)
		{
			return _CanDraw(color) && thickness >= 0.0f;
		}

		private bool _CanDraw(Color color)
		{
			return color.a > 0.0f;
		}

		protected override int _CalculateExpectedVertexCount()
		{
			return _CalculateExpectedVertexCount_Inner();
		}

		private int _CalculateExpectedVertexCount_Inner(ShapePrimitiveType? primitiveType = null,ShapeFillType? fillType = null,Color? fillColor = null,float? outlineThickness = null,Color? outlineColor = null,int? segmentCount = null,float? antiAliasing = null)
		{
			var primitiveType2 = primitiveType ?? PrimitiveType;
			var fillType2 = fillType ?? FillType;
			var fillColor2 = fillColor ?? FillColor;
			var outlineThickness2 = outlineThickness ?? OutlineThickness;
			var outlineColor2 = outlineColor ?? OutlineColor;
			var antiAliasing2 = antiAliasing ?? AntiAliasing;

			if(_TryGetStrategy(primitiveType2,out var strategy))
			{
				var segmentCount2 = segmentCount ?? strategy.GetSegmentCount();

				var vertCnt = 0;

				var realFillColor = _GetFillColor(fillType2,fillColor2,outlineColor2);

				var canDrawFill = _CanDrawFill(fillType2,realFillColor);
				var canDrawOutline = _CanDrawOutline(outlineThickness2,outlineColor2);

				var canDrawAntiAliasing = antiAliasing2 > 0.0f;

				//? Draw Fill
				if(canDrawFill)
				{
					vertCnt += strategy.CalculateExpectedFillVertexCount(segmentCount2,!canDrawOutline && canDrawAntiAliasing);
				}

				//? Draw Outline
				if(canDrawOutline)
				{
					vertCnt += strategy.CalculateExpectedOutlineVertexCount(segmentCount2,canDrawAntiAliasing);
				}

				return vertCnt;
			}
			else
			{
				return 0;
			}
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

		private Vector2 _GetOffsetVertex(Vector2 vertex,Vector2 center,Vector2 direction1,Vector2 direction2,float thickness)
		{
			if(direction1 == Vector2.zero || direction2 == Vector2.zero)
			{
				return vertex;
			}

			var bisector = (direction1+direction2).normalized;
			var normal = (center-vertex).normalized;

			if(Vector2.Dot(bisector,normal) < 0.0f) 
			{
				bisector = -bisector;
			}

			var angle = Vector2.Angle(direction1,direction2)*0.5f*Mathf.Deg2Rad;
			var sinAngle = Mathf.Sin(angle);

			var dist = (sinAngle > 0.01f) ? (thickness/sinAngle) : thickness;

			return vertex+bisector*dist;
		}

		private float _CalculateSegmentAngle(float degree,int count)
		{
			return degree*Mathf.Deg2Rad/count;
		}

		private Vector2 _CalculateLocalVertex(float cos,float sin,Vector2 radius)
		{
			return new Vector2(Center.x+cos*radius.x,Center.y+sin*radius.y);
		}

		private List<VertexInfo> _CalculateAllVertexList_CommonShape(bool isFill,int segmentCount,Vector2[] outerVertArray)
		{
			var vertInfoList = new List<VertexInfo>();

			for(var i=0;i<segmentCount;i++)
			{
				var currVert = outerVertArray[i];
				var prevVert = outerVertArray[CommonUtility.LoopClamp(i-1,segmentCount)];
				var nextVert = outerVertArray[CommonUtility.LoopClamp(i+1,segmentCount)];

				var direction1 = (prevVert-currVert).normalized;
				var direction2 = (nextVert-currVert).normalized;

				var innerVert = _GetOffsetVertex(currVert,Center,direction1,direction2,OutlineThickness);
				var antiVert = _GetOffsetVertex(isFill ? innerVert :currVert,Center,direction1,direction2,-AntiAliasing);

				vertInfoList.Add(new VertexInfo(currVert,innerVert,antiVert));
			}

			return vertInfoList;
		}
	}
}