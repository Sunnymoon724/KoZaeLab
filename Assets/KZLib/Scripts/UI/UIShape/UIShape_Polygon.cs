
namespace UnityEngine.UI
{
	[RequireComponent(typeof(CanvasRenderer))]
	public partial class UIShape : MaskableGraphic
	{
		[SerializeField] private int m_polygonSideCount = 3;

		[SerializeField] private float[] m_polygonVertexDistanceArray = new float[] { 1,1,1, };

		private void _DrawPolygon(VertexHelper vertexHelper,Vector2 centerPoint,Vector2 currentRadius,Vector2 innerRadius)
		{
			var segmentAngle = 2.0f*Mathf.PI/m_polygonSideCount;

			//? Draw Fill
			if(m_fillType != FillType.None)
			{
				var drawColor = _GetColor();

				_DrawPolygonWithFill(vertexHelper,m_polygonSideCount,segmentAngle,centerPoint,innerRadius,drawColor,true);
			}

			//? Draw Outline
			if(color != Color.clear)
			{
				_DrawPolygonWithOutline(vertexHelper,m_polygonSideCount,segmentAngle,centerPoint,currentRadius,innerRadius,true);
			}
		}
	}
}