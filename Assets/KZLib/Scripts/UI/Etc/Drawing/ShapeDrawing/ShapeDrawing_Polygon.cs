using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KZLib.UI
{
	public partial class ShapeDrawing : GraphicDrawing
	{
		internal const int c_minPolygonCount = 3;
		internal const int c_maxPolygonCount = 36;

		[SerializeField]
		private int m_polygonSideCount = c_minPolygonCount;
		internal int PolygonSideCount
		{
			get => m_polygonSideCount;
			set
			{
				int _CalculateExpectedVertexCount_PolygonSideCount(int polygonSideCount)
				{
					return _CalculateExpectedVertexCount_Inner(segmentCount:polygonSideCount);
				}

				void _SetVertexDistanceList()
				{
					var listCount = m_polygonVertexDistanceList.Count;

					if(listCount < m_polygonSideCount)
					{
						for(var i=listCount;i<m_polygonSideCount;i++)
						{
							m_polygonVertexDistanceList.Add(1);
						}
					}
					else if(listCount > m_polygonSideCount)
					{
						m_polygonVertexDistanceList.RemoveRange(m_polygonSideCount,listCount-m_polygonSideCount);
					}
				}

				_SetValueWithVertexCheck(ref m_polygonSideCount,value,_CalculateExpectedVertexCount_PolygonSideCount,_SetVertexDistanceList);
			}
		}

		[SerializeField]
		private List<float> m_polygonVertexDistanceList = new(c_minPolygonCount) { 1.0f, 1.0f, 1.0f, };
		internal int PolygonVertexDistanceCount => m_polygonVertexDistanceList.Count;
		internal float GetPolygonVertexDistance(int index)
		{
			return m_polygonVertexDistanceList[index];
		}
		internal void SetPolygonVertexDistance(int index,float value)
		{
			if(m_polygonVertexDistanceList[index] == value)
			{
				return;
			}

			m_polygonVertexDistanceList[index] = value;

			SetVerticesDirty();
		}

		private void _DrawFill_Polygon(VertexHelper vertexHelper,Color drawColor,bool canDrawAntiAliasing)
		{
			var vertCnt = vertexHelper.currentVertCount;

			_AddVert(vertexHelper,Center,drawColor);

			var vertInfoList = _CalculateAllPolygonVertexList(true);
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

			_AddFillPolygonTriangles(vertexHelper,vertCnt,canDrawAntiAliasing);
		}

		private void _DrawOutline_Polygon(VertexHelper vertexHelper,bool canDrawAntiAliasing)
		{
			var vertCnt = vertexHelper.currentVertCount;

			var transparentColor = OutlineColor.MaskAlpha();
			var vertInfoList = _CalculateAllPolygonVertexList(false);

			for(var i=0;i<vertInfoList.Count;i++)
			{
				var vertInfo = vertInfoList[i];

				_AddVert(vertexHelper,vertInfo.Outer,OutlineColor);
				_AddVert(vertexHelper,vertInfo.Inner,OutlineColor);

				if(canDrawAntiAliasing)
				{
					_AddVert(vertexHelper,vertInfo.Anti,transparentColor);
				}
			}

			_AddOutlinePolygonTriangles(vertexHelper,vertCnt,canDrawAntiAliasing);
		}

		private List<VertexInfo> _CalculateAllPolygonVertexList(bool isFill)
		{
			var outerVertArray = _CalculatePolygonOuterVertexArray();

			return _CalculateAllVertexList_CommonShape(isFill,PolygonSideCount,outerVertArray);
		}

		private Vector2[] _CalculatePolygonOuterVertexArray()
		{
			var segmentAngle = _CalculateSegmentAngle(c_fullAngle,PolygonSideCount);
			var vertArray = new Vector2[PolygonSideCount];

			for(var i=0;i<PolygonSideCount;i++)
			{
				var distRatio = GetPolygonVertexDistance(i%PolygonSideCount);

				var angle = segmentAngle*i;

				var cos = Mathf.Cos(angle)*distRatio;
				var sin = Mathf.Sin(angle)*distRatio;

				vertArray[i] = _CalculateLocalVertex(cos,sin,Radius);
			}

			return vertArray;
		}

		private void _AddFillPolygonTriangles(VertexHelper vertexHelper,int vertCnt,bool canDrawAntiAliasing)
		{
			if(canDrawAntiAliasing)
			{
				for(var i=0;i<PolygonSideCount;i++)
				{
					var currInner	= vertCnt+1+(i*2);
					var currAnti	= currInner+1;
					var nextInner	= (i == PolygonSideCount-1) ? (vertCnt+1) : (currInner+2);
					var nextAnti	= nextInner+1;

					vertexHelper.AddTriangle(vertCnt,currInner,nextInner);

					vertexHelper.AddTriangle(currInner,currAnti,nextAnti);
					vertexHelper.AddTriangle(currInner,nextAnti,nextInner);
				}
			}
			else
			{
				for(var i=0;i<PolygonSideCount;i++)
				{
					var currInner = vertCnt+1+i;
					var nextInner = (i == PolygonSideCount-1) ? (vertCnt+1) : (currInner+1);

					vertexHelper.AddTriangle(vertCnt,currInner,nextInner);
				}
			}
		}

		private void _AddOutlinePolygonTriangles(VertexHelper vertexHelper,int vertCnt,bool canDrawAntiAliasing)
		{
			if(canDrawAntiAliasing)
			{
				for(var i=0;i<PolygonSideCount;i++)
				{
					var curr = vertCnt+(i*3);
					var next = vertCnt+((i+1)%PolygonSideCount*3);

					vertexHelper.AddTriangle(curr,next,next+1);
					vertexHelper.AddTriangle(curr,next+1,curr+1);

					vertexHelper.AddTriangle(curr,curr+2,next+2);
					vertexHelper.AddTriangle(curr,next+2,next);
				}
			}
			else
			{
				for(var i=0;i<PolygonSideCount;i++)
				{
					var curr = vertCnt+(i*2);
					var next = vertCnt+((i+1)%PolygonSideCount*2);

					vertexHelper.AddTriangle(curr,next,next+1);
					vertexHelper.AddTriangle(curr,next+1,curr+1);
				}
			}
		}


		private int _GetSegmentCount_Polygon()
		{
			return PolygonSideCount;
		}


		private int _CalculateExpectedFillVertexCount_Polygon(int segmentCount,bool canDrawAntiAliasing)
		{
			return 1+(canDrawAntiAliasing ? segmentCount*2 : segmentCount);
		}

		private int _CalculateExpectedOutlineVertexCount_Polygon(int segmentCount,bool canDrawAntiAliasing)
		{
			return canDrawAntiAliasing ? segmentCount*3 : segmentCount*2;
		}
	}
}