using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.Utilities
{
	public class InfiniteBackground : MonoBehaviour
	{
		[SerializeField]
		private SpriteRenderer m_pivot = null;

		[SerializeField,MinValue(2)]
		private int m_count = 2;

		[SerializeField]
		private float m_speed = 0.0f;

		[SerializeField]
		private Vector2 m_startPoint = Vector2.zero;

		private List<Transform> m_backgroundList = null;

		private float m_width = 0.0f;

		private void Awake()
		{
			if(!m_pivot)
			{
				throw new NullReferenceException($"Pivot is null. {gameObject.name}");
			}

		m_width = m_pivot.bounds.size.x;

		if(m_width.ApproximatelyZero())
		{
			throw new InvalidOperationException($"Pivot sprite has zero width. Sprite must be assigned. {m_pivot.gameObject.name}");
		}

		m_pivot.gameObject.EnsureActive(false);

			m_backgroundList = new List<Transform>(m_count);

			for(var i=0;i<m_count;i++)
			{
				var prefab = m_pivot.gameObject.CopyObject(transform) as GameObject;

				if(!prefab)
				{
					throw new InvalidOperationException($"Failed to copy background pivot. {m_pivot.gameObject.name}");
				}

				prefab.transform.SetLocalPositionXY(m_startPoint+new Vector2(i*m_width,0.0f));
				prefab.EnsureActive(true);

				m_backgroundList.Add(prefab.transform);
			}
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

		private void _WrapScrollRight()
		{
			while(true)
			{
				var first = m_backgroundList[0];
				var last = m_backgroundList[^1];

				if(first.localPosition.x - m_width/2.0f <= m_startPoint.x)
				{
					break;
				}

				last.SetLocalPositionX(first.localPosition.x - m_width);

				m_backgroundList.RemoveAt(m_backgroundList.Count - 1);
				m_backgroundList.Insert(0,last);
			}
		}

		private void _WrapScrollLeft()
		{
			while(true)
			{
				var first = m_backgroundList[0];
				var last = m_backgroundList[^1];

				if(first.localPosition.x + m_width/2.0f >= m_startPoint.x)
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