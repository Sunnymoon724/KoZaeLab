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

		[SerializeField,MinValue(1)]
		private int m_count = 1;

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
				throw new NullReferenceException("Pivot is null.");
			}

			m_width = m_pivot.bounds.size.x;

			m_pivot.gameObject.EnsureActive(false);

			m_backgroundList = new List<Transform>(m_count);

			for(var i=0;i<m_count;i++)
			{
				var prefab = m_pivot.gameObject.CopyObject(transform) as GameObject;

				prefab.transform.SetLocalPositionXY(m_startPoint+new Vector2(i*m_width,0.0f));
				prefab.EnsureActive(true);

				m_backgroundList.Add(prefab.transform);
			}
		}

		private void Update()
		{
			if(m_speed.ApproximatelyZero())
			{
				return;
			}

			var delta = m_speed*Time.deltaTime;

			foreach(var background in m_backgroundList)
			{
				background.Translate(delta*Vector3.right);
			}

			var scrolling = m_speed > 0.0f;

			while(true)
			{
				var first = m_backgroundList[0];
				var outOfBounds = scrolling ? first.position.x-m_width/2.0f > m_startPoint.x: first.position.x+m_width/2.0f < m_startPoint.x;

				if(!outOfBounds)
				{
					break;
				}

				var last = m_backgroundList[^1];

				first.SetLocalPositionX(last.localPosition.x+m_width*Mathf.Sign(m_speed));

				m_backgroundList.RemoveAt(0);
				m_backgroundList.Add(first);
			}
		}
	}
}