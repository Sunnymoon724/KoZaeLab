using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib
{
	public class InfiniteBackground : SerializedMonoBehaviour
	{
		[SerializeField,LabelText("Pivot")]
		private SpriteRenderer m_Pivot = null;

		[SerializeField,LabelText("Count"),MinValue(1)]
		private int m_Count = 1;

		[SerializeField,LabelText("Scroll Speed")]
		private float m_Speed = 0.0f;

		[SerializeField,LabelText("Start Point")]
		private Vector2 m_StartPoint = Vector2.zero;

		private List<Transform> m_BackgroundList = null;

		private float m_Width = 0.0f;

		private void Awake()
		{
			if(!m_Pivot)
			{
				throw new NullReferenceException("Pivot is null.");
			}

			m_Width = m_Pivot.bounds.size.x*m_Pivot.transform.lossyScale.x;

			m_Pivot.gameObject.SetActiveSelf(false);

			m_BackgroundList = new List<Transform>(m_Count);

			for(var i=0;i<m_Count;i++)
			{
				var prefab = CommonUtility.CopyObject(m_Pivot.gameObject,transform);

				prefab.transform.SetLocalPositionXY(m_StartPoint+new Vector2(i*m_Width,0.0f));
				prefab.SetActiveSelf(true);

				m_BackgroundList.Add(prefab.transform);
			}
		}

		private void Update()
		{
			if(m_Speed.ApproximatelyZero())
			{
				return;
			}

			var scrolling = m_Speed > 0.0f;
			var point = m_BackgroundList[0].position.x;

			if((scrolling && point-m_Width/2.0f > m_StartPoint.x) || (!scrolling && point+m_Width/2.0f < m_StartPoint.x))
			{
				var background = m_BackgroundList[0];

				m_BackgroundList.RemoveAt(0);
				m_BackgroundList.Add(background);
			}

			m_BackgroundList[0].Translate(m_Speed*Time.deltaTime*Vector3.right);

			for(var i=1;i<m_BackgroundList.Count;i++)
			{
				m_BackgroundList[i].SetLocalPositionX(m_BackgroundList[0].localPosition.x-i*m_Width*Mathf.Sign(m_Speed));
			}
		}
	}
}