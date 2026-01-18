
namespace UnityEngine.UI
{
	[RequireComponent(typeof(CanvasRenderer))]
	public abstract class GraphicDrawing : MaskableGraphic
	{
		protected const int c_maxVertexCount = 64000;

		protected override void OnPopulateMesh(VertexHelper vertexHelper)
		{
			vertexHelper.Clear();

			_PopulateMesh(vertexHelper);
		}

		protected abstract void _PopulateMesh(VertexHelper vertexHelper);
		protected abstract int _GetTotalExpectedVertices();

		protected bool _IsValidVertex(int vertexCount)
		{
			if(vertexCount > c_maxVertexCount)
			{
				LogChannel.UI.E($"Vertices count({vertexCount}) is greater than {c_maxVertexCount}.");

				return false;
			}

			return true;
		}

		protected override void OnValidate()
		{
			base.OnValidate();

			var vertexCount = _GetTotalExpectedVertices();

			if(!_IsValidVertex(vertexCount))
			{
				return;
			}

			SetVerticesDirty();
		}
	}
}