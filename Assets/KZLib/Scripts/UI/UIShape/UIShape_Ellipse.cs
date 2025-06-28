
namespace UnityEngine.UI
{
	[RequireComponent(typeof(CanvasRenderer))]
	public partial class UIShape : MaskableGraphic
	{
		private const int c_pivotResolution = 54;

		[SerializeField] private float m_ellipseAngle = 360.0f;

		private void _DrawEllipse(VertexHelper vertexHelper,Vector2 centerPoint,Vector2 currentRadius,Vector2 innerRadius)
		{
			// angle == 0 -> dont draw anything
			if(m_ellipseAngle <= 0.0f)
			{
				m_ellipseAngle = 0.0f;
			}

			var segmentCount = Mathf.CeilToInt(c_pivotResolution*m_ellipseAngle/Global.FULL_ANGLE);
			var segmentAngle = 2.0f*Mathf.PI/c_pivotResolution;

			//? Draw Fill
			if(m_fillType != FillType.None)
			{
				var fillColor = _GetColor();

				_DrawPolygonWithFill(vertexHelper,segmentCount,segmentAngle,centerPoint,innerRadius,fillColor,false);
			}

			//? Draw Outline
			if(color != Color.clear)
			{
				_DrawPolygonWithOutline(vertexHelper,segmentCount,segmentAngle,centerPoint,currentRadius,innerRadius,false);
			}
		}
	}
}