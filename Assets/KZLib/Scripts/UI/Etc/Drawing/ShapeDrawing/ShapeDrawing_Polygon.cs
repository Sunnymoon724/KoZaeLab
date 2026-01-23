using System.Collections.Generic;

namespace UnityEngine.UI
{
	public partial class ShapeDrawing : GraphicDrawing
	{
		[SerializeField]
		private int m_polygonSideCount = Global.MIN_POLYGON_COUNT;
		internal int PolygonSideCount
		{
			get => m_polygonSideCount;
			set
			{
				if(m_polygonSideCount == value)
				{
					return;
				}

				var vertexCount = _GetExpectedVerticesInPolygon(value,FillType,FillColor,OutlineThickness,OutlineColor);

				if(!_IsValidVertex(vertexCount))
				{
					return;
				}

				m_polygonSideCount = value;

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

				SetVerticesDirty();
			}
		}

		[SerializeField]
		private List<float> m_polygonVertexDistanceList = new(Global.MIN_POLYGON_COUNT) {1, 1, 1, };
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

		internal float PolygonSegmentAngle => 2.0f*Mathf.PI/PolygonSideCount;

		private void _DrawPolygon(VertexHelper vertexHelper,Vector2 centerPoint,Vector2 currentRadius,Vector2 innerRadius)
		{
			_DrawCommonShape(vertexHelper,PolygonSideCount,PolygonSegmentAngle,centerPoint,currentRadius,innerRadius,true);
		}

		private int _GetExpectedVerticesInPolygon(int sideCount,ShapeFillType fillType,Color fillColor,float outlineThickness,Color outlineColor)
		{
			return _GetExpectedVerticesInCommonShape(sideCount,fillType,fillColor,outlineThickness,outlineColor);
		}
	}
}