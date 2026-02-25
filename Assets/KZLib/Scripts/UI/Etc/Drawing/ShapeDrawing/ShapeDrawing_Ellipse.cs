using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KZLib.UI
{
	public partial class ShapeDrawing : GraphicDrawing
	{
		internal const float c_zeroAngle = 0.0f;
		internal const float c_fullAngle = 360.0f;
		private const int c_pivotResolution = 54;

		[SerializeField]
		private float m_ellipseAngle = c_fullAngle;
		internal float EllipseAngle
		{
			get => m_ellipseAngle;
			set
			{
				int _CalculateExpectedVertexCount_EllipseAngle(float angle)
				{
					var segmentCnt = _GetSegmentCount_EllipseInner(angle);

					return _CalculateExpectedVertexCount_Inner(segmentCount:segmentCnt);
				}

				_SetValueWithVertexCheck(ref m_ellipseAngle,value,_CalculateExpectedVertexCount_EllipseAngle,null);
			}
		}

		private void _DrawFill_Ellipse(VertexHelper vertexHelper,Color drawColor,bool canDrawAntiAliasing)
		{
			var vertCnt = vertexHelper.currentVertCount;

			_AddVert(vertexHelper,Center,drawColor);

			var vertInfoList = _CalculateAllEllipseVertexList(true);
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

			_AddFillEllipseTriangles(vertexHelper,vertCnt,canDrawAntiAliasing);
		}

		private void _DrawOutline_Ellipse(VertexHelper vertexHelper,bool canDrawAntiAliasing)
		{
			var vertCnt = vertexHelper.currentVertCount;

			var vertInfoList = _CalculateAllEllipseVertexList(false);
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

			_AddOutlineEllipseTriangles(vertexHelper,vertCnt,canDrawAntiAliasing);
		}

		private List<VertexInfo> _CalculateAllEllipseVertexList(bool isFill)
		{
			var vertInfoList = new List<VertexInfo>();
			var segmentCnt = _GetSegmentCount_Ellipse();
			var segmentAngle = _CalculateSegmentAngle(EllipseAngle,segmentCnt);

			var innerRadius = Radius.Offset(-OutlineThickness);
			var outerRadius = Radius;
			var antiRadius = isFill ? innerRadius.Offset(AntiAliasing) : outerRadius.Offset(AntiAliasing);


			for(var i=0;i<=segmentCnt;i++)
			{
				var cos = Mathf.Cos(segmentAngle*i);
				var sin = Mathf.Sin(segmentAngle*i);

				var outerVert = _CalculateLocalVertex(cos,sin,outerRadius);
				var innerVert = _CalculateLocalVertex(cos,sin,innerRadius);
				var antiVert = _CalculateLocalVertex(cos,sin,antiRadius);

				vertInfoList.Add(new VertexInfo(outerVert,innerVert,antiVert));
			}

			return vertInfoList;
		}

		private void _AddFillEllipseTriangles(VertexHelper vertexHelper,int vertCnt,bool canDrawAntiAliasing)
		{
			var segmentCnt = _GetSegmentCount_Ellipse();

			if(canDrawAntiAliasing)
			{
				for(var i=0;i<segmentCnt;i++)
				{
					var currInner = vertCnt+1+(i*2);
					var currAnti  = currInner+1;

					var nextInner = currInner+2;
					var nextAnti  = nextInner+1;

					vertexHelper.AddTriangle(vertCnt,currInner,nextInner);

					vertexHelper.AddTriangle(currInner,currAnti,nextAnti);
					vertexHelper.AddTriangle(currInner,nextAnti,nextInner);
				}
			}
			else
			{
				for(var i=0;i<segmentCnt;i++)
				{
					vertexHelper.AddTriangle(vertCnt,vertCnt+1+i,vertCnt+2+i);
				}
			}
		}

		private void _AddOutlineEllipseTriangles(VertexHelper vertexHelper,int vertCnt,bool canDrawAntiAliasing)
		{
			var segmentCnt = _GetSegmentCount_Ellipse();

			if(canDrawAntiAliasing)
			{
				for(var i=0;i<segmentCnt;i++)
				{
					var curr = vertCnt+(i*3);
					var next = vertCnt+((i+1)*3);

					vertexHelper.AddTriangle(curr,curr+1,next+1);
					vertexHelper.AddTriangle(curr,next+1,next);

					vertexHelper.AddTriangle(curr+1,curr+2,next+2);
					vertexHelper.AddTriangle(curr+1,next+2,next+1);
				}
			}
			else
			{
				for(var i=0;i<segmentCnt;i++)
				{
					var curr = vertCnt+(i*2);
					var next = vertCnt+((i+1)*2);

					vertexHelper.AddTriangle(curr,curr+1,next+1);
					vertexHelper.AddTriangle(curr,next+1,next);
				}
			}
		}


		private int _GetSegmentCount_Ellipse()
		{
			return _GetSegmentCount_EllipseInner(EllipseAngle);
		}

		private int _GetSegmentCount_EllipseInner(float angle)
		{
			angle = Mathf.Clamp(angle,c_zeroAngle,c_fullAngle);

			return Mathf.CeilToInt(c_pivotResolution*angle/c_fullAngle);
		}


		private int _CalculateExpectedFillVertexCount_Ellipse(int segmentCount,bool canDrawAntiAliasing)
		{
			return 1+(canDrawAntiAliasing ? segmentCount*2 : segmentCount);
		}

		private int _CalculateExpectedOutlineVertexCount_Ellipse(int segmentCount,bool canDrawAntiAliasing)
		{
			return canDrawAntiAliasing ? segmentCount*3 : segmentCount*2;
		}
	}
}