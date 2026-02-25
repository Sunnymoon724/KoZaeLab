using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KZLib.UI
{
	[RequireComponent(typeof(CanvasRenderer))]
	public abstract class GraphicDrawing : MaskableGraphic
	{
		protected const int c_maxVertexCount = 64000;

		[SerializeField]
		protected Vector2 m_radius = Vector2.zero;
		internal Vector2 Radius
		{
			get => m_radius;
			private set => m_radius = value;
		}

		[SerializeField]
		protected Vector2 m_center = Vector2.zero;
		internal Vector2 Center
		{
			get => m_center;
			private set => m_center = value;
		}

		protected override void OnPopulateMesh(VertexHelper vertexHelper)
		{
			vertexHelper.Clear();

			var rect = GetPixelAdjustedRect();

			Radius = new Vector2(rect.width/2.0f,rect.height/2.0f);
			Center = rect.center;

			_PopulateMesh(vertexHelper);
		}

		protected abstract void _PopulateMesh(VertexHelper vertexHelper);
		protected abstract int _CalculateExpectedVertexCount();

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

			var vertexCount = _CalculateExpectedVertexCount();

			if(!_IsValidVertex(vertexCount))
			{
				return;
			}

			SetVerticesDirty();
		}

		protected void _SetValueWithVertexCheck<TValue>(ref TValue oldValue,TValue newValue,Func<TValue,int> calculateExpectedVertexCount,Action onValueChanged = null)
		{
			if(EqualityComparer<TValue>.Default.Equals(oldValue,newValue))
			{
				return;
			}

			var expectedCount = calculateExpectedVertexCount(newValue);

			if(!_IsValidVertex(expectedCount))
			{
				return;
			}

			oldValue = newValue;

			onValueChanged?.Invoke();

			SetVerticesDirty();
		}

		protected void _AddVert(VertexHelper vertexHelper,Vector3 position,Color color)
		{
			vertexHelper.AddVert(position,color,Vector2.zero);
		}

		internal Vector2[] _GetCornerArray()
		{
			return new Vector2[]
			{
				new(-Radius.x,+Radius.y),
				new(+Radius.x,+Radius.y),
				new(+Radius.x,-Radius.y),
				new(-Radius.x,-Radius.y)
			};
		}
	}
}