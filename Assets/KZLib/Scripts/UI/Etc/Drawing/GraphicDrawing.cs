using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KZLib.UI
{
	/// <summary>
	/// Base <see cref="MaskableGraphic"/> for solid-color custom meshes.
	/// Refreshes <see cref="Center"/> / <see cref="Radius"/> from <see cref="GetPixelAdjustedRect"/> before derived mesh generation.
	/// </summary>
	[RequireComponent(typeof(CanvasRenderer))]
	public abstract class GraphicDrawing : MaskableGraphic
	{
		protected const int c_maxVertexCount = 64000;
		internal const int c_cornerCount = 4;

		//? Layout (runtime cache — not serialized)
		private Vector2 m_radius = Vector2.zero;
		internal Vector2 Radius
		{
			get => m_radius;
			private set => m_radius = value;
		}

		private Vector2 m_center = Vector2.zero;
		internal Vector2 Center
		{
			get => m_center;
			private set => m_center = value;
		}

		// Corner offsets from Center [0:top-left, 1:top-right, 2:bottom-right, 3:bottom-left]
		private readonly Vector2[] m_cornerOffsetArray = new Vector2[c_cornerCount];
		// Skips duplicate <see cref="_RefreshLayout"/> in the same frame. Cleared in <see cref="OnRectTransformDimensionsChange"/>.
		private int m_layoutRefreshFrame = -1;

		protected override void OnRectTransformDimensionsChange()
		{
			base.OnRectTransformDimensionsChange();

			m_layoutRefreshFrame = -1;
		}

		protected override void OnPopulateMesh(VertexHelper vertexHelper)
		{
			vertexHelper.Clear();

			_RefreshLayout(force:true);

			_PopulateMesh(vertexHelper);
		}

		protected abstract void _PopulateMesh(VertexHelper vertexHelper);
		protected abstract int _CalculateExpectedVertexCount();

		/// <summary>Clamp serialized fields etc. Called right after layout refresh in <see cref="OnValidate"/>.</summary>
		protected virtual void _OnValidateDrawing() { }

		/// <summary>
		/// Updates Center, Radius, and corner offsets from <see cref="GetPixelAdjustedRect"/>.
		/// When <paramref name="force"/> is false, skips if already refreshed this frame.
		/// </summary>
		internal void _RefreshLayout(bool force = false)
		{
			if(!force && m_layoutRefreshFrame == Time.frameCount)
			{
				return;
			}

			m_layoutRefreshFrame = Time.frameCount;

			var rect = GetPixelAdjustedRect();

			Radius = new Vector2(rect.width/2.0f,rect.height/2.0f);
			Center = rect.center;

			_RefreshCornerOffsetArray();
		}

		private void _RefreshCornerOffsetArray()
		{
			m_cornerOffsetArray[0] = new(-Radius.x,+Radius.y);
			m_cornerOffsetArray[1] = new(+Radius.x,+Radius.y);
			m_cornerOffsetArray[2] = new(+Radius.x,-Radius.y);
			m_cornerOffsetArray[3] = new(-Radius.x,-Radius.y);
		}

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

			_RefreshLayout(force:true);
			_OnValidateDrawing();

			var vertexCount = _CalculateExpectedVertexCount();

			if(!_IsValidVertex(vertexCount))
			{
				return;
			}

			SetVerticesDirty();
		}

		/// <summary>Applies the value only when the expected vertex count stays within the limit. Returns false on rejection.</summary>
		protected bool _SetValueWithVertexCheck<TValue>(ref TValue oldValue,TValue newValue,Func<TValue,int> calculateExpectedVertexCount,Action onValueChanged = null)
		{
			if(EqualityComparer<TValue>.Default.Equals(oldValue,newValue))
			{
				return true;
			}

			var expectedCount = calculateExpectedVertexCount(newValue);

			if(!_IsValidVertex(expectedCount))
			{
				return false;
			}

			oldValue = newValue;

			onValueChanged?.Invoke();

			SetVerticesDirty();

			return true;
		}

		/// <summary>For property changes that do not affect vertex count.</summary>
		protected void _SetVerticesDirtyProperty<TValue>(ref TValue field,TValue value)
		{
			if(EqualityComparer<TValue>.Default.Equals(field,value))
			{
				return;
			}

			field = value;

			SetVerticesDirty();
		}

		protected void _AddVert(VertexHelper vertexHelper,Vector3 position,Color color)
		{
			vertexHelper.AddVert(position,color,Vector2.zero);
		}

		//? Coordinate (Center-relative — independent of pivot)

		/// <summary>Corner offset from Center. Use <see cref="_GetCornerLocalPosition"/> for local absolute position.</summary>
		internal Vector2 _GetCornerOffset(int index)
		{
			_RefreshLayout();

			return m_cornerOffsetArray[index];
		}

		internal Vector2 _GetCornerLocalPosition(int index)
		{
			_RefreshLayout();

			return Center+m_cornerOffsetArray[index];
		}

		internal Vector2 _GetLocalPositionFromCenterOffset(Vector2 centerOffset)
		{
			_RefreshLayout();

			return Center+centerOffset;
		}

		/// <summary>Tests whether a RectTransform local point lies inside <see cref="GetPixelAdjustedRect"/> (editor hit test).</summary>
		internal bool _ContainsLocalPoint(Vector2 localPoint)
		{
			_RefreshLayout();

			return GetPixelAdjustedRect().Contains(localPoint);
		}
	}
}