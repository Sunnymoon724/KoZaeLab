using System.Collections.Generic;
using R3;
using KZLib.Development;
using System;
using UnityEngine;

namespace KZLib.UI
{
	public class CarouselNavigator : MonoBehaviour
	{
		[SerializeField]
		private Carousel m_carousel = null;
		[SerializeField]
		private CarouselDot m_dot = null;

		private readonly List<bool> m_stateList = new();
		private GameObjectPoolBinder<CarouselDot,bool> m_poolBinder = null;

		private IDisposable m_subscription = null;

		private void Awake()
		{
			static void _BindDot(CarouselDot dot,bool selected)
			{
				dot.SetDot(selected);
			}

			m_poolBinder = new GameObjectPoolBinder<CarouselDot,bool>(m_dot,transform,_BindDot);
		}
		
		private void OnEnable()
		{
			m_subscription = m_carousel.OnChangedIndex.Subscribe(_OnChangedIndex);
		}

		private void OnDisable()
		{
			m_subscription?.Dispose();
		}

		public void SetNavigator(int count)
		{
			// _EnsureInitialized();

			m_stateList.Clear();

			for(var i=0;i<count;i++)
			{
				m_stateList.Add(false);
			}
		}

		private void _OnChangedIndex(int index)
		{
			for(var i=0;i<m_stateList.Count;i++)
			{
				m_stateList[i] = i == index;
			}

			if(!m_poolBinder.TrySetDataList(m_stateList))
			{
				return;
			}
		}
	}
}