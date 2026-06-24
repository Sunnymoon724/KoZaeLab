using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.Utilities
{
	/// <summary>
	/// Horizontally scrolling tiled background. Clones a hidden pivot <see cref="SpriteRenderer"/>,
	/// moves tiles in local X each frame, and recycles edge tiles to simulate infinite scroll.
	/// </summary>
	public class InfiniteBackground : MonoBehaviour
	{
		/// <summary>Template sprite; deactivated after clones are created.</summary>
		[SerializeField,Required,PropertyTooltip("Template sprite. Use a centered pivot for seamless tiling.")]
		private SpriteRenderer m_pivot = null;

		/// <summary>Number of active tiles. Must cover at least the visible camera width.</summary>
		[SerializeField,MinValue(2),PropertyTooltip("Number of tiles. Use enough to cover the camera view width.")]
		private int m_count = 2;

		/// <summary>Scroll speed in local units per second. Positive = right, negative = left.</summary>
		[SerializeField]
		private float m_speed = 0.0f;

		/// <summary>Local XY position of the leftmost tile at initialization.</summary>
		[SerializeField]
		private Vector2 m_startPoint = Vector2.zero;

		/// <summary>Active tiles ordered left to right (index 0 = leftmost).</summary>
		private List<Transform> m_backgroundList = null;

		/// <summary>Horizontal spacing between tile centers in this transform's local space.</summary>
		private float m_width = 0.0f;

		private void Awake()
		{
			if(!m_pivot)
			{
				throw new NullReferenceException($"Pivot is null. {gameObject.name}");
			}

			m_width = _GetTileWidth();

			if(m_width.ApproximatelyZero())
			{
				throw new InvalidOperationException($"Pivot sprite has zero width. Sprite must be assigned. {m_pivot.gameObject.name}");
			}

			m_pivot.gameObject.EnsureActive(false);

			m_backgroundList = new List<Transform>(m_count);

			for(var i=0;i<m_count;i++)
			{
				var instance = m_pivot.gameObject.CopyObject(transform) as GameObject;

				if(!instance)
				{
					throw new InvalidOperationException($"Failed to copy background pivot. {m_pivot.gameObject.name}");
				}

				instance.transform.SetLocalPositionXY(m_startPoint+new Vector2(i*m_width,0.0f));
				instance.EnsureActive(true);

				m_backgroundList.Add(instance.transform);
			}
		}

		/// <summary>
		/// Tile width in parent local X. Uses sprite local bounds and pivot scale
		/// so spacing matches <see cref="Transform.localPosition"/> regardless of parent world scale.
		/// </summary>
		private float _GetTileWidth()
		{
			if(!m_pivot.sprite)
			{
				throw new InvalidOperationException($"Pivot sprite is not assigned. {m_pivot.gameObject.name}");
			}

			return m_pivot.sprite.bounds.size.x*Mathf.Abs(m_pivot.transform.localScale.x);
		}

		private void Update()
		{
			if(m_backgroundList == null || m_speed.ApproximatelyZero())
			{
				return;
			}

			var delta = m_speed*Time.deltaTime;

			foreach(var background in m_backgroundList)
			{
				background.localPosition += new Vector3(delta,0.0f,0.0f);
			}

			if(m_speed > 0.0f)
			{
				_WrapScrollRight();
			}
			else
			{
				_WrapScrollLeft();
			}
		}

		/// <summary>
		/// When scrolling right, moves the rightmost tile to the left of the leftmost tile
		/// once it crosses the wrap line. Repeats within the same frame for large deltas.
		/// </summary>
		private void _WrapScrollRight()
		{
			var wrapLine = m_startPoint.x + m_count*m_width;

			while(true)
			{
				var first = m_backgroundList[0];
				var last = m_backgroundList[^1];

				if(last.localPosition.x < wrapLine)
				{
					break;
				}

				last.SetLocalPositionX(first.localPosition.x - m_width);

				m_backgroundList.RemoveAt(m_backgroundList.Count - 1);
				m_backgroundList.Insert(0,last);
			}
		}

		/// <summary>
		/// When scrolling left, moves the leftmost tile to the right of the rightmost tile
		/// once it crosses the wrap line. Repeats within the same frame for large deltas.
		/// </summary>
		private void _WrapScrollLeft()
		{
			var wrapLine = m_startPoint.x - m_width;

			while(true)
			{
				var first = m_backgroundList[0];
				var last = m_backgroundList[^1];

				if(first.localPosition.x > wrapLine)
				{
					break;
				}

				first.SetLocalPositionX(last.localPosition.x + m_width);

				m_backgroundList.RemoveAt(0);
				m_backgroundList.Add(first);
			}
		}
	}
}