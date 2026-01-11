using System.Collections.Generic;

namespace UnityEngine.UI
{
	[RequireComponent(typeof(CanvasRenderer))]
	public abstract class DrawingUI : MaskableGraphic
	{
		protected const int c_maxVertexCount = 64000;

		protected override void OnPopulateMesh(VertexHelper vertexHelper)
		{
			vertexHelper.Clear();

			_PopulateMesh(vertexHelper);

			var vertexCount = vertexHelper.currentVertCount;

			if(vertexCount > c_maxVertexCount)
			{
				LogChannel.UI.E($"Vertices count({vertexCount}) is greater than {c_maxVertexCount}. please reduce the number of points to draw.");

				vertexHelper.Clear();
			}
		}

		protected abstract void _PopulateMesh(VertexHelper vertexHelper);
	}
}