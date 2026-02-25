
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KZLib.UI
{
	public partial class ShapeDrawing : GraphicDrawing
	{
		internal const int c_rectangleSegmentCount = 4;
		internal const int c_rectangleCornerResolution = 8;

		internal const float c_minRectangleOffset = 0.0f;

		[SerializeField]
		private float m_cornerRadius = c_minRectangleOffset;
		internal float CornerRadius
		{
			get => m_cornerRadius;
			set
			{
				if(m_cornerRadius == value)
				{
					return;
				}

				m_cornerRadius = value;

				SetVerticesDirty();
			}
		}

		private void _DrawFill_Rectangle(VertexHelper vertexHelper,Color drawColor,bool canDrawAntiAliasing)
		{
			var vertCnt = vertexHelper.currentVertCount;

			var vertInfoList = _CalculateAllTriangleVertexList(true);
			var transparentColor = drawColor.MaskAlpha();

			for(var i=0;i<vertInfoList.Count;i++)
			{
				var vertInfo = vertInfoList[i];

				_AddVert(vertexHelper,vertInfo.Inner,drawColor);

				if(canDrawAntiAliasing)
				{
					_AddVert(vertexHelper,vertInfo.Anti,transparentColor);
				}
			}

			_AddFillTriangleTriangles(vertexHelper,vertCnt,canDrawAntiAliasing);
		}

		private void _DrawOutline_Rectangle(VertexHelper vertexHelper,bool canDrawAntiAliasing)
		{
			var vertCnt = vertexHelper.currentVertCount;

			var vertInfoList = _CalculateAllTriangleVertexList(false);
			var transparentColor = OutlineColor.MaskAlpha();

			for(var i=0;i<vertInfoList.Count;i++)
			{
				var vertInfo = vertInfoList[i];

				_AddVert(vertexHelper,vertInfo.Inner,OutlineColor);
				_AddVert(vertexHelper,vertInfo.Outer,OutlineColor);

				if(canDrawAntiAliasing)
				{
					_AddVert(vertexHelper,vertInfo.Anti,transparentColor);
				}
			}

			_AddOutlineTriangleTriangles(vertexHelper,vertCnt,canDrawAntiAliasing);
		}

		private List<VertexInfo> _CalculateAllRectangleVertexList(bool isFill)
		{
			var outerVertArray = _CalculateRectangleOuterVertexArray();

			return _CalculateAllVertexList_CommonShape(isFill,c_rectangleSegmentCount,outerVertArray);
		}

		private Vector2[] _CalculateRectangleOuterVertexArray()
		{
			var vertArray = new Vector2[c_rectangleSegmentCount];
			var cornerRadius = Mathf.Min(m_cornerRadius,Radius.x,Radius.y);

			var startAngleArray = new float[] { 0, 90, 180, 270 };

			var centerArray = new Vector2[]
			{
				new (+Radius.x-cornerRadius,+Radius.y-cornerRadius),
				new (-Radius.x+cornerRadius,+Radius.y-cornerRadius),
				new (-Radius.x+cornerRadius,-Radius.y+cornerRadius),
				new (+Radius.x-cornerRadius,-Radius.y+cornerRadius)
			};

			for(var i=0;i<c_rectangleSegmentCount;i++)
			{
				for(var j=0;j<=c_rectangleCornerResolution;j++)
				{
					var angle = (startAngleArray[i]+(j*90.0f/c_rectangleCornerResolution))*Mathf.Deg2Rad;
					var cos = Mathf.Cos(angle);
					var sin = Mathf.Sin(angle);

					vertArray[i] = centerArray[i]+new Vector2(cos*cornerRadius,sin*cornerRadius);
				}
			}

			return vertArray;
		}

		private int _GetSegmentCount_Rectangle()
		{
			return c_triangleSegmentCount;
		}


		private int _CalculateExpectedFillVertexCount_Rectangle(int segmentCount,bool canDrawAntiAliasing)
		{
			return segmentCount;
		}

		private int _CalculateExpectedOutlineVertexCount_Rectangle(int segmentCount,bool canDrawAntiAliasing)
		{
			return segmentCount;
		}
	}
}