using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KZLib.UI
{
	public partial class ShapeDrawing : GraphicDrawing
	{
		internal const int c_triangleSegmentCount = 3;

		internal const float c_minTriangleOffset = 0.0f;
		internal const float c_halfTriangleOffset = 0.5f;
		internal const float c_maxTriangleOffset = 1.0f;

		[SerializeField]
		private float m_triangleOffset = c_halfTriangleOffset;
		internal float TriangleOffset
		{
			get => m_triangleOffset;
			set
			{
				if(m_triangleOffset == value)
				{
					return;
				}

				m_triangleOffset = value;

				SetVerticesDirty();
			}
		}

		private void _DrawFill_Triangle(VertexHelper vertexHelper,Color drawColor,bool canDrawAntiAliasing)
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

		private void _DrawOutline_Triangle(VertexHelper vertexHelper,bool canDrawAntiAliasing)
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

		private List<VertexInfo> _CalculateAllTriangleVertexList(bool isFill)
		{
			var outerVertArray = _CalculateTriangleOuterVertexArray();

			return _CalculateAllVertexList_CommonShape(isFill,c_triangleSegmentCount,outerVertArray);
		}

		private Vector2[] _CalculateTriangleOuterVertexArray()
		{
			var cornerArray = _GetCornerArray();

			var vertArray = new Vector2[3];

			vertArray[0] = Center+cornerArray[2];
			vertArray[1] = Center+cornerArray[3];
			vertArray[2] = Center+Vector2.Lerp(cornerArray[0],cornerArray[1],TriangleOffset);

			return vertArray;
		}

		private void _AddFillTriangleTriangles(VertexHelper vertexHelper,int vertCnt,bool canDrawAntiAliasing)
		{
			if(canDrawAntiAliasing)
			{
				for(var i=0;i<c_triangleSegmentCount;i++)
				{
					int currInner = vertCnt+(i*2);
					int currAnti  = currInner+1;
					int nextInner = vertCnt+((i+1)%3*2);
					int nextAnti  = nextInner+1;

					if(i == 0)
					{
						vertexHelper.AddTriangle(vertCnt,vertCnt+2,vertCnt+4);
					}

					vertexHelper.AddTriangle(currInner,currAnti,nextAnti);
					vertexHelper.AddTriangle(currInner,nextAnti,nextInner);
				}
			}
			else
			{
				vertexHelper.AddTriangle(vertCnt,vertCnt+1,vertCnt+2);
			}
		}

		private void _AddOutlineTriangleTriangles(VertexHelper vertexHelper,int vertCnt,bool canDrawAntiAliasing)
		{
			var step = canDrawAntiAliasing ? 3 : 2;

			for(var i=0;i<c_triangleSegmentCount;i++)
			{
				var curr = vertCnt+(i*step);
				var next = vertCnt+((i+1)%3*step);

				vertexHelper.AddTriangle(curr,curr+1,next+1);
				vertexHelper.AddTriangle(curr,next+1,next);

				if(canDrawAntiAliasing)
				{
					vertexHelper.AddTriangle(curr+1,curr+2,next+2);
					vertexHelper.AddTriangle(curr+1,next+2,next+1);
				}
			}
		}


		private int _GetSegmentCount_Triangle()
		{
			return c_triangleSegmentCount;
		}


		private int _CalculateExpectedFillVertexCount_Triangle(int segmentCount,bool canDrawAntiAliasing)
		{
			return canDrawAntiAliasing ? segmentCount*2 : segmentCount;
		}

		private int _CalculateExpectedOutlineVertexCount_Triangle(int segmentCount,bool canDrawAntiAliasing)
		{
			return canDrawAntiAliasing ? segmentCount*3 : segmentCount*2;
		}
	}
}