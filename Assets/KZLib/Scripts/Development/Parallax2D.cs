using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.KZDevelop
{
	public class Parallax2D : BaseComponent
	{
		[SerializeField,LabelText("프리펩")]
		private GameObject m_Pivot = null;

		[SerializeField,LabelText("프리펩 갯수"),MinValue(1)]
		private int m_Count = 1;
		
		[SerializeField,LabelText("너비"),MinValue(0.0f)]
		private float m_Width = 0.0f;

		[SerializeField,LabelText("스크롤 속도")]
		private float m_Speed = 0.0f;

		[SerializeField,LabelText("시작 지점")]
		private Vector2 m_StartPoint = Vector2.zero;

		private List<Transform> m_ParallaxList = null;

		protected override void Awake()
		{
			base.Awake();

			if(m_Count < 1 || m_Width < 0.0f)
			{
				throw new InvalidOperationException("갯수는 1이상, 넓이는 0이상 이여야 합니다.");
			}

			m_ParallaxList = new List<Transform>(m_Count);

			for(var i=0;i<m_Count;i++)
			{
				var prefab = CommonUtility.CopyObject(m_Pivot,transform);
				prefab.transform.SetLocalPositionXY(m_StartPoint);

				m_ParallaxList.Add(prefab.transform);
			}
		}

		private void Update()
		{
			if(m_Speed == 0.0f)
			{
				return;
			}

			if((m_Speed > 0.0f && m_ParallaxList[0].position.x > m_StartPoint.x) || (m_Speed < 0.0f && m_ParallaxList[0].position.x < m_StartPoint.x))
			{
				var temp = m_ParallaxList[0];

				for(var i=0;i<m_Count-1;i++)
				{
					m_ParallaxList[i] = m_ParallaxList[i+1];
				}

				m_ParallaxList[m_Count-1] = temp;
			}

			m_ParallaxList[0].Translate(m_Speed*Time.deltaTime*Vector3.right);

			for(var i=0;i<m_ParallaxList.Count;i++)
			{
				m_ParallaxList[i].SetLocalPositionX(m_ParallaxList[0].localPosition.x-i*m_Width*Mathf.Sign(m_Speed));
			}
		}
	}
}